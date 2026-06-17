using System.Globalization;
using System.Text;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Inventory.Common;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// Generates PDF and CSV exports for the inventory valuation report.
/// </summary>
internal sealed class InventoryReportExportService : IInventoryReportExportService
{
    private static readonly CultureInfo EsMx = CultureInfo.GetCultureInfo("es-MX");

    static InventoryReportExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GeneratePdfAsync(InventoryValuationReportDto report, CancellationToken cancellationToken = default)
    {
        var document = new InventoryReportDocument(report);
        return Task.FromResult(document.GeneratePdf());
    }

    public Task<byte[]> GenerateCsvAsync(InventoryValuationReportDto report, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        // Summary KPIs
        sb.AppendLine("RESUMEN GENERAL");
        sb.AppendLine("Indicador,Valor");
        sb.AppendLine(EsMx, $"Valor Total Inventario,{report.TotalInventoryValue:F2}");
        sb.AppendLine(EsMx, $"Total Productos en Stock,{report.TotalProductCount}");
        sb.AppendLine(EsMx, $"Generado El,{report.GeneratedAt:dd/MM/yyyy HH:mm}");
        sb.AppendLine();

        // Category breakdown
        sb.AppendLine("VALUACIÓN POR CATEGORÍA");
        sb.AppendLine("Categoría,Productos,Valor Total,Porcentaje");
        foreach (var cat in report.ValueByCategory)
        {
            sb.AppendLine(EsMx, $"\"{cat.CategoryName}\",{cat.ProductCount},{cat.TotalValue:F2},{cat.Percentage:F2}%");
        }
        sb.AppendLine();

        // Product detail
        sb.AppendLine("DETALLE INVENTARIO");
        sb.AppendLine("Producto,SKU,Categoría,Cantidad,Costo Promedio,Valor Total");
        foreach (var item in report.ProductValuations)
        {
            sb.AppendLine(EsMx, $"\"{item.ProductName}\",{item.ProductSku},\"{item.CategoryName ?? "Sin categoría"}\",{item.Quantity:F4},{item.AverageCost:F4},{item.ExtendedValue:F2}");
        }
        sb.AppendLine();

        // Low stock alerts
        if (report.LowStockAlerts.Count > 0)
        {
            sb.AppendLine("ALERTAS DE STOCK MÍNIMO");
            sb.AppendLine("Producto,SKU,Cantidad,Mínimo Requerido,Costo Promedio");
            foreach (var alert in report.LowStockAlerts)
            {
                sb.AppendLine(EsMx, $"\"{alert.ProductName}\",{alert.ProductSku},{alert.Quantity:F4},{alert.MinimumLevel:F4},{alert.AverageCost:F4}");
            }
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}
