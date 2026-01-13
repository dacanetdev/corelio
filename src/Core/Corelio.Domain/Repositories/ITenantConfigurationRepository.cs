using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for TenantConfiguration aggregate.
/// </summary>
public interface ITenantConfigurationRepository
{
    /// <summary>
    /// Adds a new tenant configuration to the repository.
    /// </summary>
    /// <param name="configuration">The tenant configuration to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(TenantConfiguration configuration, CancellationToken cancellationToken = default);
}
