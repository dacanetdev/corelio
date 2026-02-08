using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Queries.GetTenantPricingConfig;

/// <summary>
/// Query to get the current tenant's pricing configuration.
/// No parameters needed — tenant is resolved from context.
/// </summary>
public record GetTenantPricingConfigQuery : IRequest<Result<TenantPricingConfigDto>>;
