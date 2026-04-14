using Corelio.Domain.Common;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Corelio.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeds realistic UAT demo data for "Ferretería Demo S.A. de C.V.":
/// 8 categories, 52 products, 5 customers, inventory stock, and 30 days of historical sales.
/// Invoke by passing --seed-uat to the WebAPI process.
/// Safe to re-run — idempotent via sentinel category check.
/// </summary>
public class UatDataSeeder(ApplicationDbContext dbContext, ILogger<UatDataSeeder> logger)
{
    // ── Fixed tenant / infrastructure IDs ───────────────────────────────────
    private static readonly Guid TenantId = Guid.Parse("b0000000-0000-0000-0000-000000000001");
    private static readonly Guid WarehouseId = Guid.Parse("e0000000-0000-0000-0000-000000000001");

    // Sentinel: if this category row exists, seeding has already run
    private static readonly Guid SentinelCategoryId = Guid.Parse("f0000000-0000-0000-0000-000000000001");

    // Pricing configuration
    private static readonly Guid PricingConfigId = Guid.Parse("f1000000-0000-0000-0000-000000000001");

    // Category IDs
    private static readonly Guid CatHerramientasManuales = Guid.Parse("f0000000-0000-0000-0000-000000000001");
    private static readonly Guid CatHerramientasElectricas = Guid.Parse("f0000000-0000-0000-0000-000000000002");
    private static readonly Guid CatPlomeria = Guid.Parse("f0000000-0000-0000-0000-000000000003");
    private static readonly Guid CatMaterialElectrico = Guid.Parse("f0000000-0000-0000-0000-000000000004");
    private static readonly Guid CatPinturas = Guid.Parse("f0000000-0000-0000-0000-000000000005");
    private static readonly Guid CatConstruccion = Guid.Parse("f0000000-0000-0000-0000-000000000006");
    private static readonly Guid CatFerreteria = Guid.Parse("f0000000-0000-0000-0000-000000000007");
    private static readonly Guid CatSeguridad = Guid.Parse("f0000000-0000-0000-0000-000000000008");

    // Customer IDs
    private static readonly Guid CustJuan = Guid.Parse("f3000000-0000-0000-0000-000000000001");
    private static readonly Guid CustConstrucciones = Guid.Parse("f3000000-0000-0000-0000-000000000002");
    private static readonly Guid CustMaria = Guid.Parse("f3000000-0000-0000-0000-000000000003");
    private static readonly Guid CustFerretera = Guid.Parse("f3000000-0000-0000-0000-000000000004");
    private static readonly Guid CustCarlos = Guid.Parse("f3000000-0000-0000-0000-000000000005");

    // ── Entry point ─────────────────────────────────────────────────────────

    /// <summary>
    /// Seeds all UAT demo data. Idempotent — safe to call multiple times.
    /// </summary>
    public async Task SeedAsync()
    {
        logger.LogInformation("UAT seeder: checking sentinel...");

        var alreadySeeded = await dbContext.Set<ProductCategory>()
            .IgnoreQueryFilters()
            .AnyAsync(c => c.Id == SentinelCategoryId);

        if (alreadySeeded)
        {
            logger.LogInformation("UAT seeder: data already present, skipping.");
            return;
        }

        logger.LogInformation("UAT seeder: seeding demo data...");

        await SeedPricingConfigAsync();
        await SeedCategoriesAsync();
        var productIds = await SeedProductsAsync();
        await SeedInventoryAsync(productIds);
        await SeedCustomersAsync();
        var saleIds = await SeedSalesAsync(productIds);
        await FixSaleDatesAsync(saleIds);

        logger.LogInformation("UAT seeder: complete — {Count} sales created.", saleIds.Count);
    }

    // ── Pricing configuration ────────────────────────────────────────────────

