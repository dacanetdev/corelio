using Corelio.Application.Inventory.Commands.AdjustStock;
using Corelio.Application.Inventory.Queries.GetInventoryItems;
using Corelio.Application.Inventory.Queries.GetInventoryTransactions;
using Corelio.Application.Inventory.Queries.GetWarehouses;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Inventory management endpoints.
/// </summary>
public static class InventoryEndpoints
{
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/inventory")
            .WithTags("Inventory")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/", GetInventoryItems)
            .WithName("GetInventoryItems")
            .WithSummary("Get paged list of inventory items with optional filters")
            .RequireAuthorization("inventory.view");

        group.MapGet("/warehouses", GetWarehouses)
            .WithName("GetWarehouses")
            .WithSummary("Get all warehouses for the current tenant")
            .RequireAuthorization("inventory.view");

        group.MapGet("/{id:guid}/transactions", GetTransactions)
            .WithName("GetInventoryTransactions")
            .WithSummary("Get transaction history for a specific inventory item")
            .RequireAuthorization("inventory.view");

        group.MapPost("/adjustments", AdjustStock)
            .WithName("AdjustStock")
            .WithSummary("Manually adjust stock for an inventory item")
            .RequireAuthorization("inventory.adjust");

        return app;
    }

    private static async Task<IResult> GetInventoryItems(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] Guid? warehouseId,
        [FromQuery] bool lowStockOnly,
        [FromQuery] string? searchTerm,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetInventoryItemsQuery(
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 20,
            WarehouseId: warehouseId,
            LowStockOnly: lowStockOnly,
            SearchTerm: searchTerm);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetWarehouses(
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetWarehousesQuery();
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetTransactions(
        Guid id,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetInventoryTransactionsQuery(
            InventoryItemId: id,
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 50);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> AdjustStock(
        AdjustStockCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }
}
