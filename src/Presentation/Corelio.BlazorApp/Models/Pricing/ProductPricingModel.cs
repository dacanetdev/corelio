namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Blazor model for a product's full pricing information.
/// </summary>
public record ProductPricingModel(
    Guid ProductId,
    string ProductName,
    string Sku,
    decimal? ListPrice,
    decimal? NetCost,
    bool IvaEnabled,
    List<ProductDiscountModel> Discounts,
    List<ProductMarginPriceModel> MarginPrices);
