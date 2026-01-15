namespace Corelio.BlazorApp.Models.Authentication;

/// <summary>
/// Request model for forgot password flow.
/// </summary>
public record ForgotPasswordRequest
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;
}
