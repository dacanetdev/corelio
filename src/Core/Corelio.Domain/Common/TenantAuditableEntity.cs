using Corelio.Domain.Common.Interfaces;

namespace Corelio.Domain.Common;

/// <summary>
/// Base entity for tenant-scoped entities with full audit trail.
/// Most business entities should inherit from this class.
/// </summary>
public abstract class TenantAuditableEntity : AuditableEntity, ITenantEntity
{
    /// <summary>
    /// The tenant identifier that owns this entity.
    /// </summary>
    public Guid TenantId { get; set; }
}
