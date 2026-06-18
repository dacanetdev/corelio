using System.Globalization;
using Corelio.Application.Reports.Purchases.Common;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// QuestPDF A4 document for the purchase summary report (in Spanish es-MX).
/// </summary>
internal sealed class PurchaseReportDocument(PurchaseSummaryReportDto report) : IDocument
{
    private static readonly CultureInfo EsMx = CultureInfo.GetCultureInfo("es-MX");

    public DocumentMetadata GetMetadata() => new()
    {
        Title = $"Reporte de Compras {report.DateFrom:dd/MM/yyyy} - {report.DateTo:dd/MM/yyyy}",
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
                        c.Item().AlignRight().Text("REPORTE DE COMPRAS").Bold().FontSize(12);
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
                    KpiCard(row, "Total Órdenes", report.TotalOrders.ToString());
                    KpiCard(row, "Total Gastado", report.TotalAmountSpent.ToString("C", EsMx));
                    KpiCard(row, "Entregas Pendientes", report.PendingDeliveries.ToString());
                });

                // By Supplier
                col.Item().PaddingBottom(6).Text("GASTO POR PROVEEDOR").Bold().FontSize(11);
                col.Item().PaddingBottom(12).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(4);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(3);
                    });

                    TableHeader(table, ["Proveedor", "Órdenes", "Total"]);

                    foreach (var s in report.BySupplier)
                    {
                        TableCell(table, s.SupplierName);
                        TableCellRight(table, s.OrderCount.ToString());
                        TableCellRight(table, s.TotalAmount.ToString("C", EsMx));
                    }

                    if (report.BySupplier.Count == 0)
                    {
                        col.Item().Text("Sin órdenes en el período.").Italic();
                    }
                });

                // By Status
                col.Item().PaddingBottom(6).Text("DESGLOSE POR ESTATUS").Bold().FontSize(11);
                col.Item().PaddingBottom(12).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(4);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(3);
                    });

                    TableHeader(table, ["Estatus", "Órdenes", "Total"]);

                    foreach (var s in report.ByStatus)
                    {
                        TableCell(table, s.StatusName);
                        TableCellRight(table, s.OrderCount.ToString());
                        TableCellRight(table, s.TotalAmount.ToString("C", EsMx));
                    }

                    if (report.ByStatus.Count == 0)
                    {
                        col.Item().Text("Sin datos de estatus.").Italic();
                    }
                });

                // Product Receipts
                if (report.ProductReceipts.Count > 0)
                {
                    col.Item().PaddingBottom(6).Text("PRODUCTOS — RECIBIDO VS PENDIENTE").Bold().FontSize(11);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(4);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                        });

                        TableHeader(table, ["Producto", "SKU", "Ordenado", "Recibido", "Pendiente"]);

                        foreach (var p in report.ProductReceipts)
                        {
                            TableCell(table, p.ProductName);
                            TableCell(table, p.ProductSku);
                            TableCellRight(table, p.OrderedQuantity.ToString("0.##"));
                            TableCellRight(table, p.ReceivedQuantity.ToString("0.##"));
                            TableCellRight(table, p.PendingQuantity.ToString("0.##"));
                        }
                    });
                }
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
