namespace Corelio.Domain.Entities;

/// <summary>
/// A single line item within a purchase order.
/// </summary>
public class PurchaseOrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Denormalized product name for historical integrity.</summary>
    public string ProductName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    /// <summary>Quantity * UnitPrice.</summary>
    public decimal Subtotal { get; set; }

    /// <summary>Quantity received so far (updated during goods receipt in US-12.3).</summary>
    public decimal ReceivedQuantity { get; set; } = 0;
}
