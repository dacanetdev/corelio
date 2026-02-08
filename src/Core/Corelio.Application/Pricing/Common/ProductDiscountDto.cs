namespace Corelio.Application.Pricing.Common;

/// <summary>
/// DTO representing a product's discount for a specific tier.
/// </summary>
public record ProductDiscountDto(
    int TierNumber,
    string TierName,
    decimal DiscountPercentage);
