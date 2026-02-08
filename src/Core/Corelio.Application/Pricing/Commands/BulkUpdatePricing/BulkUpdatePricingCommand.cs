using Corelio.Application.Common.Enums;
using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Commands.BulkUpdatePricing;

/// <summary>
/// Command to apply a bulk pricing update to multiple products.
/// </summary>
public record BulkUpdatePricingCommand(
    List<Guid> ProductIds,
    PricingUpdateType UpdateType,
    decimal Value,
    int? TierNumber = null) : IRequest<Result<int>>;
