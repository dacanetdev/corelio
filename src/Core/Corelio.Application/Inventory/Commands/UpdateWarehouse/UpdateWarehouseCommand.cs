using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Commands.UpdateWarehouse;

public record UpdateWarehouseCommand(Guid Id, string Name, WarehouseType Type, bool IsDefault)
    : IRequest<Result<bool>>;
