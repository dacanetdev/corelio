namespace Corelio.Domain.Common.Interfaces;

/// <summary>
/// Interface for entities that track audit information (created/updated timestamps and users).
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// The date and time when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time when the entity was last updated.
    /// </summary>
    DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The ID of the user who created this entity.
    /// </summary>
    Guid? CreatedBy { get; set; }

    /// <summary>
    /// The ID of the user who last updated this entity.
    /// </summary>
    Guid? UpdatedBy { get; set; }
}
