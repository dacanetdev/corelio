using Corelio.Domain.Common;

namespace Corelio.Domain.Entities;

/// <summary>
/// Records the physical arrival of goods against an approved purchase order.
/// </summary>
public class GoodsReceipt : TenantAuditableEntity
{
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public DateOnly ReceivedDate { get; set; }
    public string? Notes { get; set; }

    public ICollection<GoodsReceiptItem> Items { get; set; } = [];
}
