using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.Application.Services;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Commands.CalculatePrices;

/// <summary>
/// Handler for previewing pricing calculations (no database persistence).
/// </summary>
public class CalculatePricesCommandHandler
    : IRequestHandler<CalculatePricesCommand, Result<PricingCalculationResultDto>>
{
    private static readonly decimal[] SampleMargins = [10, 15, 20, 25, 30, 35, 40, 45, 50];

    public Task<Result<PricingCalculationResultDto>> Handle(
        CalculatePricesCommand request,
        CancellationToken cancellationToken)
    {
        var netCost = PricingCalculationService.CalculateNetCost(request.ListPrice, request.Discounts);

        var samplePrices = new List<SamplePriceDto>();

        foreach (var margin in SampleMargins)
        {
            var salePrice = PricingCalculationService.CalculateSalePriceFromMargin(netCost, margin);
            var priceWithIva = request.IvaEnabled
                ? PricingCalculationService.ApplyIva(salePrice, request.IvaPercentage)
                : salePrice;

            samplePrices.Add(new SamplePriceDto(margin, salePrice, priceWithIva));
        }

        var result = new PricingCalculationResultDto(netCost, samplePrices);
        return Task.FromResult(Result<PricingCalculationResultDto>.Success(result));
    }
}
