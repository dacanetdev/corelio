namespace Corelio.Application.Pricing.Common;

/// <summary>
/// DTO representing a tenant's pricing configuration with tier definitions.
/// </summary>
public record TenantPricingConfigDto(
    Guid Id,
    Guid TenantId,
    int DiscountTierCount,
    int MarginTierCount,
    bool DefaultIvaEnabled,
    decimal IvaPercentage,
    List<DiscountTierDto> DiscountTiers,
    List<MarginTierDto> MarginTiers);
