using Corelio.Application.Common.Models;
using Corelio.Application.Products.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Products.Queries.GetProductById;

/// <summary>
/// Query to get a product by its ID.
/// </summary>
/// <param name="Id">The product ID.</param>
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;
