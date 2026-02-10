namespace Corelio.BlazorApp.Helpers;

/// <summary>
/// Client-side pricing calculator mirroring Application.Services.PricingCalculationService.
/// Used for instant UI feedback; the API remains the source of truth on save.
/// </summary>
public static class PricingCalculator
{
    private const int DecimalPlaces = 2;
    private const MidpointRounding RoundingMode = MidpointRounding.AwayFromZero;

    /// <summary>
    /// Calculates net cost by applying cascading discount percentages.
    /// Formula: ListPrice x (1 - D1/100) x (1 - D2/100) x ... x (1 - Dn/100)
    /// </summary>
    public static decimal CalculateNetCost(decimal listPrice, List<decimal> discountPercentages)
    {
        var netCost = listPrice;

        foreach (var discount in discountPercentages)
        {
            netCost *= (1 - discount / 100m);
        }

        return Math.Round(netCost, DecimalPlaces, RoundingMode);
    }

    /// <summary>
    /// Calculates sale price from net cost and margin percentage.
    /// Formula: NetCost / (1 - MarginPercentage/100)
    /// </summary>
    public static decimal CalculateSalePriceFromMargin(decimal netCost, decimal marginPercentage)
    {
        if (marginPercentage >= 100)
        {
            return 0;
        }

        var salePrice = netCost / (1 - marginPercentage / 100m);
        return Math.Round(salePrice, DecimalPlaces, RoundingMode);
    }

    /// <summary>
    /// Calculates margin percentage from net cost and sale price.
    /// Formula: ((SalePrice - NetCost) / SalePrice) x 100
    /// </summary>
    public static decimal CalculateMarginFromSalePrice(decimal netCost, decimal salePrice)
    {
        if (salePrice == 0)
        {
            return 0;
        }

        var margin = ((salePrice - netCost) / salePrice) * 100m;
        return Math.Round(margin, DecimalPlaces, RoundingMode);
    }

    /// <summary>
    /// Applies IVA (VAT) to a sale price.
    /// Formula: SalePrice x (1 + IvaPercentage/100)
    /// </summary>
    public static decimal ApplyIva(decimal salePrice, decimal ivaPercentage = 16.00m)
    {
        var priceWithIva = salePrice * (1 + ivaPercentage / 100m);
        return Math.Round(priceWithIva, DecimalPlaces, RoundingMode);
    }

    /// <summary>
    /// Removes IVA (VAT) from a price that includes IVA.
    /// Formula: PriceWithIva / (1 + IvaPercentage/100)
    /// </summary>
    public static decimal RemoveIva(decimal priceWithIva, decimal ivaPercentage = 16.00m)
    {
        var salePrice = priceWithIva / (1 + ivaPercentage / 100m);
        return Math.Round(salePrice, DecimalPlaces, RoundingMode);
    }
}
