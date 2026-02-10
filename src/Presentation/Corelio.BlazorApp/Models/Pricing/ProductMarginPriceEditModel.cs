namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Mutable edit model for product margin price tiers (supports MudBlazor form binding).
/// </summary>
public class ProductMarginPriceEditModel
{
    public int TierNumber { get; set; }
    public string TierName { get; set; } = string.Empty;
    public decimal? MarginPercentage { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? PriceWithIva { get; set; }

    /// <summary>
    /// Creates an edit model from an immutable record.
    /// </summary>
    public static ProductMarginPriceEditModel FromRecord(ProductMarginPriceModel record) => new()
    {
        TierNumber = record.TierNumber,
        TierName = record.TierName,
        MarginPercentage = record.MarginPercentage,
        SalePrice = record.SalePrice,
        PriceWithIva = record.PriceWithIva
    };

    /// <summary>
    /// Converts to the immutable record for API calls.
    /// </summary>
    public ProductMarginPriceModel ToRecord() => new(TierNumber, TierName, MarginPercentage, SalePrice, PriceWithIva);
}
