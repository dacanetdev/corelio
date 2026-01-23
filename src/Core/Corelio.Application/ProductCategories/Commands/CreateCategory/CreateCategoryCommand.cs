using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.ProductCategories.Commands.CreateCategory;

/// <summary>
/// Command to create a new product category.
/// </summary>
public record CreateCategoryCommand(
    string Name,
    string? Description = null,
    string? ImageUrl = null,
    Guid? ParentCategoryId = null,
    int SortOrder = 0,
    string? ColorHex = null,
    string? IconName = null,
    bool IsActive = true) : IRequest<Result<Guid>>;
