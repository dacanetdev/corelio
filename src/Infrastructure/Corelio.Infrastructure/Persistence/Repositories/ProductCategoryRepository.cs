using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ProductCategory aggregate.
/// </summary>
public class ProductCategoryRepository(ApplicationDbContext context) : IProductCategoryRepository
{
    public async Task<ProductCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ProductCategories
            .Include(pc => pc.ParentCategory)
            .FirstOrDefaultAsync(pc => pc.Id == id, cancellationToken);
    }

    public async Task<List<ProductCategory>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = context.ProductCategories
            .Include(pc => pc.ParentCategory)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(pc => pc.IsActive);
        }

        return await query
            .OrderBy(pc => pc.Level)
            .ThenBy(pc => pc.SortOrder)
            .ThenBy(pc => pc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await context.ProductCategories
            .Where(pc => pc.ParentCategoryId == null && pc.IsActive)
            .OrderBy(pc => pc.SortOrder)
            .ThenBy(pc => pc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductCategory>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await context.ProductCategories
            .Where(pc => pc.ParentCategoryId == parentId && pc.IsActive)
            .OrderBy(pc => pc.SortOrder)
            .ThenBy(pc => pc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        Guid? parentId = null,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.ProductCategories
            .Where(pc => pc.Name == name && pc.ParentCategoryId == parentId);

        if (excludeId.HasValue)
        {
            query = query.Where(pc => pc.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasProductsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .AnyAsync(p => p.CategoryId == id, cancellationToken);
    }

    public void Add(ProductCategory category)
    {
        context.ProductCategories.Add(category);
    }

    public void Update(ProductCategory category)
    {
        context.ProductCategories.Update(category);
    }

    public void Delete(ProductCategory category)
    {
        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        context.ProductCategories.Update(category);
    }
}
