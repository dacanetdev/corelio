using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TenantConfiguration aggregate.
/// </summary>
public class TenantConfigurationRepository(ApplicationDbContext context) : ITenantConfigurationRepository
{
    public async Task<TenantConfiguration?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await context.TenantConfigurations
            .FirstOrDefaultAsync(tc => tc.TenantId == tenantId, cancellationToken);
    }

    public async Task AddAsync(TenantConfiguration configuration, CancellationToken cancellationToken = default)
    {
        await context.TenantConfigurations.AddAsync(configuration, cancellationToken);
    }

    public void Update(TenantConfiguration configuration)
    {
        context.TenantConfigurations.Update(configuration);
    }
}
