namespace Corelio.Application.Pricing.Common;

/// <summary>
/// DTO representing a sample price at a specific margin percentage.
/// </summary>
public record SamplePriceDto(
    decimal MarginPercentage,
    decimal SalePrice,
    decimal PriceWithIva);
