using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.UploadCfdiCertificate;

/// <summary>
/// Uploads a CSD (.pfx) certificate for the tenant.
/// Stores encrypted bytes in TenantConfiguration and updates expiry date.
/// </summary>
public record UploadCfdiCertificateCommand(
    byte[] CertificateData,
    string Password) : IRequest<Result<DateTime>>;
