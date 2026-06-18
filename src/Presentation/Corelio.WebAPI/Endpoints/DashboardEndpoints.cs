using Corelio.Application.Reports.Dashboard.Common;
using Corelio.Application.Reports.Dashboard.Queries.GetDashboardSummary;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Dashboard KPI summary endpoint — returns a lightweight operational snapshot
/// visible to any authenticated user.
/// </summary>
public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/reports/dashboard")
            .WithTags("Dashboard")
            .RequireAuthorization();

        group.MapGet("/", GetDashboardSummary)
            .WithName("GetDashboardSummary")
            .WithSummary("Get dashboard KPI summary")
            .WithDescription("Returns today's sales total and transaction count, the number of products below their minimum stock level, and the count of purchase orders in Approved or PartiallyReceived status. Available to any authenticated user — no specific permission required.")
            .Produces<DashboardSummaryDto>(200);

        return app;
    }

    private static async Task<IResult> GetDashboardSummary(
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetDashboardSummaryQuery(), ct);
        return result.ToHttpResult();
    }
}
