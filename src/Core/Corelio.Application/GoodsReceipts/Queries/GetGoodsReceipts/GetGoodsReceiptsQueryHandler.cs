using Corelio.Application.Common.Models;
using Corelio.Application.GoodsReceipts.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.GoodsReceipts.Queries.GetGoodsReceipts;

public class GetGoodsReceiptsQueryHandler(
    IGoodsReceiptRepository goodsReceiptRepository) : IRequestHandler<GetGoodsReceiptsQuery, Result<PagedResult<GoodsReceiptListDto>>>
{
    public async Task<Result<PagedResult<GoodsReceiptListDto>>> Handle(GetGoodsReceiptsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await goodsReceiptRepository.GetPagedAsync(
            request.Page,
            request.Size,
            request.PurchaseOrderId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        var dtos = items.Select(r => new GoodsReceiptListDto(
            r.Id,
            r.PurchaseOrderId,
            r.PurchaseOrder.OrderNumber,
            r.WarehouseId,
            r.Warehouse.Name,
            r.ReceivedDate,
            r.Items.Count,
            r.CreatedAt)).ToList();

        return Result<PagedResult<GoodsReceiptListDto>>.Success(
            PagedResult<GoodsReceiptListDto>.Create(dtos, request.Page, request.Size, total));
    }
}
