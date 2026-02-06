using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Tenant-specific pricing configuration that defines the number of discount and margin tiers.
/// Each tenant can configure 1-6 discount tiers and 1-5 margin/price tiers.
/// </summary>
public class TenantPricingConfiguration : TenantAuditableEntity
{
    /// <summary>
    /// Number of discount tiers (1-6). Default is 3.
    /// </summary>
    public int DiscountTierCount { get; set; } = 3;

    /// <summary>
    /// Number of margin/price tiers (1-5). Default is 3.
    /// </summary>
    public int MarginTierCount { get; set; } = 3;

    /// <summary>
    /// Whether IVA (VAT) is enabled by default for new products.
    /// </summary>
    public bool DefaultIvaEnabled { get; set; } = true;

    /// <summary>
    /// IVA percentage (0-100). Default is 16% for Mexico.
    /// </summary>
    public decimal IvaPercentage { get; set; } = 16.00m;

    // Navigation properties

    /// <summary>
    /// Discount tier definitions for this tenant.
    /// </summary>
    public List<DiscountTierDefinition> DiscountTierDefinitions { get; set; } = [];

    /// <summary>
    /// Margin tier definitions for this tenant.
    /// </summary>
    public List<MarginTierDefinition> MarginTierDefinitions { get; set; } = [];
}
