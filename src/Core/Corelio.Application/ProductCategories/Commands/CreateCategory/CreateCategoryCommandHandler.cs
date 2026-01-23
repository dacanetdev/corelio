using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.ProductCategories.Commands.CreateCategory;

/// <summary>
/// Handler for the CreateCategoryCommand that creates a new product category.
/// </summary>
public class CreateCategoryCommandHandler(
    IProductCategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Get current tenant ID
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        // Step 2: Check if category name already exists at the same level
        var nameExists = await categoryRepository.NameExistsAsync(
            request.Name,
            request.ParentCategoryId,
            null,
            cancellationToken);

        if (nameExists)
        {
            var parentInfo = request.ParentCategoryId.HasValue ? "within the same parent category" : "at root level";
            return Result<Guid>.Failure(
                new Error("Category.NameExists", $"A category with name '{request.Name}' already exists {parentInfo}.", ErrorType.Conflict));
        }

        // Step 3: If parent category is specified, validate it exists and check max depth
        int level = 0;
        string? path = null;

        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
            {
                return Result<Guid>.Failure(
                    new Error("Category.ParentNotFound", $"Parent category with ID '{request.ParentCategoryId}' not found.", ErrorType.Validation));
            }

            // Check max depth (level 0-5, so parent can be at most level 4)
            if (parentCategory.Level >= 5)
            {
                return Result<Guid>.Failure(
                    new Error("Category.MaxDepthExceeded", "Cannot create category: maximum depth of 5 levels exceeded.", ErrorType.Validation));
            }

            level = parentCategory.Level + 1;
            path = $"{parentCategory.Path}{parentCategory.Name.ToLowerInvariant().Replace(" ", "-")}/";
        }
        else
        {
            path = "/";
        }

        // Step 4: Create category entity
        var category = new ProductCategory
        {
            TenantId = tenantId.Value,
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            ParentCategoryId = request.ParentCategoryId,
            Level = level,
            Path = path,
            SortOrder = request.SortOrder,
            ColorHex = request.ColorHex,
            IconName = request.IconName,
            IsActive = request.IsActive
        };

        categoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(category.Id);
    }
}
