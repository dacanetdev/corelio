namespace Corelio.BlazorApp.Models.Reports;

/// <summary>
/// Purchase summary report returned by GET /api/v1/reports/purchases.
/// </summary>
public class PurchaseSummaryReportModel
{
    public DateTime GeneratedAt { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public Guid? SupplierId { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalAmountSpent { get; set; }
    public int PendingDeliveries { get; set; }
    public List<SupplierSpendingModel> BySupplier { get; set; } = [];
    public List<StatusBreakdownModel> ByStatus { get; set; } = [];
    public List<ProductReceiptModel> ProductReceipts { get; set; } = [];
}

public class SupplierSpendingModel
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class StatusBreakdownModel
{
    public string StatusName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class ProductReceiptModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal PendingQuantity { get; set; }
}
