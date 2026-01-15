namespace Corelio.BlazorApp.Models.Authentication;

/// <summary>
/// Request model for user registration (admin only).
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Password confirmation.
    /// </summary>
    public string ConfirmPassword { get; init; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Roles to assign to the user.
    /// </summary>
    public List<string> Roles { get; init; } = [];
}
