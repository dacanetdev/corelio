namespace Corelio.Application.Users.Common;

public record UserListDto(
    Guid Id,
    string FullName,
    string Email,
    string? Position,
    bool IsActive,
    string[] Roles,
    DateTime? LastLoginAt,
    DateTime CreatedAt);
