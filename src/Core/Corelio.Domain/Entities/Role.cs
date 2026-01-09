using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a role that groups permissions together.
/// </summary>
public class Role : AuditableEntity
{
    /// <summary>
    /// The tenant this role belongs to (null for system roles).
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// The name of the role.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the role.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether this is a system-defined role.
    /// </summary>
    public bool IsSystemRole { get; set; } = false;

    /// <summary>
    /// Whether this role is assigned by default to new users.
    /// </summary>
    public bool IsDefault { get; set; } = false;

    // Navigation properties
    /// <summary>
    /// The tenant this role belongs to (null for system roles).
    /// </summary>
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// Users assigned this role.
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = [];

    /// <summary>
    /// Permissions granted to this role.
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}

/// <summary>
/// Well-known system role identifiers.
/// </summary>
public static class SystemRoles
{
    public static readonly Guid Owner = new("00000000-0000-0000-0000-000000000001");
    public static readonly Guid Administrator = new("00000000-0000-0000-0000-000000000002");
    public static readonly Guid Cashier = new("00000000-0000-0000-0000-000000000003");
    public static readonly Guid InventoryManager = new("00000000-0000-0000-0000-000000000004");
    public static readonly Guid Accountant = new("00000000-0000-0000-0000-000000000005");
    public static readonly Guid Seller = new("00000000-0000-0000-0000-000000000006");
}
