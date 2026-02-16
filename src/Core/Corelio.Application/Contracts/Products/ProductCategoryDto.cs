namespace Corelio.Application.Contracts.Products;

/// <summary>
/// Product category DTO with hierarchical support.
/// </summary>
public class ProductCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int Level { get; set; }
    public string? Path { get; set; }
    public int SortOrder { get; set; }
    public string? ColorHex { get; set; }
    public string? IconName { get; set; }
    public bool IsActive { get; set; }
}
