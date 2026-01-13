using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Role aggregate.
/// </summary>
public class RoleRepository(ApplicationDbContext context) : IRoleRepository
{
    public async Task<List<Role>> GetByCodesAsync(IEnumerable<string> roleCodes, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        var query = context.Roles
            .Where(r => roleCodes.Contains(r.Name));

        if (tenantId.HasValue)
        {
            // Include both tenant-specific roles and system roles (TenantId == null)
            query = query.Where(r => r.TenantId == tenantId.Value || r.TenantId == null);
        }
        else
        {
            // Only system roles
            query = query.Where(r => r.TenantId == null);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await context.Roles.FindAsync([roleId], cancellationToken);
    }
}
