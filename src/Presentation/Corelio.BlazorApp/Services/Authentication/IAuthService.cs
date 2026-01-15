using Corelio.BlazorApp.Models.Authentication;
using Corelio.BlazorApp.Models.Common;

namespace Corelio.BlazorApp.Services.Authentication;

/// <summary>
/// Service for authentication operations (login, register, logout, password reset).
/// Makes HTTP API calls to the backend authentication endpoints.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// Registers a new user (admin only, requires users.create permission).
    /// </summary>
    Task<Result<Guid>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Logs out the current user by invalidating refresh tokens.
    /// </summary>
    Task<Result> LogoutAsync();

    /// <summary>
    /// Refreshes the access token using the refresh token.
    /// </summary>
    Task<Result<TokenResponse>> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Sends a password reset email to the specified address.
    /// </summary>
    Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request);

    /// <summary>
    /// Resets the password using a reset token.
    /// </summary>
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);

    /// <summary>
    /// Retrieves the current user's information from JWT claims.
    /// </summary>
    Task<UserInfo?> GetCurrentUserAsync();
}
