namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Blazor model for a sample price at a specific margin percentage.
/// </summary>
public record SamplePriceModel(
    decimal MarginPercentage,
    decimal SalePrice,
    decimal PriceWithIva);
