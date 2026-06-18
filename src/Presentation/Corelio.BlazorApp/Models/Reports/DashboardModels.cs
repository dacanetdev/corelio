namespace Corelio.BlazorApp.Models.Reports;

/// <summary>
/// Dashboard KPI snapshot returned by GET /api/v1/reports/dashboard.
/// </summary>
public class DashboardSummaryModel
{
    public decimal TodaysSalesTotal { get; set; }
    public int TodaysTransactionCount { get; set; }
    public int LowStockProductCount { get; set; }
    public int PendingPurchaseOrderCount { get; set; }
    public DateTime GeneratedAt { get; set; }
}
