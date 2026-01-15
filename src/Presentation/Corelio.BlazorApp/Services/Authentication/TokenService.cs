using Blazored.LocalStorage;

namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// Implementation of token storage service using Blazored.LocalStorage.
/// </summary>
public class TokenService(ILocalStorageService localStorage) : ITokenService
{
    private const string AccessTokenKey = "corelio_access_token";
    private const string RefreshTokenKey = "corelio_refresh_token";

    /// <inheritdoc />
    public async Task SetAccessTokenAsync(string token)
    {
        await localStorage.SetItemAsStringAsync(AccessTokenKey, token);
    }

    /// <inheritdoc />
    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            return await localStorage.GetItemAsStringAsync(AccessTokenKey);
        }
        catch
        {
            // Token doesn't exist or is corrupted
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SetRefreshTokenAsync(string token)
    {
        await localStorage.SetItemAsStringAsync(RefreshTokenKey, token);
    }

    /// <inheritdoc />
    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            return await localStorage.GetItemAsStringAsync(RefreshTokenKey);
        }
        catch
        {
            // Token doesn't exist or is corrupted
            return null;
        }
    }

    /// <inheritdoc />
    public async Task ClearTokensAsync()
    {
        await localStorage.RemoveItemAsync(AccessTokenKey);
        await localStorage.RemoveItemAsync(RefreshTokenKey);
    }

    /// <inheritdoc />
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetAccessTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    /// <inheritdoc />
    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        await SetAccessTokenAsync(accessToken);
        await SetRefreshTokenAsync(refreshToken);
    }
}
