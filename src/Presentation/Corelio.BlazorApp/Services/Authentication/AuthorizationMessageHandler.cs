using System.Net.Http.Headers;

namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// HTTP message handler that automatically attaches JWT access tokens to outgoing API requests.
/// Also handles automatic token refresh when access token expires.
/// </summary>
public partial class AuthorizationMessageHandler(
    IServiceProvider serviceProvider,
    ILogger<AuthorizationMessageHandler> logger) : DelegatingHandler
{
    /// <summary>
    /// Intercepts HTTP requests to add Authorization header with JWT token.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LogInterceptingRequest(logger, request.RequestUri?.ToString() ?? "unknown");

        // CRITICAL: Resolve TokenService from current scope (not constructor) to ensure we get the right circuit instance
        var tokenService = serviceProvider.GetRequiredService<ITokenService>();

        try
        {
            // Get access token from session storage
            var accessToken = await tokenService.GetAccessTokenAsync();

            LogRetrievedToken(logger, !string.IsNullOrWhiteSpace(accessToken), accessToken?.Length ?? 0);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                accessToken = accessToken.Replace("Bearer ", string.Empty).Trim();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                LogAuthorizationHeaderSet(logger);
            }
            else
            {
                LogNoAccessToken(logger);
            }
        }
        catch (InvalidOperationException ex)
        {
            // JS interop not available during prerendering — send without token
            LogJsInteropNotAvailable(logger, ex);
        }
        catch (Exception ex)
        {
            LogUnexpectedError(logger, ex);
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // If 401 Unauthorized, attempt token refresh
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            try
            {
                var refreshToken = await tokenService.GetRefreshTokenAsync();

                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    var authService = serviceProvider.GetService<IAuthService>();
                    if (authService is not null)
                    {
                        var refreshResult = await authService.RefreshTokenAsync(refreshToken);

                        if (refreshResult.IsSuccess && refreshResult.Value is not null)
                        {
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.Value.AccessToken);
                            response = await base.SendAsync(request, cancellationToken);
                        }
                        else
                        {
                            await tokenService.ClearTokensAsync();
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // JS interop not available — can't refresh during prerender
            }
        }

        return response;
    }

    // High-performance logging via LoggerMessage source generator

    [LoggerMessage(Level = LogLevel.Information, Message = "[AuthMessageHandler] Intercepting request to {Url}")]
    private static partial void LogInterceptingRequest(ILogger logger, string url);

    [LoggerMessage(Level = LogLevel.Information, Message = "[AuthMessageHandler] Retrieved token: HasToken={HasToken}, Length={Length}")]
    private static partial void LogRetrievedToken(ILogger logger, bool hasToken, int length);

    [LoggerMessage(Level = LogLevel.Information, Message = "[AuthMessageHandler] Authorization header set")]
    private static partial void LogAuthorizationHeaderSet(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "[AuthMessageHandler] No access token available")]
    private static partial void LogNoAccessToken(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "[AuthMessageHandler] JS interop not available (prerendering?)")]
    private static partial void LogJsInteropNotAvailable(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "[AuthMessageHandler] Unexpected error getting token")]
    private static partial void LogUnexpectedError(ILogger logger, Exception ex);
}
