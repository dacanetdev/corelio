namespace Corelio.Domain.Enums;

/// <summary>
/// Lifecycle status of a purchase order.
/// </summary>
public enum PurchaseOrderStatus
{
    /// <summary>Work-in-progress, not yet sent to supplier.</summary>
    Draft = 0,

    /// <summary>Submitted to supplier, awaiting approval.</summary>
    Submitted = 1,

    /// <summary>Approved — goods can now be received against this order.</summary>
    Approved = 2,

    /// <summary>Some items have been received but the order is not fully fulfilled.</summary>
    PartiallyReceived = 3,

    /// <summary>All ordered quantities have been received.</summary>
    Received = 4,

    /// <summary>Order cancelled; no goods will be received.</summary>
    Cancelled = 5
}
