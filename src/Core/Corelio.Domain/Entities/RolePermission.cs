namespace Corelio.Domain.Entities;

/// <summary>
/// Join entity for Role-Permission many-to-many relationship.
/// </summary>
public class RolePermission
{
    /// <summary>
    /// The role ID.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// The permission ID.
    /// </summary>
    public Guid PermissionId { get; set; }

    /// <summary>
    /// When the permission was assigned.
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Who assigned the permission.
    /// </summary>
    public Guid? AssignedBy { get; set; }

    // Navigation properties
    /// <summary>
    /// The role.
    /// </summary>
    public Role Role { get; set; } = null!;

    /// <summary>
    /// The permission.
    /// </summary>
    public Permission Permission { get; set; } = null!;
}
