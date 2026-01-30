using System.Net.Http.Json;

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
public class TenantThemeHttpService(
    IHttpClientFactory httpClientFactory,
    ILogger<TenantThemeHttpService> logger) : ITenantThemeHttpService
{
    private const string BaseUrl = "/api/v1/tenants/theme";

    /// <inheritdoc />
    public async Task<TenantThemeResponse?> GetCurrentThemeAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient("api");
            var response = await client.GetAsync($"{BaseUrl}/current");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to get tenant theme: {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TenantThemeResponse>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching tenant theme");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateThemeAsync(UpdateTenantThemeRequest request)
    {
        try
        {
            var client = httpClientFactory.CreateClient("api");
            var response = await client.PutAsJsonAsync(BaseUrl, request);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to update tenant theme: {StatusCode}", response.StatusCode);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating tenant theme");
            return false;
        }
    }
}
