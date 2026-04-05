using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Entities;
using Corelio.Domain.Entities.CFDI;

namespace Corelio.Infrastructure.CFDI;

/// <summary>
/// Generates and digitally signs CFDI 4.0 XML per SAT specifications.
/// Uses System.Xml.Linq with the official SAT namespace.
/// </summary>
public class CfdiXmlGenerator : ICfdiXmlGenerator
{
    private static readonly XNamespace CfdiNs = "http://www.sat.gob.mx/cfd/4";
    private static readonly XNamespace XsiNs = "http://www.w3.org/2001/XMLSchema-instance";
    private static readonly CultureInfo InvCulture = CultureInfo.InvariantCulture;

    public Task<string> GenerateSignedXmlAsync(
        Invoice invoice,
        TenantConfiguration config,
        X509Certificate2 certificate,
        CancellationToken cancellationToken = default)
    {
        var fecha = (invoice.CreatedAt).ToString("yyyy-MM-ddTHH:mm:ss", InvCulture);
        var lugarExpedicion = config.IssuerPostalCode ?? "06600";

        // Build Conceptos
        var conceptos = invoice.Items.OrderBy(i => i.ItemNumber).Select(item =>
        {
            var concepto = new XElement(CfdiNs + "Concepto",
                new XAttribute("ClaveProdServ", item.ProductKey),
                new XAttribute("ClaveUnidad", item.UnitKey),
                new XAttribute("Cantidad", item.Quantity.ToString("F2", InvCulture)),
                new XAttribute("Descripcion", item.Description),
                new XAttribute("ValorUnitario", item.UnitValue.ToString("F2", InvCulture)),
                new XAttribute("Importe", item.Amount.ToString("F2", InvCulture)),
                new XAttribute("ObjetoImp", item.TaxObject));

            if (item.Discount > 0)
            {
                concepto.Add(new XAttribute("Descuento", item.Discount.ToString("F2", InvCulture)));
            }

            if (item.TaxObject == "02" && item.TaxAmount > 0)
            {
                var taxBase = item.Amount - item.Discount;
                concepto.Add(new XElement(CfdiNs + "Impuestos",
                    new XElement(CfdiNs + "Traslados",
                        new XElement(CfdiNs + "Traslado",
                            new XAttribute("Base", taxBase.ToString("F2", InvCulture)),
                            new XAttribute("Impuesto", "002"),
                            new XAttribute("TipoFactor", "Tasa"),
                            new XAttribute("TasaOCuota", item.TaxRate.ToString("F6", InvCulture)),
                            new XAttribute("Importe", item.TaxAmount.ToString("F2", InvCulture))))));
            }

            return concepto;
        }).ToList();

        // Aggregate taxes
        var totalTaxes = invoice.Items.Where(i => i.TaxObject == "02").Sum(i => i.TaxAmount);
        var taxBase = invoice.Items.Where(i => i.TaxObject == "02").Sum(i => i.Amount - i.Discount);

        // Build root Comprobante element
        var comprobante = new XElement(CfdiNs + "Comprobante",
            new XAttribute(XNamespace.Xmlns + "cfdi", CfdiNs.NamespaceName),
            new XAttribute(XNamespace.Xmlns + "xsi", XsiNs.NamespaceName),
            new XAttribute(XsiNs + "schemaLocation",
                "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"),
            new XAttribute("Version", "4.0"),
            new XAttribute("Serie", invoice.Serie),
            new XAttribute("Folio", invoice.Folio),
            new XAttribute("Fecha", fecha),
            new XAttribute("FormaPago", invoice.PaymentForm),
            new XAttribute("NoCertificado", certificate.SerialNumber ?? string.Empty),
            new XAttribute("SubTotal", invoice.Subtotal.ToString("F2", InvCulture)),
            invoice.Discount > 0
                ? new XAttribute("Descuento", invoice.Discount.ToString("F2", InvCulture))
                : null!,
            new XAttribute("Moneda", "MXN"),
            new XAttribute("Total", invoice.Total.ToString("F2", InvCulture)),
            new XAttribute("TipoDeComprobante", "I"),
            new XAttribute("MetodoPago", invoice.PaymentMethod),
            new XAttribute("LugarExpedicion", lugarExpedicion),
            new XElement(CfdiNs + "Emisor",
                new XAttribute("Rfc", invoice.IssuerRfc),
                new XAttribute("Nombre", invoice.IssuerName),
                new XAttribute("RegimenFiscal", invoice.IssuerTaxRegime)),
            new XElement(CfdiNs + "Receptor",
                new XAttribute("Rfc", invoice.ReceiverRfc),
                new XAttribute("Nombre", invoice.ReceiverName),
                new XAttribute("DomicilioFiscalReceptor", invoice.ReceiverPostalCode),
                new XAttribute("RegimenFiscalReceptor", invoice.ReceiverTaxRegime),
                new XAttribute("UsoCFDI", invoice.ReceiverCfdiUse)),
            new XElement(CfdiNs + "Conceptos", conceptos),
            totalTaxes > 0
                ? new XElement(CfdiNs + "Impuestos",
                    new XAttribute("TotalImpuestosTrasladados", totalTaxes.ToString("F2", InvCulture)),
                    new XElement(CfdiNs + "Traslados",
                        new XElement(CfdiNs + "Traslado",
                            new XAttribute("Base", taxBase.ToString("F2", InvCulture)),
                            new XAttribute("Impuesto", "002"),
                            new XAttribute("TipoFactor", "Tasa"),
                            new XAttribute("TasaOCuota", "0.160000"),
                            new XAttribute("Importe", totalTaxes.ToString("F2", InvCulture)))))
                : null!);

        // Compute original chain (cadena original)
        var originalChain = BuildOriginalChain(invoice, certificate, fecha, lugarExpedicion, totalTaxes, taxBase);
        invoice.OriginalChain = originalChain;

        // Sign the original chain
        var sello = SignWithCertificate(originalChain, certificate);

        // Add Certificado (base64) and Sello to root element
        var certBase64 = Convert.ToBase64String(certificate.RawData);
        comprobante.SetAttributeValue("Certificado", certBase64);
        comprobante.SetAttributeValue("Sello", sello);

        var result = comprobante.ToString(SaveOptions.DisableFormatting);
        return Task.FromResult(result);
    }

