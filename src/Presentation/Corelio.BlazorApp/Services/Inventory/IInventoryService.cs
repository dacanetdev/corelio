using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Inventory;

namespace Corelio.BlazorApp.Services.Inventory;

/// <summary>
/// Service for inventory management API calls.
/// </summary>
public interface IInventoryService
{
    Task<Result<PagedResult<InventoryItemModel>>> GetInventoryItemsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        Guid? warehouseId = null,
        bool lowStockOnly = false,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    Task<Result<List<WarehouseModel>>> GetWarehousesAsync(
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<InventoryTransactionModel>>> GetTransactionsAsync(
        Guid inventoryItemId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> AdjustStockAsync(
        AdjustStockRequest request,
        CancellationToken cancellationToken = default);
}
