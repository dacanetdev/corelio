using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Defines a discount tier for a tenant's pricing configuration.
/// Each tenant can have up to 6 discount tiers with custom names.
/// </summary>
public class DiscountTierDefinition : TenantAuditableEntity
{
    /// <summary>
    /// The pricing configuration this tier belongs to.
    /// </summary>
    public Guid TenantPricingConfigurationId { get; set; }

    /// <summary>
    /// Tier number (1-6). Determines the order of cascading discount application.
    /// </summary>
    public int TierNumber { get; set; }

    /// <summary>
    /// Custom name for this discount tier (e.g., "Descuento Comercial", "Descuento Pronto Pago").
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
