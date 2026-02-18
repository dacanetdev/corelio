using Corelio.Domain.Common;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a physical or virtual warehouse for inventory management.
/// </summary>
public class Warehouse : TenantAuditableEntity
{
    /// <summary>
    /// Warehouse display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Warehouse type (Main, Secondary, Retail, etc.).
    /// </summary>
    public WarehouseType Type { get; set; } = WarehouseType.Main;

    /// <summary>
    /// Whether this is the default warehouse used by POS.
    /// </summary>
    public bool IsDefault { get; set; } = false;

    // Navigation
    public ICollection<InventoryItem> InventoryItems { get; set; } = [];
}
