using Corelio.Application.Reports.Purchases.Common;
using Corelio.Domain.Enums;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Aggregation service for purchase summary reporting. Implemented by Infrastructure using EF Core.
/// </summary>
public interface IPurchaseSummaryQueryService
{
    /// <summary>
    /// Returns aggregated purchase KPIs, supplier spending breakdown, status breakdown,
    /// and product receipt summary for the given filters.
    /// </summary>
    Task<PurchaseSummaryReportDto> GetPurchaseSummaryReportAsync(
        DateTime dateFrom,
        DateTime dateTo,
        Guid? supplierId = null,
        PurchaseOrderStatus? status = null,
        CancellationToken cancellationToken = default);
}
