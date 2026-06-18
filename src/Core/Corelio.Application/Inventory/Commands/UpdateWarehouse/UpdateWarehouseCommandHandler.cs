using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Commands.UpdateWarehouse;

public class UpdateWarehouseCommandHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateWarehouseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await inventoryRepository.GetWarehouseByIdAsync(request.Id, cancellationToken);
        if (warehouse is null)
        {
            return Result<bool>.Failure(
                new Error("Warehouse.NotFound", $"Warehouse '{request.Id}' not found.", ErrorType.NotFound));
        }

        if (request.IsDefault && !warehouse.IsDefault)
        {
            await inventoryRepository.UnsetDefaultWarehouseAsync(cancellationToken);
        }

        warehouse.Name = request.Name;
        warehouse.Type = request.Type;
        warehouse.IsDefault = request.IsDefault;

        inventoryRepository.UpdateWarehouse(warehouse);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
