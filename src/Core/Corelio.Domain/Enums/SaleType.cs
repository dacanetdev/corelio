namespace Corelio.Domain.Enums;

/// <summary>
/// Types of sales transactions.
/// </summary>
public enum SaleType
{
    /// <summary>
    /// Point of Sale transaction.
    /// </summary>
    Pos = 0,

    /// <summary>
    /// Invoice transaction.
    /// </summary>
    Invoice = 1,

    /// <summary>
    /// Quote/estimate.
    /// </summary>
    Quote = 2,

    /// <summary>
    /// Credit note (refund).
    /// </summary>
    CreditNote = 3,

    /// <summary>
    /// Debit note.
    /// </summary>
    DebitNote = 4
}
