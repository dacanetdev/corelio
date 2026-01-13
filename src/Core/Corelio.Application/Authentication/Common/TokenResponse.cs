namespace Corelio.Application.Authentication.Common;

/// <summary>
/// Represents the tokens returned after successful authentication or refresh.
/// </summary>
/// <param name="AccessToken">The JWT access token (1 hour expiry).</param>
/// <param name="RefreshToken">The refresh token (7 days expiry).</param>
/// <param name="ExpiresAt">The UTC date/time when the access token expires.</param>
public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
