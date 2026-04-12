using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.UpdateCfdiSettings;

/// <summary>
/// Updates the tenant's CFDI issuer configuration.
/// </summary>
public record UpdateCfdiSettingsCommand(
    string IssuerRfc,
    string IssuerName,
    string IssuerTaxRegime,
    string IssuerPostalCode,
    string CfdiSeries) : IRequest<Result<bool>>;
