namespace Corelio.WebAPI.Contracts.Pricing;

/// <summary>
/// Request DTO for previewing pricing calculations without persisting.
/// </summary>
public record CalculatePricesRequest(
    decimal ListPrice,
    List<decimal> Discounts,
    bool IvaEnabled,
    decimal IvaPercentage = 16.00m);
