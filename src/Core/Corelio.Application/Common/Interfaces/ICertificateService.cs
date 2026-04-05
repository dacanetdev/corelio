using System.Security.Cryptography.X509Certificates;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Manages CSD (Certificado de Sello Digital) certificates for CFDI signing.
/// </summary>
public interface ICertificateService
{
    /// <summary>
    /// Loads the tenant's CSD certificate from secure storage.
    /// Returns null if no certificate has been uploaded.
    /// </summary>
    Task<X509Certificate2?> LoadCertificateAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a PFX certificate for the tenant.
    /// Also updates CfdiCertificateExpiresAt on the TenantConfiguration.
    /// </summary>
    Task UploadCertificateAsync(Guid tenantId, byte[] pfxData, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the certificate expiry date for the tenant, or null if no cert uploaded.
    /// </summary>
    Task<DateTime?> GetExpiryAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
