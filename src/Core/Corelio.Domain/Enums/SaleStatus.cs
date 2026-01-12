namespace Corelio.Domain.Enums;

/// <summary>
/// Status of a sale transaction.
/// </summary>
public enum SaleStatus
{
    /// <summary>
    /// Draft sale not yet finalized.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Pending processing.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Sale completed.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Sale cancelled.
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Sale refunded.
    /// </summary>
    Refunded = 4
}
