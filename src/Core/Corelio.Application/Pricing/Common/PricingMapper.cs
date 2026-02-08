using Corelio.Domain.Entities;

namespace Corelio.Application.Pricing.Common;

/// <summary>
/// Static helper to map domain entities to pricing DTOs.
/// </summary>
public static class PricingMapper
{
    /// <summary>
    /// Maps a TenantPricingConfiguration entity to a TenantPricingConfigDto.
    /// </summary>
    public static TenantPricingConfigDto ToConfigDto(TenantPricingConfiguration config)
    {
        var discountTiers = config.DiscountTierDefinitions
            .OrderBy(d => d.TierNumber)
            .Select(d => new DiscountTierDto(d.TierNumber, d.TierName, d.IsActive))
            .ToList();

        var marginTiers = config.MarginTierDefinitions
            .OrderBy(m => m.TierNumber)
            .Select(m => new MarginTierDto(m.TierNumber, m.TierName, m.IsActive))
            .ToList();

        return new TenantPricingConfigDto(
            config.Id,
            config.TenantId,
            config.DiscountTierCount,
            config.MarginTierCount,
            config.DefaultIvaEnabled,
            config.IvaPercentage,
            discountTiers,
            marginTiers);
    }

    /// <summary>
    /// Maps a Product entity with its pricing relations to a ProductPricingDto.
    /// Requires the tenant pricing config to resolve tier names.
    /// </summary>
    public static ProductPricingDto ToProductPricingDto(
        Product product,
        TenantPricingConfiguration config)
    {
        var discountTierNames = config.DiscountTierDefinitions
            .ToDictionary(d => d.TierNumber, d => d.TierName);

        var marginTierNames = config.MarginTierDefinitions
            .ToDictionary(m => m.TierNumber, m => m.TierName);

        var discounts = product.Discounts
            .OrderBy(d => d.TierNumber)
            .Select(d => new ProductDiscountDto(
                d.TierNumber,
                discountTierNames.GetValueOrDefault(d.TierNumber, $"Tier {d.TierNumber}"),
                d.DiscountPercentage))
            .ToList();

        var marginPrices = product.MarginPrices
            .OrderBy(m => m.TierNumber)
            .Select(m => new ProductMarginPriceDto(
                m.TierNumber,
                marginTierNames.GetValueOrDefault(m.TierNumber, $"Tier {m.TierNumber}"),
                m.MarginPercentage,
                m.SalePrice,
                m.PriceWithIva))
            .ToList();

        return new ProductPricingDto(
            product.Id,
            product.Name,
            product.Sku,
            product.ListPrice,
            product.NetCost,
            product.IvaEnabled,
            discounts,
            marginPrices);
    }

    /// <summary>
    /// Maps a list of products to ProductPricingDto list using the tenant config for tier names.
    /// </summary>
    public static List<ProductPricingDto> ToProductPricingDtoList(
        List<Product> products,
        TenantPricingConfiguration config)
    {
        return products.Select(p => ToProductPricingDto(p, config)).ToList();
    }
}
