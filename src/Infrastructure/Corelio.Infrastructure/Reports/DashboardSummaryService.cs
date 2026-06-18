using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Dashboard.Common;
using Corelio.Domain.Enums;
using Corelio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Reports;

/// <summary>
/// EF Core implementation of the dashboard KPI summary service.
/// All queries are read-only (AsNoTracking) and use CountAsync to avoid loading entities.
/// </summary>
internal sealed class DashboardSummaryService(ApplicationDbContext dbContext)
    : IDashboardSummaryService
{
    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // Today's completed sales — aggregate in a single grouped query
        var todaysKpis = await dbContext.Sales
            .AsNoTracking()
            .Where(s => s.Status == SaleStatus.Completed
                && s.CreatedAt >= today
                && s.CreatedAt < tomorrow)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Sum(s => s.Total),
                Count = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        var todaysSalesTotal = todaysKpis?.Total ?? 0m;
        var todaysTransactionCount = todaysKpis?.Count ?? 0;

        // Low-stock products: distinct products where MinimumLevel is set and stock is at or below it
        var lowStockProductCount = await dbContext.InventoryItems
            .AsNoTracking()
            .Where(i => i.MinimumLevel > 0 && i.Quantity <= i.MinimumLevel)
            .Select(i => i.ProductId)
            .Distinct()
            .CountAsync(cancellationToken);

        // Pending purchase orders: Approved or PartiallyReceived
        var pendingPurchaseOrderCount = await dbContext.PurchaseOrders
            .AsNoTracking()
            .CountAsync(
                po => po.Status == PurchaseOrderStatus.Approved
                    || po.Status == PurchaseOrderStatus.PartiallyReceived,
                cancellationToken);

        return new DashboardSummaryDto(
            todaysSalesTotal,
            todaysTransactionCount,
            lowStockProductCount,
            pendingPurchaseOrderCount,
            DateTime.UtcNow);
    }
}
