namespace Corelio.Application.PurchaseOrders.Common;

public record PurchaseOrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice,
    decimal Subtotal,
    decimal ReceivedQuantity);
