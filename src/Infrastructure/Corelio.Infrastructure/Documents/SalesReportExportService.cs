using System.Globalization;
using System.Text;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Sales.Common;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// Generates PDF and CSV exports for the sales report.
/// </summary>
internal sealed class SalesReportExportService : ISalesReportExportService
{
    private static readonly CultureInfo EsMx = CultureInfo.GetCultureInfo("es-MX");

    static SalesReportExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GeneratePdfAsync(SalesReportDto report, CancellationToken cancellationToken = default)
    {
        var document = new SalesReportDocument(report);
        return Task.FromResult(document.GeneratePdf());
    }

    public Task<byte[]> GenerateCsvAsync(SalesReportDto report, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        // KPIs
        sb.AppendLine("RESUMEN GENERAL");
        sb.AppendLine("Indicador,Valor");
        sb.AppendLine(EsMx, $"Total Ventas,{report.TotalAmount:F2}");
        sb.AppendLine(EsMx, $"Transacciones,{report.TransactionCount}");
        sb.AppendLine(EsMx, $"Ticket Promedio,{report.AverageTicket:F2}");
        sb.AppendLine(EsMx, $"Período Desde,{report.DateFrom:dd/MM/yyyy}");
        sb.AppendLine(EsMx, $"Período Hasta,{report.DateTo:dd/MM/yyyy}");
        sb.AppendLine();

        // Payment breakdown
        sb.AppendLine("VENTAS POR MÉTODO DE PAGO");
        sb.AppendLine("Método,Monto,Transacciones,Porcentaje");
        foreach (var pm in report.PaymentMethodBreakdown)
        {
            sb.AppendLine(EsMx, $"{pm.MethodName},{pm.Amount:F2},{pm.TransactionCount},{pm.Percentage:F2}%");
        }
        sb.AppendLine();

        // Top 10 products
        sb.AppendLine("TOP 10 PRODUCTOS");
        sb.AppendLine("Posición,Producto,SKU,Cantidad Vendida,Ingresos");
        for (var i = 0; i < report.TopProducts.Count; i++)
        {
            var p = report.TopProducts[i];
            sb.AppendLine(EsMx, $"{i + 1},\"{p.ProductName}\",{p.ProductSku},{p.QuantitySold:F2},{p.Revenue:F2}");
        }
        sb.AppendLine();

        // Hourly distribution
        sb.AppendLine("DISTRIBUCIÓN POR HORA");
        sb.AppendLine("Hora,Monto,Transacciones");
        foreach (var h in report.HourlyDistribution)
        {
            sb.AppendLine(EsMx, $"{h.Hour:00}:00,{h.Amount:F2},{h.TransactionCount}");
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}
