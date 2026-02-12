using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// Custom authentication state provider that parses JWT tokens.
/// Handles Blazor Server prerendering gracefully by returning anonymous
/// when JS interop is unavailable, then re-evaluating once the circuit is live.
/// </summary>
public class CustomAuthenticationStateProvider(ITokenService tokenService) : AuthenticationStateProvider
{
    private static readonly JwtSecurityTokenHandler JwtHandler = new();
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    /// <summary>
    /// Gets the current authentication state by reading the JWT token from session storage.
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await tokenService.GetAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                return Anonymous;
            }

            // Remove "Bearer " prefix if present
            token = token.Replace("Bearer ", string.Empty).Trim();

            // Parse and validate the JWT
            var jwtToken = JwtHandler.ReadJwtToken(token);

            // Check expiration
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                await tokenService.ClearTokensAsync();
                return Anonymous;
            }

            // Build claims identity
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch (InvalidOperationException)
        {
            // JS interop not available during prerendering — return anonymous.
            // Blazor will re-evaluate once the circuit is established.
            return Anonymous;
        }
        catch
        {
            // Token is invalid or corrupted
            try { await tokenService.ClearTokensAsync(); } catch { /* prerender */ }
            return Anonymous;
        }
    }

    /// <summary>
    /// Call after login to immediately update all AuthorizeView components.
    /// </summary>
    public void NotifyUserAuthenticated()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Call after logout to immediately update all AuthorizeView components.
    /// </summary>
    public void NotifyUserLoggedOut()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}
