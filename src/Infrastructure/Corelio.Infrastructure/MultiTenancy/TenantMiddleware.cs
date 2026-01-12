using Corelio.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Corelio.Infrastructure.MultiTenancy;

/// <summary>
/// Middleware that resolves and sets the tenant context for each HTTP request.
/// Must be registered early in the pipeline (after authentication).
/// </summary>
public class TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware to resolve tenant context.
    /// </summary>
    public async Task InvokeAsync(HttpContext context, ITenantService tenantService, TenantProvider tenantProvider)
    {
        // Try to resolve tenant ID
        var tenantId = tenantService.GetCurrentTenantId();

        if (tenantId.HasValue)
        {
            logger.LogDebug("Tenant context set for request: {TenantId} | Path: {Path}",
                tenantId, context.Request.Path);
        }
        else
        {
            // Some endpoints may not require tenant context (e.g., tenant registration, health checks)
            logger.LogDebug("No tenant context for request: {Path}", context.Request.Path);
        }

        // Continue with the request pipeline
        await next(context);
    }
}

/// <summary>
/// Extension methods for registering TenantMiddleware.
/// </summary>
public static class TenantMiddlewareExtensions
{
    /// <summary>
    /// Adds tenant resolution middleware to the HTTP pipeline.
    /// Should be called after UseAuthentication() and before UseAuthorization().
    /// </summary>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantMiddleware>();
    }
}
