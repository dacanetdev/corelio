using Corelio.Application.Common.Models;
using Corelio.Application.Inventory.Commands.CreateWarehouse;
using Corelio.Application.Inventory.Commands.DeleteWarehouse;
using Corelio.Application.Inventory.Commands.UpdateWarehouse;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Filters;

namespace Corelio.WebAPI.Endpoints;

public static class WarehouseEndpoints
{
    public static IEndpointRouteBuilder MapWarehouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/warehouses")
            .WithTags("Warehouses")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapPost("/", CreateWarehouse)
            .WithName("CreateWarehouse")
            .WithSummary("Create a new warehouse")
            .Produces<Guid>(201)
            .ProducesProblem(400)
            .RequireAuthorization("warehouses.create");

        group.MapPut("/{id:guid}", UpdateWarehouse)
            .WithName("UpdateWarehouse")
            .WithSummary("Update warehouse name, type, and default status")
            .Produces<bool>(200)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .RequireAuthorization("warehouses.update");

        group.MapDelete("/{id:guid}", DeleteWarehouse)
            .WithName("DeleteWarehouse")
            .WithSummary("Delete a non-default warehouse with no inventory")
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(404)
            .RequireAuthorization("warehouses.delete");

        return app;
    }

    private static async Task<IResult> CreateWarehouse(
        CreateWarehouseRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreateWarehouseCommand(request.Name, request.Type, request.IsDefault);
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/v1/warehouses/{result.Value}", result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> UpdateWarehouse(
        Guid id,
        UpdateWarehouseRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateWarehouseCommand(id, request.Name, request.Type, request.IsDefault);
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error?.Type == ErrorType.NotFound
                ? Results.NotFound(result.Error)
                : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> DeleteWarehouse(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new DeleteWarehouseCommand(id);
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? Results.NoContent()
            : result.Error?.Type == ErrorType.NotFound
                ? Results.NotFound(result.Error)
                : Results.BadRequest(result.Error);
    }

    private record CreateWarehouseRequest(string Name, WarehouseType Type, bool IsDefault);
    private record UpdateWarehouseRequest(string Name, WarehouseType Type, bool IsDefault);
}
