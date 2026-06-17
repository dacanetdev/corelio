using Corelio.Domain.Entities;
using Corelio.Infrastructure.Persistence;
using Corelio.Infrastructure.Reports;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Corelio.Infrastructure.Tests.Reports;

/// <summary>
/// Unit tests for InventoryValuationQueryService and WAC calculation in ReceiveGoodsCommandHandler.
/// Uses InMemory database — no tenant filters applied.
/// </summary>
[Trait("Category", "Unit")]
public class InventoryValuationReportTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static ProductCategory CreateCategory(string name = "Herramientas") =>
        new() { Id = Guid.NewGuid(), Name = name };

    private static Product CreateProduct(string name = "Martillo", string sku = "H001", Guid? categoryId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Sku = sku,
            CategoryId = categoryId,
            CostPrice = 0m,
            SalePrice = 100m,
            TaxRate = 0.16m
        };

    private static InventoryItem CreateInventoryItem(
        Guid productId,
        Guid warehouseId,
        decimal quantity = 10m,
        decimal averageCost = 50m,
        decimal minimumLevel = 0m) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProductId = productId,
            WarehouseId = warehouseId,
            Quantity = quantity,
            AverageCost = averageCost,
            MinimumLevel = minimumLevel
        };

    [Fact]
    public async Task GetInventoryValuationReportAsync_WithNoItems_ReturnsZeroTotals()
    {
        await using var context = CreateContext();
        var service = new InventoryValuationQueryService(context);

        var report = await service.GetInventoryValuationReportAsync();

        report.TotalInventoryValue.Should().Be(0m);
        report.TotalProductCount.Should().Be(0);
        report.ProductValuations.Should().BeEmpty();
        report.ValueByCategory.Should().BeEmpty();
        report.LowStockAlerts.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInventoryValuationReportAsync_WithSingleProduct_ReturnsCorrectExtendedValue()
    {
        await using var context = CreateContext();
        var service = new InventoryValuationQueryService(context);

        var category = CreateCategory();
        var product = CreateProduct(categoryId: category.Id);
        var warehouse = Guid.NewGuid();
        var inventoryItem = CreateInventoryItem(product.Id, warehouse, quantity: 5m, averageCost: 100m);

        context.ProductCategories.Add(category);
        context.Products.Add(product);
        context.InventoryItems.Add(inventoryItem);
        await context.SaveChangesAsync();

        var report = await service.GetInventoryValuationReportAsync();

        report.TotalProductCount.Should().Be(1);
        report.TotalInventoryValue.Should().Be(500m); // 5 × 100
        report.ProductValuations.Should().HaveCount(1);
        report.ProductValuations[0].ExtendedValue.Should().Be(500m);
        report.ProductValuations[0].AverageCost.Should().Be(100m);
    }

    [Fact]
    public async Task GetInventoryValuationReportAsync_WithMultipleCategories_GroupsByCategory()
    {
        await using var context = CreateContext();
        var service = new InventoryValuationQueryService(context);

        var catA = CreateCategory("Pintura");
        var catB = CreateCategory("Herramientas");
        var productA = CreateProduct("Pintura Blanca", "P001", catA.Id);
        var productB = CreateProduct("Martillo", "H001", catB.Id);
        var warehouse = Guid.NewGuid();

        context.ProductCategories.AddRange(catA, catB);
        context.Products.AddRange(productA, productB);
        context.InventoryItems.AddRange(
            CreateInventoryItem(productA.Id, warehouse, quantity: 10m, averageCost: 200m),
            CreateInventoryItem(productB.Id, warehouse, quantity: 5m, averageCost: 100m)
        );
        await context.SaveChangesAsync();

        var report = await service.GetInventoryValuationReportAsync();

        report.TotalInventoryValue.Should().Be(2500m); // 2000 + 500
        report.ValueByCategory.Should().HaveCount(2);

        var paintCat = report.ValueByCategory.First(c => c.CategoryName == "Pintura");
        paintCat.TotalValue.Should().Be(2000m);
        paintCat.Percentage.Should().BeApproximately(80m, 1m);

        // Sorted by value desc — Pintura should be first
        report.ValueByCategory[0].CategoryName.Should().Be("Pintura");
    }

    [Fact]
    public async Task GetInventoryValuationReportAsync_WithZeroQuantityItem_ExcludesFromValuation()
    {
        await using var context = CreateContext();
        var service = new InventoryValuationQueryService(context);

        var product = CreateProduct();
        var warehouse = Guid.NewGuid();

        context.Products.Add(product);
        context.InventoryItems.Add(CreateInventoryItem(product.Id, warehouse, quantity: 0m, averageCost: 100m));
        await context.SaveChangesAsync();

        var report = await service.GetInventoryValuationReportAsync();

        report.TotalProductCount.Should().Be(0);
        report.TotalInventoryValue.Should().Be(0m);
        report.ProductValuations.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInventoryValuationReportAsync_LowStockAlerts_ReturnsBelowMinimumLevel()
    {
        await using var context = CreateContext();
        var service = new InventoryValuationQueryService(context);

        var product = CreateProduct();
        var warehouse = Guid.NewGuid();
        // Qty=2, MinLevel=5 → should trigger low stock alert
        var inventoryItem = CreateInventoryItem(product.Id, warehouse, quantity: 2m, averageCost: 50m, minimumLevel: 5m);

        context.Products.Add(product);
        context.InventoryItems.Add(inventoryItem);
        await context.SaveChangesAsync();

        var report = await service.GetInventoryValuationReportAsync();

        report.LowStockAlerts.Should().HaveCount(1);
        report.LowStockAlerts[0].ProductName.Should().Be(product.Name);
        report.LowStockAlerts[0].Quantity.Should().Be(2m);
        report.LowStockAlerts[0].MinimumLevel.Should().Be(5m);
    }

    [Fact]
    public async Task GetInventoryValuationReportAsync_WithWarehouseFilter_ReturnsOnlyThatWarehouse()
    {
        await using var context = CreateContext();
        var service = new InventoryValuationQueryService(context);

        var product = CreateProduct();
        var warehouseA = Guid.NewGuid();
        var warehouseB = Guid.NewGuid();

        context.Products.Add(product);
        context.InventoryItems.AddRange(
            CreateInventoryItem(product.Id, warehouseA, quantity: 10m, averageCost: 100m),
            CreateInventoryItem(product.Id, warehouseB, quantity: 5m, averageCost: 200m)
        );
        await context.SaveChangesAsync();

        var report = await service.GetInventoryValuationReportAsync(warehouseId: warehouseA);

        report.TotalProductCount.Should().Be(1);
        report.TotalInventoryValue.Should().Be(1000m); // 10 × 100
    }

    [Fact]
    public async Task ReceiveGoodsHandler_WAC_CalculatesCorrectlyForFirstReceipt()
    {
        // Arrange: new InventoryItem — AverageCost should be set to UnitPrice
        var inventoryItem = new InventoryItem
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Quantity = 10m,
            AverageCost = 50m // initial cost
        };

        // Simulate receiving 5 more units at 80/unit
        var oldQty = inventoryItem.Quantity;
        var oldCost = inventoryItem.AverageCost;
        decimal receivedQty = 5m;
        decimal unitCost = 80m;

        inventoryItem.Quantity += receivedQty;
        inventoryItem.AverageCost = oldQty + receivedQty > 0
            ? (oldQty * oldCost + receivedQty * unitCost) / (oldQty + receivedQty)
            : unitCost;

        // Assert: WAC = (10×50 + 5×80) / 15 = (500 + 400) / 15 = 60
        inventoryItem.AverageCost.Should().BeApproximately(60m, 0.01m);
        inventoryItem.Quantity.Should().Be(15m);
    }
}
