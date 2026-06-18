using Corelio.Application.Common.Models;
using Corelio.Application.Reports.Dashboard.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Reports.Dashboard.Queries.GetDashboardSummary;

/// <summary>
/// Query to retrieve the dashboard KPI summary snapshot.
/// </summary>
public record GetDashboardSummaryQuery : IRequest<Result<DashboardSummaryDto>>;
