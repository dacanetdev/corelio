using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Reports;

namespace Corelio.BlazorApp.Services.Reports;

/// <summary>
/// HTTP service for the inventory valuation report API.
/// </summary>
public interface IInventoryReportHttpService
{
    Task<Result<InventoryValuationReportModel>> GetInventoryValuationReportAsync(
        Guid? warehouseId = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> ExportInventoryReportAsync(
        string format,
        Guid? warehouseId = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);
}
