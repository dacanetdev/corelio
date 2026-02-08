using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Commands.UpdateTenantPricingConfig;

/// <summary>
/// Handler for creating or updating a tenant's pricing configuration.
/// </summary>
public class UpdateTenantPricingConfigCommandHandler(
    ITenantPricingConfigurationRepository configRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<UpdateTenantPricingConfigCommand, Result<TenantPricingConfigDto>>
{
    public async Task<Result<TenantPricingConfigDto>> Handle(
        UpdateTenantPricingConfigCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<TenantPricingConfigDto>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var config = await configRepository.GetWithTierDefinitionsAsync(tenantId.Value, cancellationToken);
        var isNew = config is null;

        if (isNew)
        {
            config = new TenantPricingConfiguration
            {
                TenantId = tenantId.Value
            };
        }

        config!.DiscountTierCount = request.DiscountTierCount;
        config.MarginTierCount = request.MarginTierCount;
        config.DefaultIvaEnabled = request.DefaultIvaEnabled;
        config.IvaPercentage = request.IvaPercentage;

        // Clear existing tier definitions and add new ones
        config.DiscountTierDefinitions.Clear();
        foreach (var tier in request.DiscountTiers)
        {
            config.DiscountTierDefinitions.Add(new DiscountTierDefinition
            {
                TenantId = tenantId.Value,
                TenantPricingConfigurationId = config.Id,
                TierNumber = tier.TierNumber,
                TierName = tier.TierName,
                IsActive = tier.IsActive
            });
        }

        config.MarginTierDefinitions.Clear();
        foreach (var tier in request.MarginTiers)
        {
            config.MarginTierDefinitions.Add(new MarginTierDefinition
            {
                TenantId = tenantId.Value,
                TenantPricingConfigurationId = config.Id,
                TierNumber = tier.TierNumber,
                TierName = tier.TierName,
                IsActive = tier.IsActive
            });
        }

        if (isNew)
        {
            configRepository.Add(config);
        }
        else
        {
            configRepository.Update(config);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TenantPricingConfigDto>.Success(PricingMapper.ToConfigDto(config));
    }
}
