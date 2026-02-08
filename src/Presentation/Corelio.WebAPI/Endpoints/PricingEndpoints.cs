using Corelio.Application.Pricing.Commands.BulkUpdatePricing;
using Corelio.Application.Pricing.Commands.CalculatePrices;
using Corelio.Application.Pricing.Commands.UpdateProductPricing;
using Corelio.Application.Pricing.Commands.UpdateTenantPricingConfig;
using Corelio.Application.Pricing.Queries.GetProductPricing;
using Corelio.Application.Pricing.Queries.GetProductsPricingList;
using Corelio.Application.Pricing.Queries.GetTenantPricingConfig;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Contracts.Pricing;
using Corelio.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Pricing management endpoints for tenant configuration and product pricing.
/// </summary>
public static class PricingEndpoints
{
    /// <summary>
    /// Maps all pricing endpoints to the application.
    /// </summary>
    public static IEndpointRouteBuilder MapPricingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/pricing")
            .WithTags("Pricing")
            .RequireAuthorization();

        group.MapGet("/tenant-config", GetTenantConfig)
            .WithName("GetTenantPricingConfig")
            .WithSummary("Get the current tenant's pricing configuration")
            .RequireAuthorization("pricing.view");

        group.MapPut("/tenant-config", UpdateTenantConfig)
            .WithName("UpdateTenantPricingConfig")
            .WithSummary("Create or update the tenant's pricing configuration")
            .RequireAuthorization("pricing.manage");

        group.MapGet("/products", GetProductsPricing)
            .WithName("GetProductsPricing")
            .WithSummary("Get a paged list of products with their pricing information")
            .RequireAuthorization("pricing.view");

        group.MapGet("/products/{id:guid}", GetProductPricing)
            .WithName("GetProductPricing")
            .WithSummary("Get a single product's pricing information")
            .RequireAuthorization("pricing.view");

        group.MapPut("/products/{id:guid}", UpdateProductPricing)
            .WithName("UpdateProductPricing")
            .WithSummary("Update a product's pricing (list price, discounts, margin prices)")
            .RequireAuthorization("pricing.manage");

        group.MapPost("/calculate", CalculatePrices)
            .WithName("CalculatePrices")
            .WithSummary("Preview pricing calculations without persisting to database")
            .RequireAuthorization("pricing.view");

        group.MapPost("/bulk-update", BulkUpdatePricing)
            .WithName("BulkUpdatePricing")
            .WithSummary("Apply a bulk pricing update to multiple products")
            .RequireAuthorization("pricing.manage");

        return app;
    }

    private static async Task<IResult> GetTenantConfig(
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetTenantPricingConfigQuery();
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateTenantConfig(
        [FromBody] UpdateTenantPricingConfigRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateTenantPricingConfigCommand(
            DiscountTierCount: request.DiscountTierCount,
            MarginTierCount: request.MarginTierCount,
            DefaultIvaEnabled: request.DefaultIvaEnabled,
            IvaPercentage: request.IvaPercentage,
            DiscountTiers: request.DiscountTiers,
            MarginTiers: request.MarginTiers);

        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetProductsPricing(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? categoryId,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetProductsPricingListQuery(
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 20,
            SearchTerm: searchTerm,
            CategoryId: categoryId);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetProductPricing(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetProductPricingQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateProductPricing(
        Guid id,
        [FromBody] UpdateProductPricingRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateProductPricingCommand(
            ProductId: id,
            ListPrice: request.ListPrice,
            IvaEnabled: request.IvaEnabled,
            Discounts: request.Discounts,
            MarginPrices: request.MarginPrices);

        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CalculatePrices(
        [FromBody] CalculatePricesRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CalculatePricesCommand(
            ListPrice: request.ListPrice,
            Discounts: request.Discounts,
            IvaEnabled: request.IvaEnabled,
            IvaPercentage: request.IvaPercentage);

        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> BulkUpdatePricing(
        [FromBody] BulkUpdatePricingRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new BulkUpdatePricingCommand(
            ProductIds: request.ProductIds,
            UpdateType: request.UpdateType,
            Value: request.Value,
            TierNumber: request.TierNumber);

        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }
}
