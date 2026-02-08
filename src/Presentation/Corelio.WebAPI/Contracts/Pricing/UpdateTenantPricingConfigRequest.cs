using Corelio.Application.Pricing.Common;

namespace Corelio.WebAPI.Contracts.Pricing;

/// <summary>
/// Request DTO for creating or updating a tenant's pricing configuration.
/// </summary>
public record UpdateTenantPricingConfigRequest(
    int DiscountTierCount,
    int MarginTierCount,
    bool DefaultIvaEnabled,
    decimal IvaPercentage,
    List<DiscountTierDto> DiscountTiers,
    List<MarginTierDto> MarginTiers);
