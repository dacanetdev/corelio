using Corelio.Application.Common.Models;
using Corelio.Application.GoodsReceipts.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.GoodsReceipts.Queries.GetGoodsReceiptById;

public class GetGoodsReceiptByIdQueryHandler(
    IGoodsReceiptRepository goodsReceiptRepository) : IRequestHandler<GetGoodsReceiptByIdQuery, Result<GoodsReceiptDto>>
{
    public async Task<Result<GoodsReceiptDto>> Handle(GetGoodsReceiptByIdQuery request, CancellationToken cancellationToken)
    {
        var receipt = await goodsReceiptRepository.GetByIdAsync(request.Id, cancellationToken);
        if (receipt is null)
        {
            return Result<GoodsReceiptDto>.Failure(new Error("GoodsReceipt.NotFound", $"Goods receipt with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        var dto = new GoodsReceiptDto(
            receipt.Id,
            receipt.PurchaseOrderId,
            receipt.PurchaseOrder.OrderNumber,
            receipt.WarehouseId,
            receipt.Warehouse.Name,
            receipt.ReceivedDate,
            receipt.Notes,
            receipt.CreatedAt,
            receipt.Items.Select(i => new GoodsReceiptItemDto(
                i.Id,
                i.PurchaseOrderItemId,
                i.ProductId,
                i.ProductName,
                i.QuantityReceived)).ToList());

        return Result<GoodsReceiptDto>.Success(dto);
    }
}
