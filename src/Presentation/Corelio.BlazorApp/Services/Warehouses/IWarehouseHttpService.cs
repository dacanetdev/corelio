using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Warehouses;

namespace Corelio.BlazorApp.Services.Warehouses;

public interface IWarehouseHttpService
{
    Task<Result<List<WarehouseListModel>>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateWarehouseAsync(WarehouseFormModel model, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateWarehouseAsync(Guid id, WarehouseFormModel model, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteWarehouseAsync(Guid id, CancellationToken cancellationToken = default);
}
