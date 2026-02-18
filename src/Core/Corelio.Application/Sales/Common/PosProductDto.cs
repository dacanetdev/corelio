using Corelio.Domain.Enums;

namespace Corelio.Application.Sales.Common;

/// <summary>
/// Product data returned by POS search (includes stock level).
/// </summary>
public record PosProductDto(
    Guid Id,
    string Sku,
    string Name,
    string? Barcode,
    decimal SalePrice,
    decimal Stock,
    UnitOfMeasure UnitOfMeasure,
    bool IvaEnabled,
    decimal TaxRate);
