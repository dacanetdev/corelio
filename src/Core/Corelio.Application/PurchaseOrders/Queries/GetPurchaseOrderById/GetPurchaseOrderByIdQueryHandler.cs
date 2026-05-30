using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Queries.GetPurchaseOrderById;

public class GetPurchaseOrderByIdQueryHandler(
    IPurchaseOrderRepository purchaseOrderRepository) : IRequestHandler<GetPurchaseOrderByIdQuery, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(
        GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (purchaseOrder is null)
        {
            return Result<PurchaseOrderDto>.Failure(new Error("PurchaseOrder.NotFound", $"Purchase order with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        var dto = new PurchaseOrderDto(
            purchaseOrder.Id,
            purchaseOrder.OrderNumber,
            purchaseOrder.SupplierId,
            purchaseOrder.Supplier?.Name ?? string.Empty,
            purchaseOrder.Status,
            purchaseOrder.ExpectedDate,
            purchaseOrder.Notes,
            purchaseOrder.Subtotal,
            purchaseOrder.IvaAmount,
            purchaseOrder.Total,
            purchaseOrder.CreatedAt,
            purchaseOrder.Items.Select(i => new PurchaseOrderItemDto(
                i.Id,
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice,
                i.Subtotal,
                i.ReceivedQuantity)).ToList());

        return Result<PurchaseOrderDto>.Success(dto);
    }
}
