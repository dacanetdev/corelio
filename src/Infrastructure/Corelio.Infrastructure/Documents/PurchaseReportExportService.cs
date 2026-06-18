using System.Globalization;
using System.Text;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Purchases.Common;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// Generates PDF and CSV exports for the purchase summary report.
/// </summary>
internal sealed class PurchaseReportExportService : IPurchaseReportExportService
{
    private static readonly CultureInfo EsMx = CultureInfo.GetCultureInfo("es-MX");

    static PurchaseReportExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GeneratePdfAsync(PurchaseSummaryReportDto report, CancellationToken cancellationToken = default)
    {
        var document = new PurchaseReportDocument(report);
        return Task.FromResult(document.GeneratePdf());
    }

    public Task<byte[]> GenerateCsvAsync(PurchaseSummaryReportDto report, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        // Resumen
        sb.AppendLine("RESUMEN");
        sb.AppendLine("Indicador,Valor");
        sb.AppendLine(EsMx, $"Total Órdenes,{report.TotalOrders}");
        sb.AppendLine(EsMx, $"Total Gastado,{report.TotalAmountSpent:F2}");
        sb.AppendLine(EsMx, $"Entregas Pendientes,{report.PendingDeliveries}");
        sb.AppendLine(EsMx, $"Período Desde,{report.DateFrom:dd/MM/yyyy}");
        sb.AppendLine(EsMx, $"Período Hasta,{report.DateTo:dd/MM/yyyy}");
        sb.AppendLine();

        // Gasto por Proveedor
        sb.AppendLine("GASTO POR PROVEEDOR");
        sb.AppendLine("Proveedor,Órdenes,Total");
        foreach (var s in report.BySupplier)
        {
            sb.AppendLine(EsMx, $"\"{s.SupplierName}\",{s.OrderCount},{s.TotalAmount:F2}");
        }
        sb.AppendLine();

        // Desglose por Estatus
        sb.AppendLine("DESGLOSE POR ESTATUS");
        sb.AppendLine("Estatus,Órdenes,Total");
        foreach (var s in report.ByStatus)
        {
            sb.AppendLine(EsMx, $"\"{s.StatusName}\",{s.OrderCount},{s.TotalAmount:F2}");
        }
        sb.AppendLine();

        // Productos Recibido vs Pendiente
        sb.AppendLine("PRODUCTOS RECIBIDO VS PENDIENTE");
        sb.AppendLine("Producto,SKU,Ordenado,Recibido,Pendiente");
        foreach (var p in report.ProductReceipts)
        {
            sb.AppendLine(EsMx, $"\"{p.ProductName}\",{p.ProductSku},{p.OrderedQuantity:F2},{p.ReceivedQuantity:F2},{p.PendingQuantity:F2}");
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}
