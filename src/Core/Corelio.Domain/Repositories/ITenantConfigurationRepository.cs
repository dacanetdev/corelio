using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for TenantConfiguration aggregate.
/// </summary>
public interface ITenantConfigurationRepository
{
    /// <summary>
    /// Gets the configuration for the specified tenant.
    /// </summary>
    Task<TenantConfiguration?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new tenant configuration to the repository.
    /// </summary>
    Task AddAsync(TenantConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the configuration as modified so changes are persisted on next SaveChanges.
    /// </summary>
    void Update(TenantConfiguration configuration);
}
