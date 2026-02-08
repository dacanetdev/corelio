using Corelio.Application.Common.Enums;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Application.Services;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Commands.BulkUpdatePricing;

/// <summary>
/// Handler for applying bulk pricing updates to multiple products.
/// </summary>
public class BulkUpdatePricingCommandHandler(
    IProductPricingRepository productPricingRepository,
    ITenantPricingConfigurationRepository configRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<BulkUpdatePricingCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        BulkUpdatePricingCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<int>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var config = await configRepository.GetWithTierDefinitionsAsync(tenantId.Value, cancellationToken);
        if (config is null)
        {
            return Result<int>.Failure(
                new Error("PricingConfig.NotFound", "Pricing configuration not found for this tenant.", ErrorType.NotFound));
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var updatedCount = 0;

            foreach (var productId in request.ProductIds)
            {
                var product = await productPricingRepository.GetProductPricingAsync(productId, cancellationToken);
                if (product is null)
                {
                    continue;
                }

                var updated = ApplyBulkUpdate(product, request, config, tenantId.Value);
                if (updated)
                {
                    await productPricingRepository.UpdateProductPricingAsync(
                        product.Id, product.Discounts, product.MarginPrices, cancellationToken);
                    updatedCount++;
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<int>.Success(updatedCount);
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static bool ApplyBulkUpdate(
        Product product,
        BulkUpdatePricingCommand request,
        TenantPricingConfiguration config,
        Guid tenantId)
    {
        switch (request.UpdateType)
        {
            case PricingUpdateType.PercentageIncrease:
                return ApplyPercentageChange(product, request.Value, config, tenantId, increase: true);

            case PricingUpdateType.PercentageDecrease:
                return ApplyPercentageChange(product, request.Value, config, tenantId, increase: false);

            case PricingUpdateType.FixedAmountIncrease:
                return ApplyFixedAmountChange(product, request.Value, config, tenantId, increase: true);

            case PricingUpdateType.FixedAmountDecrease:
                return ApplyFixedAmountChange(product, request.Value, config, tenantId, increase: false);

            case PricingUpdateType.SetNewMargin:
                return ApplyNewMargin(product, request.Value, request.TierNumber!.Value, config, tenantId);

            default:
                return false;
        }
    }

    private static bool ApplyPercentageChange(
        Product product, decimal percentage, TenantPricingConfiguration config, Guid tenantId, bool increase)
    {
        foreach (var marginPrice in product.MarginPrices)
        {
            if (!marginPrice.SalePrice.HasValue || marginPrice.SalePrice.Value <= 0)
            {
                continue;
            }

            var factor = increase ? (1 + percentage / 100m) : (1 - percentage / 100m);
            var newSalePrice = Math.Round(marginPrice.SalePrice.Value * factor, 2, MidpointRounding.AwayFromZero);

            marginPrice.SalePrice = newSalePrice;

            if (product.NetCost.HasValue && product.NetCost.Value > 0 && newSalePrice > 0)
            {
                marginPrice.MarginPercentage = PricingCalculationService.CalculateMarginFromSalePrice(
                    product.NetCost.Value, newSalePrice);
            }

            if (product.IvaEnabled && newSalePrice > 0)
            {
                marginPrice.PriceWithIva = PricingCalculationService.ApplyIva(newSalePrice, config.IvaPercentage);
            }
            else
            {
                marginPrice.PriceWithIva = null;
            }
        }

        return true;
    }

    private static bool ApplyFixedAmountChange(
        Product product, decimal amount, TenantPricingConfiguration config, Guid tenantId, bool increase)
    {
        foreach (var marginPrice in product.MarginPrices)
        {
            if (!marginPrice.SalePrice.HasValue)
            {
                continue;
            }

            var newSalePrice = increase
                ? marginPrice.SalePrice.Value + amount
                : marginPrice.SalePrice.Value - amount;

            if (newSalePrice < 0)
            {
                newSalePrice = 0;
            }

            marginPrice.SalePrice = Math.Round(newSalePrice, 2, MidpointRounding.AwayFromZero);

            if (product.NetCost.HasValue && product.NetCost.Value > 0 && newSalePrice > 0)
            {
                marginPrice.MarginPercentage = PricingCalculationService.CalculateMarginFromSalePrice(
                    product.NetCost.Value, newSalePrice);
            }

            if (product.IvaEnabled && newSalePrice > 0)
            {
                marginPrice.PriceWithIva = PricingCalculationService.ApplyIva(newSalePrice, config.IvaPercentage);
            }
            else
            {
                marginPrice.PriceWithIva = null;
            }
        }

        return true;
    }

    private static bool ApplyNewMargin(
        Product product, decimal newMarginPercentage, int tierNumber, TenantPricingConfiguration config, Guid tenantId)
    {
        var marginPrice = product.MarginPrices.FirstOrDefault(m => m.TierNumber == tierNumber);
        if (marginPrice is null)
        {
            // Create new margin price if it doesn't exist
            marginPrice = new ProductMarginPrice
            {
                TenantId = tenantId,
                ProductId = product.Id,
                TierNumber = tierNumber
            };
            product.MarginPrices.Add(marginPrice);
        }

        marginPrice.MarginPercentage = newMarginPercentage;

        if (product.NetCost.HasValue && product.NetCost.Value > 0)
        {
            marginPrice.SalePrice = PricingCalculationService.CalculateSalePriceFromMargin(
                product.NetCost.Value, newMarginPercentage);

            if (product.IvaEnabled && marginPrice.SalePrice.Value > 0)
            {
                marginPrice.PriceWithIva = PricingCalculationService.ApplyIva(
                    marginPrice.SalePrice.Value, config.IvaPercentage);
            }
            else
            {
                marginPrice.PriceWithIva = null;
            }
        }

        return true;
    }
}
