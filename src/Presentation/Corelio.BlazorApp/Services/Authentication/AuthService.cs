using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Corelio.BlazorApp.Models.Authentication;
using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Services.Http;
using Microsoft.AspNetCore.Components.Authorization;

namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// Implementation of authentication service using HttpClient to call backend API.
/// Uses the "api-auth" named client to avoid circular dependency with AuthorizationMessageHandler.
/// </summary>
public class AuthService : IAuthService
{
    private const string BaseUrl = "/api/v1/auth";
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClientFactory.CreateClient("api-auth");
        _tokenService = tokenService;
        _authenticationStateProvider = authenticationStateProvider;
    }

    /// <inheritdoc />
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            Console.WriteLine($"[AuthService] Attempting login for: {request.Email}");

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", request);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse is not null)
                {
                    // Store tokens in session storage
                    await _tokenService.SetTokensAsync(loginResponse.AccessToken, loginResponse.RefreshToken);

                    // Notify authentication state changed
                    if (_authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
                    {
                        customProvider.NotifyUserAuthenticated();
                    }

                    return Result<LoginResponse>.Success(loginResponse);
                }

                return Result<LoginResponse>.Failure("Failed to parse login response");
            }

            var errorMessage = await response.GetErrorMessageAsync();
            return Result<LoginResponse>.Failure(errorMessage);
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
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

            if (response.IsSuccessStatusCode)
            {
                var userId = await response.Content.ReadFromJsonAsync<Guid>();
                return Result<Guid>.Success(userId);
            }

            var errorMessage = await response.GetErrorMessageAsync();
            return Result<Guid>.Failure(errorMessage);
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
            var refreshToken = await _tokenService.GetRefreshTokenAsync();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Call backend to invalidate refresh token
                await _httpClient.PostAsJsonAsync($"{BaseUrl}/logout", new { RefreshToken = refreshToken });
            }

            // Clear tokens from localStorage
            await _tokenService.ClearTokensAsync();

            // Notify authentication state changed
            if (_authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
            {
                customProvider.NotifyUserLoggedOut();
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
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/refresh", new { RefreshToken = refreshToken });

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (tokenResponse is not null)
                {
                    // Update tokens in localStorage
                    await _tokenService.SetTokensAsync(tokenResponse.AccessToken, tokenResponse.RefreshToken);

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
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/forgot-password", request);

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
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/reset-password", request);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            var errorMessage = await response.GetErrorMessageAsync();
            return Result.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Password reset error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
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
