using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Common;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Commands.CompleteSale;

/// <summary>
/// Handler for CompleteSaleCommand. Validates payments, deducts inventory, marks sale complete.
/// </summary>
public class CompleteSaleCommandHandler(
    ISaleRepository saleRepository,
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CompleteSaleCommand, Result<SaleDto>>
{
    public async Task<Result<SaleDto>> Handle(CompleteSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
        {
            return Result<SaleDto>.Failure(
                new Error("Sale.NotFound", $"Sale '{request.SaleId}' not found.", ErrorType.NotFound));
        }

        if (sale.Status != SaleStatus.Draft)
        {
            return Result<SaleDto>.Failure(
                new Error("Sale.InvalidStatus", $"Sale '{sale.Folio}' cannot be completed — it is already '{sale.Status}'.", ErrorType.Conflict));
        }

        // Validate total payment coverage
        var totalPaid = request.Payments.Sum(p => p.Amount);
        if (totalPaid < sale.Total)
        {
            return Result<SaleDto>.Failure(
                new Error("Sale.PaymentShort",
                    $"Payment total ({totalPaid:C}) is less than sale total ({sale.Total:C}).",
                    ErrorType.Validation));
        }

        // Deduct inventory for each item
        foreach (var item in sale.Items)
        {
            var inventoryItem = await inventoryRepository.GetByProductAndWarehouseAsync(
                item.ProductId, sale.WarehouseId, cancellationToken);

            if (inventoryItem is null)
            {
                // Create inventory record if not exists (first sale of this product)
                inventoryItem = new InventoryItem
                {
                    TenantId = sale.TenantId,
                    ProductId = item.ProductId,
                    WarehouseId = sale.WarehouseId,
                    Quantity = 0,
                    ReservedQuantity = 0,
                    MinimumLevel = 0
                };
                inventoryRepository.AddInventoryItem(inventoryItem);
            }

            var previousQty = inventoryItem.Quantity;
            inventoryItem.Quantity -= item.Quantity;

            var transaction = new InventoryTransaction
            {
                TenantId = sale.TenantId,
                InventoryItemId = inventoryItem.Id,
                Type = InventoryTransactionType.Sale,
                Quantity = -item.Quantity,
                PreviousQuantity = previousQty,
                NewQuantity = inventoryItem.Quantity,
                ReferenceId = sale.Id,
                Notes = $"Sale {sale.Folio}"
            };

            inventoryRepository.UpdateInventoryItem(inventoryItem);
            inventoryRepository.AddTransaction(transaction);
        }

        // Record payments
        foreach (var paymentRequest in request.Payments)
        {
            var payment = new Payment
            {
                TenantId = sale.TenantId,
                SaleId = sale.Id,
                Method = paymentRequest.Method,
                Amount = paymentRequest.Amount,
                Reference = paymentRequest.Reference,
                Status = PaymentStatus.Paid
            };
            sale.Payments.Add(payment);
        }

        // Finalize sale
        sale.Status = SaleStatus.Completed;
        sale.CompletedAt = DateTime.UtcNow;
        saleRepository.Update(sale);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = MapToDto(sale);
        return Result<SaleDto>.Success(dto);
    }

    private static SaleDto MapToDto(Sale sale)
    {
        var items = sale.Items.Select(i => new SaleItemDto(
            i.Id, i.ProductId, i.ProductName, i.ProductSku,
            i.UnitPrice, i.Quantity, i.DiscountPercentage, i.TaxPercentage, i.LineTotal)).ToList();

        var payments = sale.Payments.Select(p => new PaymentDto(
            p.Id, p.Method, p.Amount, p.Reference, p.Status)).ToList();

        return new SaleDto(
            sale.Id,
            sale.Folio,
            sale.Type,
            sale.Status,
            sale.CustomerId,
            sale.Customer?.FullName,
            sale.WarehouseId,
            sale.Warehouse?.Name ?? string.Empty,
            sale.SubTotal,
            sale.DiscountAmount,
            sale.TaxAmount,
            sale.Total,
            sale.Notes,
            sale.CompletedAt,
            sale.CreatedAt,
            items,
            payments);
    }
}
