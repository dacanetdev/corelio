using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Commands.CreateWarehouse;

public record CreateWarehouseCommand(string Name, WarehouseType Type, bool IsDefault)
    : IRequest<Result<Guid>>;
