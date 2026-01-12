namespace Corelio.Domain.Common.Interfaces;

/// <summary>
/// Provides tenant context for the current request.
/// Used by DbContext for query filters and interceptors.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Gets the current tenant ID.
    /// </summary>
    Guid TenantId { get; }

    /// <summary>
    /// Gets whether there is a valid tenant context.
    /// </summary>
    bool HasTenantContext { get; }
}
