using System.Globalization;
using Corelio.Application.Reports.Inventory.Common;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// QuestPDF A4 document for the inventory valuation report (in Spanish es-MX).
/// </summary>
internal sealed class InventoryReportDocument(InventoryValuationReportDto report) : IDocument
{
    private static readonly CultureInfo EsMx = CultureInfo.GetCultureInfo("es-MX");

    public DocumentMetadata GetMetadata() => new()
    {
        Title = $"Reporte de Valuación de Inventario {report.GeneratedAt.ToString("dd/MM/yyyy", EsMx)}",
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
                    row.ConstantItem(220, Unit.Point).Column(c =>
                    {
                        c.Item().AlignRight().Text("REPORTE DE VALUACIÓN DE INVENTARIO").Bold().FontSize(11);
                        c.Item().AlignRight().Text($"Generado: {report.GeneratedAt.ToString("dd/MM/yyyy HH:mm", EsMx)}");
                        if (report.WarehouseId.HasValue)
                        {
                            c.Item().AlignRight().Text($"Almacén: {report.WarehouseId}").FontSize(8);
                        }
                    });
                });
                col.Item().PaddingTop(6).LineHorizontal(1);
            });

            page.Content().PaddingTop(12).Column(col =>
            {
                // KPI row
                col.Item().PaddingBottom(6).Text("Resumen General").Bold().FontSize(11);
                col.Item().PaddingBottom(12).Row(row =>
                {
                    KpiCard(row, "Valor Total Inventario", report.TotalInventoryValue.ToString("C", EsMx));
                    KpiCard(row, "Productos en Stock", report.TotalProductCount.ToString(EsMx));
                });

                // Category breakdown
                col.Item().PaddingBottom(6).Text("VALUACIÓN POR CATEGORÍA").Bold().FontSize(11);
                col.Item().PaddingBottom(12).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(4);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                    });

                    TableHeader(table, ["Categoría", "Productos", "Valor Total", "Porcentaje"]);

                    foreach (var cat in report.ValueByCategory)
                    {
                        TableCell(table, cat.CategoryName);
                        TableCellRight(table, cat.ProductCount.ToString(EsMx));
                        TableCellRight(table, cat.TotalValue.ToString("C", EsMx));
                        TableCellRight(table, $"{cat.Percentage:0.##}%");
                    }

                    if (report.ValueByCategory.Count == 0)
                    {
                        col.Item().Text("Sin datos de categorías.").Italic();
                    }
                });

                // Product detail
                col.Item().PaddingBottom(6).Text("DETALLE DE INVENTARIO").Bold().FontSize(11);
                col.Item().PaddingBottom(12).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(4);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(3);
                    });

                    TableHeader(table, ["Producto", "SKU", "Categoría", "Cantidad", "Costo Prom.", "Valor Total"]);

                    foreach (var item in report.ProductValuations)
                    {
                        TableCell(table, item.ProductName);
                        TableCell(table, item.ProductSku);
                        TableCell(table, item.CategoryName ?? "Sin categoría");
                        TableCellRight(table, item.Quantity.ToString("0.##", EsMx));
                        TableCellRight(table, item.AverageCost.ToString("C", EsMx));
                        TableCellRight(table, item.ExtendedValue.ToString("C", EsMx));
                    }

                    if (report.ProductValuations.Count == 0)
                    {
                        col.Item().Text("Sin productos en inventario.").Italic();
                    }
                });

                // Low stock alerts (only if any exist)
                if (report.LowStockAlerts.Count > 0)
                {
                    col.Item().PaddingBottom(6).Text("ALERTAS DE STOCK MÍNIMO").Bold().FontSize(11).FontColor("#CC0000");
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

                        TableHeader(table, ["Producto", "SKU", "Cantidad", "Mín. Requerido", "Costo Prom."]);

                        foreach (var alert in report.LowStockAlerts)
                        {
                            TableCell(table, alert.ProductName);
                            TableCell(table, alert.ProductSku);
                            TableCellRight(table, alert.Quantity.ToString("0.##", EsMx));
                            TableCellRight(table, alert.MinimumLevel.ToString("0.##", EsMx));
                            TableCellRight(table, alert.AverageCost.ToString("C", EsMx));
                        }
                    });
                }
            });

            page.Footer().AlignCenter().Text(t =>
            {
                t.Span($"Generado el {report.GeneratedAt.ToString("dd/MM/yyyy HH:mm", EsMx)} | Corelio ERP   Página ");
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
