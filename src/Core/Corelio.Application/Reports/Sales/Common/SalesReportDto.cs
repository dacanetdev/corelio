using Corelio.Domain.Enums;

namespace Corelio.Application.Reports.Sales.Common;

/// <summary>
/// Aggregated daily sales report data.
/// </summary>
public record SalesReportDto(
    decimal TotalAmount,
    int TransactionCount,
    decimal AverageTicket,
    DateTime DateFrom,
    DateTime DateTo,
    List<PaymentMethodBreakdownDto> PaymentMethodBreakdown,
    List<TopProductDto> TopProducts,
    List<HourlyDistributionDto> HourlyDistribution,
    List<CashierBreakdownDto> CashierBreakdown);

/// <summary>
/// Sales amount and count broken down by payment method.
/// </summary>
public record PaymentMethodBreakdownDto(
    PaymentMethod Method,
    string MethodName,
    decimal Amount,
    int TransactionCount,
    decimal Percentage);

/// <summary>
/// Top-selling product by quantity.
/// </summary>
public record TopProductDto(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    decimal QuantitySold,
    decimal Revenue);

/// <summary>
/// Sales aggregated by hour of day (0–23).
/// </summary>
public record HourlyDistributionDto(
    int Hour,
    decimal Amount,
    int TransactionCount);

/// <summary>
/// Sales aggregated by cashier.
/// </summary>
public record CashierBreakdownDto(
    string CashierName,
    decimal Amount,
    int TransactionCount);
