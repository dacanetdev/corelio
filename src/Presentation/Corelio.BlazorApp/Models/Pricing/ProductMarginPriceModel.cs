namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Blazor model for a product's margin price on a specific tier.
/// </summary>
public record ProductMarginPriceModel(
    int TierNumber,
    string TierName,
    decimal? MarginPercentage,
    decimal? SalePrice,
    decimal? PriceWithIva);
