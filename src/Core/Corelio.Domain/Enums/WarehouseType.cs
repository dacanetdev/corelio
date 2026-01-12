namespace Corelio.Domain.Enums;

/// <summary>
/// Types of warehouse locations.
/// </summary>
public enum WarehouseType
{
    /// <summary>
    /// Main warehouse.
    /// </summary>
    Main = 0,

    /// <summary>
    /// Secondary warehouse.
    /// </summary>
    Secondary = 1,

    /// <summary>
    /// Retail location.
    /// </summary>
    Retail = 2,

    /// <summary>
    /// Virtual warehouse (for dropshipping, etc.).
    /// </summary>
    Virtual = 3,

    /// <summary>
    /// Consignment warehouse.
    /// </summary>
    Consignment = 4
}
