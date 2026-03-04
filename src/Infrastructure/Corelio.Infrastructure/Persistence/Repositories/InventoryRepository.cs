using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Inventory aggregate.
/// </summary>
public class InventoryRepository(ApplicationDbContext context) : IInventoryRepository
{
    public async Task<InventoryItem?> GetByProductAndWarehouseAsync(
        Guid productId, Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await context.InventoryItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId, cancellationToken);
    }

    public async Task<List<InventoryItem>> GetByWarehouseAsync(
        Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await context.InventoryItems
            .Include(i => i.Product)
            .Where(i => i.WarehouseId == warehouseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Warehouse?> GetDefaultWarehouseAsync(CancellationToken cancellationToken = default)
    {
        return await context.Warehouses
            .FirstOrDefaultAsync(w => w.IsDefault, cancellationToken);
    }

    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.InventoryItems
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<(List<InventoryItem> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? warehouseId = null,
        bool lowStockOnly = false,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.InventoryItems
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .AsNoTracking()
            .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(i => i.WarehouseId == warehouseId.Value);
        }

        if (lowStockOnly)
        {
            query = query.Where(i => (i.Quantity - i.ReservedQuantity) <= i.MinimumLevel);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(i =>
                EF.Functions.ILike(i.Product.Name, $"%{searchTerm}%") ||
                EF.Functions.ILike(i.Product.Sku, $"%{searchTerm}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(i => i.Product.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<InventoryTransaction> Items, int TotalCount)> GetTransactionsPagedAsync(
        Guid inventoryItemId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.InventoryTransactions
            .Where(t => t.InventoryItemId == inventoryItemId)
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<Warehouse>> GetAllWarehousesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Warehouses
            .AsNoTracking()
            .OrderByDescending(w => w.IsDefault)
            .ThenBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }

    public void AddInventoryItem(InventoryItem item)
    {
        context.InventoryItems.Add(item);
    }

    public void UpdateInventoryItem(InventoryItem item)
    {
        context.InventoryItems.Update(item);
    }

    public void AddTransaction(InventoryTransaction transaction)
    {
        context.InventoryTransactions.Add(transaction);
    }
}
