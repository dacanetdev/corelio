using System.Text.Json;
using System.Text.RegularExpressions;
using Corelio.Application.Common.Interfaces;
using Corelio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Corelio.Infrastructure.Services;

/// <summary>
/// Service for managing tenant theme customization with Redis caching.
/// </summary>
public partial class TenantThemeService(
    ApplicationDbContext dbContext,
    IDistributedCache cache,
    ILogger<TenantThemeService> logger) : ITenantThemeService
{
    private const string CacheKeyPrefix = "tenant-theme:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(2);

    /// <inheritdoc />
    public async Task<TenantThemeDto?> GetTenantThemeAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        // Check cache first
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        try
        {
            var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedData))
            {
                logger.LogDebug("Tenant theme found in cache: {TenantId}", tenantId);
                return JsonSerializer.Deserialize<TenantThemeDto>(cachedData);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read tenant theme from cache, falling back to database");
        }

        // Query database
        var config = await dbContext.TenantConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(tc => tc.TenantId == tenantId, cancellationToken);

        if (config is null)
        {
            logger.LogDebug("Tenant configuration not found: {TenantId}", tenantId);
            return null;
        }

        // Return null if tenant is not using custom theme
        if (!config.UseCustomTheme)
        {
            logger.LogDebug("Tenant is not using custom theme: {TenantId}", tenantId);
            return null;
        }

        var themeDto = new TenantThemeDto(
            config.TenantId,
            config.PrimaryColor,
            config.LogoUrl,
            config.UseCustomTheme);

        // Cache the result
        try
        {
            var serialized = JsonSerializer.Serialize(themeDto);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            };

            await cache.SetStringAsync(cacheKey, serialized, options, cancellationToken);
            logger.LogDebug("Tenant theme cached: {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to cache tenant theme, continuing without caching");
        }

        return themeDto;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateTenantThemeAsync(
        Guid tenantId,
        string? primaryColor,
        bool useCustomTheme,
        CancellationToken cancellationToken = default)
    {
        // Validate hex color if provided and custom theme is enabled
        if (useCustomTheme && !string.IsNullOrEmpty(primaryColor) && !IsValidHexColor(primaryColor))
        {
            logger.LogWarning("Invalid hex color format: {PrimaryColor}", primaryColor);
            return false;
        }

        var config = await dbContext.TenantConfigurations
            .FirstOrDefaultAsync(tc => tc.TenantId == tenantId, cancellationToken);

        if (config is null)
        {
            logger.LogWarning("Tenant configuration not found for update: {TenantId}", tenantId);
            return false;
        }

        // Update theme settings
        config.UseCustomTheme = useCustomTheme;
        config.PrimaryColor = useCustomTheme ? primaryColor : null;

        await dbContext.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await InvalidateThemeCacheAsync(tenantId, cancellationToken);

        logger.LogInformation(
            "Tenant theme updated: TenantId={TenantId}, UseCustomTheme={UseCustomTheme}, PrimaryColor={PrimaryColor}",
            tenantId, useCustomTheme, primaryColor);

        return true;
    }

    /// <inheritdoc />
    public async Task InvalidateThemeCacheAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        try
        {
            await cache.RemoveAsync(cacheKey, cancellationToken);
            logger.LogDebug("Tenant theme cache invalidated: {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to invalidate tenant theme cache");
        }
    }

    /// <inheritdoc />
    public bool IsValidHexColor(string? color)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            return false;
        }

        // Validate #RRGGBB format (7 characters including #)
        return HexColorRegex().IsMatch(color);
    }

    /// <summary>
    /// Generated regex for validating hex color format.
    /// </summary>
    [GeneratedRegex(@"^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled)]
    private static partial Regex HexColorRegex();
}
