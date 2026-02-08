namespace Corelio.Application.Pricing.Common;

/// <summary>
/// DTO representing a product's margin price for a specific tier.
/// </summary>
public record ProductMarginPriceDto(
    int TierNumber,
    string TierName,
    decimal? MarginPercentage,
    decimal? SalePrice,
    decimal? PriceWithIva);
