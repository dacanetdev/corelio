using Corelio.Application.Reports.Purchases.Common;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Infrastructure.Persistence;
using Corelio.Infrastructure.Reports;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Tests.Reports;

[Trait("Category", "Unit")]
public class PurchaseSummaryReportTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static Supplier CreateSupplier(string name = "Proveedor A") =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = name,
            ContactName = "Contacto",
            Email = "test@proveedor.mx",
            Phone = "5551234567",
            Rfc = "PRV000101AAA"
        };

    private static PurchaseOrder CreatePurchaseOrder(
        Guid supplierId,
        decimal total = 1000m,
        PurchaseOrderStatus status = PurchaseOrderStatus.Received,
        DateTime? createdAt = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            OrderNumber = $"PO-2026-{Guid.NewGuid().ToString("N")[..4]}",
            SupplierId = supplierId,
            Status = status,
            Subtotal = total / 1.16m,
            IvaAmount = total - (total / 1.16m),
            Total = total,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };

    private static PurchaseOrderItem CreateItem(
        Guid purchaseOrderId,
        Guid? productId = null,
        string productName = "Producto",
        decimal quantity = 10m,
        decimal receivedQty = 0m,
        decimal unitPrice = 100m) =>
        new()
        {
            Id = Guid.NewGuid(),
            PurchaseOrderId = purchaseOrderId,
            ProductId = productId ?? Guid.NewGuid(),
            ProductName = productName,
            Quantity = quantity,
            ReceivedQuantity = receivedQty,
            UnitPrice = unitPrice,
            Subtotal = quantity * unitPrice
        };

    private static Product CreateProduct(string sku = "P001") =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Producto Test",
            Sku = sku,
            CostPrice = 50m,
            SalePrice = 100m,
            TaxRate = 0.16m
        };

    [Fact]
    public async Task GetPurchaseSummaryReportAsync_WithNoOrders_ReturnsZeroTotals()
    {
        await using var context = CreateContext();
        var service = new PurchaseSummaryQueryService(context);

        var from = DateTime.UtcNow.AddDays(-30);
        var to = DateTime.UtcNow;
        var report = await service.GetPurchaseSummaryReportAsync(from, to);

        report.TotalOrders.Should().Be(0);
        report.TotalAmountSpent.Should().Be(0m);
        report.PendingDeliveries.Should().Be(0);
        report.BySupplier.Should().BeEmpty();
        report.ByStatus.Should().BeEmpty();
        report.ProductReceipts.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPurchaseSummaryReportAsync_OnlyCountsSpendingForValidStatuses()
    {
        await using var context = CreateContext();
        var service = new PurchaseSummaryQueryService(context);

        var supplier = CreateSupplier();
        context.Suppliers.Add(supplier);

        var received = CreatePurchaseOrder(supplier.Id, 1000m, PurchaseOrderStatus.Received);
        var approved = CreatePurchaseOrder(supplier.Id, 500m, PurchaseOrderStatus.Approved);
        var draft = CreatePurchaseOrder(supplier.Id, 200m, PurchaseOrderStatus.Draft);
        var cancelled = CreatePurchaseOrder(supplier.Id, 300m, PurchaseOrderStatus.Cancelled);

        context.PurchaseOrders.AddRange(received, approved, draft, cancelled);
        await context.SaveChangesAsync();

        var report = await service.GetPurchaseSummaryReportAsync(
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

        report.TotalOrders.Should().Be(4);
        report.TotalAmountSpent.Should().Be(1500m); // only Received + Approved
        report.PendingDeliveries.Should().Be(1); // only Approved
    }

    [Fact]
    public async Task GetPurchaseSummaryReportAsync_CountsPendingAsApprovedOrPartiallyReceived()
    {
        await using var context = CreateContext();
        var service = new PurchaseSummaryQueryService(context);

        var supplier = CreateSupplier();
        context.Suppliers.Add(supplier);

        var approved = CreatePurchaseOrder(supplier.Id, status: PurchaseOrderStatus.Approved);
        var partial = CreatePurchaseOrder(supplier.Id, status: PurchaseOrderStatus.PartiallyReceived);
        var received = CreatePurchaseOrder(supplier.Id, status: PurchaseOrderStatus.Received);

        context.PurchaseOrders.AddRange(approved, partial, received);
        await context.SaveChangesAsync();

        var report = await service.GetPurchaseSummaryReportAsync(
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

        report.PendingDeliveries.Should().Be(2); // Approved + PartiallyReceived
    }

    [Fact]
    public async Task GetPurchaseSummaryReportAsync_GroupsBySupplierSortedByTotalDesc()
    {
        await using var context = CreateContext();
        var service = new PurchaseSummaryQueryService(context);

        var supplierA = CreateSupplier("Acero SA");
        var supplierB = CreateSupplier("Pintura MX");
        context.Suppliers.AddRange(supplierA, supplierB);

        // A: 2 orders totaling 3000
        context.PurchaseOrders.AddRange(
            CreatePurchaseOrder(supplierA.Id, 2000m, PurchaseOrderStatus.Received),
            CreatePurchaseOrder(supplierA.Id, 1000m, PurchaseOrderStatus.Received),
            CreatePurchaseOrder(supplierB.Id, 500m, PurchaseOrderStatus.Received)
        );
        await context.SaveChangesAsync();

        var report = await service.GetPurchaseSummaryReportAsync(
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

        report.BySupplier.Should().HaveCount(2);
        report.BySupplier[0].SupplierName.Should().Be("Acero SA");
        report.BySupplier[0].OrderCount.Should().Be(2);
        report.BySupplier[0].TotalAmount.Should().Be(3000m);
        report.BySupplier[1].SupplierName.Should().Be("Pintura MX");
    }

    [Fact]
    public async Task GetPurchaseSummaryReportAsync_WithSupplierFilter_ReturnsOnlyThatSupplier()
    {
        await using var context = CreateContext();
        var service = new PurchaseSummaryQueryService(context);

        var supplierA = CreateSupplier("Acero SA");
        var supplierB = CreateSupplier("Pintura MX");
        context.Suppliers.AddRange(supplierA, supplierB);

        context.PurchaseOrders.AddRange(
            CreatePurchaseOrder(supplierA.Id, 1000m, PurchaseOrderStatus.Received),
            CreatePurchaseOrder(supplierB.Id, 500m, PurchaseOrderStatus.Received)
        );
        await context.SaveChangesAsync();

        var report = await service.GetPurchaseSummaryReportAsync(
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1),
            supplierId: supplierA.Id);

        report.TotalOrders.Should().Be(1);
        report.BySupplier.Should().HaveCount(1);
        report.BySupplier[0].SupplierName.Should().Be("Acero SA");
    }

    [Fact]
    public async Task GetPurchaseSummaryReportAsync_StatusBreakdown_UsesSpanishNames()
    {
        await using var context = CreateContext();
        var service = new PurchaseSummaryQueryService(context);

        var supplier = CreateSupplier();
        context.Suppliers.Add(supplier);

        context.PurchaseOrders.AddRange(
            CreatePurchaseOrder(supplier.Id, 1000m, PurchaseOrderStatus.Received),
            CreatePurchaseOrder(supplier.Id, 200m, PurchaseOrderStatus.Draft)
        );
        await context.SaveChangesAsync();

        var report = await service.GetPurchaseSummaryReportAsync(
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

        var statusNames = report.ByStatus.Select(s => s.StatusName).ToList();
        statusNames.Should().Contain("Recibida");
        statusNames.Should().Contain("Borrador");
    }
}
