namespace Corelio.BlazorApp.Models.Authentication;

/// <summary>
/// User information returned from authentication.
/// </summary>
public record UserInfo
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// User's full name (FirstName + LastName).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// User's tenant ID.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Tenant name/business name.
    /// </summary>
    public string TenantName { get; init; } = string.Empty;

    /// <summary>
    /// User's roles.
    /// </summary>
    public List<string> Roles { get; init; } = [];

    /// <summary>
    /// User's permissions.
    /// </summary>
    public List<string> Permissions { get; init; } = [];
}
