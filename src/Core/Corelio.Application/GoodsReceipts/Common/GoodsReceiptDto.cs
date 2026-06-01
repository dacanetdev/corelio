namespace Corelio.Application.GoodsReceipts.Common;

public record GoodsReceiptDto(
    Guid Id,
    Guid PurchaseOrderId,
    string PurchaseOrderNumber,
    Guid WarehouseId,
    string WarehouseName,
    DateOnly ReceivedDate,
    string? Notes,
    DateTimeOffset CreatedAt,
    List<GoodsReceiptItemDto> Items);
