namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Result from a successful PAC stamp operation.
/// </summary>
public record PacStampResult(
    string Uuid,
    DateTime StampDate,
    string SatCertificateNumber,
    string PacStampSignature,
    string SatStampSignature,
    string QrCodeData,
    string StampedXml);

/// <summary>
/// Abstracts the PAC (Proveedor Autorizado de Certificación) for CFDI stamping.
/// Swap MockPACProvider → FinkelPACProvider when real PAC credentials are available.
/// </summary>
public interface IPACProvider
{
    /// <summary>
    /// Sends the signed XML to the PAC for stamping and returns the stamp data.
    /// </summary>
    Task<PacStampResult> StampAsync(string signedXml, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests cancellation of a stamped invoice.
    /// </summary>
    Task<bool> CancelAsync(string uuid, string issuerRfc, string reason, CancellationToken cancellationToken = default);
}
