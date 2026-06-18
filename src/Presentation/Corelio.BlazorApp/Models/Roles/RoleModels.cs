namespace Corelio.BlazorApp.Models.Roles;

public class RoleModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public bool IsDefault { get; set; }
    public PermissionModel[] Permissions { get; set; } = [];
}

public class PermissionModel
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? Category { get; set; }
    public bool IsDangerous { get; set; }
}
