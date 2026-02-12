using System.Net.Http.Json;
using Corelio.BlazorApp.Services.Http;

namespace Corelio.BlazorApp.Services.Theming;

/// <summary>
/// HTTP service for fetching and updating tenant theme configuration from the API.
/// </summary>
public interface ITenantThemeHttpService
{
    /// <summary>
    /// Gets the current tenant's theme configuration.
    /// </summary>
    /// <returns>The theme configuration, or null if not using custom theme.</returns>
    Task<TenantThemeResponse?> GetCurrentThemeAsync();

    /// <summary>
    /// Updates the current tenant's theme configuration.
    /// </summary>
    /// <param name="request">The theme update request.</param>
    /// <returns>True if successful, false otherwise.</returns>
    Task<bool> UpdateThemeAsync(UpdateTenantThemeRequest request);
}

/// <summary>
/// Response DTO for tenant theme configuration.
/// </summary>
public record TenantThemeResponse(
    Guid TenantId,
    string? PrimaryColor,
    string? LogoUrl,
    bool UseCustomTheme);

/// <summary>
/// Request DTO for updating tenant theme configuration.
/// </summary>
public record UpdateTenantThemeRequest(
    string? PrimaryColor,
    bool UseCustomTheme);

/// <summary>
/// HTTP service implementation for tenant theme management.
/// </summary>
public partial class TenantThemeHttpService(
    AuthenticatedHttpClient httpClient,
    ILogger<TenantThemeHttpService> logger) : ITenantThemeHttpService
{
    private const string BaseUrl = "/api/v1/tenants/theme";

    /// <inheritdoc />
    public async Task<TenantThemeResponse?> GetCurrentThemeAsync()
    {
        try
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/current");

            if (!response.IsSuccessStatusCode)
            {
                LogGetThemeFailed(logger, response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TenantThemeResponse>();
        }
        catch (Exception ex)
        {
            LogGetThemeError(logger, ex);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateThemeAsync(UpdateTenantThemeRequest request)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync(BaseUrl, request);

            if (!response.IsSuccessStatusCode)
            {
                LogUpdateThemeFailed(logger, response.StatusCode);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            LogUpdateThemeError(logger, ex);
            return false;
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to get tenant theme: {StatusCode}")]
    private static partial void LogGetThemeFailed(ILogger logger, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error fetching tenant theme")]
    private static partial void LogGetThemeError(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to update tenant theme: {StatusCode}")]
    private static partial void LogUpdateThemeFailed(ILogger logger, System.Net.HttpStatusCode statusCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error updating tenant theme")]
    private static partial void LogUpdateThemeError(ILogger logger, Exception ex);
}
