using System.Security.Claims;
using System.Text.Json;
using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Entities;
using Corelio.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Corelio.Infrastructure.MultiTenancy;

/// <summary>
/// Service for managing tenant context and resolution in a multi-tenant application.
/// Handles tenant resolution from JWT claims, HTTP headers, or subdomain.
/// Implements caching for tenant data to improve performance.
/// </summary>
public class TenantService(
    TenantProvider tenantProvider,
    IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext dbContext,
    IDistributedCache cache,
    ILogger<TenantService> logger) : ITenantService
{
    private const string TenantClaimType = "tenant_id";
    private const string TenantHeaderName = "X-Tenant-ID";
    private const string CacheKeyPrefix = "tenant:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    /// <inheritdoc />
    public Guid? GetCurrentTenantId()
    {
        // Return cached tenant ID if already set
        if (tenantProvider.HasTenantContext)
        {
            return tenantProvider.TenantId;
        }

        // Try to resolve tenant ID from HTTP context
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            logger.LogWarning("No HTTP context available for tenant resolution");
            return null;
        }

        // Strategy 1: Try to get tenant ID from JWT claims
        var tenantId = ResolveTenantFromJwt(httpContext);
        if (tenantId.HasValue)
        {
            logger.LogDebug("Tenant resolved from JWT: {TenantId}", tenantId);
            tenantProvider.SetTenant(tenantId.Value);
            return tenantId;
        }

        // Strategy 2: Try to get tenant ID from HTTP header
        tenantId = ResolveTenantFromHeader(httpContext);
        if (tenantId.HasValue)
        {
            logger.LogDebug("Tenant resolved from header: {TenantId}", tenantId);
            tenantProvider.SetTenant(tenantId.Value);
            return tenantId;
        }

        // Strategy 3: Try to resolve tenant from subdomain
        tenantId = ResolveTenantFromSubdomainAsync(httpContext).GetAwaiter().GetResult();
        if (tenantId.HasValue)
        {
            logger.LogDebug("Tenant resolved from subdomain: {TenantId}", tenantId);
            tenantProvider.SetTenant(tenantId.Value);
            return tenantId;
        }

        logger.LogWarning("Could not resolve tenant ID from any source");
        return null;
    }

    /// <inheritdoc />
    public void SetCurrentTenantId(Guid? tenantId)
    {
        if (tenantId.HasValue)
        {
            tenantProvider.SetTenant(tenantId.Value);
            logger.LogDebug("Tenant ID set manually: {TenantId}", tenantId);
        }
        else
        {
            tenantProvider.ClearTenant();
            logger.LogDebug("Tenant context cleared");
        }
    }

    /// <inheritdoc />
    public async Task<Tenant?> GetCurrentTenantAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return null;
        }

        return await GetTenantByIdAsync(tenantId.Value, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> TenantExistsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        // Check cache first
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return true;
        }

        // Check database
        return await dbContext.Tenants
            .AsNoTracking()
            .AnyAsync(t => t.Id == tenantId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(subdomain))
        {
            return null;
        }

        // Check cache first
        var cacheKey = $"{CacheKeyPrefix}subdomain:{subdomain.ToLowerInvariant()}";
        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogDebug("Tenant found in cache for subdomain: {Subdomain}", subdomain);
            return JsonSerializer.Deserialize<Tenant>(cachedData);
        }

        // Query database
        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain.ToLowerInvariant(), cancellationToken);

        if (tenant is not null)
        {
            // Cache the result
            var serialized = JsonSerializer.Serialize(tenant);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            };

            await cache.SetStringAsync(cacheKey, serialized, options, cancellationToken);
            await cache.SetStringAsync($"{CacheKeyPrefix}{tenant.Id}", serialized, options, cancellationToken);

            logger.LogDebug("Tenant cached for subdomain: {Subdomain}", subdomain);
        }

        return tenant;
    }

    /// <inheritdoc />
    public async Task<T> BypassTenantFilterAsync<T>(Func<Task<T>> operation)
    {
        logger.LogWarning("Bypassing tenant filter - use with caution!");

        var originalTenantId = tenantProvider.HasTenantContext ? tenantProvider.TenantId : (Guid?)null;

        try
        {
            // Clear tenant context temporarily
            tenantProvider.ClearTenant();

            // Execute the operation
            return await operation();
        }
        finally
        {
            // Restore original tenant context
            if (originalTenantId.HasValue)
            {
                tenantProvider.SetTenant(originalTenantId.Value);
            }
        }
    }

    /// <inheritdoc />
    public async Task InvalidateTenantCacheAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        await cache.RemoveAsync(cacheKey, cancellationToken);

        // Also need to remove subdomain cache if exists
        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant is not null)
        {
            var subdomainCacheKey = $"{CacheKeyPrefix}subdomain:{tenant.Subdomain.ToLowerInvariant()}";
            await cache.RemoveAsync(subdomainCacheKey, cancellationToken);
        }

        logger.LogInformation("Tenant cache invalidated: {TenantId}", tenantId);
    }

    #region Private Helper Methods

    private Guid? ResolveTenantFromJwt(HttpContext httpContext)
    {
        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var tenantClaim = user.FindFirst(TenantClaimType)?.Value;
        if (string.IsNullOrEmpty(tenantClaim))
        {
            return null;
        }

        return Guid.TryParse(tenantClaim, out var tenantId) ? tenantId : null;
    }

    private Guid? ResolveTenantFromHeader(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue(TenantHeaderName, out var headerValue))
        {
            return null;
        }

        var tenantIdString = headerValue.ToString();
        if (string.IsNullOrEmpty(tenantIdString))
        {
            return null;
        }

        return Guid.TryParse(tenantIdString, out var tenantId) ? tenantId : null;
    }

    private async Task<Guid?> ResolveTenantFromSubdomainAsync(HttpContext httpContext)
    {
        var host = httpContext.Request.Host.Host;
        if (string.IsNullOrEmpty(host))
        {
            return null;
        }

        // Extract subdomain (e.g., "ferreteria-lopez" from "ferreteria-lopez.corelio.com.mx")
        var parts = host.Split('.');
        if (parts.Length < 3)
        {
            // Not a subdomain (e.g., "localhost" or "corelio.com")
            return null;
        }

        var subdomain = parts[0];
        if (string.IsNullOrEmpty(subdomain) || subdomain == "www")
        {
            return null;
        }

        var tenant = await GetTenantBySubdomainAsync(subdomain);
        return tenant?.Id;
    }

    private async Task<Tenant?> GetTenantByIdAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        // Check cache first
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogDebug("Tenant found in cache: {TenantId}", tenantId);
            return JsonSerializer.Deserialize<Tenant>(cachedData);
        }

        // Query database
        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        if (tenant is not null)
        {
            // Cache the result
            var serialized = JsonSerializer.Serialize(tenant);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            };

            await cache.SetStringAsync(cacheKey, serialized, options, cancellationToken);
            logger.LogDebug("Tenant cached: {TenantId}", tenantId);
        }

        return tenant;
    }

    #endregion
}
