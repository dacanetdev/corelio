using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Reports.Dashboard.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Reports.Dashboard.Queries.GetDashboardSummary;

/// <summary>
/// Handler for GetDashboardSummaryQuery — delegates aggregation to IDashboardSummaryService.
/// </summary>
public class GetDashboardSummaryQueryHandler(IDashboardSummaryService dashboardSummaryService)
    : IRequestHandler<GetDashboardSummaryQuery, Result<DashboardSummaryDto>>
{
    public async Task<Result<DashboardSummaryDto>> Handle(
        GetDashboardSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var summary = await dashboardSummaryService.GetDashboardSummaryAsync(cancellationToken);
        return Result<DashboardSummaryDto>.Success(summary);
    }
}
