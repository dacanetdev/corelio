using Corelio.Application.Common.Models;
using Corelio.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;
using Corelio.Application.PurchaseOrders.Commands.CancelPurchaseOrder;
using Corelio.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
using Corelio.Application.PurchaseOrders.Commands.SubmitPurchaseOrder;
using Corelio.Application.PurchaseOrders.Commands.UpdatePurchaseOrder;
using Corelio.Application.PurchaseOrders.Common;
using Corelio.Application.PurchaseOrders.Queries.GetPurchaseOrderById;
using Corelio.Application.PurchaseOrders.Queries.GetPurchaseOrders;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Purchase order lifecycle endpoints.
/// </summary>
public static class PurchaseOrderEndpoints
{
    public static IEndpointRouteBuilder MapPurchaseOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/purchase-orders")
            .WithTags("Purchase Orders")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/", GetPurchaseOrders)
            .WithName("GetPurchaseOrders")
            .WithSummary("Get paged list of purchase orders")
            .WithDescription("Returns a paginated list of purchase orders for the current tenant. Optionally filter by status, supplier, and date range. Results are ordered by creation date descending.")
            .Produces<PagedResult<PurchaseOrderListDto>>(200)
            .ProducesProblem(400)
            .RequireAuthorization("purchases.view");

        group.MapGet("/{id:guid}", GetPurchaseOrderById)
            .WithName("GetPurchaseOrderById")
            .WithSummary("Get purchase order by ID")
            .WithDescription("Returns the complete purchase order including header, line items, and supplier details. Returns 404 if the order does not exist or belongs to a different tenant.")
            .Produces<PurchaseOrderDto>(200)
            .Produces(404)
            .RequireAuthorization("purchases.view");

        group.MapPost("/", CreatePurchaseOrder)
            .WithName("CreatePurchaseOrder")
            .WithSummary("Create a new purchase order")
            .WithDescription("Creates a new purchase order in Draft status. At least one line item is required. OrderNumber is auto-generated as PO-{YYYY}-{NNNN}. Returns 201 with the new purchase order ID.")
            .Produces<object>(201)
            .ProducesProblem(400)
            .RequireAuthorization("purchases.create");

        group.MapPut("/{id:guid}", UpdatePurchaseOrder)
            .WithName("UpdatePurchaseOrder")
            .WithSummary("Update a draft purchase order")
            .WithDescription("Updates header fields and replaces all line items on a Draft purchase order. Totals are recalculated server-side. Returns 409 if the order is not in Draft status.")
            .Produces(204)
            .Produces(404)
            .ProducesProblem(400)
            .ProducesProblem(409)
            .RequireAuthorization("purchases.create");

        group.MapPost("/{id:guid}/submit", SubmitPurchaseOrder)
            .WithName("SubmitPurchaseOrder")
            .WithSummary("Submit a purchase order")
            .WithDescription("Transitions a Draft purchase order to Submitted status, indicating it has been sent to the supplier. Returns 409 if the order is not in Draft status.")
            .Produces(204)
            .Produces(404)
            .ProducesProblem(409)
            .RequireAuthorization("purchases.submit");

        group.MapPost("/{id:guid}/approve", ApprovePurchaseOrder)
            .WithName("ApprovePurchaseOrder")
            .WithSummary("Approve a submitted purchase order")
            .WithDescription("Transitions a Submitted purchase order to Approved status, enabling goods receipt. Returns 409 if the order is not in Submitted status.")
            .Produces(204)
            .Produces(404)
            .ProducesProblem(409)
            .RequireAuthorization("purchases.approve");

        group.MapPost("/{id:guid}/cancel", CancelPurchaseOrder)
            .WithName("CancelPurchaseOrder")
            .WithSummary("Cancel a purchase order")
            .WithDescription("Cancels a Draft or Submitted purchase order. Returns 409 if the order is Approved, PartiallyReceived, or Received (cannot be cancelled once approved).")
            .Produces(204)
            .Produces(404)
            .ProducesProblem(409)
            .RequireAuthorization("purchases.cancel");

        return app;
    }

    private static async Task<IResult> GetPurchaseOrders(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] PurchaseOrderStatus? status,
        [FromQuery] Guid? supplierId,
        [FromQuery] DateTimeOffset? startDate,
        [FromQuery] DateTimeOffset? endDate,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPurchaseOrdersQuery(
            Page: pageNumber > 0 ? pageNumber : 1,
            Size: pageSize > 0 ? pageSize : 20,
            Status: status,
            SupplierId: supplierId,
            StartDate: startDate,
            EndDate: endDate);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetPurchaseOrderById(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPurchaseOrderByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreatePurchaseOrder(
        [FromBody] CreatePurchaseOrderRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreatePurchaseOrderCommand(
            request.SupplierId,
            request.ExpectedDate,
            request.Notes,
            request.Items.Select(i => new PurchaseOrderItemRequest(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList());

        var result = await mediator.Send(command, ct);
        return result.ToCreatedResult(
            $"/api/v1/purchase-orders/{result.Value}",
            new { purchaseOrderId = result.Value });
    }

    private static async Task<IResult> UpdatePurchaseOrder(
        Guid id,
        [FromBody] UpdatePurchaseOrderRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdatePurchaseOrderCommand(
            id,
            request.SupplierId,
            request.ExpectedDate,
            request.Notes,
            request.Items.Select(i => new PurchaseOrderItemRequest(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList());

        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> SubmitPurchaseOrder(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new SubmitPurchaseOrderCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> ApprovePurchaseOrder(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new ApprovePurchaseOrderCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> CancelPurchaseOrder(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CancelPurchaseOrderCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }
}

/// <summary>Line item within a create/update purchase order request.</summary>
public record PurchaseOrderItemLineRequest(
    Guid ProductId,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice);

/// <summary>Request body for creating a purchase order.</summary>
public record CreatePurchaseOrderRequest(
    Guid SupplierId,
    DateTimeOffset? ExpectedDate,
    string? Notes,
    List<PurchaseOrderItemLineRequest> Items);

/// <summary>Request body for updating a purchase order.</summary>
public record UpdatePurchaseOrderRequest(
    Guid SupplierId,
    DateTimeOffset? ExpectedDate,
    string? Notes,
    List<PurchaseOrderItemLineRequest> Items);
