using Corelio.Domain.Common.Interfaces;

namespace Corelio.Domain.Common;

/// <summary>
/// Base entity with full audit trail (created/updated timestamps and users).
/// Use this for entities that need to track who made changes.
/// </summary>
public abstract class AuditableEntity : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// The date and time when the entity was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The ID of the user who created this entity.
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// The ID of the user who last updated this entity.
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    protected AuditableEntity() : base()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
