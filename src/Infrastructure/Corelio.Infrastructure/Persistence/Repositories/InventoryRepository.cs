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
