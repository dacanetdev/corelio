namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// Implementation of token storage service using in-memory storage scoped to the Blazor circuit.
/// Tokens are stored only in memory and will be lost on page refresh (by design for security).
/// </summary>
public partial class TokenService(ILogger<TokenService> logger) : ITokenService
{
    // In-memory cache for the current circuit — tokens persist only for the circuit lifetime
    private string? _accessToken;
    private string? _refreshToken;

    /// <inheritdoc />
    public Task SetAccessTokenAsync(string token)
    {
        ArgumentNullException.ThrowIfNull(token);

        LogSettingAccessToken(logger, token.Length);
        _accessToken = token;
        LogTokenCached(logger);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetAccessTokenAsync()
    {
        var hasToken = !string.IsNullOrWhiteSpace(_accessToken);
        LogGetAccessTokenCalled(logger, hasToken, _accessToken?.Length ?? 0);

        if (hasToken)
        {
            LogReturningCachedToken(logger, _accessToken!.Length);
        }
        else
        {
            LogNoTokenAvailable(logger);
        }

        return Task.FromResult(_accessToken);
    }

    /// <inheritdoc />
    public Task SetRefreshTokenAsync(string token)
    {
        ArgumentNullException.ThrowIfNull(token);
        _refreshToken = token;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetRefreshTokenAsync()
    {
        return Task.FromResult(_refreshToken);
    }

    /// <inheritdoc />
    public Task ClearTokensAsync()
    {
        LogClearingTokens(logger);
        _accessToken = null;
        _refreshToken = null;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> IsAuthenticatedAsync()
    {
        var isAuthenticated = !string.IsNullOrWhiteSpace(_accessToken);
        return Task.FromResult(isAuthenticated);
    }

    /// <inheritdoc />
    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        await SetAccessTokenAsync(accessToken);
        await SetRefreshTokenAsync(refreshToken);
    }

    // High-performance logging via LoggerMessage source generator

    [LoggerMessage(Level = LogLevel.Information, Message = "[TokenService] Setting access token, Length: {Length}")]
    private static partial void LogSettingAccessToken(ILogger logger, int length);

    [LoggerMessage(Level = LogLevel.Information, Message = "[TokenService] Token cached in memory")]
    private static partial void LogTokenCached(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "[TokenService] GetAccessToken called, HasToken: {HasToken}, Length: {Length}")]
    private static partial void LogGetAccessTokenCalled(ILogger logger, bool hasToken, int length);

    [LoggerMessage(Level = LogLevel.Information, Message = "[TokenService] Returning cached token, Length: {Length}")]
    private static partial void LogReturningCachedToken(ILogger logger, int length);

    [LoggerMessage(Level = LogLevel.Warning, Message = "[TokenService] No token available in memory")]
    private static partial void LogNoTokenAvailable(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "[TokenService] Clearing all tokens from memory")]
    private static partial void LogClearingTokens(ILogger logger);
}
