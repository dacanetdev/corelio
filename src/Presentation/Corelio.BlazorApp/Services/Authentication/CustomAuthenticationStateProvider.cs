using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// Custom authentication state provider that parses JWT tokens from localStorage.
/// Provides authentication state to Blazor components.
/// </summary>
public class CustomAuthenticationStateProvider(ITokenService tokenService) : AuthenticationStateProvider
{
    private readonly JwtSecurityTokenHandler _jwtHandler = new();

    /// <summary>
    /// Gets the current authentication state by parsing the JWT token from localStorage.
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("[AuthStateProvider] GetAuthenticationStateAsync called");
        var token = await tokenService.GetAccessTokenAsync();
        Console.WriteLine($"[AuthStateProvider] Token length: {token?.Length ?? 0}");

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("[AuthStateProvider] No token, returning anonymous");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            // Remove "Bearer " prefix if present
            token = token.Replace("Bearer ", string.Empty).Trim();

            // Parse JWT token to extract claims
            var jwtToken = _jwtHandler.ReadJwtToken(token);

            // Check if token is expired
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                // Token expired, clear it
                await tokenService.ClearTokensAsync();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Create claims identity from JWT claims
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            Console.WriteLine($"[AuthStateProvider] Authenticated as: {user.Identity?.Name ?? "unknown"}");
            return new AuthenticationState(user);
        }
        catch
        {
            // Token is invalid or corrupted
            await tokenService.ClearTokensAsync();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    /// <summary>
    /// Notifies Blazor that the authentication state has changed (after login/logout).
    /// </summary>
    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
