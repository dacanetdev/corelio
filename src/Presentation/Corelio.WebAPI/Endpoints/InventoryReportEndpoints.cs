using System.Globalization;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Inventory.Common;
using Corelio.Application.Reports.Inventory.Queries.GetInventoryValuationReport;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Inventory valuation report endpoints.
/// </summary>
public static class InventoryReportEndpoints
{
    public static IEndpointRouteBuilder MapInventoryReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/reports/inventory")
            .WithTags("Reports")
            .RequireAuthorization();

        group.MapGet("/", GetInventoryValuationReport)
            .WithName("GetInventoryValuationReport")
            .WithSummary("Get inventory valuation report")
            .WithDescription("Returns total inventory value, per-category breakdown (sorted by value descending), full product-level detail (sorted by extended value descending), and low-stock alerts. Optionally filter by warehouseId or categoryId. Only items with Quantity > 0 are included in valuations.")
            .Produces<InventoryValuationReportDto>(200)
            .ProducesProblem(400)
            .RequireAuthorization("reports.view");

        group.MapGet("/export", ExportInventoryReport)
            .WithName("ExportInventoryReport")
            .WithSummary("Export inventory valuation report as PDF or CSV")
            .WithDescription("Generates and returns the inventory valuation report as a downloadable file. Use format=pdf for a formatted A4 PDF or format=csv for a multi-section CSV suitable for Excel. Same filters as GET /api/v1/reports/inventory apply.")
            .Produces(200)
            .ProducesProblem(400)
            .RequireAuthorization("reports.export");

        return app;
    }

    private static async Task<IResult> GetInventoryValuationReport(
        [FromQuery] Guid? warehouseId,
        [FromQuery] Guid? categoryId,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetInventoryValuationReportQuery(warehouseId, categoryId);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> ExportInventoryReport(
        [FromQuery] string format,
        [FromQuery] Guid? warehouseId,
        [FromQuery] Guid? categoryId,
        IMediator mediator,
        IInventoryReportExportService exportService,
        CancellationToken ct)
    {
        var query = new GetInventoryValuationReportQuery(warehouseId, categoryId);
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
            return Results.File(csv, "text/csv; charset=utf-8", $"inventario-{dateLabel}.csv");
        }

        var pdf = await exportService.GeneratePdfAsync(report, ct);
        return Results.File(pdf, "application/pdf", $"inventario-{dateLabel}.pdf");
    }
}
