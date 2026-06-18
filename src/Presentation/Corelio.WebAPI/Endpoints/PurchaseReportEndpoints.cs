using System.Globalization;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Purchases.Common;
using Corelio.Application.Reports.Purchases.Queries.GetPurchaseSummaryReport;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Purchase summary report endpoints.
/// </summary>
public static class PurchaseReportEndpoints
{
    public static IEndpointRouteBuilder MapPurchaseReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/reports/purchases")
            .WithTags("Reports")
            .RequireAuthorization();

        group.MapGet("/", GetPurchaseSummaryReport)
            .WithName("GetPurchaseSummaryReport")
            .WithSummary("Get aggregated purchase summary report for a date range")
            .WithDescription("Returns total orders, total amount spent (Approved/PartiallyReceived/Received orders), pending deliveries count, spending breakdown by supplier (descending by amount), status breakdown for all orders, and product-level ordered vs. received quantities. Optionally filter by supplierId or status.")
            .Produces<PurchaseSummaryReportDto>(200)
            .ProducesProblem(400)
            .RequireAuthorization("reports.view");

        group.MapGet("/export", ExportPurchaseReport)
            .WithName("ExportPurchaseReport")
            .WithSummary("Export purchase summary report as PDF or CSV")
            .WithDescription("Generates and returns the purchase summary report as a downloadable file. Use format=pdf for a formatted A4 PDF or format=csv for a multi-section CSV suitable for Excel. Same filters as GET /api/v1/reports/purchases apply.")
            .Produces(200)
            .ProducesProblem(400)
            .RequireAuthorization("reports.export");

        return app;
    }

    private static async Task<IResult> GetPurchaseSummaryReport(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? supplierId,
        [FromQuery] int? status,
        IMediator mediator,
        CancellationToken ct)
    {
        var from = dateFrom ?? DateTime.UtcNow.Date.AddDays(-30);
        var to = dateTo ?? DateTime.UtcNow.Date;

        if (from > to)
        {
            return Results.BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        PurchaseOrderStatus? parsedStatus = status.HasValue ? (PurchaseOrderStatus)status.Value : null;

        var query = new GetPurchaseSummaryReportQuery(from, to, supplierId, parsedStatus);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> ExportPurchaseReport(
        [FromQuery] string format,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? supplierId,
        [FromQuery] int? status,
        IMediator mediator,
        IPurchaseReportExportService exportService,
        CancellationToken ct)
    {
        var from = dateFrom ?? DateTime.UtcNow.Date.AddDays(-30);
        var to = dateTo ?? DateTime.UtcNow.Date;

        if (from > to)
        {
            return Results.BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        PurchaseOrderStatus? parsedStatus = status.HasValue ? (PurchaseOrderStatus)status.Value : null;

        var query = new GetPurchaseSummaryReportQuery(from, to, supplierId, parsedStatus);
        var result = await mediator.Send(query, ct);

        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }

        var report = result.Value!;
        var dateLabel = report.GeneratedAt.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
        {
            var csv = await exportService.GenerateCsvAsync(report, ct);
            return Results.File(csv, "text/csv; charset=utf-8", $"compras-{dateLabel}.csv");
        }

        var pdf = await exportService.GeneratePdfAsync(report, ct);
        return Results.File(pdf, "application/pdf", $"compras-{dateLabel}.pdf");
    }
}
