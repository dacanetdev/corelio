namespace Corelio.Application.Services;

/// <summary>
/// Static service providing pure pricing calculation functions.
/// All monetary results are rounded to 2 decimal places using MidpointRounding.AwayFromZero.
/// </summary>
public static class PricingCalculationService
{
    private const int DecimalPlaces = 2;
    private const MidpointRounding RoundingMode = MidpointRounding.AwayFromZero;

    /// <summary>
    /// Calculates the net cost by applying cascading discount percentages to a list price.
    /// Formula: ListPrice × (1 - D1/100) × (1 - D2/100) × ... × (1 - Dn/100)
    /// </summary>
    /// <param name="listPrice">The original list price before discounts.</param>
    /// <param name="discountPercentages">Cascading discount percentages (e.g., [10, 5, 2]).</param>
    /// <returns>The net cost after all discounts applied.</returns>
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
    /// Calculates the sale price from net cost and a margin percentage.
    /// Formula: NetCost / (1 - MarginPercentage/100)
    /// </summary>
    /// <param name="netCost">The net cost of the product.</param>
    /// <param name="marginPercentage">The desired margin percentage (0-99.99).</param>
    /// <returns>The calculated sale price.</returns>
    /// <exception cref="ArgumentException">Thrown when margin percentage is 100 or greater.</exception>
    public static decimal CalculateSalePriceFromMargin(decimal netCost, decimal marginPercentage)
    {
        if (marginPercentage >= 100)
        {
            throw new ArgumentException(
                "Margin percentage must be less than 100.", nameof(marginPercentage));
        }

        var salePrice = netCost / (1 - marginPercentage / 100m);
        return Math.Round(salePrice, DecimalPlaces, RoundingMode);
    }

    /// <summary>
    /// Calculates the margin percentage from net cost and sale price.
    /// Formula: ((SalePrice - NetCost) / SalePrice) × 100
    /// </summary>
    /// <param name="netCost">The net cost of the product.</param>
    /// <param name="salePrice">The sale price of the product.</param>
    /// <returns>The margin percentage.</returns>
    /// <exception cref="ArgumentException">Thrown when sale price is zero.</exception>
    public static decimal CalculateMarginFromSalePrice(decimal netCost, decimal salePrice)
    {
        if (salePrice == 0)
        {
            throw new ArgumentException(
                "Sale price cannot be zero.", nameof(salePrice));
        }

        var margin = ((salePrice - netCost) / salePrice) * 100m;
        return Math.Round(margin, DecimalPlaces, RoundingMode);
    }

    /// <summary>
    /// Applies IVA (VAT) to a sale price.
    /// Formula: SalePrice × (1 + IvaPercentage/100)
    /// </summary>
    /// <param name="salePrice">The sale price before IVA.</param>
    /// <param name="ivaPercentage">The IVA percentage (default 16%).</param>
    /// <returns>The price with IVA included.</returns>
    public static decimal ApplyIva(decimal salePrice, decimal ivaPercentage = 16.00m)
    {
        var priceWithIva = salePrice * (1 + ivaPercentage / 100m);
        return Math.Round(priceWithIva, DecimalPlaces, RoundingMode);
    }

    /// <summary>
    /// Removes IVA (VAT) from a price that includes IVA.
    /// Formula: PriceWithIva / (1 + IvaPercentage/100)
    /// </summary>
    /// <param name="priceWithIva">The price including IVA.</param>
    /// <param name="ivaPercentage">The IVA percentage (default 16%).</param>
    /// <returns>The price without IVA.</returns>
    public static decimal RemoveIva(decimal priceWithIva, decimal ivaPercentage = 16.00m)
    {
        var salePrice = priceWithIva / (1 + ivaPercentage / 100m);
        return Math.Round(salePrice, DecimalPlaces, RoundingMode);
    }
}
