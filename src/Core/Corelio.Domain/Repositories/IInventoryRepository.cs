using Corelio.Domain.Entities;

namespace Corelio.Domain.Repositories;

/// <summary>
/// Repository interface for Inventory aggregate (InventoryItem + InventoryTransaction).
/// </summary>
public interface IInventoryRepository
{
    Task<InventoryItem?> GetByProductAndWarehouseAsync(
        Guid productId, Guid warehouseId, CancellationToken cancellationToken = default);

    Task<List<InventoryItem>> GetByWarehouseAsync(
        Guid warehouseId, CancellationToken cancellationToken = default);

    Task<Warehouse?> GetDefaultWarehouseAsync(CancellationToken cancellationToken = default);

    void AddInventoryItem(InventoryItem item);
    void UpdateInventoryItem(InventoryItem item);
    void AddTransaction(InventoryTransaction transaction);
}
