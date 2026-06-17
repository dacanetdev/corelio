using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Inventory.Common;
using Corelio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Reports;

/// <summary>
/// EF Core implementation of the inventory valuation report aggregation service.
/// Only items with Quantity > 0 are included in the valuation totals.
/// </summary>
internal sealed class InventoryValuationQueryService(ApplicationDbContext dbContext)
    : IInventoryValuationQueryService
{
    private const string UncategorizedName = "Sin categoría";

    public async Task<InventoryValuationReportDto> GetInventoryValuationReportAsync(
        Guid? warehouseId = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var itemsQuery = dbContext.InventoryItems
            .AsNoTracking()
            .Include(i => i.Product)
                .ThenInclude(p => p.Category)
            .Where(i => i.Quantity > 0);

        if (warehouseId.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.WarehouseId == warehouseId.Value);
        }

        if (categoryId.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.Product.CategoryId == categoryId.Value);
        }

        var items = await itemsQuery.ToListAsync(cancellationToken);

        var valuations = items.Select(i => new ProductValuationDto(
            i.ProductId,
            i.Product.Name,
            i.Product.Sku,
            i.Product.CategoryId,
            i.Product.Category?.Name,
            i.WarehouseId,
            i.Quantity,
            i.AverageCost,
            Math.Round(i.Quantity * i.AverageCost, 4)
        )).OrderByDescending(v => v.ExtendedValue).ToList();

        var totalInventoryValue = valuations.Sum(v => v.ExtendedValue);

        var categoryGroups = valuations
            .GroupBy(v => new { v.CategoryId, Name = v.CategoryName ?? UncategorizedName })
            .Select(g => new CategoryValueDto(
                g.Key.CategoryId ?? Guid.Empty,
                g.Key.Name,
                g.Sum(v => v.ExtendedValue),
                totalInventoryValue > 0
                    ? Math.Round(g.Sum(v => v.ExtendedValue) / totalInventoryValue * 100, 2)
                    : 0m,
                g.Select(v => v.ProductId).Distinct().Count()
            ))
            .OrderByDescending(c => c.TotalValue)
            .ToList();

        var lowStockAlerts = items
            .Where(i => i.MinimumLevel > 0
                ? i.Quantity <= i.MinimumLevel
                : i.Quantity == 0)
            .Select(i => new LowStockProductDto(
                i.ProductId,
                i.Product.Name,
                i.Product.Sku,
                i.WarehouseId,
                i.Quantity,
                i.MinimumLevel,
                i.AverageCost
            ))
            .ToList();

        return new InventoryValuationReportDto(
            DateTime.UtcNow,
            warehouseId,
            categoryId,
            totalInventoryValue,
            valuations.Select(v => v.ProductId).Distinct().Count(),
            categoryGroups,
            valuations,
            lowStockAlerts);
    }
}
