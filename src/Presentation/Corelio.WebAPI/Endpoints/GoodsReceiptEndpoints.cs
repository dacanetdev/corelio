using Corelio.Application.Common.Models;
using Corelio.Application.GoodsReceipts.Commands.ReceiveGoods;
using Corelio.Application.GoodsReceipts.Common;
using Corelio.Application.GoodsReceipts.Queries.GetGoodsReceiptById;
using Corelio.Application.GoodsReceipts.Queries.GetGoodsReceipts;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Goods receipt endpoints for recording stock arrivals against purchase orders.
/// </summary>
public static class GoodsReceiptEndpoints
{
    public static IEndpointRouteBuilder MapGoodsReceiptEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/goods-receipts")
            .WithTags("Goods Receipts")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/", GetGoodsReceipts)
            .WithName("GetGoodsReceipts")
            .WithSummary("Get paged list of goods receipts")
            .WithDescription("Returns a paginated list of goods receipts for the current tenant. Optionally filter by purchaseOrderId and/or received date range. Results are ordered by received date descending.")
            .Produces<PagedResult<GoodsReceiptListDto>>(200)
            .ProducesProblem(400)
            .RequireAuthorization("receipts.view");

        group.MapGet("/{id:guid}", GetGoodsReceiptById)
            .WithName("GetGoodsReceiptById")
            .WithSummary("Get goods receipt by ID")
            .WithDescription("Returns the complete goods receipt including all line items, product names, and warehouse details. Returns 404 if the receipt does not exist or belongs to a different tenant.")
            .Produces<GoodsReceiptDto>(200)
            .Produces(404)
            .RequireAuthorization("receipts.view");

        group.MapPost("/", ReceiveGoods)
            .WithName("ReceiveGoods")
            .WithSummary("Record goods receipt against a purchase order")
            .WithDescription("Records the physical arrival of goods against an Approved or PartiallyReceived purchase order. Atomically updates InventoryItem quantities in the specified warehouse and ReceivedQuantity on each PO line item. Transitions the PO to PartiallyReceived or Received. Over-receiving (more than ordered) returns 409.")
            .Produces<object>(201)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(409)
            .RequireAuthorization("receipts.create");

        return app;
    }

    private static async Task<IResult> GetGoodsReceipts(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] Guid? purchaseOrderId,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGoodsReceiptsQuery(
            Page: pageNumber > 0 ? pageNumber : 1,
            Size: pageSize > 0 ? pageSize : 20,
            PurchaseOrderId: purchaseOrderId,
            StartDate: startDate,
            EndDate: endDate);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetGoodsReceiptById(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGoodsReceiptByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> ReceiveGoods(
        [FromBody] ReceiveGoodsRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new ReceiveGoodsCommand(
            request.PurchaseOrderId,
            request.WarehouseId,
            request.ReceivedDate,
            request.Notes,
            request.Items.Select(i => new ReceiveGoodsItemRequest(i.PurchaseOrderItemId, i.QuantityReceived)).ToList());

        var result = await mediator.Send(command, ct);
        return result.ToCreatedResult(
            $"/api/v1/goods-receipts/{result.Value}",
            new { goodsReceiptId = result.Value });
    }
}

/// <summary>Request body for recording a goods receipt.</summary>
public record ReceiveGoodsRequest(
    Guid PurchaseOrderId,
    Guid WarehouseId,
    DateOnly ReceivedDate,
    string? Notes,
    IReadOnlyList<ReceiveGoodsItemRequestBody> Items);

/// <summary>A single line item in a goods receipt request.</summary>
public record ReceiveGoodsItemRequestBody(
    Guid PurchaseOrderItemId,
    decimal QuantityReceived);
