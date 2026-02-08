namespace Corelio.Application.Common.Enums;

/// <summary>
/// Defines the type of bulk pricing update to apply.
/// </summary>
public enum PricingUpdateType
{
    /// <summary>
    /// Increase price by a percentage.
    /// </summary>
    PercentageIncrease,

    /// <summary>
    /// Decrease price by a percentage.
    /// </summary>
    PercentageDecrease,

    /// <summary>
    /// Increase price by a fixed amount.
    /// </summary>
    FixedAmountIncrease,

    /// <summary>
    /// Decrease price by a fixed amount.
    /// </summary>
    FixedAmountDecrease,

    /// <summary>
    /// Set a new margin percentage directly.
    /// </summary>
    SetNewMargin
}
