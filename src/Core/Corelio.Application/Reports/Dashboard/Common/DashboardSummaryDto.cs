namespace Corelio.Application.Reports.Dashboard.Common;

/// <summary>
/// Snapshot of key operational KPIs shown on the reports dashboard.
/// </summary>
public sealed record DashboardSummaryDto(
    decimal TodaysSalesTotal,
    int TodaysTransactionCount,
    int LowStockProductCount,
    int PendingPurchaseOrderCount,
    DateTime GeneratedAt);
