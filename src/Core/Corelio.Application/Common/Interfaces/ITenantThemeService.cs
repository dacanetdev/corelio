namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Service for managing tenant theme customization.
/// </summary>
public interface ITenantThemeService
{
    /// <summary>
    /// Gets the theme configuration for a tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tenant theme configuration, or null if not using custom theme.</returns>
    Task<TenantThemeDto?> GetTenantThemeAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the theme configuration for a tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="primaryColor">The primary color in hex format (e.g., #E74C3C).</param>
    /// <param name="useCustomTheme">Whether to use the custom theme.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    Task<bool> UpdateTenantThemeAsync(
        Guid tenantId,
        string? primaryColor,
        bool useCustomTheme,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the cached theme configuration for a tenant.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task InvalidateThemeCacheAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a string is a valid hex color format.
    /// </summary>
    /// <param name="color">The color string to validate.</param>
    /// <returns>True if the color is a valid hex format (#RRGGBB), false otherwise.</returns>
    bool IsValidHexColor(string? color);
}

/// <summary>
/// Data transfer object for tenant theme configuration.
/// </summary>
/// <param name="TenantId">The tenant ID.</param>
/// <param name="PrimaryColor">The primary color in hex format (e.g., #E74C3C).</param>
/// <param name="LogoUrl">The URL to the tenant's logo image.</param>
/// <param name="UseCustomTheme">Whether the tenant uses a custom theme.</param>
public record TenantThemeDto(
    Guid TenantId,
    string? PrimaryColor,
    string? LogoUrl,
    bool UseCustomTheme);
