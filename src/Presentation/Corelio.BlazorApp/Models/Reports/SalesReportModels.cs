using Corelio.Domain.Enums;

namespace Corelio.BlazorApp.Models.Reports;

/// <summary>
/// Aggregated sales report returned by GET /api/v1/reports/sales.
/// </summary>
public class SalesReportModel
{
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageTicket { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public List<PaymentMethodBreakdownModel> PaymentMethodBreakdown { get; set; } = [];
    public List<TopProductModel> TopProducts { get; set; } = [];
    public List<HourlyDistributionModel> HourlyDistribution { get; set; } = [];
    public List<CashierBreakdownModel> CashierBreakdown { get; set; } = [];
}

public class PaymentMethodBreakdownModel
{
    public PaymentMethod Method { get; set; }
    public string MethodName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
    public decimal Percentage { get; set; }
}

public class TopProductModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public decimal QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class HourlyDistributionModel
{
    public int Hour { get; set; }
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
}

public class CashierBreakdownModel
{
    public string CashierName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
}
