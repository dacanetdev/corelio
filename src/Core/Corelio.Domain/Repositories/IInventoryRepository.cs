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

    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(List<InventoryItem> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? warehouseId = null,
        bool lowStockOnly = false,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<(List<InventoryTransaction> Items, int TotalCount)> GetTransactionsPagedAsync(
        Guid inventoryItemId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<List<Warehouse>> GetAllWarehousesAsync(CancellationToken cancellationToken = default);

    void AddInventoryItem(InventoryItem item);
    void UpdateInventoryItem(InventoryItem item);
    void AddTransaction(InventoryTransaction transaction);
}
