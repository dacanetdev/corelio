using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for ProductCategory entity operations.
/// </summary>
public interface IProductCategoryRepository
{
    /// <summary>
    /// Gets a product category by its ID.
    /// </summary>
    /// <param name="id">The category ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category if found, otherwise null.</returns>
    Task<ProductCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all product categories.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive categories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all categories.</returns>
    Task<List<ProductCategory>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all root-level categories (no parent).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of root categories.</returns>
    Task<List<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets child categories of a parent category.
    /// </summary>
    /// <param name="parentId">The parent category ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of child categories.</returns>
    Task<List<ProductCategory>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category name already exists within the same parent.
    /// </summary>
    /// <param name="name">The category name.</param>
    /// <param name="parentId">The parent category ID (null for root level).</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the name exists, otherwise false.</returns>
    Task<bool> NameExistsAsync(string name, Guid? parentId = null, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category has any products assigned to it.
    /// </summary>
    /// <param name="id">The category ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the category has products, otherwise false.</returns>
    Task<bool> HasProductsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new product category.
    /// </summary>
    /// <param name="category">The category to add.</param>
    void Add(ProductCategory category);

    /// <summary>
    /// Updates an existing product category.
    /// </summary>
    /// <param name="category">The category to update.</param>
    void Update(ProductCategory category);

    /// <summary>
    /// Deletes a product category (soft delete).
    /// </summary>
    /// <param name="category">The category to delete.</param>
    void Delete(ProductCategory category);
}
