using Corelio.Application.Common.Interfaces;

namespace Corelio.Infrastructure.CFDI;

/// <summary>
/// Mock PAC provider for development/testing.
/// Returns deterministic fake stamp data without hitting a real PAC.
/// Replace with FinkelPACProvider when sandbox credentials are available.
/// </summary>
public class MockPACProvider : IPACProvider
{
    public Task<PacStampResult> StampAsync(string signedXml, CancellationToken cancellationToken = default)
    {
        var uuid = Guid.NewGuid().ToString().ToUpper();
        var stampDate = DateTime.UtcNow;
        var certNumber = "20001000000300022323"; // Fake SAT certificate number

        // Build a mock QR code URL (format used by SAT verification portal)
        var qrData = $"https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id={uuid}";

        var result = new PacStampResult(
            Uuid: uuid,
            StampDate: stampDate,
            SatCertificateNumber: certNumber,
            PacStampSignature: $"MOCK-PAC-SIGNATURE-{Guid.NewGuid():N}",
            SatStampSignature: $"MOCK-SAT-SIGNATURE-{Guid.NewGuid():N}",
            QrCodeData: qrData,
            StampedXml: signedXml); // Mock returns same XML (real PAC adds TimbreFiscalDigital node)

        return Task.FromResult(result);
    }

    public Task<bool> CancelAsync(string uuid, string issuerRfc, string reason, CancellationToken cancellationToken = default)
    {
        // Mock always succeeds
        return Task.FromResult(true);
    }
}
