using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Reports.Inventory.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Reports.Inventory.Queries.GetInventoryValuationReport;

/// <summary>
/// Handler for GetInventoryValuationReportQuery — delegates aggregation to IInventoryValuationQueryService.
/// </summary>
public sealed class GetInventoryValuationReportQueryHandler(IInventoryValuationQueryService inventoryValuationQueryService)
    : IRequestHandler<GetInventoryValuationReportQuery, Result<InventoryValuationReportDto>>
{
    public async Task<Result<InventoryValuationReportDto>> Handle(
        GetInventoryValuationReportQuery request,
        CancellationToken cancellationToken)
    {
        var report = await inventoryValuationQueryService.GetInventoryValuationReportAsync(
            request.WarehouseId,
            request.CategoryId,
            cancellationToken);

        return Result<InventoryValuationReportDto>.Success(report);
    }
}
