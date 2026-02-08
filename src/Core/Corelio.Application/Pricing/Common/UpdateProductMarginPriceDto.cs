namespace Corelio.Application.Pricing.Common;

/// <summary>
/// Input DTO for updating a product's margin price on a specific tier.
/// Provide either MarginPercentage or SalePrice (the other will be calculated).
/// </summary>
public record UpdateProductMarginPriceDto(
    int TierNumber,
    decimal? MarginPercentage,
    decimal? SalePrice);
