using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Commands.UpdateTenantPricingConfig;

/// <summary>
/// Command to create or update a tenant's pricing configuration.
/// </summary>
public record UpdateTenantPricingConfigCommand(
    int DiscountTierCount,
    int MarginTierCount,
    bool DefaultIvaEnabled,
    decimal IvaPercentage,
    List<DiscountTierDto> DiscountTiers,
    List<MarginTierDto> MarginTiers) : IRequest<Result<TenantPricingConfigDto>>;
