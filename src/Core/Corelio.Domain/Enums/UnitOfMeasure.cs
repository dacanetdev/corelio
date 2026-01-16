namespace Corelio.Domain.Enums;

/// <summary>
/// Units of measure for products.
/// Aligned with SAT (Mexican Tax Authority) unit codes.
/// </summary>
public enum UnitOfMeasure
{
    /// <summary>
    /// Pieces (SAT code: H87).
    /// </summary>
    PCS,

    /// <summary>
    /// Kilogram (SAT code: KGM).
    /// </summary>
    KG,

    /// <summary>
    /// Meter (SAT code: MTR).
    /// </summary>
    M,

    /// <summary>
    /// Liter (SAT code: LTR).
    /// </summary>
    L,

    /// <summary>
    /// Box (SAT code: XBX).
    /// </summary>
    BOX,

    /// <summary>
    /// Pack (SAT code: XPK).
    /// </summary>
    PACK,

    /// <summary>
    /// Dozen (SAT code: DZN).
    /// </summary>
    DOZEN,

    /// <summary>
    /// Pair (SAT code: PR).
    /// </summary>
    PAIR,

    /// <summary>
    /// Set (SAT code: SET).
    /// </summary>
    SET,

    /// <summary>
    /// Square meter (SAT code: MTK).
    /// </summary>
    M2,

    /// <summary>
    /// Cubic meter (SAT code: MTQ).
    /// </summary>
    M3
}
