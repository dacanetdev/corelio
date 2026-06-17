using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Reports;
using Corelio.Domain.Enums;

namespace Corelio.BlazorApp.Services.Reports;

/// <summary>
/// HTTP client service for the sales report API.
/// </summary>
public interface ISalesReportHttpService
{
    Task<Result<SalesReportModel>> GetSalesReportAsync(
        DateTime dateFrom,
        DateTime dateTo,
        Guid? warehouseId = null,
        PaymentMethod? paymentMethod = null,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> ExportSalesReportAsync(
        string format,
        DateTime dateFrom,
        DateTime dateTo,
        Guid? warehouseId = null,
        PaymentMethod? paymentMethod = null,
        CancellationToken cancellationToken = default);
}
