using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;

namespace Corelio.Infrastructure.CFDI;

/// <summary>
/// Orchestrates the full CFDI workflow:
/// LoadCert → GenerateSignedXML → PAC.Stamp → Persist UUID + seals
/// </summary>
public class CfdiService(
    IInvoiceRepository invoiceRepository,
    ITenantConfigurationRepository configRepository,
    ICertificateService certificateService,
    ICfdiXmlGenerator xmlGenerator,
    IPACProvider pacProvider,
    IUnitOfWork unitOfWork) : ICFDIService
{
    public async Task<string> StampAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken)
            ?? throw new InvalidOperationException($"Invoice '{invoiceId}' not found.");

        var config = await configRepository.GetByTenantIdAsync(invoice.TenantId, cancellationToken)
            ?? throw new InvalidOperationException($"TenantConfiguration not found for tenant '{invoice.TenantId}'.");

        // Load CSD certificate
        var certificate = await certificateService.LoadCertificateAsync(invoice.TenantId, cancellationToken);
        if (certificate is null)
        {
            throw new InvalidOperationException(
                "No CSD certificate found. Please upload a certificate in CFDI Settings.");
        }

        string signedXml;
        try
        {
            signedXml = await xmlGenerator.GenerateSignedXmlAsync(invoice, config, certificate, cancellationToken);
        }
        finally
        {
            // Always dispose certificate to release private key memory
            certificate.Dispose();
        }

        // Send to PAC for stamping
        invoice.Status = CfdiStatus.Stamping;
        invoiceRepository.Update(invoice);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var stampResult = await pacProvider.StampAsync(signedXml, cancellationToken);

        // Persist stamp data
        invoice.Status = CfdiStatus.Stamped;
        invoice.Uuid = stampResult.Uuid;
        invoice.StampDate = stampResult.StampDate;
        invoice.SatCertificateNumber = stampResult.SatCertificateNumber;
        invoice.PacStampSignature = stampResult.PacStampSignature;
        invoice.SatStampSignature = stampResult.SatStampSignature;
        invoice.QrCodeData = stampResult.QrCodeData;
        invoice.XmlContent = stampResult.StampedXml;

        invoiceRepository.Update(invoice);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return stampResult.Uuid;
    }

    public async Task CancelAsync(Guid invoiceId, string reason, CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken)
            ?? throw new InvalidOperationException($"Invoice '{invoiceId}' not found.");

        if (string.IsNullOrWhiteSpace(invoice.Uuid))
        {
            throw new InvalidOperationException("Invoice has no UUID — it has not been stamped.");
        }

        await pacProvider.CancelAsync(invoice.Uuid, invoice.IssuerRfc, reason, cancellationToken);

        invoice.Status = CfdiStatus.Cancelled;
        invoice.CancellationReason = reason;
        invoice.CancellationDate = DateTime.UtcNow;

        invoiceRepository.Update(invoice);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
