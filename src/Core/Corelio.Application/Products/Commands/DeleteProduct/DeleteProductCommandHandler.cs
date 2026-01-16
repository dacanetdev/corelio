using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Handler for the DeleteProductCommand that performs a soft delete on a product.
/// </summary>
public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Get existing product
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            return Result<bool>.Failure(
                new Error("Product.NotFound", $"Product with ID '{request.Id}' not found.", ErrorType.NotFound));
        }

        // Step 2: Check if product is already deleted
        if (product.IsDeleted)
        {
            return Result<bool>.Failure(
                new Error("Product.AlreadyDeleted", "Product is already deleted.", ErrorType.Validation));
        }

        // Step 3: Perform soft delete
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        // Note: DeletedBy will be set by the audit interceptor if available

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
