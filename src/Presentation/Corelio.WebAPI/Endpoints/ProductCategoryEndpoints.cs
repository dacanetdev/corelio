using Corelio.Application.ProductCategories.Commands.CreateCategory;
using Corelio.Application.ProductCategories.Commands.DeleteCategory;
using Corelio.Application.ProductCategories.Commands.UpdateCategory;
using Corelio.Application.ProductCategories.Queries.GetCategories;
using Corelio.Application.ProductCategories.Queries.GetCategoryById;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Contracts.ProductCategories;
using Corelio.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Product category management endpoints for CRUD operations.
/// </summary>
public static class ProductCategoryEndpoints
{
    /// <summary>
    /// Maps all product category endpoints to the application.
    /// </summary>
    public static IEndpointRouteBuilder MapProductCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/product-categories")
            .WithTags("Product Categories")
            .RequireAuthorization();

        group.MapGet("/", GetCategories)
            .WithName("GetCategories")
            .WithSummary("Get all product categories")
            .RequireAuthorization("products.view");

        group.MapGet("/{id:guid}", GetCategoryById)
            .WithName("GetCategoryById")
            .WithSummary("Get a product category by its ID")
            .RequireAuthorization("products.view");

        group.MapPost("/", CreateCategory)
            .WithName("CreateCategory")
            .WithSummary("Create a new product category")
            .RequireAuthorization("products.create");

        group.MapPut("/{id:guid}", UpdateCategory)
            .WithName("UpdateCategory")
            .WithSummary("Update an existing product category")
            .RequireAuthorization("products.update");

        group.MapDelete("/{id:guid}", DeleteCategory)
            .WithName("DeleteCategory")
            .WithSummary("Delete a product category (soft delete)")
            .RequireAuthorization("products.delete");

        return app;
    }

    private static async Task<IResult> GetCategories(
        [FromQuery] bool includeInactive,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetCategoriesQuery(includeInactive);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetCategoryById(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToCreatedResult(
            $"/api/v1/product-categories/{result.Value}",
            new { categoryId = result.Value });
    }

    private static async Task<IResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(
            Id: id,
            Name: request.Name,
            Description: request.Description,
            ImageUrl: request.ImageUrl,
            ParentCategoryId: request.ParentCategoryId,
            SortOrder: request.SortOrder,
            ColorHex: request.ColorHex,
            IconName: request.IconName,
            IsActive: request.IsActive);

        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> DeleteCategory(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }
}
