using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for Tenant aggregate.
/// </summary>
public interface ITenantRepository
{
    /// <summary>
    /// Gets a tenant by subdomain.
    /// </summary>
    /// <param name="subdomain">The tenant's subdomain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tenant if found, otherwise null.</returns>
    Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a tenant by ID.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tenant if found, otherwise null.</returns>
    Task<Tenant?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tenant with the given subdomain exists.
    /// </summary>
    /// <param name="subdomain">The subdomain to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the subdomain exists, otherwise false.</returns>
    Task<bool> ExistsBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new tenant to the repository.
    /// </summary>
    /// <param name="tenant">The tenant to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing tenant.
    /// </summary>
    /// <param name="tenant">The tenant to update.</param>
    void Update(Tenant tenant);
}
