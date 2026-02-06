using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TenantPricingConfiguration aggregate.
/// </summary>
public class TenantPricingConfigurationRepository(ApplicationDbContext context) : ITenantPricingConfigurationRepository
{
    public async Task<TenantPricingConfiguration?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await context.TenantPricingConfigurations
            .FirstOrDefaultAsync(tpc => tpc.TenantId == tenantId, cancellationToken);
    }

    public async Task<TenantPricingConfiguration?> GetWithTierDefinitionsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await context.TenantPricingConfigurations
            .Include(tpc => tpc.DiscountTierDefinitions.OrderBy(d => d.TierNumber))
            .Include(tpc => tpc.MarginTierDefinitions.OrderBy(m => m.TierNumber))
            .FirstOrDefaultAsync(tpc => tpc.TenantId == tenantId, cancellationToken);
    }

    public void Add(TenantPricingConfiguration configuration)
    {
        context.TenantPricingConfigurations.Add(configuration);
    }

    public void Update(TenantPricingConfiguration configuration)
    {
        context.TenantPricingConfigurations.Update(configuration);
    }
}
