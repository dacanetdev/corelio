using Corelio.Application.Sales.Common;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Cache service for POS product search results.
/// Supports version-based invalidation per tenant so that product mutations
/// immediately bust cached search results without requiring key scanning.
/// </summary>
public interface IProductSearchCacheService
{
    /// <summary>
    /// Returns cached search results for the given tenant and search term,
    /// or null if the cache entry does not exist.
    /// </summary>
    Task<IEnumerable<PosProductDto>?> GetAsync(Guid tenantId, string term, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores search results in the cache for 5 minutes.
    /// </summary>
    Task SetAsync(Guid tenantId, string term, IEnumerable<PosProductDto> results, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all cached search results for the given tenant by rotating
    /// the tenant's cache version. Old entries expire naturally after 5 minutes.
    /// </summary>
    Task InvalidateAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
