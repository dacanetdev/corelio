using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Common;
using Corelio.Application.Sales.Commands.CancelSale;
using Corelio.Application.Sales.Commands.ConvertQuoteToSale;
using Corelio.Application.Sales.Queries.GenerateSaleReceipt;
using Corelio.Application.Sales.Queries.GetSaleById;
using Corelio.Application.Sales.Queries.GetSales;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Sales management endpoints (for reporting and history views).
/// </summary>
public static class SaleEndpoints
{
    public static IEndpointRouteBuilder MapSaleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/sales")
            .WithTags("Sales")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/", GetSales)
            .WithName("GetSales")
            .WithSummary("Get paged list of sales with optional filters")
            .WithDescription("Returns a paginated list of sales for the current tenant. Supports filtering by status (Draft, Completed, Cancelled), sale type (Pos, Quote), date range (dateFrom/dateTo), and a search term matching the folio number or customer name. Results default to page 1, 20 items per page, sorted by creation date descending.")
            .Produces<PagedResult<SaleListDto>>(200)
            .ProducesProblem(400)
            .RequireAuthorization("sales.view");

        group.MapGet("/{id:guid}", GetSaleById)
            .WithName("GetSaleById")
            .WithSummary("Get a sale by ID")
            .WithDescription("Returns the complete sale record including all line items, payment records, customer details, and folio number. Returns 404 if the sale does not exist or belongs to a different tenant.")
            .Produces<SaleDto>(200)
            .Produces(404)
            .RequireAuthorization("sales.view");

        group.MapGet("/{id:guid}/receipt", GetReceipt)
            .WithName("GetSaleReceipt")
            .WithSummary("Generate and download a PDF receipt for a sale")
            .WithDescription("Generates a PDF receipt (ticket) on-demand using QuestPDF and returns it as a file download (Content-Type: application/pdf). The receipt includes folio number, sale date, line items, totals, payment breakdown, and change due. Returns 404 if the sale is not found.")
            .Produces(200)
            .Produces(404)
            .RequireAuthorization("sales.view");

        group.MapDelete("/{id:guid}", CancelSale)
            .WithName("CancelSale")
            .WithSummary("Cancel a sale (restores inventory for completed sales)")
            .WithDescription("Cancels a sale by changing its status to Cancelled. For Completed sales, inventory quantities are automatically restored. For Draft sales, no inventory adjustment is needed. An optional cancellation reason can be provided via the `reason` query parameter. Returns 404 if not found, 409 if the sale is already cancelled.")
            .Produces(200)
            .Produces(404)
            .ProducesProblem(409)
            .RequireAuthorization("sales.cancel");

        group.MapPost("/{id:guid}/convert", ConvertQuote)
            .WithName("ConvertQuoteToSale")
            .WithSummary("Convert an open quote to sale (marks quote as cancelled; POS pre-loads cart)")
            .WithDescription("Converts an open Quote-type sale into a new Draft sale, pre-populated with the same line items and customer. The original quote is marked as Cancelled. The response includes the new Draft sale ID which can be loaded in the POS to complete payment. Returns 404 if the quote is not found, 422 if the quote is not in an open state.")
            .Produces<SaleDto>(200)
            .Produces(404)
            .ProducesProblem(422)
            .RequireAuthorization("sales.create");

        return app;
    }

    private static async Task<IResult> GetSales(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] SaleStatus? status,
        [FromQuery] string? searchTerm,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] SaleType? saleType,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetSalesQuery(
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 20,
            Status: status,
            SearchTerm: searchTerm,
            DateFrom: dateFrom,
            DateTo: dateTo,
            Type: saleType);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetSaleById(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetSaleByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetReceipt(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GenerateSaleReceiptQuery(id);
        var result = await mediator.Send(query, ct);
        if (!result.IsSuccess)
        {
            return result.Error!.Type == ErrorType.NotFound
                ? Results.NotFound(result.Error.Message)
                : Results.Problem(result.Error.Message);
        }

        return Results.File(
            result.Value!.PdfBytes,
            "application/pdf",
            $"Recibo_{result.Value.Folio}.pdf");
    }

    private static async Task<IResult> CancelSale(
        Guid id,
        [FromQuery] string? reason,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CancelSaleCommand(id, reason);
        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> ConvertQuote(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new ConvertQuoteToSaleCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }
}
