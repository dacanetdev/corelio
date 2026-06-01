using Corelio.Domain.Enums;

namespace Corelio.BlazorApp.Models.PurchaseOrders;

/// <summary>
/// Purchase order list item model.
/// </summary>
public class PurchaseOrderListModel
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public PurchaseOrderStatus Status { get; set; }
    public DateTimeOffset? ExpectedDate { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Full purchase order detail model.
/// </summary>
public class PurchaseOrderModel
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public PurchaseOrderStatus Status { get; set; }
    public DateTimeOffset? ExpectedDate { get; set; }
    public string? Notes { get; set; }
    public decimal Subtotal { get; set; }
    public decimal IvaAmount { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<PurchaseOrderItemModel> Items { get; set; } = [];
}

/// <summary>
/// Purchase order line item model.
/// </summary>
public class PurchaseOrderItemModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ReceivedQuantity { get; set; }
}

/// <summary>
/// Form model for creating/editing a purchase order.
/// </summary>
public class PurchaseOrderFormModel
{
    public Guid SupplierId { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseOrderItemFormModel> Items { get; set; } = [];
}

/// <summary>
/// Form model for a single purchase order line item.
/// </summary>
public class PurchaseOrderItemFormModel
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}

/// <summary>
/// Form model for the receive goods dialog.
/// </summary>
public class ReceiveGoodsFormModel
{
    public Guid WarehouseId { get; set; }
    public DateTime ReceivedDate { get; set; } = DateTime.Today;
    public string? Notes { get; set; }
    public List<ReceiveGoodsItemFormModel> Items { get; set; } = [];
}

/// <summary>
/// A single line item in the receive goods form.
/// </summary>
public class ReceiveGoodsItemFormModel
{
    public Guid PurchaseOrderItemId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal OrderedQuantity { get; set; }
    public decimal AlreadyReceived { get; set; }
    public decimal RemainingQuantity => OrderedQuantity - AlreadyReceived;
    public decimal QuantityToReceive { get; set; }
}
