using System.Globalization;
using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// Generates an A4 PDF representation of a stamped CFDI 4.0 invoice.
/// </summary>
internal sealed class InvoiceDocument(Invoice invoice) : IDocument
{
    private static readonly CultureInfo EsMx = CultureInfo.GetCultureInfo("es-MX");

    public DocumentMetadata GetMetadata() => new()
    {
        Title = $"Factura {invoice.Serie}{invoice.Folio}",
        Author = "Corelio ERP"
    };

    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30, Unit.Point);
            page.DefaultTextStyle(t => t.FontSize(9));

            page.Content().Column(col =>
            {
                // Header
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(invoice.IssuerName).Bold().FontSize(14);
                        c.Item().Text($"RFC: {invoice.IssuerRfc}").FontSize(10);
                        c.Item().Text($"Régimen Fiscal: {invoice.IssuerTaxRegime}");
                    });

                    row.ConstantItem(180, Unit.Point).Column(c =>
                    {
                        c.Item().AlignRight().Text("FACTURA ELECTRÓNICA").Bold().FontSize(12);
                        c.Item().AlignRight().Text($"Serie: {invoice.Serie}  Folio: {invoice.Folio}").Bold();
                        var date = invoice.StampDate ?? invoice.CreatedAt;
                        c.Item().AlignRight().Text($"Fecha: {date.ToString("dd/MM/yyyy HH:mm", EsMx)}");
                    });
                });

                col.Item().PaddingTop(6).LineHorizontal(1f);

                // Receiver section
                col.Item().PaddingTop(8).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("RECEPTOR").Bold().FontSize(8);
                        c.Item().Text(invoice.ReceiverName).FontSize(10);
                        c.Item().Text($"RFC: {invoice.ReceiverRfc}");
                        c.Item().Text($"Régimen Fiscal: {invoice.ReceiverTaxRegime}");
                        c.Item().Text($"Uso CFDI: {invoice.ReceiverCfdiUse}");
                        c.Item().Text($"Código Postal: {invoice.ReceiverPostalCode}");
                    });

                    row.ConstantItem(180, Unit.Point).Column(c =>
                    {
                        c.Item().Text("PAGO").Bold().FontSize(8);
                        c.Item().Text($"Forma: {invoice.PaymentForm}");
                        c.Item().Text($"Método: {invoice.PaymentMethod}");
                        c.Item().Text($"Moneda: MXN");
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(0.5f);

                // Items table
                col.Item().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(50, Unit.Point);  // Clave SAT
                        c.RelativeColumn(5);               // Descripción
                        c.RelativeColumn(1.5f);            // Cantidad
                        c.RelativeColumn(2);               // Precio Unit.
                        c.RelativeColumn(2);               // Importe
                        c.RelativeColumn(1.5f);            // IVA
                        c.RelativeColumn(2);               // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Clave").Bold();
                        header.Cell().Text("Descripción").Bold();
                        header.Cell().AlignCenter().Text("Cant.").Bold();
                        header.Cell().AlignRight().Text("P. Unit.").Bold();
                        header.Cell().AlignRight().Text("Importe").Bold();
                        header.Cell().AlignRight().Text("IVA").Bold();
                        header.Cell().AlignRight().Text("Total").Bold();
                    });

                    foreach (var item in invoice.Items.OrderBy(i => i.ItemNumber))
                    {
                        var itemTotal = item.Amount + item.TaxAmount - item.Discount;
                        table.Cell().Text(item.ProductKey);
                        table.Cell().Text(item.Description);
                        table.Cell().AlignCenter().Text(item.Quantity.ToString("G29", EsMx));
                        table.Cell().AlignRight().Text(item.UnitValue.ToString("C", EsMx));
                        table.Cell().AlignRight().Text(item.Amount.ToString("C", EsMx));
                        table.Cell().AlignRight().Text(item.TaxAmount.ToString("C", EsMx));
                        table.Cell().AlignRight().Text(itemTotal.ToString("C", EsMx));
                    }
                });

                col.Item().PaddingTop(6).LineHorizontal(0.5f);

                // Totals
                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem();
                    row.ConstantItem(200, Unit.Point).Column(totals =>
                    {
                        totals.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Subtotal:");
                            r.ConstantItem(90, Unit.Point).AlignRight().Text(invoice.Subtotal.ToString("C", EsMx));
                        });
                        if (invoice.Discount > 0)
                        {
                            totals.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Descuento:");
                                r.ConstantItem(90, Unit.Point).AlignRight().Text($"-{invoice.Discount.ToString("C", EsMx)}");
                            });
                        }
                        var taxAmount = invoice.Total - (invoice.Subtotal - invoice.Discount);
                        totals.Item().Row(r =>
                        {
                            r.RelativeItem().Text("IVA (16%):");
                            r.ConstantItem(90, Unit.Point).AlignRight().Text(taxAmount.ToString("C", EsMx));
                        });
                        totals.Item().PaddingTop(2).Row(r =>
                        {
                            r.RelativeItem().Text("TOTAL:").Bold().FontSize(11);
                            r.ConstantItem(90, Unit.Point).AlignRight().Text(invoice.Total.ToString("C", EsMx)).Bold().FontSize(11);
                        });
                    });
                });

                // Stamp data (if stamped)
                if (!string.IsNullOrEmpty(invoice.Uuid))
                {
                    col.Item().PaddingTop(12).LineHorizontal(0.5f);
                    col.Item().PaddingTop(6).Column(stamp =>
                    {
                        stamp.Item().Text("TIMBRE FISCAL DIGITAL").Bold().FontSize(8);
                        stamp.Item().Text($"UUID (Folio Fiscal): {invoice.Uuid}").FontSize(7);
                        var stampDate = invoice.StampDate?.ToString("dd/MM/yyyy HH:mm:ss", EsMx) ?? "-";
                        stamp.Item().Text($"Fecha Timbrado: {stampDate}").FontSize(7);
                        stamp.Item().Text($"No. Certificado SAT: {invoice.SatCertificateNumber}").FontSize(7);

                        if (!string.IsNullOrEmpty(invoice.QrCodeData))
                        {
                            stamp.Item().PaddingTop(4).Text($"Verificar en: {invoice.QrCodeData}").FontSize(7).Italic();
                        }
                    });
                }

                // Cancellation notice
                if (invoice.Status == CfdiStatus.Cancelled)
                {
                    col.Item().PaddingTop(10).AlignCenter()
                        .Text("*** FACTURA CANCELADA ***")
                        .Bold().FontSize(14)
                        .FontColor(Colors.Red.Medium);
                }
            });
        });
    }
}
