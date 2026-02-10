namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Mutable edit model for product discount tiers (supports MudBlazor form binding).
/// </summary>
public class ProductDiscountEditModel
{
    public int TierNumber { get; set; }
    public string TierName { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Creates an edit model from an immutable record.
    /// </summary>
    public static ProductDiscountEditModel FromRecord(ProductDiscountModel record) => new()
    {
        TierNumber = record.TierNumber,
        TierName = record.TierName,
        DiscountPercentage = record.DiscountPercentage
    };

    /// <summary>
    /// Converts to the immutable record for API calls.
    /// </summary>
    public ProductDiscountModel ToRecord() => new(TierNumber, TierName, DiscountPercentage);
}
