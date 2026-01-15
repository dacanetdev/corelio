namespace Corelio.BlazorApp.Models.Authentication;

/// <summary>
/// Request model for user login.
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Optional tenant identifier (subdomain or tenant ID).
    /// </summary>
    public string? TenantIdentifier { get; init; }
}
