namespace Corelio.BlazorApp.Models.Authentication;

/// <summary>
/// Response model for successful login.
/// </summary>
public record LoginResponse
{
    /// <summary>
    /// JWT access token (short-lived, typically 15 minutes).
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Refresh token (long-lived, typically 7 days).
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// User information.
    /// </summary>
    public UserInfo User { get; init; } = new();

    /// <summary>
    /// Token expiration time in seconds.
    /// </summary>
    public int ExpiresIn { get; init; }

    /// <summary>
    /// Token type (typically "Bearer").
    /// </summary>
    public string TokenType { get; init; } = "Bearer";
}
