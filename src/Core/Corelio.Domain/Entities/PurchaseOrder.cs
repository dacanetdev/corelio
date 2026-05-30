using Corelio.Domain.Common;
using Corelio.Domain.Common.Interfaces;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a purchase order issued to a supplier for restocking inventory.
/// </summary>
public class PurchaseOrder : TenantAuditableEntity, ISoftDeletable
{
    /// <summary>Auto-generated order number in the format PO-{YYYY}-{NNNN}.</summary>
    public string OrderNumber { get; set; } = string.Empty;

    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

    public DateTimeOffset? ExpectedDate { get; set; }
    public string? Notes { get; set; }

    /// <summary>Sum of all line item subtotals (before IVA).</summary>
    public decimal Subtotal { get; set; }

    /// <summary>IVA at 16% of Subtotal.</summary>
    public decimal IvaAmount { get; set; }

    /// <summary>Subtotal + IvaAmount.</summary>
    public decimal Total { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; } = [];

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
