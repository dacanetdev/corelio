using Corelio.Domain.Enums;

namespace Corelio.Application.Products.Common;

/// <summary>
/// Lightweight DTO for product list views.
/// </summary>
public record ProductListDto(
    Guid Id,
    string Sku,
    string Name,
    decimal SalePrice,
    decimal CostPrice,
    UnitOfMeasure UnitOfMeasure,
    string? CategoryName,
    string? Barcode,
    string? Brand,
    bool IsActive,
    bool IsFeatured,
    decimal ProfitMargin,
    string? PrimaryImageUrl);
