using System.Security.Cryptography.X509Certificates;
using Corelio.Domain.Entities;
using Corelio.Domain.Entities.CFDI;

namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Generates and digitally signs CFDI 4.0 XML documents.
/// </summary>
public interface ICfdiXmlGenerator
{
    /// <summary>
    /// Builds the CFDI 4.0 XML, computes the original chain, signs it with the given
    /// certificate, and returns the signed XML string ready for PAC stamping.
    /// </summary>
    Task<string> GenerateSignedXmlAsync(
        Invoice invoice,
        TenantConfiguration config,
        X509Certificate2 certificate,
        CancellationToken cancellationToken = default);
}
