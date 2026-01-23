namespace Corelio.Application.ProductCategories.Common;

/// <summary>
/// Data transfer object for ProductCategory.
/// </summary>
public record ProductCategoryDto(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    Guid? ParentCategoryId,
    string? ParentCategoryName,
    int Level,
    string? Path,
    int SortOrder,
    string? ColorHex,
    string? IconName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
