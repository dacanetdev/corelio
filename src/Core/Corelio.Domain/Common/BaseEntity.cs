namespace Corelio.Domain.Common;

/// <summary>
/// Base entity with identity and creation tracking.
/// All domain entities should inherit from this class.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
