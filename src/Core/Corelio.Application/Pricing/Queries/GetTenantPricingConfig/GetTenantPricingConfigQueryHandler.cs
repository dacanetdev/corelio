using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Queries.GetTenantPricingConfig;

/// <summary>
/// Handler for retrieving the current tenant's pricing configuration with tier definitions.
/// </summary>
public class GetTenantPricingConfigQueryHandler(
    ITenantPricingConfigurationRepository configRepository,
    ITenantService tenantService) : IRequestHandler<GetTenantPricingConfigQuery, Result<TenantPricingConfigDto>>
{
    public async Task<Result<TenantPricingConfigDto>> Handle(
        GetTenantPricingConfigQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<TenantPricingConfigDto>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var config = await configRepository.GetWithTierDefinitionsAsync(tenantId.Value, cancellationToken);
        if (config is null)
        {
            return Result<TenantPricingConfigDto>.Failure(
                new Error("PricingConfig.NotFound", "Pricing configuration not found for this tenant.", ErrorType.NotFound));
        }

        return Result<TenantPricingConfigDto>.Success(PricingMapper.ToConfigDto(config));
    }
}
