using Corelio.Domain.Entities;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Service for managing tenant context and resolution in a multi-tenant application.
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// Gets the current tenant ID from the execution context.
    /// </summary>
    /// <returns>The current tenant ID, or null if no tenant context is available.</returns>
    Guid? GetCurrentTenantId();

    /// <summary>
    /// Sets the current tenant ID for the execution context.
    /// </summary>
    /// <param name="tenantId">The tenant ID to set.</param>
    /// <remarks>
    /// This should be used with caution and primarily for testing or administrative operations.
    /// In production, the tenant ID should be resolved from the HTTP request.
    /// </remarks>
    void SetCurrentTenantId(Guid? tenantId);

    /// <summary>
    /// Gets the current tenant entity with full details.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current tenant entity, or null if no tenant context is available.</returns>
    Task<Tenant?> GetCurrentTenantAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tenant exists by ID.
    /// </summary>
    /// <param name="tenantId">The tenant ID to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the tenant exists, false otherwise.</returns>
    Task<bool> TenantExistsAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a tenant by subdomain.
    /// </summary>
    /// <param name="subdomain">The subdomain to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tenant entity, or null if not found.</returns>
    Task<Tenant?> GetTenantBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with tenant filters bypassed.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <remarks>
    /// Use with EXTREME CAUTION. This bypasses multi-tenancy isolation.
    /// Should only be used for system-level operations.
    /// </remarks>
    Task<T> BypassTenantFilterAsync<T>(Func<Task<T>> operation);

    /// <summary>
    /// Invalidates the cached tenant data for the specified tenant ID.
    /// </summary>
    /// <param name="tenantId">The tenant ID whose cache should be invalidated.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task InvalidateTenantCacheAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
