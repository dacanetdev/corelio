using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.ProductCategories.Commands.UpdateCategory;

/// <summary>
/// Handler for the UpdateCategoryCommand that updates an existing product category.
/// </summary>
public class UpdateCategoryCommandHandler(
    IProductCategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Get existing category
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result<bool>.Failure(
                new Error("Category.NotFound", $"Category with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        // Step 2: Check if category name already exists at the same level (exclude current)
        var nameExists = await categoryRepository.NameExistsAsync(
            request.Name,
            request.ParentCategoryId,
            request.Id,
            cancellationToken);

        if (nameExists)
        {
            var parentInfo = request.ParentCategoryId.HasValue ? "within the same parent category" : "at root level";
            return Result<bool>.Failure(
                new Error("Category.NameExists", $"A category with name '{request.Name}' already exists {parentInfo}.", ErrorType.Conflict));
        }

        // Step 3: If parent category is changing, validate and recalculate level/path
        int level = 0;
        string? path = null;

        if (request.ParentCategoryId.HasValue)
        {
            // Prevent setting self as parent
            if (request.ParentCategoryId.Value == request.Id)
            {
                return Result<bool>.Failure(
                    new Error("Category.SelfParent", "A category cannot be its own parent.", ErrorType.Validation));
            }

            var parentCategory = await categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
            {
                return Result<bool>.Failure(
                    new Error("Category.ParentNotFound", $"Parent category with ID '{request.ParentCategoryId}' not found.", ErrorType.Validation));
            }

            // Check max depth
            if (parentCategory.Level >= 5)
            {
                return Result<bool>.Failure(
                    new Error("Category.MaxDepthExceeded", "Cannot move category: maximum depth of 5 levels exceeded.", ErrorType.Validation));
            }

            level = parentCategory.Level + 1;
            path = $"{parentCategory.Path}{parentCategory.Name.ToLowerInvariant().Replace(" ", "-")}/";
        }
        else
        {
            path = "/";
        }

        // Step 4: Update category properties
        category.Name = request.Name;
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;
        category.ParentCategoryId = request.ParentCategoryId;
        category.Level = level;
        category.Path = path;
        category.SortOrder = request.SortOrder;
        category.ColorHex = request.ColorHex;
        category.IconName = request.IconName;
        category.IsActive = request.IsActive;

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
