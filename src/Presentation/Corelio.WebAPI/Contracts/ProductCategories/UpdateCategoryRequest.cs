namespace Corelio.WebAPI.Contracts.ProductCategories;

/// <summary>
/// Request DTO for updating a product category.
/// </summary>
public record UpdateCategoryRequest(
    string Name,
    string? Description = null,
    string? ImageUrl = null,
    Guid? ParentCategoryId = null,
    int SortOrder = 0,
    string? ColorHex = null,
    string? IconName = null,
    bool IsActive = true);
