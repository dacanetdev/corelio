using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Infrastructure.Persistence;
using Corelio.Infrastructure.Reports;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Tests.Reports;

/// <summary>
/// Unit tests for SalesReportQueryService aggregation logic.
/// Uses InMemory database — no tenant filters applied (tenantProvider = null).
/// </summary>
[Trait("Category", "Unit")]
public class SalesReportQueryServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static Sale CreateCompletedSale(
        Guid? warehouseId = null,
        decimal total = 100m,
        DateTime? completedAt = null,
        Guid? createdBy = null)
    {
        return new Sale
        {
            WarehouseId = warehouseId ?? Guid.NewGuid(),
            Status = SaleStatus.Completed,
            CompletedAt = completedAt ?? DateTime.UtcNow,
            Total = total,
            SubTotal = total,
            Folio = "V-TEST",
            CreatedBy = createdBy
        };
    }

    [Fact]
    public async Task GetSalesReportAsync_WhenNoSalesInDateRange_ReturnsZeroKpisAnd24HourlySlots()
    {
        // Arrange
        await using var context = CreateContext();
        var service = new SalesReportQueryService(context);

        var oldSale = CreateCompletedSale(completedAt: DateTime.UtcNow.AddDays(-100));
        context.Sales.Add(oldSale);
        await context.SaveChangesAsync();

        var dateFrom = DateTime.UtcNow.AddDays(-10);
        var dateTo = DateTime.UtcNow;

        // Act
        var report = await service.GetSalesReportAsync(dateFrom, dateTo);

        // Assert
        report.TotalAmount.Should().Be(0m);
        report.TransactionCount.Should().Be(0);
        report.AverageTicket.Should().Be(0m);
        report.PaymentMethodBreakdown.Should().BeEmpty();
        report.TopProducts.Should().BeEmpty();
        report.HourlyDistribution.Should().HaveCount(24);
        report.CashierBreakdown.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSalesReportAsync_WithSingleCompletedSale_ReturnsCorrectKpis()
    {
        // Arrange
        await using var context = CreateContext();
        var service = new SalesReportQueryService(context);

        const decimal total = 500m;
        var completedAt = DateTime.UtcNow;
        var sale = CreateCompletedSale(total: total, completedAt: completedAt);
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        // Act
        var report = await service.GetSalesReportAsync(
            completedAt.AddDays(-1), completedAt.AddDays(1));

        // Assert
        report.TotalAmount.Should().Be(total);
        report.TransactionCount.Should().Be(1);
        report.AverageTicket.Should().Be(total);
        report.DateFrom.Should().BeBefore(completedAt);
        report.DateTo.Should().BeAfter(completedAt);
    }

    [Fact]
    public async Task GetSalesReportAsync_ExcludesNonCompletedSales()
    {
        // Arrange
        await using var context = CreateContext();
        var service = new SalesReportQueryService(context);

        var now = DateTime.UtcNow;
        var completedSale = CreateCompletedSale(total: 200m, completedAt: now);

        var draftSale = new Sale
        {
            WarehouseId = Guid.NewGuid(),
            Status = SaleStatus.Draft,
            CompletedAt = now,
            Total = 999m,
            SubTotal = 999m,
            Folio = "V-DRAFT"
        };
        var cancelledSale = new Sale
        {
            WarehouseId = Guid.NewGuid(),
            Status = SaleStatus.Cancelled,
            CompletedAt = now,
            Total = 999m,
            SubTotal = 999m,
            Folio = "V-CANCEL"
        };

        context.Sales.AddRange(completedSale, draftSale, cancelledSale);
        await context.SaveChangesAsync();

        // Act
        var report = await service.GetSalesReportAsync(now.AddDays(-1), now.AddDays(1));

        // Assert
        report.TransactionCount.Should().Be(1);
        report.TotalAmount.Should().Be(200m);
    }

    [Fact]
    public async Task GetSalesReportAsync_WithMultiplePaymentMethods_ReturnsCorrectBreakdownOrderedByAmount()
    {
        // Arrange
        await using var context = CreateContext();
        var service = new SalesReportQueryService(context);

        var now = DateTime.UtcNow;
        var sale = CreateCompletedSale(total: 300m, completedAt: now);
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        context.Payments.AddRange(
            new Payment { SaleId = sale.Id, Method = PaymentMethod.Cash, Amount = 200m, Status = PaymentStatus.Paid },
            new Payment { SaleId = sale.Id, Method = PaymentMethod.Card, Amount = 100m, Status = PaymentStatus.Paid }
        );
        await context.SaveChangesAsync();

        // Act
        var report = await service.GetSalesReportAsync(now.AddDays(-1), now.AddDays(1));

        // Assert
        report.PaymentMethodBreakdown.Should().HaveCount(2);

        var cash = report.PaymentMethodBreakdown.First(p => p.Method == PaymentMethod.Cash);
        cash.Amount.Should().Be(200m);
        cash.MethodName.Should().Be("Efectivo");
        cash.Percentage.Should().BeApproximately(66.67m, 1m);

        var card = report.PaymentMethodBreakdown.First(p => p.Method == PaymentMethod.Card);
        card.Amount.Should().Be(100m);
        card.MethodName.Should().Be("Tarjeta");

        // Ordered by amount descending
        report.PaymentMethodBreakdown[0].Method.Should().Be(PaymentMethod.Cash);
    }

    [Fact]
    public async Task GetSalesReportAsync_WithMultipleProducts_ReturnsTopProductsSortedByQuantityDesc()
    {
        // Arrange
        await using var context = CreateContext();
        var service = new SalesReportQueryService(context);

        var now = DateTime.UtcNow;
        var sale = CreateCompletedSale(total: 600m, completedAt: now);
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        context.SaleItems.AddRange(
            new SaleItem { SaleId = sale.Id, ProductId = Guid.NewGuid(), ProductName = "Pintura", ProductSku = "P001", Quantity = 10m, LineTotal = 300m, UnitPrice = 30m },
            new SaleItem { SaleId = sale.Id, ProductId = Guid.NewGuid(), ProductName = "Brocha", ProductSku = "P002", Quantity = 5m, LineTotal = 100m, UnitPrice = 20m },
            new SaleItem { SaleId = sale.Id, ProductId = Guid.NewGuid(), ProductName = "Rodillo", ProductSku = "P003", Quantity = 3m, LineTotal = 200m, UnitPrice = 66.67m }
        );
        await context.SaveChangesAsync();

        // Act
        var report = await service.GetSalesReportAsync(now.AddDays(-1), now.AddDays(1));

        // Assert
        report.TopProducts.Should().HaveCount(3);
        report.TopProducts[0].ProductName.Should().Be("Pintura");
        report.TopProducts[0].QuantitySold.Should().Be(10m);
        report.TopProducts[1].ProductName.Should().Be("Brocha");
        report.TopProducts[2].ProductName.Should().Be("Rodillo");
    }

    [Fact]
    public async Task GetSalesReportAsync_ReturnsHourlyDistributionWith24SlotsAndCorrectAmounts()
    {
        // Arrange
        await using var context = CreateContext();
        var service = new SalesReportQueryService(context);

        var today = DateTime.UtcNow.Date;
        var saleAt10Am = CreateCompletedSale(total: 150m, completedAt: today.AddHours(10));
        context.Sales.Add(saleAt10Am);
        await context.SaveChangesAsync();

        // Act
        var report = await service.GetSalesReportAsync(today, today.AddDays(1));

        // Assert
        report.HourlyDistribution.Should().HaveCount(24);
        report.HourlyDistribution.Select(h => h.Hour).Should().BeEquivalentTo(Enumerable.Range(0, 24));

        var hour10 = report.HourlyDistribution.First(h => h.Hour == 10);
        hour10.Amount.Should().Be(150m);
        hour10.TransactionCount.Should().Be(1);

        var hour9 = report.HourlyDistribution.First(h => h.Hour == 9);
        hour9.Amount.Should().Be(0m);
        hour9.TransactionCount.Should().Be(0);
    }

    [Fact]
    public async Task GetSalesReportAsync_WithWarehouseIdFilter_ReturnsOnlySalesForThatWarehouse()
    {
        // Arrange
        await using var context = CreateContext();
        var service = new SalesReportQueryService(context);

        var warehouseA = Guid.NewGuid();
        var warehouseB = Guid.NewGuid();
        var now = DateTime.UtcNow;

        context.Sales.AddRange(
            CreateCompletedSale(warehouseId: warehouseA, total: 300m, completedAt: now),
            CreateCompletedSale(warehouseId: warehouseA, total: 100m, completedAt: now),
            CreateCompletedSale(warehouseId: warehouseB, total: 999m, completedAt: now)
        );
        await context.SaveChangesAsync();

        // Act
        var report = await service.GetSalesReportAsync(now.AddDays(-1), now.AddDays(1), warehouseId: warehouseA);

        // Assert
        report.TransactionCount.Should().Be(2);
        report.TotalAmount.Should().Be(400m);
    }
}
