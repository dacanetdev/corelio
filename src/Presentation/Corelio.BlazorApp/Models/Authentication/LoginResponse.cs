namespace Corelio.BlazorApp.Models.Authentication;

/// <summary>
/// Response model for successful login (matches API response structure).
/// </summary>
public record LoginResponse
{
    /// <summary>
    /// User ID.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// User email.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Tenant ID.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// User roles.
    /// </summary>
    public List<string> Roles { get; init; } = [];

    /// <summary>
    /// User permissions.
    /// </summary>
    public List<string> Permissions { get; init; } = [];

    /// <summary>
    /// Token information.
    /// </summary>
    public TokenInfo Tokens { get; init; } = new();

    // Convenience properties to access tokens directly
    public string AccessToken => Tokens?.AccessToken ?? string.Empty;
    public string RefreshToken => Tokens?.RefreshToken ?? string.Empty;
}

/// <summary>
/// Token information from login response.
/// </summary>
public record TokenInfo
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
