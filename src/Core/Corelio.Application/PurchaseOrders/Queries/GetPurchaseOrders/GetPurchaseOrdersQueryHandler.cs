using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.PurchaseOrders.Queries.GetPurchaseOrders;

public class GetPurchaseOrdersQueryHandler(
    IPurchaseOrderRepository purchaseOrderRepository) : IRequestHandler<GetPurchaseOrdersQuery, Result<PagedResult<PurchaseOrderListDto>>>
{
    public async Task<Result<PagedResult<PurchaseOrderListDto>>> Handle(
        GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await purchaseOrderRepository.GetPagedAsync(
            request.Page,
            request.Size,
            request.Status,
            request.SupplierId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        var dtos = items.Select(po => new PurchaseOrderListDto(
            po.Id,
            po.OrderNumber,
            po.SupplierId,
            po.Supplier?.Name ?? string.Empty,
            po.Status,
            po.ExpectedDate,
            po.Total,
            po.CreatedAt)).ToList();

        return Result<PagedResult<PurchaseOrderListDto>>.Success(
            PagedResult<PurchaseOrderListDto>.Create(dtos, request.Page, request.Size, totalCount));
    }
}
