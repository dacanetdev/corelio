using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.ProductCategories.Commands.DeleteCategory;

/// <summary>
/// Command to delete a product category (soft delete).
/// </summary>
/// <param name="Id">The category ID to delete.</param>
public record DeleteCategoryCommand(Guid Id) : IRequest<Result<bool>>;