    private async Task SeedPricingConfigAsync()
    {
        var exists = await dbContext.Set<TenantPricingConfiguration>()
            .IgnoreQueryFilters()
            .AnyAsync(p => p.Id == PricingConfigId);

        if (exists)
        {
            return;
        }

        var config = new TenantPricingConfiguration
        {
            Id = PricingConfigId,
            TenantId = TenantId,
            DiscountTierCount = 3,
            MarginTierCount = 3,
            DefaultIvaEnabled = true,
            IvaPercentage = 16.00m
        };

        dbContext.Set<TenantPricingConfiguration>().Add(config);

        dbContext.Set<DiscountTierDefinition>().AddRange(
            Tier<DiscountTierDefinition>(Guid.Parse("f1100000-0000-0000-0000-000000000001"), PricingConfigId, 1, "Descuento 1"),
            Tier<DiscountTierDefinition>(Guid.Parse("f1100000-0000-0000-0000-000000000002"), PricingConfigId, 2, "Descuento 2"),
            Tier<DiscountTierDefinition>(Guid.Parse("f1100000-0000-0000-0000-000000000003"), PricingConfigId, 3, "Descuento 3")
        );

        dbContext.Set<MarginTierDefinition>().AddRange(
            Tier<MarginTierDefinition>(Guid.Parse("f1200000-0000-0000-0000-000000000001"), PricingConfigId, 1, "Menudeo"),
            Tier<MarginTierDefinition>(Guid.Parse("f1200000-0000-0000-0000-000000000002"), PricingConfigId, 2, "Mayoreo"),
            Tier<MarginTierDefinition>(Guid.Parse("f1200000-0000-0000-0000-000000000003"), PricingConfigId, 3, "Distribuidor")
        );

        await dbContext.SaveChangesAsync();
    }

    private static T Tier<T>(Guid id, Guid configId, int tierNumber, string tierName) where T : TenantAuditableEntity, new()
    {
        var tier = new T { Id = id, TenantId = TenantId };

        // Set common fields via reflection-free property assignment
        if (tier is DiscountTierDefinition d)
        {
            d.TenantPricingConfigurationId = configId;
            d.TierNumber = tierNumber;
            d.TierName = tierName;
            d.IsActive = true;
        }
        else if (tier is MarginTierDefinition m)
        {
            m.TenantPricingConfigurationId = configId;
            m.TierNumber = tierNumber;
            m.TierName = tierName;
            m.IsActive = true;
        }

        return tier;
    }

    // ── Categories ───────────────────────────────────────────────────────────

    private async Task SeedCategoriesAsync()
    {
        var categories = new[]
        {
            Cat(CatHerramientasManuales, "Herramientas Manuales", "#E65100", "hardware-store", 1),
            Cat(CatHerramientasElectricas, "Herramientas Eléctricas", "#1565C0", "power-plug", 2),
            Cat(CatPlomeria, "Plomería", "#00695C", "water-pump", 3),
            Cat(CatMaterialElectrico, "Material Eléctrico", "#F9A825", "lightning-bolt", 4),
            Cat(CatPinturas, "Pinturas y Acabados", "#6A1B9A", "palette", 5),
            Cat(CatConstruccion, "Construcción", "#37474F", "domain", 6),
            Cat(CatFerreteria, "Ferretería General", "#4E342E", "cog", 7),
            Cat(CatSeguridad, "Seguridad Industrial", "#C62828", "shield", 8),
        };

        dbContext.Set<ProductCategory>().AddRange(categories);
        await dbContext.SaveChangesAsync();
    }

    private static ProductCategory Cat(Guid id, string name, string color, string icon, int sort) =>
        new()
        {
            Id = id,
            TenantId = TenantId,
            Name = name,
            Level = 0,
            Path = $"/{name.ToLowerInvariant().Replace(' ', '-').Replace('é', 'e').Replace('ó', 'o').Replace('í', 'i')}/",
            SortOrder = sort,
            ColorHex = color,
            IconName = icon,
            IsActive = true
        };

