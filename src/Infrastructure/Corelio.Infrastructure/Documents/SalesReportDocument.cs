using System.Globalization;
using Corelio.Application.Reports.Sales.Common;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// QuestPDF A4 document for the daily sales report (in Spanish es-MX).
/// </summary>
internal sealed class SalesReportDocument(SalesReportDto report) : IDocument
{
    private static readonly CultureInfo EsMx = CultureInfo.GetCultureInfo("es-MX");

    public DocumentMetadata GetMetadata() => new()
    {
        Title = $"Reporte de Ventas {report.DateFrom:dd/MM/yyyy} - {report.DateTo:dd/MM/yyyy}",
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

            page.Header().Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Corelio ERP").Bold().FontSize(14);
                        c.Item().Text("Sistema de Gestión").FontSize(10);
                    });
                    row.ConstantItem(200, Unit.Point).Column(c =>
                    {
                        c.Item().AlignRight().Text("REPORTE DE VENTAS").Bold().FontSize(12);
                        c.Item().AlignRight().Text($"Período: {report.DateFrom.ToString("dd/MM/yyyy", EsMx)} — {report.DateTo.ToString("dd/MM/yyyy", EsMx)}");
                        c.Item().AlignRight().Text($"Generado: {DateTime.Now.ToString("dd/MM/yyyy HH:mm", EsMx)}");
                    });
                });
                col.Item().PaddingTop(6).LineHorizontal(1);
            });

            page.Content().PaddingTop(12).Column(col =>
            {
                // KPI Summary
                col.Item().PaddingBottom(6).Text("Resumen General").Bold().FontSize(11);
                col.Item().PaddingBottom(12).Row(row =>
                {
                    KpiCard(row, "Total Ventas", report.TotalAmount.ToString("C", EsMx));
                    KpiCard(row, "Transacciones", report.TransactionCount.ToString());
                    KpiCard(row, "Ticket Promedio", report.AverageTicket.ToString("C", EsMx));
                });

                // Payment method breakdown
                col.Item().PaddingBottom(6).Text("Ventas por Método de Pago").Bold().FontSize(11);
                col.Item().PaddingBottom(12).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(2);
                    });

                    TableHeader(table, ["Método", "Monto", "Transacciones", "Porcentaje"]);

                    foreach (var pm in report.PaymentMethodBreakdown)
                    {
                        TableCell(table, pm.MethodName);
                        TableCellRight(table, pm.Amount.ToString("C", EsMx));
                        TableCellRight(table, pm.TransactionCount.ToString());
                        TableCellRight(table, $"{pm.Percentage:0.##}%");
                    }

                    if (report.PaymentMethodBreakdown.Count == 0)
                    {
                        col.Item().Text("Sin datos de pagos.").Italic();
                    }
                });

                // Top 10 products
                col.Item().PaddingBottom(6).Text("Top 10 Productos").Bold().FontSize(11);
                col.Item().PaddingBottom(12).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(20);
                        cols.RelativeColumn(4);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                    });

                    TableHeader(table, ["#", "Producto", "SKU", "Cant. Vendida", "Ingresos"]);

                    for (var i = 0; i < report.TopProducts.Count; i++)
                    {
                        var p = report.TopProducts[i];
                        TableCell(table, (i + 1).ToString());
                        TableCell(table, p.ProductName);
                        TableCell(table, p.ProductSku);
                        TableCellRight(table, p.QuantitySold.ToString("0.##"));
                        TableCellRight(table, p.Revenue.ToString("C", EsMx));
                    }

                    if (report.TopProducts.Count == 0)
                    {
                        col.Item().Text("Sin datos de productos.").Italic();
                    }
                });

                // Hourly distribution
                col.Item().PaddingBottom(6).Text("Distribución por Hora").Bold().FontSize(11);
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(50);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                    });

                    TableHeader(table, ["Hora", "Monto", "Transacciones"]);

                    foreach (var h in report.HourlyDistribution.Where(h => h.TransactionCount > 0))
                    {
                        TableCell(table, $"{h.Hour:00}:00");
                        TableCellRight(table, h.Amount.ToString("C", EsMx));
                        TableCellRight(table, h.TransactionCount.ToString());
                    }

                    if (!report.HourlyDistribution.Any(h => h.TransactionCount > 0))
                    {
                        col.Item().Text("Sin ventas en el período.").Italic();
                    }
                });
            });

            page.Footer().AlignCenter().Text(t =>
            {
                t.Span("Página ");
                t.CurrentPageNumber();
                t.Span(" de ");
                t.TotalPages();
            });
        });
    }

    private static void KpiCard(RowDescriptor row, string label, string value)
    {
        row.RelativeItem().Border(0.5f).Padding(8).Column(c =>
        {
            c.Item().AlignCenter().Text(label).FontSize(8).FontColor("#666666");
            c.Item().AlignCenter().Text(value).Bold().FontSize(13);
        });
    }

    private static void TableHeader(TableDescriptor table, IReadOnlyList<string> headers)
    {
        foreach (var header in headers)
        {
            table.Header(h => h.Cell().Background("#333333").Padding(4)
                .Text(header).FontColor("#FFFFFF").Bold());
        }
    }

    private static void TableCell(TableDescriptor table, string text)
    {
        table.Cell().BorderBottom(0.5f).BorderColor("#DDDDDD").Padding(4).Text(text);
    }

    private static void TableCellRight(TableDescriptor table, string text)
    {
        table.Cell().BorderBottom(0.5f).BorderColor("#DDDDDD").Padding(4).AlignRight().Text(text);
    }
}
