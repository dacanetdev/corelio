using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.ProductCategories.Commands.DeleteCategory;

/// <summary>
/// Handler for the DeleteCategoryCommand that soft deletes a product category.
/// </summary>
public class DeleteCategoryCommandHandler(
    IProductCategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Get existing category
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result<bool>.Failure(
                new Error("Category.NotFound", $"Category with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        // Step 2: Check if category is already deleted
        if (category.IsDeleted)
        {
            return Result<bool>.Failure(
                new Error("Category.AlreadyDeleted", "Category has already been deleted.", ErrorType.Conflict));
        }

        // Step 3: Check if category has any products
        var hasProducts = await categoryRepository.HasProductsAsync(request.Id, cancellationToken);
        if (hasProducts)
        {
            return Result<bool>.Failure(
                new Error("Category.HasProducts", "Cannot delete category because it has products assigned. Please reassign or delete the products first.", ErrorType.Conflict));
        }

        // Step 4: Check if category has child categories
        var childCategories = await categoryRepository.GetChildCategoriesAsync(request.Id, cancellationToken);
        if (childCategories.Count > 0)
        {
            return Result<bool>.Failure(
                new Error("Category.HasChildren", "Cannot delete category because it has child categories. Please delete or move the child categories first.", ErrorType.Conflict));
        }

        // Step 5: Soft delete the category
        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
