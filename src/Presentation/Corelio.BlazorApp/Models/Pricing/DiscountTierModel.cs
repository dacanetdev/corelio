namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Blazor model for a discount tier definition.
/// </summary>
public class DiscountTierModel
{
    public int TierNumber { get; set; }
    public string TierName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
