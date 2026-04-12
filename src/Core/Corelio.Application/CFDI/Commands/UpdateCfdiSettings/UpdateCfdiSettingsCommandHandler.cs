using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.UpdateCfdiSettings;

/// <summary>
/// Handler for UpdateCfdiSettingsCommand.
/// </summary>
public class UpdateCfdiSettingsCommandHandler(
    ITenantConfigurationRepository tenantConfigRepository,
    ITenantService tenantService,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCfdiSettingsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateCfdiSettingsCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<bool>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var config = await tenantConfigRepository.GetByTenantIdAsync(tenantId.Value, cancellationToken);
        if (config is null)
        {
            config = new TenantConfiguration { TenantId = tenantId.Value };
            await tenantConfigRepository.AddAsync(config, cancellationToken);
        }

        config.IssuerRfc = request.IssuerRfc;
        config.IssuerName = request.IssuerName;
        config.IssuerTaxRegime = request.IssuerTaxRegime;
        config.IssuerPostalCode = request.IssuerPostalCode;
        config.CfdiSeries = request.CfdiSeries;

        tenantConfigRepository.Update(config);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
