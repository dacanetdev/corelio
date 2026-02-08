namespace Corelio.Application.Pricing.Common;

/// <summary>
/// DTO representing a product's full pricing information including discounts and margin prices.
/// </summary>
public record ProductPricingDto(
    Guid ProductId,
    string ProductName,
    string Sku,
    decimal? ListPrice,
    decimal? NetCost,
    bool IvaEnabled,
    List<ProductDiscountDto> Discounts,
    List<ProductMarginPriceDto> MarginPrices);
