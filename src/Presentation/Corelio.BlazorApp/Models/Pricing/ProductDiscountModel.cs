namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Blazor model for a product's discount on a specific tier.
/// </summary>
public record ProductDiscountModel(
    int TierNumber,
    string TierName,
    decimal DiscountPercentage);
