namespace Corelio.Application.Reports.Purchases.Common;

/// <summary>
/// Root DTO returned by the purchase summary report query.
/// </summary>
public sealed record PurchaseSummaryReportDto(
    DateTime GeneratedAt,
    DateTime DateFrom,
    DateTime DateTo,
    Guid? SupplierId,
    int TotalOrders,
    decimal TotalAmountSpent,
    int PendingDeliveries,
    List<SupplierSpendingDto> BySupplier,
    List<StatusBreakdownDto> ByStatus,
    List<ProductReceiptDto> ProductReceipts);

/// <summary>
/// Spending aggregation per supplier within the report period.
/// </summary>
public sealed record SupplierSpendingDto(
    Guid SupplierId,
    string SupplierName,
    int OrderCount,
    decimal TotalAmount);

/// <summary>
/// Order count and monetary total grouped by purchase order status.
/// </summary>
public sealed record StatusBreakdownDto(
    string StatusName,
    int OrderCount,
    decimal TotalAmount);

/// <summary>
/// Ordered vs. received quantities per product across all purchase orders in the period.
/// </summary>
public sealed record ProductReceiptDto(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    decimal OrderedQuantity,
    decimal ReceivedQuantity,
    decimal PendingQuantity);
