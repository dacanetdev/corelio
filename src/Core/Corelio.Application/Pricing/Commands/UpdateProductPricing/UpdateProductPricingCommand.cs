using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Commands.UpdateProductPricing;

/// <summary>
/// Command to update a product's pricing (list price, discounts, and margin prices).
/// </summary>
public record UpdateProductPricingCommand(
    Guid ProductId,
    decimal? ListPrice,
    bool IvaEnabled,
    List<UpdateProductDiscountDto> Discounts,
    List<UpdateProductMarginPriceDto> MarginPrices) : IRequest<Result<ProductPricingDto>>;
