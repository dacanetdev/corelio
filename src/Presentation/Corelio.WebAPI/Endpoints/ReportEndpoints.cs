using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Sales.Common;
using Corelio.Application.Reports.Sales.Queries.GetSalesReport;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Reporting and analytics endpoints.
/// </summary>
public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/reports")
            .WithTags("Reports")
            .RequireAuthorization();

        group.MapGet("/sales", GetSalesReport)
            .WithName("GetSalesReport")
            .WithSummary("Get aggregated sales report for a date range")
            .WithDescription("Returns KPIs (total amount, transaction count, average ticket), payment method breakdown, top-10 products by quantity, hourly sales distribution (24 slots), and cashier breakdown. Only Completed sales are included. Optionally filter by warehouseId or paymentMethod.")
            .Produces<SalesReportDto>(200)
            .ProducesProblem(400)
            .RequireAuthorization("reports.view");

        group.MapGet("/sales/export", ExportSalesReport)
            .WithName("ExportSalesReport")
            .WithSummary("Export sales report as PDF or CSV")
            .WithDescription("Generates and returns the sales report as a downloadable file. Use format=pdf for a formatted A4 PDF or format=csv for a multi-section CSV suitable for Excel. Same filters as GET /api/v1/reports/sales apply.")
            .Produces(200)
            .ProducesProblem(400)
            .RequireAuthorization("reports.export");

        return app;
    }

    private static async Task<IResult> GetSalesReport(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? warehouseId,
        [FromQuery] PaymentMethod? paymentMethod,
        IMediator mediator,
        CancellationToken ct)
    {
        var from = dateFrom ?? DateTime.UtcNow.Date;
        var to = dateTo ?? DateTime.UtcNow.Date;

        if (from > to)
        {
            return Results.BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        var query = new GetSalesReportQuery(from, to, warehouseId, paymentMethod);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> ExportSalesReport(
        [FromQuery] string format,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? warehouseId,
        [FromQuery] PaymentMethod? paymentMethod,
        IMediator mediator,
        ISalesReportExportService exportService,
        CancellationToken ct)
    {
        var from = dateFrom ?? DateTime.UtcNow.Date;
        var to = dateTo ?? DateTime.UtcNow.Date;

        if (from > to)
        {
            return Results.BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        var query = new GetSalesReportQuery(from, to, warehouseId, paymentMethod);
        var result = await mediator.Send(query, ct);

        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }

        var report = result.Value!;
        var dateLabel = $"{from:yyyyMMdd}-{to:yyyyMMdd}";

        if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
        {
            var csv = await exportService.GenerateCsvAsync(report, ct);
            return Results.File(csv, "text/csv; charset=utf-8", $"Ventas_{dateLabel}.csv");
        }

        var pdf = await exportService.GeneratePdfAsync(report, ct);
        return Results.File(pdf, "application/pdf", $"Ventas_{dateLabel}.pdf");
    }
}
