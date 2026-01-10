using Corelio.Domain.Common.Interfaces;

namespace Corelio.Infrastructure.MultiTenancy;

/// <summary>
/// Provides tenant context for the current request scope.
/// </summary>
public class TenantProvider : ITenantProvider
{
    private Guid _tenantId;
    private bool _hasTenantContext;

    /// <inheritdoc />
    public Guid TenantId => _tenantId;

    /// <inheritdoc />
    public bool HasTenantContext => _hasTenantContext;

    /// <summary>
    /// Sets the tenant context for the current scope.
    /// Called by middleware or service after resolving tenant.
    /// </summary>
    public void SetTenant(Guid tenantId)
    {
        _tenantId = tenantId;
        _hasTenantContext = true;
    }

    /// <summary>
    /// Clears the tenant context.
    /// </summary>
    public void ClearTenant()
    {
        _tenantId = Guid.Empty;
        _hasTenantContext = false;
    }
}
