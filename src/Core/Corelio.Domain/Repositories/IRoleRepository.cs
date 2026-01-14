using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for Role aggregate.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Gets roles by their codes within a specific tenant or system roles.
    /// </summary>
    /// <param name="roleCodes">The role codes to find.</param>
    /// <param name="tenantId">The tenant ID (null for system roles).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of roles matching the codes.</returns>
    Task<List<Role>> GetByCodesAsync(IEnumerable<string> roleCodes, Guid? tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by ID.
    /// </summary>
    /// <param name="roleId">The role ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The role if found, otherwise null.</returns>
    Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
}
