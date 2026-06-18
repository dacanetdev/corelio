namespace Corelio.Application.Users.Common;

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? Phone,
    string? Mobile,
    string? Position,
    string? EmployeeCode,
    DateOnly? HireDate,
    bool IsActive,
    string[] Roles,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
