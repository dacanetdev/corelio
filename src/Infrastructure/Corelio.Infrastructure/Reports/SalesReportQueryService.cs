using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Sales.Common;
using Corelio.Domain.Enums;
using Corelio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Reports;

/// <summary>
/// EF Core implementation of the sales report aggregation service.
/// Only counts Sales with Status = Completed.
/// </summary>
internal sealed class SalesReportQueryService(ApplicationDbContext dbContext)
    : ISalesReportQueryService
{
    public async Task<SalesReportDto> GetSalesReportAsync(
        DateTime dateFrom,
        DateTime dateTo,
        Guid? warehouseId = null,
        PaymentMethod? paymentMethod = null,
        CancellationToken cancellationToken = default)
    {
        // Normalize date range to cover full days
        var from = dateFrom.Date;
        var to = dateTo.Date.AddDays(1).AddTicks(-1);

        var salesQuery = dbContext.Sales
            .AsNoTracking()
            .Where(s => s.Status == SaleStatus.Completed
                && s.CompletedAt >= from
                && s.CompletedAt <= to);

        if (warehouseId.HasValue)
        {
            salesQuery = salesQuery.Where(s => s.WarehouseId == warehouseId.Value);
        }

        // --- KPIs ---
        var kpis = await salesQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalAmount = g.Sum(s => s.Total),
                TransactionCount = g.Count(),
                AverageTicket = g.Average(s => s.Total)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var totalAmount = kpis?.TotalAmount ?? 0m;
        var transactionCount = kpis?.TransactionCount ?? 0;
        var averageTicket = kpis?.AverageTicket ?? 0m;

        // --- Payment method breakdown ---
        var saleIds = salesQuery.Select(s => s.Id);

        var paymentQuery = dbContext.Payments
            .AsNoTracking()
            .Where(p => saleIds.Contains(p.SaleId));

        if (paymentMethod.HasValue)
        {
            paymentQuery = paymentQuery.Where(p => p.Method == paymentMethod.Value);
        }

        var paymentGroups = await paymentQuery
            .GroupBy(p => p.Method)
            .Select(g => new
            {
                Method = g.Key,
                Amount = g.Sum(p => p.Amount),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var totalPaymentAmount = paymentGroups.Sum(p => p.Amount);
        var paymentBreakdown = paymentGroups.Select(p => new PaymentMethodBreakdownDto(
            p.Method,
            GetPaymentMethodName(p.Method),
            p.Amount,
            p.Count,
            totalPaymentAmount > 0 ? Math.Round(p.Amount / totalPaymentAmount * 100, 2) : 0m
        )).OrderByDescending(p => p.Amount).ToList();

        // --- Top 10 products ---
        var topProducts = await dbContext.SaleItems
            .AsNoTracking()
            .Where(si => saleIds.Contains(si.SaleId))
            .GroupBy(si => new { si.ProductId, si.ProductName, si.ProductSku })
            .Select(g => new TopProductDto(
                g.Key.ProductId,
                g.Key.ProductName,
                g.Key.ProductSku,
                g.Sum(si => si.Quantity),
                g.Sum(si => si.LineTotal)))
            .OrderByDescending(p => p.QuantitySold)
            .Take(10)
            .ToListAsync(cancellationToken);

        // --- Hourly distribution (0–23, fill missing with zeros) ---
        var hourlyRaw = await salesQuery
            .GroupBy(s => s.CompletedAt!.Value.Hour)
            .Select(g => new
            {
                Hour = g.Key,
                Amount = g.Sum(s => s.Total),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var hourlyLookup = hourlyRaw.ToDictionary(h => h.Hour);
        var hourlyDistribution = Enumerable.Range(0, 24)
            .Select(h => hourlyLookup.TryGetValue(h, out var data)
                ? new HourlyDistributionDto(h, data.Amount, data.Count)
                : new HourlyDistributionDto(h, 0m, 0))
            .ToList();

        // --- Cashier breakdown ---
        var cashierBreakdown = await salesQuery
            .GroupBy(s => s.CreatedBy.HasValue ? s.CreatedBy.Value.ToString() : "Desconocido")
            .Select(g => new CashierBreakdownDto(
                g.Key,
                g.Sum(s => s.Total),
                g.Count()))
            .OrderByDescending(c => c.Amount)
            .ToListAsync(cancellationToken);

        return new SalesReportDto(
            totalAmount,
            transactionCount,
            averageTicket,
            from,
            to,
            paymentBreakdown,
            topProducts,
            hourlyDistribution,
            cashierBreakdown);
    }

    private static string GetPaymentMethodName(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Efectivo",
        PaymentMethod.Card => "Tarjeta",
        PaymentMethod.Transfer => "Transferencia",
        PaymentMethod.Check => "Cheque",
        PaymentMethod.Credit => "Crédito",
        PaymentMethod.Mixed => "Mixto",
        PaymentMethod.PayPal => "PayPal",
        PaymentMethod.Stripe => "Stripe",
        _ => method.ToString()
    };
}
