using Corelio.Application.Common.Models;
using Corelio.Application.Pricing.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Pricing.Commands.CalculatePrices;

/// <summary>
/// Command to preview pricing calculations without persisting to database.
/// Returns net cost and sample prices at various margin percentages.
/// </summary>
public record CalculatePricesCommand(
    decimal ListPrice,
    List<decimal> Discounts,
    bool IvaEnabled,
    decimal IvaPercentage = 16.00m) : IRequest<Result<PricingCalculationResultDto>>;
