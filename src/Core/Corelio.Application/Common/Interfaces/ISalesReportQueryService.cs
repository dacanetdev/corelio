using Corelio.Application.Reports.Sales.Common;
using Corelio.Domain.Enums;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Aggregation service for sales reporting. Implemented by Infrastructure using EF Core.
/// </summary>
public interface ISalesReportQueryService
{
    /// <summary>
    /// Returns aggregated sales KPIs, payment breakdown, top products, hourly distribution,
    /// and cashier breakdown for the given filters.
    /// Only completed sales are included.
    /// </summary>
    Task<SalesReportDto> GetSalesReportAsync(
        DateTime dateFrom,
        DateTime dateTo,
        Guid? warehouseId = null,
        PaymentMethod? paymentMethod = null,
        CancellationToken cancellationToken = default);
}
