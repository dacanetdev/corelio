using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Queries.GetInventoryItems;

/// <summary>
/// Handler for GetInventoryItemsQuery.
/// </summary>
public class GetInventoryItemsQueryHandler(
    IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryItemsQuery, Result<PagedResult<InventoryItemDto>>>
{
    public async Task<Result<PagedResult<InventoryItemDto>>> Handle(
        GetInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await inventoryRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.WarehouseId,
            request.LowStockOnly,
            request.SearchTerm,
            cancellationToken);

        var dtos = items.Select(item => new InventoryItemDto(
            item.Id,
            item.ProductId,
            item.Product.Name,
            item.Product.Sku,
            item.Product.Barcode,
            item.WarehouseId,
            item.Warehouse.Name,
            item.Quantity,
            item.ReservedQuantity,
            item.AvailableQuantity,
            item.MinimumLevel,
            GetStockStatus(item.AvailableQuantity, item.MinimumLevel))).ToList();

        var pagedResult = PagedResult<InventoryItemDto>.Create(
            dtos,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return Result<PagedResult<InventoryItemDto>>.Success(pagedResult);
    }

    private static string GetStockStatus(decimal availableQuantity, decimal minimumLevel)
    {
        if (availableQuantity <= 0)
        {
            return "OutOfStock";
        }

        if (availableQuantity <= minimumLevel)
        {
            return "LowStock";
        }

        return "Adequate";
    }
}
