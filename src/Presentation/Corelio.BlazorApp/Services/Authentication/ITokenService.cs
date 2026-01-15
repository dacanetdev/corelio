namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// Service for managing JWT access tokens and refresh tokens in browser localStorage.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Stores the access token in localStorage.
    /// </summary>
    Task SetAccessTokenAsync(string token);

    /// <summary>
    /// Retrieves the access token from localStorage.
    /// </summary>
    Task<string?> GetAccessTokenAsync();

    /// <summary>
    /// Stores the refresh token in localStorage.
    /// </summary>
    Task SetRefreshTokenAsync(string token);

    /// <summary>
    /// Retrieves the refresh token from localStorage.
    /// </summary>
    Task<string?> GetRefreshTokenAsync();

    /// <summary>
    /// Clears all authentication tokens from localStorage.
    /// </summary>
    Task ClearTokensAsync();

    /// <summary>
    /// Checks if the user has a valid access token.
    /// </summary>
    Task<bool> IsAuthenticatedAsync();

    /// <summary>
    /// Stores both access and refresh tokens.
    /// </summary>
    Task SetTokensAsync(string accessToken, string refreshToken);
}
