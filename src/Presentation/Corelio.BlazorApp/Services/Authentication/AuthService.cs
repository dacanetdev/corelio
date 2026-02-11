using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Corelio.BlazorApp.Models.Authentication;
using Corelio.BlazorApp.Models.Common;
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
            Console.WriteLine($"[AuthService] API Base URL: {_httpClient.BaseAddress}");

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", request);
            Console.WriteLine($"[AuthService] Response status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[AuthService] Response content: {content[..Math.Min(200, content.Length)]}...");

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse is not null)
                {
                    Console.WriteLine($"[AuthService] Parsed response - UserId: {loginResponse.UserId}, AccessToken length: {loginResponse.AccessToken?.Length ?? 0}");

                    // Store tokens in localStorage
                    Console.WriteLine("[AuthService] Storing tokens...");
                    await _tokenService.SetTokensAsync(loginResponse.AccessToken ?? "", loginResponse.RefreshToken ?? "");
                    Console.WriteLine("[AuthService] Tokens stored successfully");

                    // Notify authentication state changed
                    Console.WriteLine("[AuthService] Notifying auth state change...");
                    if (_authenticationStateProvider is CustomAuthenticationStateProvider customProvider)
                    {
                        customProvider.NotifyAuthenticationStateChanged();
                    }
                    Console.WriteLine("[AuthService] Auth state notified, returning success");

                    return Result<LoginResponse>.Success(loginResponse);
                }

                Console.WriteLine("[AuthService] Failed to parse login response");
                return Result<LoginResponse>.Failure("Failed to parse login response");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[AuthService] Login failed: {errorContent}");
            var errorMessage = ParseErrorMessage(errorContent);
            return Result<LoginResponse>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] Exception: {ex}");
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

    /// <summary>
    /// Parses a ProblemDetails JSON response to extract the error title code.
    /// Falls back to a generic error key if parsing fails.
    /// </summary>
    private static string ParseErrorMessage(string responseContent)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseContent);
            if (doc.RootElement.TryGetProperty("title", out var titleElement))
            {
                return titleElement.GetString() ?? "LoginFailed";
            }
        }
        catch (JsonException)
        {
            // Not JSON - return as-is if it looks like a simple message
        }

        return "LoginFailed";
    }
}
