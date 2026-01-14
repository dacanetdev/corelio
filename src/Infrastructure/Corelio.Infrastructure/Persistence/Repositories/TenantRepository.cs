using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Tenant aggregate.
/// </summary>
public class TenantRepository(ApplicationDbContext context) : ITenantRepository
{
    public async Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        return await context.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain, cancellationToken);
    }

    public async Task<Tenant?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await context.Tenants.FindAsync([tenantId], cancellationToken);
    }

    public async Task<bool> ExistsBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        return await context.Tenants
            .AnyAsync(t => t.Subdomain == subdomain, cancellationToken);
    }

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await context.Tenants.AddAsync(tenant, cancellationToken);
    }

    public void Update(Tenant tenant)
    {
        context.Tenants.Update(tenant);
    }
}
