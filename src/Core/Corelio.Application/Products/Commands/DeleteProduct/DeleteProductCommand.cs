using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Command to delete a product (soft delete).
/// </summary>
/// <param name="Id">The product ID to delete.</param>
public record DeleteProductCommand(Guid Id) : IRequest<Result<bool>>;
