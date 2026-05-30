using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;

public class UpdatePurchaseOrderCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<UpdatePurchaseOrderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<bool>.Failure(new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (purchaseOrder is null)
        {
            return Result<bool>.Failure(new Error("PurchaseOrder.NotFound", $"Purchase order with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        if (purchaseOrder.Status != PurchaseOrderStatus.Draft)
        {
            return Result<bool>.Failure(new Error("PurchaseOrder.InvalidTransition", "Only Draft purchase orders can be updated.", ErrorType.Conflict));
        }

        var items = request.Items.Select(i => new PurchaseOrderItem
        {
            PurchaseOrderId = purchaseOrder.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            Subtotal = i.Quantity * i.UnitPrice
        }).ToList();

        var subtotal = items.Sum(i => i.Subtotal);
        var ivaAmount = Math.Round(subtotal * 0.16m, 2);

        purchaseOrder.SupplierId = request.SupplierId;
        purchaseOrder.ExpectedDate = request.ExpectedDate;
        purchaseOrder.Notes = request.Notes;
        purchaseOrder.Items = items;
        purchaseOrder.Subtotal = subtotal;
        purchaseOrder.IvaAmount = ivaAmount;
        purchaseOrder.Total = subtotal + ivaAmount;

        purchaseOrderRepository.Update(purchaseOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
