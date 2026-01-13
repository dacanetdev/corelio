using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TenantConfiguration aggregate.
/// </summary>
public class TenantConfigurationRepository(ApplicationDbContext context) : ITenantConfigurationRepository
{
    public async Task AddAsync(TenantConfiguration configuration, CancellationToken cancellationToken = default)
    {
        await context.TenantConfigurations.AddAsync(configuration, cancellationToken);
    }
}
