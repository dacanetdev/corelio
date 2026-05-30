using Corelio.Domain.Enums;

namespace Corelio.Application.PurchaseOrders.Common;

public record PurchaseOrderDto(
    Guid Id,
    string OrderNumber,
    Guid SupplierId,
    string SupplierName,
    PurchaseOrderStatus Status,
    DateTimeOffset? ExpectedDate,
    string? Notes,
    decimal Subtotal,
    decimal IvaAmount,
    decimal Total,
    DateTimeOffset CreatedAt,
    List<PurchaseOrderItemDto> Items);
