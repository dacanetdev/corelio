namespace Corelio.BlazorApp.Models.Authentication;

/// <summary>
/// Response model for token refresh.
/// </summary>
public record TokenResponse
{
    /// <summary>
    /// New JWT access token.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// New refresh token.
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// Token expiration time in seconds.
    /// </summary>
    public int ExpiresIn { get; init; }

    /// <summary>
    /// Token type (typically "Bearer").
    /// </summary>
    public string TokenType { get; init; } = "Bearer";
}
