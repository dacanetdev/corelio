using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Products;

namespace Corelio.BlazorApp.Services.Products;

/// <summary>
/// Service for product management operations.
/// Makes HTTP API calls to the backend product endpoints.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets a paginated list of products with optional filters.
    /// </summary>
    Task<Result<PagedResult<ProductListDto>>> GetProductsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        Guid? categoryId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by its ID.
    /// </summary>
    Task<Result<ProductDto>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for products by barcode, SKU, or name (optimized for POS).
    /// </summary>
    Task<Result<List<ProductListDto>>> SearchProductsAsync(
        string query,
        int limit = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new product.
    /// </summary>
    Task<Result<Guid>> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    Task<Result<bool>> UpdateProductAsync(
        UpdateProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a product (soft delete).
    /// </summary>
    Task<Result<bool>> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all product categories.
    /// </summary>
    Task<Result<List<ProductCategoryDto>>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
