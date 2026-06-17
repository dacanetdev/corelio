using Corelio.Application.Common.Models;
using Corelio.Application.Reports.Inventory.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Reports.Inventory.Queries.GetInventoryValuationReport;

/// <summary>
/// Query to retrieve aggregated inventory valuation data, optionally scoped to a warehouse or category.
/// </summary>
public sealed record GetInventoryValuationReportQuery(
    Guid? WarehouseId = null,
    Guid? CategoryId = null
) : IRequest<Result<InventoryValuationReportDto>>;
