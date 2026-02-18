using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a line item within a sale, with snapshot of product data at time of sale.
/// </summary>
public class SaleItem : TenantAuditableEntity
{
    /// <summary>
    /// Parent sale.
    /// </summary>
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    /// <summary>
    /// Product reference.
    /// </summary>
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Product name snapshot (preserved even if product is renamed).
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Product SKU snapshot.
    /// </summary>
    public string ProductSku { get; set; } = string.Empty;

    /// <summary>
    /// Unit price at time of sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Quantity sold.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Discount percentage applied to this line (0-100).
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// IVA percentage for this item (0 if exempt).
    /// </summary>
    public decimal TaxPercentage { get; set; }

    /// <summary>
    /// Computed line total: (UnitPrice * Quantity * (1 - DiscountPct/100)) * (1 + TaxPct/100).
    /// Stored for reporting.
    /// </summary>
    public decimal LineTotal { get; set; }
}
