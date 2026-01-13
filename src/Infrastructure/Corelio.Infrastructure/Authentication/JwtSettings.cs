namespace Corelio.Infrastructure.Authentication;

/// <summary>
/// Configuration settings for JWT token generation and validation.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// The secret key used for signing JWT tokens (256-bit minimum recommended).
    /// </summary>
    public string Secret { get; init; } = string.Empty;

    /// <summary>
    /// The issuer of the JWT tokens (typically the application name).
    /// </summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// The audience for the JWT tokens (typically the client application).
    /// </summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// The expiry time for access tokens in minutes (default: 60 minutes).
    /// </summary>
    public int ExpiryMinutes { get; init; } = 60;
}
