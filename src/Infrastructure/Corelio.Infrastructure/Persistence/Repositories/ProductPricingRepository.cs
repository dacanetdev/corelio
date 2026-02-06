using System.Globalization;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for product pricing operations.
/// </summary>
public class ProductPricingRepository(ApplicationDbContext context) : IProductPricingRepository
{
    public async Task<Product?> GetProductPricingAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Discounts.OrderBy(d => d.TierNumber))
            .Include(p => p.MarginPrices.OrderBy(m => m.TierNumber))
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task<(List<Product> Products, int TotalCount)> GetProductsPricingListAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products
            .Include(p => p.Category)
            .Include(p => p.Discounts.OrderBy(d => d.TierNumber))
            .Include(p => p.MarginPrices.OrderBy(m => m.TierNumber))
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

    public async Task UpdateProductPricingAsync(
        Guid productId,
        List<ProductDiscount> discounts,
        List<ProductMarginPrice> marginPrices,
        CancellationToken cancellationToken = default)
    {
        // Remove existing discounts for this product
        var existingDiscounts = await context.ProductDiscounts
            .Where(pd => pd.ProductId == productId)
            .ToListAsync(cancellationToken);
        context.ProductDiscounts.RemoveRange(existingDiscounts);

        // Remove existing margin prices for this product
        var existingMarginPrices = await context.ProductMarginPrices
            .Where(pm => pm.ProductId == productId)
            .ToListAsync(cancellationToken);
        context.ProductMarginPrices.RemoveRange(existingMarginPrices);

        // Add new discounts and margin prices
        if (discounts.Count > 0)
        {
            context.ProductDiscounts.AddRange(discounts);
        }

        if (marginPrices.Count > 0)
        {
            context.ProductMarginPrices.AddRange(marginPrices);
        }
    }
}
