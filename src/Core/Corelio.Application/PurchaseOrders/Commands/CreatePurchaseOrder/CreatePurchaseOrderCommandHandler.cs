using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<CreatePurchaseOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var year = DateTimeOffset.UtcNow.Year;
        var sequence = await purchaseOrderRepository.GetNextSequenceAsync(year, cancellationToken);
        var orderNumber = $"PO-{year}-{sequence:D4}";

        var items = request.Items.Select(i => new PurchaseOrderItem
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            Subtotal = i.Quantity * i.UnitPrice
        }).ToList();

        var subtotal = items.Sum(i => i.Subtotal);
        var ivaAmount = Math.Round(subtotal * 0.16m, 2);
        var total = subtotal + ivaAmount;

        var purchaseOrder = new PurchaseOrder
        {
            TenantId = tenantId.Value,
            OrderNumber = orderNumber,
            SupplierId = request.SupplierId,
            ExpectedDate = request.ExpectedDate,
            Notes = request.Notes,
            Subtotal = subtotal,
            IvaAmount = ivaAmount,
            Total = total,
            Items = items
        };

        purchaseOrderRepository.Add(purchaseOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(purchaseOrder.Id);
    }
}
