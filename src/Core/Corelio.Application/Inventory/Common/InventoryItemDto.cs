namespace Corelio.Application.Inventory.Common;

/// <summary>
/// Data transfer object for an inventory item with stock status.
/// </summary>
public record InventoryItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSku,
    string? ProductBarcode,
    Guid WarehouseId,
    string WarehouseName,
    decimal Quantity,
    decimal ReservedQuantity,
    decimal AvailableQuantity,
    decimal MinimumLevel,
    string StockStatus);   // "Adequate" | "LowStock" | "OutOfStock"
