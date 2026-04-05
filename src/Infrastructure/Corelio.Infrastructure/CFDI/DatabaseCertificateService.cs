using System.Security.Cryptography.X509Certificates;
using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Repositories;

namespace Corelio.Infrastructure.CFDI;

/// <summary>
/// Loads and stores CSD certificates in the TenantConfiguration database record.
/// The PFX bytes and password are stored encrypted at rest via the DB provider.
/// </summary>
public class DatabaseCertificateService(
    ITenantConfigurationRepository configRepository,
    IUnitOfWork unitOfWork) : ICertificateService
{
    public async Task<X509Certificate2?> LoadCertificateAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await configRepository.GetByTenantIdAsync(tenantId, cancellationToken);

        if (config?.CfdiCertificateData is null || config.CfdiCertificateData.Length == 0)
        {
            return null;
        }

        var password = config.CfdiCertificatePassword ?? string.Empty;

        return X509CertificateLoader.LoadPkcs12(
            config.CfdiCertificateData,
            password,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);
    }

    public async Task UploadCertificateAsync(Guid tenantId, byte[] pfxData, string password, CancellationToken cancellationToken = default)
    {
        var config = await configRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        if (config is null)
        {
            throw new InvalidOperationException($"TenantConfiguration not found for tenant {tenantId}.");
        }

        // Validate the PFX by loading it before saving
        using var cert = X509CertificateLoader.LoadPkcs12(pfxData, password, X509KeyStorageFlags.EphemeralKeySet);

        config.CfdiCertificateData = pfxData;
        config.CfdiCertificatePassword = password;
        config.CfdiCertificateExpiresAt = cert.NotAfter.ToUniversalTime();

        configRepository.Update(config);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<DateTime?> GetExpiryAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await configRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        return config?.CfdiCertificateExpiresAt;
    }
}
