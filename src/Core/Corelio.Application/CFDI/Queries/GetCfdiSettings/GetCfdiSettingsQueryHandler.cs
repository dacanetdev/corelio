using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Queries.GetCfdiSettings;

/// <summary>
/// Handler for GetCfdiSettingsQuery.
/// </summary>
public class GetCfdiSettingsQueryHandler(
    ITenantConfigurationRepository tenantConfigRepository,
    ITenantService tenantService) : IRequestHandler<GetCfdiSettingsQuery, Result<CfdiSettingsDto>>
{
    public async Task<Result<CfdiSettingsDto>> Handle(
        GetCfdiSettingsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<CfdiSettingsDto>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var config = await tenantConfigRepository.GetByTenantIdAsync(tenantId.Value, cancellationToken);
        if (config is null)
        {
            // Return empty settings — tenant hasn't configured CFDI yet
            return Result<CfdiSettingsDto>.Success(new CfdiSettingsDto(
                null, null, null, null, "A", 1, null, false));
        }

        var dto = new CfdiSettingsDto(
            config.IssuerRfc,
            config.IssuerName,
            config.IssuerTaxRegime,
            config.IssuerPostalCode,
            config.CfdiSeries,
            config.CfdiNextFolio,
            config.CfdiCertificateExpiresAt,
            config.CfdiCertificateData is { Length: > 0 });

        return Result<CfdiSettingsDto>.Success(dto);
    }
}
