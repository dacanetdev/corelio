using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for TenantPricingConfiguration aggregate.
/// </summary>
public interface ITenantPricingConfigurationRepository
{
    /// <summary>
    /// Gets the pricing configuration for a tenant by tenant ID.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The pricing configuration if found, otherwise null.</returns>
    Task<TenantPricingConfiguration?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the pricing configuration for a tenant including tier definitions.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The pricing configuration with tier definitions if found, otherwise null.</returns>
    Task<TenantPricingConfiguration?> GetWithTierDefinitionsAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new tenant pricing configuration.
    /// </summary>
    /// <param name="configuration">The pricing configuration to add.</param>
    void Add(TenantPricingConfiguration configuration);

    /// <summary>
    /// Updates an existing tenant pricing configuration.
    /// </summary>
    /// <param name="configuration">The pricing configuration to update.</param>
    void Update(TenantPricingConfiguration configuration);
}
