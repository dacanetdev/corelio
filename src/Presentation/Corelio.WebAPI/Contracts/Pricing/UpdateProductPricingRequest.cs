using Corelio.Application.Pricing.Common;

namespace Corelio.WebAPI.Contracts.Pricing;

/// <summary>
/// Request DTO for updating a product's pricing (list price, discounts, margin prices).
/// ProductId is provided via the route parameter.
/// </summary>
public record UpdateProductPricingRequest(
    decimal? ListPrice,
    bool IvaEnabled,
    List<UpdateProductDiscountDto> Discounts,
    List<UpdateProductMarginPriceDto> MarginPrices);
