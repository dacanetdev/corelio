using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for product pricing operations.
/// </summary>
public interface IProductPricingRepository
{
    /// <summary>
    /// Gets a product with its pricing details (discounts and margin prices).
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product with pricing data if found, otherwise null.</returns>
    Task<Product?> GetProductPricingAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of products with pricing data.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term for name, SKU, or barcode.</param>
    /// <param name="categoryId">Optional category ID filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of (products with pricing, totalCount).</returns>
    Task<(List<Product> Products, int TotalCount)> GetProductsPricingListAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the pricing data (discounts and margin prices) for a product.
    /// Uses a replace strategy: removes existing entries and adds new ones.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="discounts">The new discount entries.</param>
    /// <param name="marginPrices">The new margin price entries.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateProductPricingAsync(
        Guid productId,
        List<ProductDiscount> discounts,
        List<ProductMarginPrice> marginPrices,
        CancellationToken cancellationToken = default);
}
