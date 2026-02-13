using Corelio.Application.Products.Commands.CreateProduct;
using Corelio.Application.Products.Commands.DeleteProduct;
using Corelio.Application.Products.Commands.UpdateProduct;
using Corelio.Application.Products.Queries.GetProductById;
using Corelio.Application.Products.Queries.GetProducts;
using Corelio.Application.Products.Queries.SearchProducts;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Contracts.Products;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Product management endpoints for CRUD operations.
/// </summary>
public static class ProductEndpoints
{
    /// <summary>
    /// Maps all product endpoints to the application.
    /// </summary>
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/products")
            .WithTags("Products")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/", GetProducts)
            .WithName("GetProducts")
            .WithSummary("Get a paged list of products with optional filtering")
            .RequireAuthorization("products.view");

        group.MapGet("/{id:guid}", GetProductById)
            .WithName("GetProductById")
            .WithSummary("Get a product by its ID")
            .RequireAuthorization("products.view");

        group.MapGet("/search", SearchProducts)
            .WithName("SearchProducts")
            .WithSummary("Search products by barcode, SKU, or name (optimized for POS)")
            .RequireAuthorization("products.view");

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .RequireAuthorization("products.create");

        group.MapPut("/{id:guid}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product")
            .RequireAuthorization("products.update");

        group.MapDelete("/{id:guid}", DeleteProduct)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product (soft delete)")
            .RequireAuthorization("products.delete");

        return app;
    }

    private static async Task<IResult> GetProducts(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? categoryId,
        [FromQuery] bool? isActive,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetProductsQuery(
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 20,
            SearchTerm: searchTerm,
            CategoryId: categoryId,
            IsActive: isActive);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetProductById(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetProductByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> SearchProducts(
        [FromQuery] string q,
        [FromQuery] int limit,
        IMediator mediator,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "ValidationError",
                detail: "Search query cannot be empty.");
        }

        var query = new SearchProductsQuery(
            Query: q,
            Limit: limit > 0 ? limit : 20);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, ct);
        return result.ToCreatedResult(
            $"/api/v1/products/{result.Value}",
            new { productId = result.Value });
    }

    private static async Task<IResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = request.ToCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> DeleteProduct(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new DeleteProductCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }
}
