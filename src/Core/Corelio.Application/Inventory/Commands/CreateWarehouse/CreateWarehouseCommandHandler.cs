using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Commands.CreateWarehouse;

public class CreateWarehouseCommandHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<CreateWarehouseCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        if (request.IsDefault)
        {
            await inventoryRepository.UnsetDefaultWarehouseAsync(cancellationToken);
        }

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            Name = request.Name,
            Type = request.Type,
            IsDefault = request.IsDefault
        };

        await inventoryRepository.AddWarehouseAsync(warehouse, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(warehouse.Id);
    }
}
