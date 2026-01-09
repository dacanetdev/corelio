namespace Corelio.Domain.Common.Interfaces;

/// <summary>
/// Interface for entities that support soft deletion.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Indicates whether the entity has been soft deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// The date and time when the entity was deleted.
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// The ID of the user who deleted this entity.
    /// </summary>
    Guid? DeletedBy { get; set; }
}
