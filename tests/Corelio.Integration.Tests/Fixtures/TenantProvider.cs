using Corelio.Domain.Common.Interfaces;

namespace Corelio.Integration.Tests.Fixtures;

/// <summary>
/// Simple tenant provider implementation for integration tests.
/// Provides a fixed tenant ID without requiring HTTP context.
/// </summary>
public class TenantProvider(Guid tenantId) : ITenantProvider
{
    public Guid TenantId { get; } = tenantId;

    public bool HasTenantContext => TenantId != Guid.Empty;
}
