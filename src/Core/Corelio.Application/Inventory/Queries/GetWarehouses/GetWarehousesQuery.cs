using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Inventory.Queries.GetWarehouses;

/// <summary>
/// Query to get all warehouses for the current tenant.
/// </summary>
public record GetWarehousesQuery : IRequest<Result<List<WarehouseDto>>>;
