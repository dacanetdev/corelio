namespace Corelio.BlazorApp.Models.Pricing;

/// <summary>
/// Blazor model for the result of a pricing calculation preview.
/// </summary>
public record PricingCalculationResultModel(
    decimal NetCost,
    List<SamplePriceModel> SamplePrices);
