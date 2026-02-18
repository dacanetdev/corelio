using Corelio.Domain.Common;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a ledger entry for an inventory movement (sale, purchase, adjustment, etc.).
/// </summary>
public class InventoryTransaction : TenantAuditableEntity
{
    /// <summary>
    /// The inventory item affected.
    /// </summary>
    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;

    /// <summary>
    /// Type of transaction.
    /// </summary>
    public InventoryTransactionType Type { get; set; }

    /// <summary>
    /// Quantity moved. Positive = stock in, negative = stock out.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Stock level before this transaction.
    /// </summary>
    public decimal PreviousQuantity { get; set; }

    /// <summary>
    /// Stock level after this transaction.
    /// </summary>
    public decimal NewQuantity { get; set; }

    /// <summary>
    /// Reference to the source document (SaleId, AdjustmentId, etc.).
    /// </summary>
    public Guid? ReferenceId { get; set; }

    /// <summary>
    /// Optional notes about the transaction.
    /// </summary>
    public string? Notes { get; set; }
}
