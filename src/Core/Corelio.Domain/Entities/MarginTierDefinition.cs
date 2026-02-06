using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Defines a margin/price tier for a tenant's pricing configuration.
/// Each tenant can have up to 5 margin tiers with custom names.
/// </summary>
public class MarginTierDefinition : TenantAuditableEntity
{
    /// <summary>
    /// The pricing configuration this tier belongs to.
    /// </summary>
    public Guid TenantPricingConfigurationId { get; set; }

    /// <summary>
    /// Tier number (1-5). Determines the price level.
    /// </summary>
    public int TierNumber { get; set; }

    /// <summary>
    /// Custom name for this margin tier (e.g., "Menudeo", "Mayoreo", "Distribuidor").
    /// </summary>
    public string TierName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this tier is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties

    /// <summary>
    /// The pricing configuration this tier belongs to.
    /// </summary>
    public TenantPricingConfiguration TenantPricingConfiguration { get; set; } = null!;
}
