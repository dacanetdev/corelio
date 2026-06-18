namespace Corelio.Application.Roles.Common;

/// <summary>
/// DTO for a role with its assigned permissions.
/// </summary>
public record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystemRole,
    bool IsDefault,
    PermissionSummaryDto[] Permissions);

/// <summary>
/// Summary DTO for a permission assigned to a role.
/// </summary>
public record PermissionSummaryDto(
    string Code,
    string Name,
    string Module,
    string? Category,
    bool IsDangerous);
