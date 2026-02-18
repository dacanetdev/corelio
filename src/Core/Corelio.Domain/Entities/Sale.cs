using Corelio.Domain.Common;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a sale transaction (POS, Invoice, or Quote).
/// </summary>
public class Sale : TenantAuditableEntity
{
    /// <summary>
    /// Auto-generated sale folio (e.g., "V-00001").
    /// </summary>
    public string Folio { get; set; } = string.Empty;

    /// <summary>
    /// Type of sale: POS, Invoice, or Quote.
    /// </summary>
    public SaleType Type { get; set; } = SaleType.Pos;

    /// <summary>
    /// Current status of the sale.
    /// </summary>
    public SaleStatus Status { get; set; } = SaleStatus.Draft;

    /// <summary>
    /// Optional customer (walk-in sales may not have a customer).
    /// </summary>
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    /// <summary>
    /// Warehouse from which inventory is deducted.
    /// </summary>
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// Subtotal before discounts and taxes.
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Total discount amount applied to the sale.
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Total IVA (VAT) amount.
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Final total (SubTotal - DiscountAmount + TaxAmount).
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Optional notes or remarks for the sale.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Timestamp when the sale was completed (payments confirmed).
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public ICollection<SaleItem> Items { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
}
