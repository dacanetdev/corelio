using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents the inventory stock of a product in a specific warehouse.
/// </summary>
public class InventoryItem : TenantAuditableEntity
{
    /// <summary>
    /// Product being tracked.
    /// </summary>
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Warehouse where stock is held.
    /// </summary>
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// Total quantity on hand.
    /// </summary>
    public decimal Quantity { get; set; } = 0;

    /// <summary>
    /// Quantity reserved by open quotes (not yet deducted).
    /// </summary>
    public decimal ReservedQuantity { get; set; } = 0;

    /// <summary>
    /// Minimum stock level that triggers low stock alert.
    /// </summary>
    public decimal MinimumLevel { get; set; } = 0;

    /// <summary>
    /// Available quantity (Quantity - ReservedQuantity). Computed, not stored.
    /// </summary>
    public decimal AvailableQuantity => Quantity - ReservedQuantity;

    // Navigation
    public ICollection<InventoryTransaction> Transactions { get; set; } = [];
}
