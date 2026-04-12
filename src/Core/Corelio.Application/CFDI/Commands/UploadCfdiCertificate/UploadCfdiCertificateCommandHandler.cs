using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.UploadCfdiCertificate;

/// <summary>
/// Handler for UploadCfdiCertificateCommand.
/// Delegates to ICertificateService which stores PFX bytes and updates expiry.
/// </summary>
public class UploadCfdiCertificateCommandHandler(
    ICertificateService certificateService,
    ITenantService tenantService) : IRequestHandler<UploadCfdiCertificateCommand, Result<DateTime>>
{
    public async Task<Result<DateTime>> Handle(
        UploadCfdiCertificateCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<DateTime>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        try
        {
            await certificateService.UploadCertificateAsync(
                tenantId.Value, request.CertificateData, request.Password, cancellationToken);

            var expiry = await certificateService.GetExpiryAsync(tenantId.Value, cancellationToken);
            return Result<DateTime>.Success(expiry ?? DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            return Result<DateTime>.Failure(
                new Error("Certificate.Invalid",
                    $"Certificate could not be loaded: {ex.Message}",
                    ErrorType.Validation));
        }
    }
}
