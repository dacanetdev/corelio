namespace Corelio.Domain.Entities;

/// <summary>
/// Join entity for User-Role many-to-many relationship.
/// </summary>
public class UserRole
{
    /// <summary>
    /// The user ID.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The role ID.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// When the role was assigned.
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Who assigned the role.
    /// </summary>
    public Guid? AssignedBy { get; set; }

    /// <summary>
    /// When the role assignment expires (null for permanent).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Whether the role assignment has expired.
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

    // Navigation properties
    /// <summary>
    /// The user.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The role.
    /// </summary>
    public Role Role { get; set; } = null!;
}
