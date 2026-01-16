using System.Globalization;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Product aggregate.
/// </summary>
public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Barcode == barcode, cancellationToken);
    }

    public async Task<bool> SkuExistsAsync(string sku, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = context.Products.Where(p => p.Sku == sku);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> BarcodeExistsAsync(string barcode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = context.Products.Where(p => p.Barcode == barcode);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public void Add(Product product)
    {
        context.Products.Add(product);
    }

    public void Update(Product product)
    {
        context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        context.Products.Update(product);
    }

    public async Task<(List<Product> Products, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products
            .Include(p => p.Category)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            query = query.Where(p =>
                p.Name.ToLower(CultureInfo.InvariantCulture).Contains(lowerSearchTerm) ||
                p.Sku.ToLower(CultureInfo.InvariantCulture).Contains(lowerSearchTerm) ||
                (p.Barcode != null && p.Barcode.ToLower(CultureInfo.InvariantCulture).Contains(lowerSearchTerm)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var products = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }

    public async Task<List<Product>> SearchAsync(
        string query,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var lowerQuery = query.ToLowerInvariant();

        return await context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive &&
                (p.Barcode != null && p.Barcode.ToLower(CultureInfo.InvariantCulture) == lowerQuery ||
                 p.Sku.ToLower(CultureInfo.InvariantCulture) == lowerQuery ||
                 p.Name.ToLower(CultureInfo.InvariantCulture).Contains(lowerQuery)))
            .OrderByDescending(p => p.Barcode != null && p.Barcode.ToLower(CultureInfo.InvariantCulture) == lowerQuery ? 3 :
                                    p.Sku.ToLower(CultureInfo.InvariantCulture) == lowerQuery ? 2 :
                                    p.Name.ToLower(CultureInfo.InvariantCulture).StartsWith(lowerQuery) ? 1 : 0)
            .ThenBy(p => p.Name)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
