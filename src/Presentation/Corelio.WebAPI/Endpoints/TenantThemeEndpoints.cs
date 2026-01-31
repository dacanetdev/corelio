using System.Security.Claims;
using Corelio.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Tenant theme management endpoints for customizing tenant branding.
/// </summary>
public static class TenantThemeEndpoints
{
    /// <summary>
    /// Maps all tenant theme endpoints to the application.
    /// </summary>
    public static IEndpointRouteBuilder MapTenantThemeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/tenants/theme")
            .WithTags("Tenant Theme")
            .RequireAuthorization();

        group.MapGet("/current", GetCurrentTheme)
            .WithName("GetCurrentTenantTheme")
            .WithSummary("Gets the current tenant's theme configuration");

        group.MapPut("/", UpdateTheme)
            .WithName("UpdateTenantTheme")
            .WithSummary("Updates the current tenant's theme configuration");

        return app;
    }

    /// <summary>
    /// Gets the current tenant's theme configuration.
    /// </summary>
    private static async Task<IResult> GetCurrentTheme(
        ClaimsPrincipal user,
        ITenantThemeService themeService,
        CancellationToken ct)
    {
        var tenantIdClaim = user.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: "No valid tenant context found in authentication claims.");
        }

        var theme = await themeService.GetTenantThemeAsync(tenantId, ct);

        // Return empty response if no custom theme configured (tenant uses default)
        if (theme is null)
        {
            return Results.Ok(new TenantThemeResponse(tenantId, null, null, false));
        }

        return Results.Ok(new TenantThemeResponse(
            theme.TenantId,
            theme.PrimaryColor,
            theme.LogoUrl,
            theme.UseCustomTheme));
    }

    /// <summary>
    /// Updates the current tenant's theme configuration.
    /// </summary>
    private static async Task<IResult> UpdateTheme(
        ClaimsPrincipal user,
        [FromBody] UpdateTenantThemeRequest request,
        ITenantThemeService themeService,
        CancellationToken ct)
    {
        var tenantIdClaim = user.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: "No valid tenant context found in authentication claims.");
        }

        // Validate hex color format if provided
        if (request.UseCustomTheme && !string.IsNullOrEmpty(request.PrimaryColor))
        {
            if (!themeService.IsValidHexColor(request.PrimaryColor))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "InvalidColor",
                    detail: "Primary color must be in hex format (#RRGGBB), e.g., #E74C3C");
            }
        }

        var success = await themeService.UpdateTenantThemeAsync(
            tenantId,
            request.PrimaryColor,
            request.UseCustomTheme,
            ct);

        if (!success)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "NotFound",
                detail: "Tenant configuration not found.");
        }

        return Results.Ok(new { message = "Theme updated successfully" });
    }
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
