namespace Corelio.Domain.Enums;

/// <summary>
/// Payment status for sales.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// No payment received.
    /// </summary>
    Unpaid = 0,

    /// <summary>
    /// Partial payment received.
    /// </summary>
    Partial = 1,

    /// <summary>
    /// Fully paid.
    /// </summary>
    Paid = 2,

    /// <summary>
    /// Overpaid (excess payment).
    /// </summary>
    Overpaid = 3
}
