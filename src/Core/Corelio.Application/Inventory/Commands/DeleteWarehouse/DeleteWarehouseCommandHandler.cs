using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Commands.DeleteWarehouse;

public class DeleteWarehouseCommandHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteWarehouseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await inventoryRepository.GetWarehouseByIdAsync(request.Id, cancellationToken);
        if (warehouse is null)
        {
            return Result<bool>.Failure(
                new Error("Warehouse.NotFound", $"Warehouse '{request.Id}' not found.", ErrorType.NotFound));
        }

        if (warehouse.IsDefault)
        {
            return Result<bool>.Failure(
                new Error("Warehouse.IsDefault", "Cannot delete the default warehouse.", ErrorType.Conflict));
        }

        var hasInventory = await inventoryRepository.WarehouseHasInventoryAsync(request.Id, cancellationToken);
        if (hasInventory)
        {
            return Result<bool>.Failure(
                new Error("Warehouse.HasInventory", "Cannot delete a warehouse that has inventory items.", ErrorType.Conflict));
        }

        inventoryRepository.DeleteWarehouse(warehouse);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
