namespace Corelio.Application.Reports.Inventory.Common;

/// <summary>
/// Aggregated inventory valuation report data.
/// </summary>
public sealed record InventoryValuationReportDto(
    DateTime GeneratedAt,
    Guid? WarehouseId,
    Guid? CategoryId,
    decimal TotalInventoryValue,
    int TotalProductCount,
    List<CategoryValueDto> ValueByCategory,
    List<ProductValuationDto> ProductValuations,
    List<LowStockProductDto> LowStockAlerts
);

/// <summary>
/// Inventory value aggregated by product category.
/// </summary>
public sealed record CategoryValueDto(
    Guid CategoryId,
    string CategoryName,
    decimal TotalValue,
    decimal Percentage,
    int ProductCount
);

/// <summary>
/// Inventory valuation for a single product-warehouse combination.
/// </summary>
public sealed record ProductValuationDto(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    Guid? CategoryId,
    string? CategoryName,
    Guid WarehouseId,
    decimal Quantity,
    decimal AverageCost,
    decimal ExtendedValue
);

/// <summary>
/// Product whose stock is at or below the configured minimum level.
/// </summary>
public sealed record LowStockProductDto(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    Guid WarehouseId,
    decimal Quantity,
    decimal MinimumLevel,
    decimal AverageCost
);
