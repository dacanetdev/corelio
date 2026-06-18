using Corelio.BlazorApp.Models.Common;
using Corelio.BlazorApp.Models.Reports;

namespace Corelio.BlazorApp.Services.Reports;

/// <summary>
/// HTTP client service for the dashboard KPI summary API.
/// </summary>
public interface IDashboardHttpService
{
    Task<Result<DashboardSummaryModel>> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default);
}
