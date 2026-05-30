using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Commands.SubmitPurchaseOrder;

public class SubmitPurchaseOrderCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<SubmitPurchaseOrderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SubmitPurchaseOrderCommand request, CancellationToken cancellationToken)
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
            return Result<bool>.Failure(new Error("PurchaseOrder.InvalidTransition", $"Cannot submit a purchase order in '{purchaseOrder.Status}' status. Only Draft orders can be submitted.", ErrorType.Conflict));
        }

        purchaseOrder.Status = PurchaseOrderStatus.Submitted;

        purchaseOrderRepository.Update(purchaseOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
