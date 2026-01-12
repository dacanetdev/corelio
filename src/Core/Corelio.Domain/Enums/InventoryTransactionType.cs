namespace Corelio.Domain.Enums;

/// <summary>
/// Types of inventory transactions.
/// </summary>
public enum InventoryTransactionType
{
    /// <summary>
    /// Purchase/receiving inventory.
    /// </summary>
    Purchase = 0,

    /// <summary>
    /// Sale deduction.
    /// </summary>
    Sale = 1,

    /// <summary>
    /// Positive adjustment (increase).
    /// </summary>
    AdjustmentPositive = 2,

    /// <summary>
    /// Negative adjustment (decrease).
    /// </summary>
    AdjustmentNegative = 3,

    /// <summary>
    /// Transfer in from another warehouse.
    /// </summary>
    TransferIn = 4,

    /// <summary>
    /// Transfer out to another warehouse.
    /// </summary>
    TransferOut = 5,

    /// <summary>
    /// Customer return.
    /// </summary>
    Return = 6,

    /// <summary>
    /// Damaged goods write-off.
    /// </summary>
    Damaged = 7,

    /// <summary>
    /// Lost inventory.
    /// </summary>
    Lost = 8,

    /// <summary>
    /// Found/recovered inventory.
    /// </summary>
    Found = 9
}
