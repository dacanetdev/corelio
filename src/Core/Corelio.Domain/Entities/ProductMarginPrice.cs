using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a margin/price tier value for a specific product.
/// Stores the margin percentage, calculated sale price, and price with IVA for each tier (1-5).
/// </summary>
public class ProductMarginPrice : TenantAuditableEntity
{
    /// <summary>
    /// The product this margin price belongs to.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Margin/price tier number (1-5).
    /// </summary>
    public int TierNumber { get; set; }

    /// <summary>
    /// Margin percentage for this tier (0-100). Nullable if not set.
    /// </summary>
    public decimal? MarginPercentage { get; set; }

    /// <summary>
    /// Calculated sale price: NetCost / (1 - MarginPercent/100). Nullable if not calculated.
    /// </summary>
    public decimal? SalePrice { get; set; }

    /// <summary>
    /// Calculated price with IVA: SalePrice * (1 + IvaPercentage/100). Nullable if not calculated.
    /// </summary>
    public decimal? PriceWithIva { get; set; }

    // Navigation properties

    /// <summary>
    /// The product this margin price belongs to.
    /// </summary>
    public Product Product { get; set; } = null!;
}
