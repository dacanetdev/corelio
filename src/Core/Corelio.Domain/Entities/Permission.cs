using Corelio.Domain.Common;
using Corelio.Domain.Enums;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a granular permission in the system.
/// </summary>
public class Permission : BaseEntity
{
    /// <summary>
    /// The unique permission code (e.g., 'sales.create', 'products.delete').
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable name of the permission.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what the permission allows.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The module this permission belongs to.
    /// </summary>
    public PermissionModule Module { get; set; }

    /// <summary>
    /// Category of the permission (create, read, update, delete, manage).
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Whether this permission is dangerous and requires extra confirmation.
    /// </summary>
    public bool IsDangerous { get; set; } = false;

    // Navigation property
    /// <summary>
    /// Roles that have this permission.
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}

/// <summary>
/// Well-known permission codes.
/// </summary>
public static class Permissions
{
    // Products
    public const string ProductsView = "products.view";
    public const string ProductsCreate = "products.create";
    public const string ProductsEdit = "products.edit";
    public const string ProductsDelete = "products.delete";
    public const string ProductsPricing = "products.pricing";

    // Sales
    public const string SalesCreate = "sales.create";
    public const string SalesView = "sales.view";
    public const string SalesCancel = "sales.cancel";
    public const string SalesDiscount = "sales.discount";

    // Inventory
    public const string InventoryView = "inventory.view";
    public const string InventoryAdjust = "inventory.adjust";
    public const string InventoryTransfer = "inventory.transfer";

    // Customers
    public const string CustomersView = "customers.view";
    public const string CustomersCreate = "customers.create";
    public const string CustomersEdit = "customers.edit";
    public const string CustomersDelete = "customers.delete";

    // CFDI
    public const string CfdiGenerate = "cfdi.generate";
    public const string CfdiCancel = "cfdi.cancel";
    public const string CfdiView = "cfdi.view";

    // Users
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersEdit = "users.edit";
    public const string UsersDelete = "users.delete";
    public const string UsersRoles = "users.roles";

    // Reports
    public const string ReportsView = "reports.view";
    public const string ReportsFinancial = "reports.financial";

    // Settings
    public const string SettingsView = "settings.view";
    public const string SettingsEdit = "settings.edit";

    // Pricing
    public const string PricingView = "pricing.view";
    public const string PricingManage = "pricing.manage";
}
