namespace Corelio.Application.ProductCategories.Common;

/// <summary>
/// Product category DTO with hierarchical support.
/// </summary>
public class ProductCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    // Hierarchy
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int Level { get; set; }
    public string? Path { get; set; }

    // Display
    public int SortOrder { get; set; }
    public string? ColorHex { get; set; }
    public string? IconName { get; set; }

    // Status
    public bool IsActive { get; set; }

    // Audit Properties
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
