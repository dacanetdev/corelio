using Corelio.Application.Common.Interfaces;
using Corelio.Application.Sales.Common;
using Corelio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Services;

/// <summary>
/// High-performance POS product search using EF Core queries.
/// Searches by SKU (exact), Barcode (exact), or Name (partial match).
/// Uses PostgreSQL ILIKE for case-insensitive matching.
/// </summary>
public class PosSearchService(ApplicationDbContext context) : IPosSearchService
{
    public async Task<IEnumerable<PosProductDto>> SearchProductsAsync(
        string term,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return [];
        }

        var pattern = $"%{term}%";
        var startsWith = $"{term}%";

        var products = await context.Products
            .AsNoTracking()
            .Where(p => p.IsActive &&
                (EF.Functions.ILike(p.Sku, term) ||
                 (p.Barcode != null && EF.Functions.ILike(p.Barcode, term)) ||
                 EF.Functions.ILike(p.Name, pattern) ||
                 EF.Functions.ILike(p.Sku, pattern)))
            .OrderByDescending(p =>
                EF.Functions.ILike(p.Sku, term) ? 3 :
                p.Barcode != null && EF.Functions.ILike(p.Barcode, term) ? 3 :
                EF.Functions.ILike(p.Name, startsWith) ? 2 : 1)
            .ThenBy(p => p.Name)
            .Take(limit)
            .ToListAsync(cancellationToken);

        // Get inventory for these products (left join to default warehouse)
        var productIds = products.Select(p => p.Id).ToList();
        var defaultWarehouse = await context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.IsDefault, cancellationToken);

        Dictionary<Guid, decimal> stockLevels = [];

        if (defaultWarehouse is not null)
        {
            stockLevels = await context.InventoryItems
                .AsNoTracking()
                .Where(i => i.WarehouseId == defaultWarehouse.Id && productIds.Contains(i.ProductId))
                .ToDictionaryAsync(i => i.ProductId, i => i.Quantity, cancellationToken);
        }

        return products.Select(p => new PosProductDto(
            p.Id,
            p.Sku,
            p.Name,
            p.Barcode,
            p.SalePrice,
            stockLevels.TryGetValue(p.Id, out var qty) ? qty : 0,
            p.UnitOfMeasure,
            p.IvaEnabled,
            p.TaxRate));
    }
}
