using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;

public class ApprovePurchaseOrderCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<ApprovePurchaseOrderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ApprovePurchaseOrderCommand request, CancellationToken cancellationToken)
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

        if (purchaseOrder.Status != PurchaseOrderStatus.Submitted)
        {
            return Result<bool>.Failure(new Error("PurchaseOrder.InvalidTransition", $"Cannot approve a purchase order in '{purchaseOrder.Status}' status. Only Submitted orders can be approved.", ErrorType.Conflict));
        }

        purchaseOrder.Status = PurchaseOrderStatus.Approved;

        purchaseOrderRepository.Update(purchaseOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
