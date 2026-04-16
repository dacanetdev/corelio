using Corelio.Application.Common.Models;
using Corelio.Application.Products.Common;
using Corelio.Application.Products.Commands.CreateProduct;
using Corelio.Application.Products.Commands.DeleteProduct;
using Corelio.Application.Products.Commands.UpdateProduct;
using Corelio.Application.Products.Queries.GetProductById;
using Corelio.Application.Products.Queries.GetProducts;
using Corelio.Application.Products.Queries.SearchProducts;
using Corelio.SharedKernel.Messaging;
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
            .WithDescription("Returns a paginated list of products for the current tenant. Supports filtering by category ID, active status, and a free-text search term (matches name, SKU, or barcode). Results default to page 1 with 20 items per page.")
            .Produces<PagedResult<ProductListDto>>(200)
            .ProducesProblem(400)
            .RequireAuthorization("products.view");

        group.MapGet("/{id:guid}", GetProductById)
            .WithName("GetProductById")
            .WithSummary("Get a product by its ID")
            .WithDescription("Returns the full product record including pricing details, SAT classification codes, and category assignment. Returns 404 if the product does not exist or belongs to a different tenant.")
            .Produces<ProductDto>(200)
            .Produces(404)
            .RequireAuthorization("products.view");

        group.MapGet("/search", SearchProducts)
            .WithName("SearchProducts")
            .WithSummary("Search products by barcode, SKU, or name (optimized for POS)")
            .WithDescription("Fast product search intended for the POS screen. Results are cached in Redis (TTL: 5 min, version-based invalidation on product mutations). Requires query parameter `q` (minimum 1 character). Returns up to `limit` results (default: 20).")
            .Produces<List<ProductListDto>>(200)
            .ProducesProblem(400)
            .RequireAuthorization("products.view");

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .WithDescription("Creates a new active product in the current tenant's catalog. SKU must be unique within the tenant. Invalidates the Redis POS search cache upon success. Returns the new product ID and a 201 Created response with the resource location.")
            .Produces<object>(201)
            .ProducesProblem(400)
            .RequireAuthorization("products.create");

        group.MapPut("/{id:guid}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product")
            .WithDescription("Updates all mutable fields of an existing product. Returns 204 No Content on success. Invalidates the Redis POS search cache for the tenant. Returns 404 if the product is not found.")
            .Produces(204)
            .Produces(404)
            .ProducesProblem(400)
            .RequireAuthorization("products.update");

        group.MapDelete("/{id:guid}", DeleteProduct)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product (soft delete)")
            .WithDescription("Soft-deletes a product by setting its `IsActive` flag to false. The product is removed from POS search results and the active catalog, but its data is retained for sales history and audit purposes. Returns 204 No Content on success.")
            .Produces(204)
            .Produces(404)
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
