using Corelio.Application.Common.Enums;

namespace Corelio.WebAPI.Contracts.Pricing;

/// <summary>
/// Request DTO for applying a bulk pricing update to multiple products.
/// </summary>
public record BulkUpdatePricingRequest(
    List<Guid> ProductIds,
    PricingUpdateType UpdateType,
    decimal Value,
    int? TierNumber = null);
