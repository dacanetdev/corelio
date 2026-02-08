using Corelio.Application.Products.Commands.CreateProduct;
using Corelio.Application.Products.Commands.DeleteProduct;
using Corelio.Application.Products.Commands.UpdateProduct;
using Corelio.Application.Products.Queries.GetProductById;
using Corelio.Application.Products.Queries.GetProducts;
using Corelio.Application.Products.Queries.SearchProducts;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Contracts.Products;
using Corelio.WebAPI.Extensions;
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
            .RequireAuthorization();

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
        [FromBody] CreateProductCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
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
        var command = new UpdateProductCommand(
            Id: id,
            Sku: request.Sku,
            Name: request.Name,
            SalePrice: request.SalePrice,
            UnitOfMeasure: request.UnitOfMeasure,
            CategoryId: request.CategoryId,
            Barcode: request.Barcode,
            BarcodeType: request.BarcodeType,
            Description: request.Description,
            ShortDescription: request.ShortDescription,
            Brand: request.Brand,
            Manufacturer: request.Manufacturer,
            ModelNumber: request.ModelNumber,
            CostPrice: request.CostPrice,
            WholesalePrice: request.WholesalePrice,
            Msrp: request.Msrp,
            TaxRate: request.TaxRate,
            IsTaxExempt: request.IsTaxExempt,
            TrackInventory: request.TrackInventory,
            MinStockLevel: request.MinStockLevel,
            MaxStockLevel: request.MaxStockLevel,
            ReorderPoint: request.ReorderPoint,
            ReorderQuantity: request.ReorderQuantity,
            WeightKg: request.WeightKg,
            LengthCm: request.LengthCm,
            WidthCm: request.WidthCm,
            HeightCm: request.HeightCm,
            VolumeCm3: request.VolumeCm3,
            SatProductCode: request.SatProductCode,
            SatUnitCode: request.SatUnitCode,
            SatHazardousMaterial: request.SatHazardousMaterial,
            PrimaryImageUrl: request.PrimaryImageUrl,
            ImagesJson: request.ImagesJson,
            IsService: request.IsService,
            IsBundle: request.IsBundle,
            IsVariantParent: request.IsVariantParent,
            IsActive: request.IsActive,
            IsFeatured: request.IsFeatured);

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
