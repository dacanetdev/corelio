using System.Text.Json;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Sales.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Corelio.Infrastructure.Services;

/// <summary>
/// Redis-backed cache for POS product search results.
/// Uses a per-tenant version key to enable O(1) cache invalidation:
/// rotating the version makes all prior search entries unreachable
/// without needing to scan or delete individual keys.
/// </summary>
public sealed class ProductSearchCacheService(
    IDistributedCache cache,
    ILogger<ProductSearchCacheService> logger) : IProductSearchCacheService
{
    private static readonly TimeSpan SearchTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan VersionTtl = TimeSpan.FromHours(1);

    public async Task<IEnumerable<PosProductDto>?> GetAsync(
        Guid tenantId,
        string term,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var version = await GetVersionAsync(tenantId, cancellationToken);
            if (version is null)
            {
                return null;
            }

            var key = BuildSearchKey(tenantId, version, term);
            var json = await cache.GetStringAsync(key, cancellationToken);

            if (json is null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<List<PosProductDto>>(json);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache read failed for tenant {TenantId} term '{Term}' — falling back to database", tenantId, term);
            return null;
        }
    }

    public async Task SetAsync(
        Guid tenantId,
        string term,
        IEnumerable<PosProductDto> results,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var version = await GetOrCreateVersionAsync(tenantId, cancellationToken);
            var key = BuildSearchKey(tenantId, version, term);
            var json = JsonSerializer.Serialize(results);

            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = SearchTtl
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache write failed for tenant {TenantId} term '{Term}' — continuing without cache", tenantId, term);
        }
    }

    public async Task InvalidateAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var newVersion = Guid.NewGuid().ToString("N");
            var versionKey = BuildVersionKey(tenantId);

            await cache.SetStringAsync(versionKey, newVersion, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = VersionTtl
            }, cancellationToken);

            logger.LogDebug("Search cache invalidated for tenant {TenantId} (new version {Version})", tenantId, newVersion);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache invalidation failed for tenant {TenantId} — cache may serve stale results until TTL expires", tenantId);
        }
    }

    private async Task<string?> GetVersionAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        return await cache.GetStringAsync(BuildVersionKey(tenantId), cancellationToken);
    }

    private async Task<string> GetOrCreateVersionAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var versionKey = BuildVersionKey(tenantId);
        var version = await cache.GetStringAsync(versionKey, cancellationToken);

        if (version is not null)
        {
            return version;
        }

        var newVersion = Guid.NewGuid().ToString("N");
        await cache.SetStringAsync(versionKey, newVersion, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = VersionTtl
        }, cancellationToken);

        return newVersion;
    }

    private static string BuildVersionKey(Guid tenantId) =>
        $"pos:ver:{tenantId:N}";

    private static string BuildSearchKey(Guid tenantId, string version, string term) =>
        $"pos:search:{tenantId:N}:{version}:{term.ToLowerInvariant()}";
}
