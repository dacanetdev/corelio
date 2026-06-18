using Corelio.Application.Common.Interfaces;
using Corelio.Application.Reports.Purchases.Common;
using Corelio.Domain.Enums;
using Corelio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Reports;

/// <summary>
/// EF Core implementation of the purchase summary report aggregation service.
/// </summary>
internal sealed class PurchaseSummaryQueryService(ApplicationDbContext dbContext)
    : IPurchaseSummaryQueryService
{
    public async Task<PurchaseSummaryReportDto> GetPurchaseSummaryReportAsync(
        DateTime dateFrom,
        DateTime dateTo,
        Guid? supplierId = null,
        PurchaseOrderStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var from = dateFrom.Date;
        var to = dateTo.Date.AddDays(1).AddTicks(-1);

        var ordersQuery = dbContext.PurchaseOrders
            .AsNoTracking()
            .Where(po => po.CreatedAt >= from && po.CreatedAt <= to);

        if (supplierId.HasValue)
        {
            ordersQuery = ordersQuery.Where(po => po.SupplierId == supplierId.Value);
        }

        if (status.HasValue)
        {
            ordersQuery = ordersQuery.Where(po => po.Status == status.Value);
        }

        // --- KPIs ---
        var allOrders = await ordersQuery
            .Select(po => new { po.Id, po.SupplierId, po.Status, po.Total })
            .ToListAsync(cancellationToken);

        var totalOrders = allOrders.Count;

        var spendingStatuses = new[]
        {
            PurchaseOrderStatus.Approved,
            PurchaseOrderStatus.PartiallyReceived,
            PurchaseOrderStatus.Received
        };

        var totalAmountSpent = allOrders
            .Where(po => spendingStatuses.Contains(po.Status))
            .Sum(po => po.Total);

        var pendingDeliveries = allOrders
            .Count(po => po.Status is PurchaseOrderStatus.Approved or PurchaseOrderStatus.PartiallyReceived);

        // --- By Supplier ---
        var supplierIds = allOrders.Select(po => po.SupplierId).Distinct().ToList();

        var supplierNames = await dbContext.Suppliers
            .AsNoTracking()
            .Where(s => supplierIds.Contains(s.Id))
            .Select(s => new { s.Id, s.Name })
            .ToDictionaryAsync(s => s.Id, s => s.Name, cancellationToken);

        var bySupplier = allOrders
            .GroupBy(po => po.SupplierId)
            .Select(g => new SupplierSpendingDto(
                g.Key,
                supplierNames.GetValueOrDefault(g.Key, "Desconocido"),
                g.Count(),
                g.Sum(po => po.Total)))
            .OrderByDescending(s => s.TotalAmount)
            .ToList();

        // --- By Status ---
        var byStatus = allOrders
            .GroupBy(po => po.Status)
            .Select(g => new StatusBreakdownDto(
                GetStatusName(g.Key),
                g.Count(),
                g.Sum(po => po.Total)))
            .ToList();

        // --- Product Receipts ---
        var orderIds = allOrders.Select(po => po.Id).ToList();

        var items = await dbContext.PurchaseOrderItems
            .AsNoTracking()
            .Where(i => orderIds.Contains(i.PurchaseOrderId))
            .Join(
                dbContext.Products.AsNoTracking(),
                i => i.ProductId,
                p => p.Id,
                (i, p) => new
                {
                    i.ProductId,
                    i.ProductName,
                    p.Sku,
                    i.Quantity,
                    i.ReceivedQuantity
                })
            .ToListAsync(cancellationToken);

        var productReceipts = items
            .GroupBy(i => new { i.ProductId, i.ProductName, i.Sku })
            .Select(g =>
            {
                var ordered = g.Sum(i => i.Quantity);
                var received = g.Sum(i => i.ReceivedQuantity);
                return new ProductReceiptDto(
                    g.Key.ProductId,
                    g.Key.ProductName,
                    g.Key.Sku,
                    ordered,
                    received,
                    ordered - received);
            })
            .OrderBy(p => p.ProductName)
            .ToList();

        return new PurchaseSummaryReportDto(
            DateTime.UtcNow,
            from,
            to,
            supplierId,
            totalOrders,
            totalAmountSpent,
            pendingDeliveries,
            bySupplier,
            byStatus,
            productReceipts);
    }

    private static string GetStatusName(PurchaseOrderStatus status) => status switch
    {
        PurchaseOrderStatus.Draft => "Borrador",
        PurchaseOrderStatus.Submitted => "Enviada",
        PurchaseOrderStatus.Approved => "Aprobada",
        PurchaseOrderStatus.PartiallyReceived => "Parcialmente Recibida",
        PurchaseOrderStatus.Received => "Recibida",
        PurchaseOrderStatus.Cancelled => "Cancelada",
        _ => status.ToString()
    };
}
