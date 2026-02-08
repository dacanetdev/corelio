using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.Application.Services;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Commands.UpdateProductPricing;

/// <summary>
/// Handler for updating a product's pricing including discounts and margin prices.
/// </summary>
public class UpdateProductPricingCommandHandler(
    IProductPricingRepository productPricingRepository,
    ITenantPricingConfigurationRepository configRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<UpdateProductPricingCommand, Result<ProductPricingDto>>
{
    public async Task<Result<ProductPricingDto>> Handle(
        UpdateProductPricingCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<ProductPricingDto>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var product = await productPricingRepository.GetProductPricingAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return Result<ProductPricingDto>.Failure(
                new Error("Product.NotFound", $"Product with ID '{request.ProductId}' not found.", ErrorType.NotFound));
        }

        var config = await configRepository.GetWithTierDefinitionsAsync(tenantId.Value, cancellationToken);
        if (config is null)
        {
            return Result<ProductPricingDto>.Failure(
                new Error("PricingConfig.NotFound", "Pricing configuration not found for this tenant.", ErrorType.NotFound));
        }

        // Update product pricing fields
        if (request.ListPrice.HasValue)
        {
            product.ListPrice = request.ListPrice.Value;
        }

        product.IvaEnabled = request.IvaEnabled;

        // Calculate net cost from list price and discounts
        var discountPercentages = request.Discounts
            .OrderBy(d => d.TierNumber)
            .Select(d => d.DiscountPercentage)
            .ToList();

        var netCost = product.ListPrice.HasValue
            ? PricingCalculationService.CalculateNetCost(product.ListPrice.Value, discountPercentages)
            : 0m;

        product.NetCost = netCost;

        // Build discount entities
        var discounts = request.Discounts.Select(d => new ProductDiscount
        {
            TenantId = tenantId.Value,
            ProductId = product.Id,
            TierNumber = d.TierNumber,
            DiscountPercentage = d.DiscountPercentage
        }).ToList();

        // Build margin price entities with calculations
        var marginPrices = new List<ProductMarginPrice>();
        foreach (var margin in request.MarginPrices)
        {
            var marginPrice = new ProductMarginPrice
            {
                TenantId = tenantId.Value,
                ProductId = product.Id,
                TierNumber = margin.TierNumber
            };

            if (margin.MarginPercentage.HasValue)
            {
                marginPrice.MarginPercentage = margin.MarginPercentage.Value;
                marginPrice.SalePrice = netCost > 0
                    ? PricingCalculationService.CalculateSalePriceFromMargin(netCost, margin.MarginPercentage.Value)
                    : 0m;
            }
            else if (margin.SalePrice.HasValue)
            {
                marginPrice.SalePrice = margin.SalePrice.Value;
                marginPrice.MarginPercentage = margin.SalePrice.Value > 0 && netCost > 0
                    ? PricingCalculationService.CalculateMarginFromSalePrice(netCost, margin.SalePrice.Value)
                    : 0m;
            }

            if (request.IvaEnabled && marginPrice.SalePrice.HasValue && marginPrice.SalePrice.Value > 0)
            {
                marginPrice.PriceWithIva = PricingCalculationService.ApplyIva(
                    marginPrice.SalePrice.Value, config.IvaPercentage);
            }
            else
            {
                marginPrice.PriceWithIva = null;
            }

            marginPrices.Add(marginPrice);
        }

        await productPricingRepository.UpdateProductPricingAsync(
            product.Id, discounts, marginPrices, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload product for response
        var updatedProduct = await productPricingRepository.GetProductPricingAsync(request.ProductId, cancellationToken);

        return Result<ProductPricingDto>.Success(
            PricingMapper.ToProductPricingDto(updatedProduct!, config));
    }
}
