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
            .RequireAuthorization("sales.view");

        group.MapGet("/{id:guid}", GetSaleById)
            .WithName("GetSaleById")
            .WithSummary("Get a sale by ID")
            .RequireAuthorization("sales.view");

        return app;
    }

    private static async Task<IResult> GetSales(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] SaleStatus? status,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetSalesQuery(
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 20,
            Status: status,
            DateFrom: dateFrom,
            DateTo: dateTo);

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
}
