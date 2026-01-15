namespace Corelio.BlazorApp.Models.Authentication;

/// <summary>
/// Request model for resetting password with token.
/// </summary>
public record ResetPasswordRequest
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Password reset token from email link.
    /// </summary>
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// New password.
    /// </summary>
    public string NewPassword { get; init; } = string.Empty;

    /// <summary>
    /// New password confirmation.
    /// </summary>
    public string ConfirmPassword { get; init; } = string.Empty;
}
