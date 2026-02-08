namespace Corelio.Application.Pricing.Common;

/// <summary>
/// DTO representing a discount tier definition.
/// </summary>
public record DiscountTierDto(
    int TierNumber,
    string TierName,
    bool IsActive);
