using Corelio.Domain.Enums;

namespace Corelio.Application.PurchaseOrders.Common;

public record PurchaseOrderListDto(
    Guid Id,
    string OrderNumber,
    Guid SupplierId,
    string SupplierName,
    PurchaseOrderStatus Status,
    DateTimeOffset? ExpectedDate,
    decimal Total,
    DateTimeOffset CreatedAt);
