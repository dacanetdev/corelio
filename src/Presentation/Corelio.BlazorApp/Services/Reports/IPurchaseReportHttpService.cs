using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Reports;

namespace Corelio.BlazorApp.Services.Reports;

/// <summary>
/// HTTP service for the purchase summary report API.
/// </summary>
public interface IPurchaseReportHttpService
{
    Task<Result<PurchaseSummaryReportModel>> GetPurchaseSummaryReportAsync(
        DateTime dateFrom,
        DateTime dateTo,
        Guid? supplierId = null,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> ExportPurchaseReportAsync(
        string format,
        DateTime dateFrom,
        DateTime dateTo,
        Guid? supplierId = null,
        CancellationToken cancellationToken = default);
}
