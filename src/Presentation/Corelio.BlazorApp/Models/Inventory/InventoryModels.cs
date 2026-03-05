namespace Corelio.BlazorApp.Models.Inventory;

/// <summary>
/// Inventory item summary for the stock list view.
/// </summary>
public class InventoryItemModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string? ProductBarcode { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal MinimumLevel { get; set; }
    public string StockStatus { get; set; } = "Adequate"; // "Adequate" | "LowStock" | "OutOfStock"
}

/// <summary>
/// Inventory transaction entry in transaction history.
/// </summary>
public class InventoryTransactionModel
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal PreviousQuantity { get; set; }
    public decimal NewQuantity { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}

/// <summary>
/// Warehouse summary for filter dropdowns.
/// </summary>
public class WarehouseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

/// <summary>
/// Request body for a stock adjustment.
/// </summary>
public class AdjustStockRequest
{
    public Guid InventoryItemId { get; set; }
    public decimal Quantity { get; set; }
    public bool IsIncrease { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
