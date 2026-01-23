namespace Corelio.Domain.Enums;

/// <summary>
/// Types of barcodes supported by the system.
/// </summary>
public enum BarcodeType
{
    /// <summary>
    /// EAN-13 barcode (European Article Number - 13 digits).
    /// </summary>
    EAN13,

    /// <summary>
    /// UPC barcode (Universal Product Code).
    /// </summary>
    UPC,

    /// <summary>
    /// CODE128 barcode.
    /// </summary>
    CODE128,

    /// <summary>
    /// QR Code (Quick Response Code).
    /// </summary>
    QR,

    /// <summary>
    /// Custom barcode format.
    /// </summary>
    Custom
}
