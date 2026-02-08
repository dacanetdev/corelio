using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.ProductCategories.Commands.UpdateCategory;

/// <summary>
/// Command to update an existing product category.
/// </summary>
public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description = null,
    string? ImageUrl = null,
    Guid? ParentCategoryId = null,
    int SortOrder = 0,
    string? ColorHex = null,
    string? IconName = null,
    bool IsActive = true) : IRequest<Result<bool>>;
