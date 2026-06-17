namespace Corelio.Domain.Entities;

/// <summary>
/// A single line item within a goods receipt, recording how much of a PO line was received.
/// </summary>
public class GoodsReceiptItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid GoodsReceiptId { get; set; }
    public GoodsReceipt GoodsReceipt { get; set; } = null!;

    public Guid PurchaseOrderItemId { get; set; }
    public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Denormalized product name for historical integrity.</summary>
    public string ProductName { get; set; } = string.Empty;

    public decimal QuantityReceived { get; set; }

    /// <summary>Unit cost from the purchase order item at time of receipt.</summary>
    public decimal UnitCost { get; set; } = 0m;
}
