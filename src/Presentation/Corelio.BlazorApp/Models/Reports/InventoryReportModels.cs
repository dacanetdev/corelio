namespace Corelio.BlazorApp.Models.Reports;

/// <summary>
/// Inventory valuation report returned by GET /api/v1/reports/inventory.
/// </summary>
public class InventoryValuationReportModel
{
    public DateTime GeneratedAt { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int TotalProductCount { get; set; }
    public List<CategoryValueModel> ValueByCategory { get; set; } = [];
    public List<ProductValuationModel> ProductValuations { get; set; } = [];
    public List<LowStockProductModel> LowStockAlerts { get; set; } = [];
}

public class CategoryValueModel
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public decimal Percentage { get; set; }
    public int ProductCount { get; set; }
}

public class ProductValuationModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal ExtendedValue { get; set; }
}

public class LowStockProductModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public decimal MinimumLevel { get; set; }
    public decimal AverageCost { get; set; }
}
