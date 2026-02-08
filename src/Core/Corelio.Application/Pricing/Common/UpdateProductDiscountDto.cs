namespace Corelio.Application.Pricing.Common;

/// <summary>
/// Input DTO for updating a product's discount on a specific tier.
/// </summary>
public record UpdateProductDiscountDto(
    int TierNumber,
    decimal DiscountPercentage);