    // ── Products ─────────────────────────────────────────────────────────────

    private sealed record ProductSpec(Guid Id, string Sku, string Name, Guid CategoryId,
        decimal Cost, decimal Sale, int StockQty,
        string SatCode = "27111700", string SatUnit = "H87",
        UnitOfMeasure Uom = UnitOfMeasure.PCS);

    private async Task<List<Guid>> SeedProductsAsync()
    {
        var specs = GetProductSpecs();
        var products = new List<Product>();
        var discounts = new List<ProductDiscount>();
        var marginPrices = new List<ProductMarginPrice>();

        foreach (var (spec, index) in specs.Select((s, i) => (s, i)))
        {
            var product = new Product
            {
                Id = spec.Id,
                TenantId = TenantId,
                Sku = spec.Sku,
                Name = spec.Name,
                CategoryId = spec.CategoryId,
                CostPrice = spec.Cost,
                SalePrice = spec.Sale,
                WholesalePrice = Math.Round(spec.Sale * 0.88m, 2),
                NetCost = spec.Cost,
                TaxRate = 0.16m,
                IvaEnabled = true,
                TrackInventory = true,
                UnitOfMeasure = spec.Uom,
                SatProductCode = spec.SatCode,
                SatUnitCode = spec.SatUnit,
                MinStockLevel = 10,
                MaxStockLevel = 500,
                ReorderPoint = 20,
                ReorderQuantity = 100,
                IsActive = true
            };

            products.Add(product);

            // Discount tiers: 5%, 8%, 12%
            decimal[] discountPcts = [5m, 8m, 12m];
            for (var tier = 1; tier <= 3; tier++)
            {
                discounts.Add(new ProductDiscount
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantId,
                    ProductId = spec.Id,
                    TierNumber = tier,
                    DiscountPercentage = discountPcts[tier - 1]
                });
            }

            // Margin tiers: Menudeo (30%), Mayoreo (40%), Distribuidor (50%)
            decimal[] margins = [30m, 40m, 50m];
            for (var tier = 1; tier <= 3; tier++)
            {
                var margin = margins[tier - 1];
                var salePrice = Math.Round(spec.Cost / (1m - margin / 100m), 2);
                var priceWithIva = Math.Round(salePrice * 1.16m, 2);
                marginPrices.Add(new ProductMarginPrice
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantId,
                    ProductId = spec.Id,
                    TierNumber = tier,
                    MarginPercentage = margin,
                    SalePrice = salePrice,
                    PriceWithIva = priceWithIva
                });
            }
        }

        dbContext.Set<Product>().AddRange(products);
        dbContext.Set<ProductDiscount>().AddRange(discounts);
        dbContext.Set<ProductMarginPrice>().AddRange(marginPrices);
        await dbContext.SaveChangesAsync();

        return specs.Select(s => s.Id).ToList();
    }

    private static List<ProductSpec> GetProductSpecs() =>
    [
        // ── Herramientas Manuales ───────────────────────────────────────────
        new(Guid.Parse("f2000000-0000-0000-0000-000000000001"), "HM-001", "Martillo de uña 16 oz", CatHerramientasManuales, 95m, 185m, 150),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000002"), "HM-002", "Juego de desarmadores 6 pzas", CatHerramientasManuales, 120m, 235m, 120),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000003"), "HM-003", "Llave perica 10\"", CatHerramientasManuales, 75m, 145m, 80),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000004"), "HM-004", "Pinzas de punta 7\"", CatHerramientasManuales, 55m, 110m, 100),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000005"), "HM-005", "Cinta métrica 5m", CatHerramientasManuales, 45m, 89m, 200),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000006"), "HM-006", "Sierra para madera 26\"", CatHerramientasManuales, 180m, 350m, 60),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000007"), "HM-007", "Nivel de burbuja 24\"", CatHerramientasManuales, 95m, 185m, 75),

        // ── Herramientas Eléctricas ─────────────────────────────────────────
        new(Guid.Parse("f2000000-0000-0000-0000-000000000008"), "HE-001", "Taladro percutor 1/2\"", CatHerramientasElectricas, 480m, 950m, 35, "25221720"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000009"), "HE-002", "Lijadora orbital 1/4 hoja", CatHerramientasElectricas, 320m, 620m, 30, "25221720"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000010"), "HE-003", "Esmeriladora angular 4.5\"", CatHerramientasElectricas, 350m, 680m, 40, "25221720"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000011"), "HE-004", "Atornillador inalámbrico 12V", CatHerramientasElectricas, 420m, 820m, 25, "25221720"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000012"), "HE-005", "Sierra circular 7.25\"", CatHerramientasElectricas, 580m, 1150m, 20, "25221720"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000013"), "HE-006", "Rotomartillo SDS-Plus", CatHerramientasElectricas, 650m, 1280m, 18, "25221720"),

        // ── Plomería ────────────────────────────────────────────────────────
        new(Guid.Parse("f2000000-0000-0000-0000-000000000014"), "PL-001", "Tubo PVC hidráulico 1/2\" x 3m", CatPlomeria, 45m, 89m, 300, "24121502"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000015"), "PL-002", "Tubo PVC hidráulico 3/4\" x 3m", CatPlomeria, 65m, 125m, 250, "24121502"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000016"), "PL-003", "Codo PVC 1/2\" 90°", CatPlomeria, 4m, 8m, 500, "24121503"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000017"), "PL-004", "Llave de nariz 1/2\"", CatPlomeria, 85m, 165m, 80, "40141709"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000018"), "PL-005", "Manguera PVC 1/2\" x 25m", CatPlomeria, 195m, 385m, 60, "24121502"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000019"), "PL-006", "Cemento para PVC 120ml", CatPlomeria, 32m, 62m, 150, "12352100"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000020"), "PL-007", "Válvula esférica 3/4\"", CatPlomeria, 95m, 185m, 70, "40141709"),

        // ── Material Eléctrico ──────────────────────────────────────────────
        new(Guid.Parse("f2000000-0000-0000-0000-000000000021"), "ME-001", "Cable THHN calibre 12 AWG x 100m", CatMaterialElectrico, 280m, 550m, 50, "26121600"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000022"), "ME-002", "Contacto doble polar con tierra", CatMaterialElectrico, 28m, 55m, 200, "39121000"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000023"), "ME-003", "Apagador sencillo", CatMaterialElectrico, 22m, 42m, 200, "39121000"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000024"), "ME-004", "Foco LED 10W E27 6500K", CatMaterialElectrico, 35m, 68m, 300, "39101600"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000025"), "ME-005", "Cinta aislante 18m", CatMaterialElectrico, 12m, 24m, 400, "31201504"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000026"), "ME-006", "Pastilla termomagnética 2x20A", CatMaterialElectrico, 95m, 185m, 80, "39121200"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000027"), "ME-007", "Conduit PVC 3/4\" x 3m", CatMaterialElectrico, 22m, 42m, 180, "26121700"),

        // ── Pinturas y Acabados ─────────────────────────────────────────────
        new(Guid.Parse("f2000000-0000-0000-0000-000000000028"), "PA-001", "Pintura vinílica blanca 19L", CatPinturas, 380m, 745m, 80, "12352100"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000029"), "PA-002", "Impermeabilizante elastomérico 19L", CatPinturas, 420m, 820m, 60, "12352100"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000030"), "PA-003", "Pintura de aceite café oscuro 1L", CatPinturas, 85m, 165m, 120, "12352100"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000031"), "PA-004", "Thinner industrial 1L", CatPinturas, 35m, 68m, 150, "12352200"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000032"), "PA-005", "Espátula plástica 4\"", CatPinturas, 18m, 35m, 200, "27111700"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000033"), "PA-006", "Rodillo de pintura 9\"", CatPinturas, 45m, 88m, 150, "27111700"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000034"), "PA-007", "Brocha para pintura 3\"", CatPinturas, 28m, 54m, 200, "27111700"),

        // ── Construcción ────────────────────────────────────────────────────
        new(Guid.Parse("f2000000-0000-0000-0000-000000000035"), "CO-001", "Cemento Portland 50kg", CatConstruccion, 120m, 235m, 200, "11111600"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000036"), "CO-002", "Block de concreto 15x20x40", CatConstruccion, 12m, 22m, 500, "30101504"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000037"), "CO-003", "Varilla corrugada 3/8\" x 12m", CatConstruccion, 195m, 380m, 100, "30102202"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000038"), "CO-004", "Arena gruesa m³", CatConstruccion, 350m, 680m, 30, "11111502", "M65", UnitOfMeasure.M3),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000039"), "CO-005", "Clavo para concreto 3\" caja 100pz", CatConstruccion, 45m, 88m, 120, "31161501", "XBX", UnitOfMeasure.BOX),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000040"), "CO-006", "Malla electrosoldada 6x6", CatConstruccion, 285m, 555m, 50, "30102204"),

        // ── Ferretería General ──────────────────────────────────────────────
        new(Guid.Parse("f2000000-0000-0000-0000-000000000041"), "FG-001", "Tornillo drywall 3.5x25mm x100", CatFerreteria, 22m, 42m, 300, "31161501", "XBX", UnitOfMeasure.BOX),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000042"), "FG-002", "Bisagra 3.5\" par", CatFerreteria, 18m, 35m, 250),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000043"), "FG-003", "Candado 40mm", CatFerreteria, 65m, 125m, 120),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000044"), "FG-004", "Cerradura entrada 3 puntos", CatFerreteria, 285m, 555m, 40),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000045"), "FG-005", "Ancla Fischer 6x35 caja 50pz", CatFerreteria, 38m, 74m, 200, "31161501", "XBX", UnitOfMeasure.BOX),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000046"), "FG-006", "Silicón blanco 280ml", CatFerreteria, 32m, 62m, 150, "12352100"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000047"), "FG-007", "Cinta teflón 3/4\" x 10m", CatFerreteria, 8m, 16m, 400),

        // ── Seguridad Industrial ────────────────────────────────────────────
        new(Guid.Parse("f2000000-0000-0000-0000-000000000048"), "SI-001", "Casco de seguridad blanco", CatSeguridad, 95m, 185m, 80, "46181501"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000049"), "SI-002", "Lentes de seguridad", CatSeguridad, 28m, 55m, 150, "46181504"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000050"), "SI-003", "Guantes de carnaza par", CatSeguridad, 55m, 108m, 120, "46181603"),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000051"), "SI-004", "Mascarilla N95 paquete 5", CatSeguridad, 85m, 165m, 100, "42131600", "PK", UnitOfMeasure.PACK),
        new(Guid.Parse("f2000000-0000-0000-0000-000000000052"), "SI-005", "Chaleco de seguridad naranja", CatSeguridad, 65m, 125m, 100, "46181502"),
    ];

    // ── Inventory ────────────────────────────────────────────────────────────

    private async Task SeedInventoryAsync(List<Guid> productIds)
    {
        var specs = GetProductSpecs();
        var items = specs.Select(spec => new InventoryItem
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            ProductId = spec.Id,
            WarehouseId = WarehouseId,
            Quantity = spec.StockQty,
            ReservedQuantity = 0,
            MinimumLevel = 10
        }).ToList();

        dbContext.Set<InventoryItem>().AddRange(items);
        await dbContext.SaveChangesAsync();
    }

    // ── Customers ────────────────────────────────────────────────────────────

    private async Task SeedCustomersAsync()
    {
        var customers = new[]
        {
            new Customer
            {
                Id = CustJuan,
                TenantId = TenantId,
                CustomerType = CustomerType.Individual,
                FirstName = "Juan",
                LastName = "García López",
                Rfc = "GALJ800101HDF",
                Email = "jgarcia@email.com",
                Phone = "5512345678",
                TaxRegime = "605",
                CfdiUse = "G03",
                PreferredPaymentMethod = PaymentMethod.Cash
            },
            new Customer
            {
                Id = CustConstrucciones,
                TenantId = TenantId,
                CustomerType = CustomerType.Business,
                FirstName = string.Empty,
                LastName = string.Empty,
                BusinessName = "Construcciones del Norte S.A. de C.V.",
                Rfc = "CNO010101AAA",
                Email = "compras@constructdelnorte.com",
                Phone = "5598765432",
                TaxRegime = "601",
                CfdiUse = "G01",
                PreferredPaymentMethod = PaymentMethod.Transfer
            },
            new Customer
            {
                Id = CustMaria,
                TenantId = TenantId,
                CustomerType = CustomerType.Individual,
                FirstName = "María",
                LastName = "Martínez González",
                Rfc = "MAGM900215MDF",
                Email = "mmartinez@email.com",
                Phone = "5587654321",
                TaxRegime = "605",
                CfdiUse = "G03",
                PreferredPaymentMethod = PaymentMethod.Card
            },
            new Customer
            {
                Id = CustFerretera,
                TenantId = TenantId,
                CustomerType = CustomerType.Business,
                FirstName = string.Empty,
                LastName = string.Empty,
                BusinessName = "Ferretera del Valle S. de R.L. de C.V.",
                Rfc = "FVA050101BBB",
                Email = "ventas@ferreteradvalle.com",
                Phone = "5576543219",
                TaxRegime = "601",
                CfdiUse = "G01",
                PreferredPaymentMethod = PaymentMethod.Card
            },
            new Customer
            {
                Id = CustCarlos,
                TenantId = TenantId,
                CustomerType = CustomerType.Individual,
                FirstName = "Carlos",
                LastName = "Rodríguez Hernández",
                Rfc = "ROHC750510HDF",
                Email = "crodriguez@email.com",
                Phone = "5565432109",
                TaxRegime = "605",
                CfdiUse = "P01",
                PreferredPaymentMethod = PaymentMethod.Cash
            }
        };

        dbContext.Set<Customer>().AddRange(customers);
        await dbContext.SaveChangesAsync();
    }

    // ── Sales ────────────────────────────────────────────────────────────────

    private async Task<List<(Guid SaleId, int DaysAgo)>> SeedSalesAsync(List<Guid> productIds)
    {
        // Deterministic customer rotation for linked sales (every 3rd sale gets a customer)
        Guid?[] customerCycle = [null, null, CustJuan, null, null, CustConstrucciones, null, null, CustMaria, null, null, CustFerretera, null, null, CustCarlos, null, null, CustJuan, null, null, CustConstrucciones, null, null, CustMaria, null, null, CustFerretera, null, null, CustCarlos];
        PaymentMethod[] methodCycle = [PaymentMethod.Cash, PaymentMethod.Card, PaymentMethod.Cash, PaymentMethod.Transfer, PaymentMethod.Cash, PaymentMethod.Card, PaymentMethod.Cash, PaymentMethod.Cash, PaymentMethod.Card, PaymentMethod.Cash, PaymentMethod.Transfer, PaymentMethod.Card, PaymentMethod.Cash, PaymentMethod.Cash, PaymentMethod.Card, PaymentMethod.Cash, PaymentMethod.Cash, PaymentMethod.Transfer, PaymentMethod.Cash, PaymentMethod.Card, PaymentMethod.Cash, PaymentMethod.Cash, PaymentMethod.Card, PaymentMethod.Transfer, PaymentMethod.Cash, PaymentMethod.Card, PaymentMethod.Cash, PaymentMethod.Cash, PaymentMethod.Transfer, PaymentMethod.Cash];

        var saleIds = new List<(Guid, int)>();
        var sales = new List<Sale>();
        var allItems = new List<SaleItem>();
        var allPayments = new List<Payment>();

        for (var i = 0; i < 30; i++)
        {
            var saleId = Guid.Parse($"f4000000-0000-0000-0000-{(i + 1):D12}");
            var customerId = customerCycle[i];
            var method = methodCycle[i];
            var folio = $"V-UAT-{(i + 1):D3}";

            // Pick 2-4 products deterministically based on sale index
            var itemCount = 2 + (i % 3);
            var saleItemData = Enumerable.Range(0, itemCount)
                .Select(j => productIds[(i * 3 + j * 7) % productIds.Count])
                .Distinct()
                .Take(itemCount)
                .Select(pid =>
                {
                    var spec = GetProductSpecs().First(s => s.Id == pid);
                    return (pid, spec.Sku, spec.Name, spec.Sale, Qty: (decimal)(1 + (i + pid.GetHashCode()) % 5));
                })
                .ToList();

            decimal subTotal = 0m;
            decimal taxAmount = 0m;
            var lineItems = new List<SaleItem>();

            foreach (var (pid, sku, name, unitPrice, qty) in saleItemData)
            {
                var lineBase = Math.Round(unitPrice * qty, 2);
                var lineTax = Math.Round(lineBase * 0.16m, 2);
                subTotal += lineBase;
                taxAmount += lineTax;

                lineItems.Add(new SaleItem
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantId,
                    SaleId = saleId,
                    ProductId = pid,
                    ProductName = name,
                    ProductSku = sku,
                    UnitPrice = unitPrice,
                    Quantity = qty,
                    DiscountPercentage = 0m,
                    TaxPercentage = 16m,
                    LineTotal = lineBase + lineTax
                });
            }

            var total = Math.Round(subTotal + taxAmount, 2);

            var sale = new Sale
            {
                Id = saleId,
                TenantId = TenantId,
                Folio = folio,
                Type = SaleType.Pos,
                Status = SaleStatus.Completed,
                CustomerId = customerId,
                WarehouseId = WarehouseId,
                SubTotal = Math.Round(subTotal, 2),
                DiscountAmount = 0m,
                TaxAmount = Math.Round(taxAmount, 2),
                Total = total,
                CompletedAt = DateTime.UtcNow // will be overwritten by FixSaleDatesAsync
            };

            sales.Add(sale);
            allItems.AddRange(lineItems);
            allPayments.Add(new Payment
            {
                Id = Guid.NewGuid(),
                TenantId = TenantId,
                SaleId = saleId,
                Method = method,
                Amount = total,
                Status = PaymentStatus.Paid
            });

            saleIds.Add((saleId, 30 - i)); // sale 0 is 30 days ago, sale 29 is today
        }

        dbContext.Set<Sale>().AddRange(sales);
        await dbContext.SaveChangesAsync();

        dbContext.Set<SaleItem>().AddRange(allItems);
        dbContext.Set<Payment>().AddRange(allPayments);
        await dbContext.SaveChangesAsync();

        return saleIds;
    }

    /// <summary>
    /// Backdates sale created_at and completed_at using raw SQL to bypass the AuditInterceptor,
    /// which always sets created_at = now() on inserted rows.
    /// </summary>
    private async Task FixSaleDatesAsync(List<(Guid SaleId, int DaysAgo)> saleIds)
    {
        foreach (var (saleId, daysAgo) in saleIds)
        {
            var saleDate = DateTime.UtcNow.Date.AddDays(-daysAgo).AddHours(10); // 10:00 AM each day
            await dbContext.Database.ExecuteSqlRawAsync(
                "UPDATE sales SET created_at = {0}, updated_at = {0}, completed_at = {0} WHERE id = {1}",
                saleDate, saleId);
        }
    }
}
