namespace Corelio.Application.GoodsReceipts.Common;

public record GoodsReceiptListDto(
    Guid Id,
    Guid PurchaseOrderId,
    string PurchaseOrderNumber,
    Guid WarehouseId,
    string WarehouseName,
    DateOnly ReceivedDate,
    int ItemCount,
    DateTimeOffset CreatedAt);
