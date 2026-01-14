using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// HTTP message handler that automatically attaches JWT access tokens to outgoing API requests.
/// Also handles automatic token refresh when access token expires.
/// </summary>
public class AuthorizationMessageHandler(
    ITokenService tokenService,
    IAuthService authService,
    NavigationManager navigationManager) : DelegatingHandler
{
    /// <summary>
    /// Intercepts HTTP requests to add Authorization header with JWT token.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Get access token from localStorage
        var accessToken = await tokenService.GetAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            // Remove "Bearer " prefix if present
            accessToken = accessToken.Replace("Bearer ", string.Empty).Trim();

            // Attach token to Authorization header
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // If 401 Unauthorized, attempt token refresh
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshToken = await tokenService.GetRefreshTokenAsync();

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                // Attempt to refresh the access token
                var refreshResult = await authService.RefreshTokenAsync(refreshToken);

                if (refreshResult.IsSuccess && refreshResult.Value is not null)
                {
                    // Retry the original request with new token
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.Value.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
                else
                {
                    // Refresh failed, redirect to login
                    await tokenService.ClearTokensAsync();
                    navigationManager.NavigateTo("/auth/login?expired=true", forceLoad: true);
                }
            }
            else
            {
                // No refresh token, redirect to login
                navigationManager.NavigateTo("/auth/login", forceLoad: true);
            }
        }

        return response;
    }
}
