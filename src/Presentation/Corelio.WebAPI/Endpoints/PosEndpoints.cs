using Corelio.Application.Common.Interfaces;
using Corelio.Application.Sales.Commands.CancelSale;
using Corelio.Application.Sales.Commands.CompleteSale;
using Corelio.Application.Sales.Commands.CreateSale;
using Corelio.Application.Sales.Common;
using Corelio.Application.Sales.Queries.GetSaleById;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Point-of-Sale endpoints for real-time product search and sale processing.
/// </summary>
public static class PosEndpoints
{
    public static IEndpointRouteBuilder MapPosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/pos")
            .WithTags("POS")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/search", SearchProducts)
            .WithName("PosSearchProducts")
            .WithSummary("Search products by name, SKU, or barcode for POS")
            .RequireAuthorization("sales.create");

        group.MapPost("/sales", CreateSale)
            .WithName("CreateSale")
            .WithSummary("Create a new Draft sale")
            .RequireAuthorization("sales.create");

        group.MapGet("/sales/{id:guid}", GetSale)
            .WithName("GetPosSale")
            .WithSummary("Get sale details by ID")
            .RequireAuthorization("sales.create");

        group.MapPost("/sales/{id:guid}/complete", CompleteSale)
            .WithName("CompleteSale")
            .WithSummary("Complete a Draft sale with payment(s)")
            .RequireAuthorization("sales.create");

        group.MapDelete("/sales/{id:guid}", CancelSale)
            .WithName("CancelSale")
            .WithSummary("Cancel a Draft sale")
            .RequireAuthorization("sales.create");

        return app;
    }

    private static async Task<IResult> SearchProducts(
        [FromQuery] string q,
        [FromQuery] int limit,
        IPosSearchService posSearchService,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Results.Ok(Array.Empty<object>());
        }

        var results = await posSearchService.SearchProductsAsync(q, limit > 0 ? limit : 20, ct);
        return Results.Ok(results);
    }

    private static async Task<IResult> CreateSale(
        [FromBody] CreateSaleRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreateSaleCommand(
            request.Items.Select(i => new CartItemRequest(
                i.ProductId, i.Quantity, i.UnitPrice, i.DiscountPercentage)).ToList(),
            request.CustomerId,
            request.WarehouseId,
            request.Type,
            request.Notes);

        var result = await mediator.Send(command, ct);
        return result.ToCreatedResult(
            $"/api/v1/pos/sales/{result.Value}",
            new { saleId = result.Value });
    }

    private static async Task<IResult> GetSale(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetSaleByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CompleteSale(
        Guid id,
        [FromBody] CompleteSaleRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CompleteSaleCommand(
            id,
            request.Payments.Select(p => new PaymentRequest(p.Method, p.Amount, p.Reference)).ToList());

        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CancelSale(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CancelSaleCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }
}

/// <summary>Request body for creating a sale.</summary>
public record CreateSaleRequest(
    List<SaleItemLineRequest> Items,
    Guid? CustomerId = null,
    Guid? WarehouseId = null,
    SaleType Type = SaleType.Pos,
    string? Notes = null);

/// <summary>Single line item in a sale creation request.</summary>
public record SaleItemLineRequest(
    Guid ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage = 0);

/// <summary>Request body for completing a sale with payment(s).</summary>
public record CompleteSaleRequest(List<PaymentLineRequest> Payments);

/// <summary>Single payment entry in a complete sale request.</summary>
public record PaymentLineRequest(
    PaymentMethod Method,
    decimal Amount,
    string? Reference = null);
