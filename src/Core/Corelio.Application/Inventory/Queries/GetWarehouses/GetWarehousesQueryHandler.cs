using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Queries.GetWarehouses;

/// <summary>
/// Handler for GetWarehousesQuery.
/// </summary>
public class GetWarehousesQueryHandler(
    IInventoryRepository inventoryRepository)
    : IRequestHandler<GetWarehousesQuery, Result<List<WarehouseDto>>>
{
    public async Task<Result<List<WarehouseDto>>> Handle(
        GetWarehousesQuery request, CancellationToken cancellationToken)
    {
        var warehouses = await inventoryRepository.GetAllWarehousesAsync(cancellationToken);

        var dtos = warehouses.Select(w => new WarehouseDto(
            w.Id,
            w.Name,
            w.IsDefault)).ToList();

        return Result<List<WarehouseDto>>.Success(dtos);
    }
}
