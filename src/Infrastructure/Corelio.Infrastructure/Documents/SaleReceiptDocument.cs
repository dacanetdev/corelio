using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// QuestPDF document that renders a sale receipt (ticket) in Spanish (es-MX).
/// </summary>
internal sealed class SaleReceiptDocument(Sale sale) : IDocument
{
    private static readonly System.Globalization.CultureInfo EsMx =
        System.Globalization.CultureInfo.GetCultureInfo("es-MX");

    public DocumentMetadata GetMetadata() => new()
    {
        Title = $"Recibo {sale.Folio}",
        Author = "Corelio ERP"
    };

    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            // Narrow receipt-style page (80mm wide)
            page.Size(PageSizes.A7.Landscape());
            page.Size(227, 600, Unit.Point); // ~80mm wide
            page.Margin(12, Unit.Point);
            page.DefaultTextStyle(t => t.FontSize(8));

            page.Content().Column(col =>
            {
                // Header
                col.Item().AlignCenter().Text("CORELIO").Bold().FontSize(14);
                col.Item().AlignCenter().Text("Ferretería y Materiales").FontSize(9);
                col.Item().PaddingTop(4).LineHorizontal(0.5f);

                // Sale info
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("Folio: ").Bold();
                        t.Span(sale.Folio);
                    });
                    row.RelativeItem().AlignRight().Text(t =>
                    {
                        var date = (sale.CompletedAt ?? sale.CreatedAt).ToString("dd/MM/yyyy HH:mm", EsMx);
                        t.Span(date);
                    });
                });

                if (sale.Customer is not null)
                {
                    col.Item().Text(t =>
                    {
                        t.Span("Cliente: ").Bold();
                        t.Span(sale.Customer.FullName);
                    });
                }

                col.Item().PaddingTop(4).LineHorizontal(0.5f);

                // Items table
                col.Item().PaddingTop(4).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(4);   // Producto
                        c.RelativeColumn(1.5f); // Cant.
                        c.RelativeColumn(2);   // Precio
                        c.RelativeColumn(2);   // Total
                    });

                    // Header row
                    table.Header(header =>
                    {
                        header.Cell().Text("Producto").Bold();
                        header.Cell().AlignCenter().Text("Cant.").Bold();
                        header.Cell().AlignRight().Text("P.Unit").Bold();
                        header.Cell().AlignRight().Text("Total").Bold();
                    });

                    foreach (var item in sale.Items)
                    {
                        table.Cell().Text(item.ProductName);
                        table.Cell().AlignCenter().Text(item.Quantity.ToString("G29", EsMx));
                        table.Cell().AlignRight().Text(item.UnitPrice.ToString("C", EsMx));
                        table.Cell().AlignRight().Text(item.LineTotal.ToString("C", EsMx));
                    }
                });

                col.Item().PaddingTop(4).LineHorizontal(0.5f);

                // Totals
                col.Item().PaddingTop(4).Column(totals =>
                {
                    totals.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Subtotal:");
                        row.ConstantItem(70, Unit.Point).AlignRight().Text(sale.SubTotal.ToString("C", EsMx));
                    });

                    if (sale.DiscountAmount > 0)
                    {
                        totals.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Descuento:");
                            row.ConstantItem(70, Unit.Point).AlignRight().Text($"-{sale.DiscountAmount.ToString("C", EsMx)}");
                        });
                    }

                    totals.Item().Row(row =>
                    {
                        row.RelativeItem().Text("IVA (16%):");
                        row.ConstantItem(70, Unit.Point).AlignRight().Text(sale.TaxAmount.ToString("C", EsMx));
                    });

                    totals.Item().PaddingTop(2).Row(row =>
                    {
                        row.RelativeItem().Text("TOTAL:").Bold().FontSize(10);
                        row.ConstantItem(70, Unit.Point).AlignRight().Text(sale.Total.ToString("C", EsMx)).Bold().FontSize(10);
                    });
                });

                col.Item().PaddingTop(4).LineHorizontal(0.5f);

                // Payments
                col.Item().PaddingTop(4).Text("Pagos:").Bold();
                foreach (var payment in sale.Payments)
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text(GetPaymentMethodLabel(payment.Method));
                        row.ConstantItem(70, Unit.Point).AlignRight().Text(payment.Amount.ToString("C", EsMx));
                    });
                }

                // Cash change
                var cashPayments = sale.Payments.Where(p => p.Method == PaymentMethod.Cash).ToList();
                if (cashPayments.Count > 0)
                {
                    var totalCash = cashPayments.Sum(p => p.Amount);
                    var change = totalCash - sale.Total;
                    if (change > 0)
                    {
                        col.Item().PaddingTop(2).Row(row =>
                        {
                            row.RelativeItem().Text("Cambio:").Bold();
                            row.ConstantItem(70, Unit.Point).AlignRight().Text(change.ToString("C", EsMx)).Bold();
                        });
                    }
                }

                col.Item().PaddingTop(8).LineHorizontal(0.5f);
                col.Item().PaddingTop(6).AlignCenter().Text("¡Gracias por su compra!").Italic();
            });
        });
    }

    private static string GetPaymentMethodLabel(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Efectivo",
        PaymentMethod.Card => "Tarjeta",
        PaymentMethod.Transfer => "Transferencia",
        PaymentMethod.Check => "Cheque",
        PaymentMethod.Credit => "Crédito",
        _ => method.ToString()
    };
}
