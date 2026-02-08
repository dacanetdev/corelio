using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Queries.GetProductPricing;

/// <summary>
/// Query to get a single product's pricing information.
/// </summary>
public record GetProductPricingQuery(Guid ProductId) : IRequest<Result<ProductPricingDto>>;
