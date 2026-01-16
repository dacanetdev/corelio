using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for Product entity operations.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Gets a product by its ID.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found, otherwise null.</returns>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by its SKU.
    /// </summary>
    /// <param name="sku">The product SKU.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found, otherwise null.</returns>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by its barcode.
    /// </summary>
    /// <param name="barcode">The product barcode.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found, otherwise null.</returns>
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a SKU already exists.
    /// </summary>
    /// <param name="sku">The SKU to check.</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the SKU exists, otherwise false.</returns>
    Task<bool> SkuExistsAsync(string sku, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a barcode already exists.
    /// </summary>
    /// <param name="barcode">The barcode to check.</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the barcode exists, otherwise false.</returns>
    Task<bool> BarcodeExistsAsync(string barcode, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product to add.</param>
    void Add(Product product);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product to update.</param>
    void Update(Product product);

    /// <summary>
    /// Deletes a product (soft delete).
    /// </summary>
    /// <param name="product">The product to delete.</param>
    void Delete(Product product);

    /// <summary>
    /// Gets a paged list of products with optional filtering.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term for name, SKU, or barcode.</param>
    /// <param name="categoryId">Optional category ID filter.</param>
    /// <param name="isActive">Optional active status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of (products, totalCount).</returns>
    Task<(List<Product> Products, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for products by barcode, SKU, or name.
    /// Optimized for fast POS search.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching products.</returns>
    Task<List<Product>> SearchAsync(
        string query,
        int limit,
        CancellationToken cancellationToken = default);
}
