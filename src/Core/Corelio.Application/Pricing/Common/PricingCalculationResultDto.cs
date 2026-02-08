namespace Corelio.Application.Pricing.Common;

/// <summary>
/// DTO representing the result of a pricing calculation preview.
/// </summary>
public record PricingCalculationResultDto(
    decimal NetCost,
    List<SamplePriceDto> SamplePrices);
