using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a discount tier value for a specific product.
/// Stores the discount percentage for each tier (1-6) applied to calculate the net cost.
/// </summary>
public class ProductDiscount : TenantAuditableEntity
{
    /// <summary>
    /// The product this discount belongs to.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Discount tier number (1-6). Corresponds to the cascading discount order.
    /// </summary>
    public int TierNumber { get; set; }

    /// <summary>
    /// Discount percentage for this tier (0-100).
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    // Navigation properties

    /// <summary>
    /// The product this discount belongs to.
    /// </summary>
    public Product Product { get; set; } = null!;
}
