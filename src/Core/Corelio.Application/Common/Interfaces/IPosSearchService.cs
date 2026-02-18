using Corelio.Application.Sales.Common;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// High-performance product search service for the Point-of-Sale screen.
/// </summary>
public interface IPosSearchService
{
    /// <summary>
    /// Searches products by name, SKU, or barcode for POS quick lookup.
    /// Returns product with current stock level.
    /// </summary>
    Task<IEnumerable<PosProductDto>> SearchProductsAsync(
        string term,
        int limit = 20,
        CancellationToken cancellationToken = default);
}
