namespace Corelio.Application.GoodsReceipts.Common;

public record GoodsReceiptItemDto(
    Guid Id,
    Guid PurchaseOrderItemId,
    Guid ProductId,
    string ProductName,
    decimal QuantityReceived);
