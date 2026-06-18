using Corelio.Application.Reports.Dashboard.Common;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Aggregation service for dashboard KPI summary. Implemented by Infrastructure using EF Core.
/// </summary>
public interface IDashboardSummaryService
{
    /// <summary>
    /// Returns a lightweight snapshot of today's sales total, transaction count,
    /// low-stock product count, and pending purchase order count.
    /// </summary>
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
}
