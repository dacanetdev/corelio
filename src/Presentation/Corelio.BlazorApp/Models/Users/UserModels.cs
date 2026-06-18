namespace Corelio.BlazorApp.Models.Users;

/// <summary>
/// User list item model.
/// </summary>
public class UserListModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Position { get; set; }
    public bool IsActive { get; set; }
    public string[] Roles { get; set; } = [];
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Full user detail model.
/// </summary>
public class UserModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Position { get; set; }
    public string? EmployeeCode { get; set; }
    public DateOnly? HireDate { get; set; }
    public bool IsActive { get; set; }
    public string[] Roles { get; set; } = [];
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Form model for editing an existing user.
/// </summary>
public class UserFormModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Position { get; set; }
    public string? EmployeeCode { get; set; }
    public DateOnly? HireDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string[] RoleCodes { get; set; } = [];
}

/// <summary>
/// Form model for creating a new user via auth/register.
/// </summary>
public class CreateUserFormModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string[] RoleCodes { get; set; } = [];
}
