using System.Net.Http.Json;
using System.Security.Claims;
using Corelio.BlazorApp.Models.Authentication;
using Corelio.BlazorApp.Models.Common;
using Microsoft.AspNetCore.Components.Authorization;

namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// Implementation of authentication service using HttpClient to call backend API.
/// </summary>
public class AuthService(
    HttpClient httpClient,
    ITokenService tokenService,
    AuthenticationStateProvider authenticationStateProvider) : IAuthService
{
    private const string BaseUrl = "/api/v1/auth";

    /// <inheritdoc />
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/login", request);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse is not null)
                {
                    // Store tokens in localStorage
                    await tokenService.SetTokensAsync(loginResponse.AccessToken, loginResponse.RefreshToken);

                    // Notify authentication state changed
                    if (authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
                    {
                        customProvider.NotifyAuthenticationStateChanged();
                    }

                    return Result<LoginResponse>.Success(loginResponse);
                }
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return Result<LoginResponse>.Failure(errorMessage ?? "Login failed");
        }
        catch (Exception ex)
        {
            return Result<LoginResponse>.Failure($"Login error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

            if (response.IsSuccessStatusCode)
            {
                var userId = await response.Content.ReadFromJsonAsync<Guid>();
                return Result<Guid>.Success(userId);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return Result<Guid>.Failure(errorMessage ?? "Registration failed");
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Registration error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> LogoutAsync()
    {
        try
        {
            var refreshToken = await tokenService.GetRefreshTokenAsync();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Call backend to invalidate refresh token
                await httpClient.PostAsJsonAsync($"{BaseUrl}/logout", new { RefreshToken = refreshToken });
            }

            // Clear tokens from localStorage
            await tokenService.ClearTokensAsync();

            // Notify authentication state changed
            if (authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
            {
                customProvider.NotifyAuthenticationStateChanged();
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Logout error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/refresh", new { RefreshToken = refreshToken });

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (tokenResponse is not null)
                {
                    // Update tokens in localStorage
                    await tokenService.SetTokensAsync(tokenResponse.AccessToken, tokenResponse.RefreshToken);

                    return Result<TokenResponse>.Success(tokenResponse);
                }
            }

            return Result<TokenResponse>.Failure("Token refresh failed");
        }
        catch (Exception ex)
        {
            return Result<TokenResponse>.Failure($"Token refresh error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/forgot-password", request);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            return Result.Failure("Failed to send password reset email");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Forgot password error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/reset-password", request);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return Result.Failure(errorMessage ?? "Password reset failed");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Password reset error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return null;
        }

        return new UserInfo
        {
            Id = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString()),
            Email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            FirstName = user.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
            LastName = user.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
            TenantId = Guid.Parse(user.FindFirstValue("tenant_id") ?? Guid.Empty.ToString()),
            TenantName = user.FindFirstValue("tenant_name") ?? string.Empty,
            Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
            Permissions = user.FindAll("permission").Select(c => c.Value).ToList()
        };
    }
}
