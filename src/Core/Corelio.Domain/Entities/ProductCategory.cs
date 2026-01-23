using Corelio.Domain.Common;
using Corelio.Domain.Common.Interfaces;

namespace Corelio.Domain.Entities;

/// <summary>
/// Represents a hierarchical product category.
/// Supports up to 5 levels of nesting.
/// </summary>
public class ProductCategory : TenantAuditableEntity, ISoftDeletable
{
    /// <summary>
    /// The category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The category description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// URL of the category image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Parent category ID for hierarchical structure.
    /// Null for root-level categories.
    /// </summary>
    public Guid? ParentCategoryId { get; set; }

    /// <summary>
    /// The level in the category hierarchy (0-5).
    /// Root level = 0, maximum depth = 5.
    /// </summary>
    public int Level { get; set; } = 0;

    /// <summary>
    /// Materialized path for efficient querying.
    /// Format: /hardware/tools/power-tools/
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Sort order for display.
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Hex color code for UI display (e.g., #FF5733).
    /// </summary>
    public string? ColorHex { get; set; }

    /// <summary>
    /// Icon name for UI display (e.g., 'hammer', 'wrench').
    /// </summary>
    public string? IconName { get; set; }

    /// <summary>
    /// Whether the category is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates whether the category has been soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// The date and time when the category was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// The ID of the user who deleted this category.
    /// </summary>
    public Guid? DeletedBy { get; set; }

    // Navigation properties
    /// <summary>
    /// Parent category.
    /// </summary>
    public ProductCategory? ParentCategory { get; set; }

    /// <summary>
    /// Child categories.
    /// </summary>
    public ICollection<ProductCategory> ChildCategories { get; set; } = [];

    /// <summary>
    /// Products in this category.
    /// </summary>
    public ICollection<Product> Products { get; set; } = [];
}