    private static string BuildOriginalChain(
        Invoice invoice,
        X509Certificate2 certificate,
        string fecha,
        string lugarExpedicion,
        decimal totalTaxes,
        decimal taxBase)
    {
        // SAT original chain: pipe-separated values in a specific order
        // All decimal values MUST use InvariantCulture (dot separator) as required by SAT
        var ic = InvCulture;
        var sb = new StringBuilder();
        sb.Append("||4.0|");
        sb.Append(string.Format(ic, "{0}|{1}|{2}|", invoice.Serie, invoice.Folio, fecha));
        sb.Append(string.Format(ic, "{0}|{1}|", invoice.PaymentForm, certificate.SerialNumber));
        sb.Append(string.Format(ic, "{0:F2}|", invoice.Subtotal));
        if (invoice.Discount > 0) { sb.Append(string.Format(ic, "{0:F2}|", invoice.Discount)); }
        sb.Append(string.Format(ic, "MXN|{0:F2}|I|{1}|{2}|", invoice.Total, invoice.PaymentMethod, lugarExpedicion));
        sb.Append(string.Format(ic, "{0}|{1}|{2}|", invoice.IssuerRfc, invoice.IssuerName, invoice.IssuerTaxRegime));
        sb.Append(string.Format(ic, "{0}|{1}|{2}|", invoice.ReceiverRfc, invoice.ReceiverName, invoice.ReceiverPostalCode));
        sb.Append(string.Format(ic, "{0}|{1}|", invoice.ReceiverTaxRegime, invoice.ReceiverCfdiUse));

        foreach (var item in invoice.Items.OrderBy(i => i.ItemNumber))
        {
            sb.Append(string.Format(ic, "{0}|{1}|{2:F2}|", item.ProductKey, item.UnitKey, item.Quantity));
            sb.Append(string.Format(ic, "{0}|{1:F2}|{2:F2}|{3}|", item.Description, item.UnitValue, item.Amount, item.TaxObject));
            if (item.TaxAmount > 0)
            {
                var ib = item.Amount - item.Discount;
                sb.Append(string.Format(ic, "{0:F2}|002|Tasa|{1:F6}|{2:F2}|", ib, item.TaxRate, item.TaxAmount));
            }
        }

        if (totalTaxes > 0)
        {
            sb.Append(string.Format(ic, "{0:F2}|{1:F2}|002|Tasa|0.160000|{2:F2}|", totalTaxes, taxBase, totalTaxes));
        }

        sb.Append('|');
        return sb.ToString();
    }

    private static string SignWithCertificate(string originalChain, X509Certificate2 certificate)
    {
        var rsa = certificate.GetRSAPrivateKey();
        if (rsa is null)
        {
            throw new InvalidOperationException("Certificate does not contain an RSA private key.");
        }

        var dataBytes = Encoding.UTF8.GetBytes(originalChain);
        var signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signatureBytes);
    }
}
