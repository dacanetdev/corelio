namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Blazor model for tenant pricing configuration (form-bindable).
/// </summary>
public class TenantPricingConfigModel
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public int DiscountTierCount { get; set; } = 3;
    public int MarginTierCount { get; set; } = 3;
    public bool DefaultIvaEnabled { get; set; } = true;
    public decimal IvaPercentage { get; set; } = 16.00m;
    public List<DiscountTierModel> DiscountTiers { get; set; } = [];
    public List<MarginTierModel> MarginTiers { get; set; } = [];
}
