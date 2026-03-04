using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Queries.GetInventoryItems;

/// <summary>
/// Query to get a paged list of inventory items with optional filters.
/// </summary>
public record GetInventoryItemsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? WarehouseId = null,
    bool LowStockOnly = false,
    string? SearchTerm = null) : IRequest<Result<PagedResult<InventoryItemDto>>>;
