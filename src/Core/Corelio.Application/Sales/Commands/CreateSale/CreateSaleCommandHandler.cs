using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Commands.CreateSale;

/// <summary>
/// Handler for CreateSaleCommand. Creates a Draft sale without deducting inventory.
/// </summary>
public class CreateSaleCommandHandler(
    ISaleRepository saleRepository,
    IInventoryRepository inventoryRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<CreateSaleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        // Resolve warehouse
        Guid warehouseId;
        if (request.WarehouseId.HasValue)
        {
            warehouseId = request.WarehouseId.Value;
        }
        else
        {
            var defaultWarehouse = await inventoryRepository.GetDefaultWarehouseAsync(cancellationToken);
            if (defaultWarehouse is null)
            {
                return Result<Guid>.Failure(
                    new Error("Warehouse.NotFound", "No default warehouse found. Please configure a warehouse.", ErrorType.NotFound));
            }

            warehouseId = defaultWarehouse.Id;
        }

        // Build sale items with product snapshots
        var saleItems = new List<SaleItem>();
        decimal subTotal = 0;
        decimal totalTax = 0;
        decimal totalDiscount = 0;

        foreach (var item in request.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
            {
                return Result<Guid>.Failure(
                    new Error("Product.NotFound", $"Product '{item.ProductId}' not found.", ErrorType.NotFound));
            }

            var taxPct = product.IvaEnabled ? product.TaxRate * 100 : 0;
            var lineBase = item.UnitPrice * item.Quantity;
            var lineDiscount = lineBase * (item.DiscountPercentage / 100m);
            var lineNet = lineBase - lineDiscount;
            var lineTax = lineNet * (taxPct / 100m);
            var lineTotal = lineNet + lineTax;

            subTotal += lineNet;
            totalTax += lineTax;
            totalDiscount += lineDiscount;

            saleItems.Add(new SaleItem
            {
                TenantId = tenantId.Value,
                ProductId = item.ProductId,
                ProductName = product.Name,
                ProductSku = product.Sku,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                DiscountPercentage = item.DiscountPercentage,
                TaxPercentage = taxPct,
                LineTotal = lineTotal
            });
        }

        // Generate folio
        var folioNumber = await saleRepository.GetNextFolioNumberAsync(cancellationToken);
        var folio = $"V-{folioNumber:D5}";

        var sale = new Sale
        {
            TenantId = tenantId.Value,
            Folio = folio,
            Type = request.Type,
            Status = SaleStatus.Draft,
            CustomerId = request.CustomerId,
            WarehouseId = warehouseId,
            SubTotal = subTotal,
            DiscountAmount = totalDiscount,
            TaxAmount = totalTax,
            Total = subTotal + totalTax,
            Notes = request.Notes,
            Items = saleItems
        };

        saleRepository.Add(sale);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(sale.Id);
    }
}
