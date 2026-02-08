# Tenant-Configurable Pricing Module - Production-Ready User Stories

## Document Purpose

This document contains **backlog-ready user stories** for implementing the Tenant-Configurable Pricing Module in Corelio. Each story is ready to be copied directly into JIRA, Azure DevOps, or your backlog management tool.

**Status:** Ready for Backlog
**Date:** 2026-02-06
**Total Effort:** 49 Story Points
**Sprint:** Sprint 6 (2-3 week sprint)

---

## Epic Definition

### Epic: Tenant-Configurable Pricing Module

**Epic ID:** EPIC-PRICING-001

**Epic Description:**
As a **Product Owner**, I want to replace FERRESYS's rigid 6-tier pricing structure with a tenant-configurable pricing module so that hardware store owners can define their own discount tiers (1-6) and margin/price tiers (1-5), manage list prices, and perform bulk price changes efficiently.

**Business Value:**
- **Flexibility:** Tenants can configure their own pricing structure (not forced into 6-tier system)
- **Efficiency:** Bulk price change screen enables rapid price updates across products
- **Accuracy:** Cascading discount math matches FERRESYS pricing logic exactly
- **Transparency:** Dedicated Costos tab shows all pricing calculations in one place
- **CFDI Compliance:** Proper margin/price tier structure supports Mexican tax invoice requirements

**Success Metrics:**
1. **Flexibility:** 100% of tenants can configure 1-6 discount tiers and 1-5 margin tiers per business needs
2. **Adoption:** 80% of tenants use the pricing configuration feature within 1 month of launch
3. **Efficiency:** Bulk price change reduces time to update 100 products from 30 minutes to < 2 minutes
4. **Accuracy:** Pricing calculations match FERRESYS logic with 100% accuracy (validated via integration tests)
5. **User Satisfaction:** Product management stakeholders rate pricing module ≥ 8/10 for ease of use

**FERRESYS Pricing System Context:**
FERRESYS uses a **6-tier cascading discount system** and **5-tier margin/price structure** with the following math:

**Discount Calculation (Cascading):**
```
NetCost = ListPrice × (1 - D1/100) × (1 - D2/100) × ... × (1 - Dn/100)
```
Where D1-D6 are discount percentages per tier. If a tenant only uses 3 tiers, the remaining 3 are zero.

**Margin Calculation:**
```
SalePrice = NetCost / (1 - MarginPercent/100)
```

**IVA (VAT) Application:**
```
PriceWithVAT = SalePrice × 1.16  (if IVA enabled, default 16%)
```

**Corelio Improvements over FERRESYS:**
- **Configurable tier counts:** Tenants choose 1-6 discounts, 1-5 margins (not forced to 6)
- **Named tiers:** Tenants name tiers ("Mayoreo", "Distribuidor", "Público General") instead of "Tier 1, Tier 2"
- **Per-tenant VAT defaults:** Some tenants may default IVA on, others off
- **Bulk price changes:** FERRESYS lacks bulk update screen (Corelio adds this)
- **Tabbed product detail:** Separates product data (Datos) from pricing (Costos) for clarity

**Out of Scope (Future Enhancements):**
- Promotional pricing (time-bound discounts)
- Quantity-based pricing (bulk discounts)
- Customer-specific pricing overrides
- Price history and audit trail (beyond standard AuditableEntity)
- Currency conversion (Mexican pesos only in MVP)

**Epic Owner:** Product Owner
**Epic Status:** Ready for Sprint 6
**Target Start:** 2026-02-06
**Estimated Completion:** 2-3 weeks from start (49 Story Points)

---

## Sprint 6: Tenant-Configurable Pricing Module (49 SP)

### User Story 6.1: Pricing Domain Model & Infrastructure

**Story ID:** US-6.1
**Story Title:** Implement Pricing Domain Model, Repositories, and Database Schema

**Priority:** P0 (Critical - Foundation)
**Story Points:** 8
**Effort Estimate:** 10-12 hours
**Sprint:** Sprint 6

---

#### User Story

**As a** developer,
**I want** a complete pricing domain model with tenant configuration and product pricing entities,
**So that** the application can store and retrieve tenant-specific pricing structures and per-product pricing data with proper multi-tenancy isolation.

---

#### Description

Implement the foundational domain model for tenant-configurable pricing, including entities for tenant pricing configuration (discount/margin tier definitions) and product pricing (per-product discount/margin values). Create repository interfaces, EF Core configurations, and database migration. This provides the data layer foundation for all pricing features.

**Context:**
- Corelio currently stores only SalePrice on Product entity (single price)
- No support for multiple pricing tiers or discount structures
- FERRESYS uses 6-tier cascading discounts + 5-tier margin/price system
- Need flexible, tenant-configurable tier system (1-6 discounts, 1-5 margins)

**Related Documents:**
- `CLAUDE.md` (Clean Architecture patterns, multi-tenancy requirements)
- `docs/planning/01-database-schema-design.md` (existing schema reference)

---

#### Acceptance Criteria

**TenantPricingConfiguration Entity:**
- [ ] **AC1.1:** TenantPricingConfiguration.cs created in `src/Core/Corelio.Domain/Entities/`
- [ ] **AC1.2:** Inherits from TenantAuditableEntity (includes Id, TenantId, CreatedAt, etc.)
- [ ] **AC1.3:** Property: DiscountTierCount (int, required, range 1-6, default 3)
- [ ] **AC1.4:** Property: MarginTierCount (int, required, range 1-5, default 3)
- [ ] **AC1.5:** Property: DefaultIvaEnabled (bool, required, default true) - 16% VAT default
- [ ] **AC1.6:** Property: IvaPercentage (decimal, required, default 16.00m, precision 5,2)
- [ ] **AC1.7:** Navigation property: DiscountTierDefinitions (List<DiscountTierDefinition>, one-to-many)
- [ ] **AC1.8:** Navigation property: MarginTierDefinitions (List<MarginTierDefinition>, one-to-many)
- [ ] **AC1.9:** Validation: DiscountTierDefinitions.Count must equal DiscountTierCount (enforced in application layer)
- [ ] **AC1.10:** Validation: MarginTierDefinitions.Count must equal MarginTierCount (enforced in application layer)

**DiscountTierDefinition Entity:**
- [ ] **AC2.1:** DiscountTierDefinition.cs created in `src/Core/Corelio.Domain/Entities/`
- [ ] **AC2.2:** Inherits from TenantAuditableEntity
- [ ] **AC2.3:** Property: TenantPricingConfigurationId (Guid, required, foreign key)
- [ ] **AC2.4:** Property: TierNumber (int, required, range 1-6) - position in cascade (1=first applied, 6=last)
- [ ] **AC2.5:** Property: TierName (string, required, max 50 chars) - e.g., "Descuento Mayorista", "Descuento Distribuidor"
- [ ] **AC2.6:** Property: IsActive (bool, required, default true) - allows disabling a tier without deleting
- [ ] **AC2.7:** Navigation property: TenantPricingConfiguration (TenantPricingConfiguration, many-to-one)
- [ ] **AC2.8:** Unique constraint: (TenantId, TierNumber) - no duplicate tier numbers per tenant

**MarginTierDefinition Entity:**
- [ ] **AC3.1:** MarginTierDefinition.cs created in `src/Core/Corelio.Domain/Entities/`
- [ ] **AC3.2:** Inherits from TenantAuditableEntity
- [ ] **AC3.3:** Property: TenantPricingConfigurationId (Guid, required, foreign key)
- [ ] **AC3.4:** Property: TierNumber (int, required, range 1-5) - e.g., 1=Mayoreo, 2=Distribuidor, 3=Público
- [ ] **AC3.5:** Property: TierName (string, required, max 50 chars) - e.g., "Precio Mayoreo", "Precio Distribuidor"
- [ ] **AC3.6:** Property: IsActive (bool, required, default true)
- [ ] **AC3.7:** Navigation property: TenantPricingConfiguration (TenantPricingConfiguration, many-to-one)
- [ ] **AC3.8:** Unique constraint: (TenantId, TierNumber) - no duplicate tier numbers per tenant

**ProductDiscount Entity:**
- [ ] **AC4.1:** ProductDiscount.cs created in `src/Core/Corelio.Domain/Entities/`
- [ ] **AC4.2:** Inherits from TenantAuditableEntity
- [ ] **AC4.3:** Property: ProductId (Guid, required, foreign key)
- [ ] **AC4.4:** Property: TierNumber (int, required, range 1-6)
- [ ] **AC4.5:** Property: DiscountPercentage (decimal, required, range 0-100, precision 5,2) - e.g., 15.50% = 15.50m
- [ ] **AC4.6:** Navigation property: Product (Product, many-to-one)
- [ ] **AC4.7:** Unique constraint: (ProductId, TierNumber) - one discount per tier per product

**ProductMarginPrice Entity:**
- [ ] **AC5.1:** ProductMarginPrice.cs created in `src/Core/Corelio.Domain/Entities/`
- [ ] **AC5.2:** Inherits from TenantAuditableEntity
- [ ] **AC5.3:** Property: ProductId (Guid, required, foreign key)
- [ ] **AC5.4:** Property: TierNumber (int, required, range 1-5)
- [ ] **AC5.5:** Property: MarginPercentage (decimal, nullable, range 0-100, precision 5,2) - e.g., 30% margin = 30.00m
- [ ] **AC5.6:** Property: SalePrice (decimal, nullable, precision 18,2) - calculated or manually set
- [ ] **AC5.7:** Property: PriceWithIva (decimal, nullable, precision 18,2) - SalePrice × 1.16 if IVA enabled
- [ ] **AC5.8:** Navigation property: Product (Product, many-to-one)
- [ ] **AC5.9:** Unique constraint: (ProductId, TierNumber) - one margin/price per tier per product
- [ ] **AC5.10:** Validation: Either MarginPercentage OR SalePrice must be set (not both null)

**Product Entity Modifications:**
- [ ] **AC6.1:** Product.cs modified to add Property: ListPrice (decimal, nullable, precision 18,2) - "Precio Lista" (before discounts)
- [ ] **AC6.2:** Property: NetCost (decimal, nullable, precision 18,2) - "Costo Neto" (after all discounts)
- [ ] **AC6.3:** Property: IvaEnabled (bool, required, default false) - override tenant default per product
- [ ] **AC6.4:** Navigation property: Discounts (List<ProductDiscount>, one-to-many)
- [ ] **AC6.5:** Navigation property: MarginPrices (List<ProductMarginPrice>, one-to-many)
- [ ] **AC6.6:** Existing SalePrice property retained (used as default/fallback price for non-pricing-module tenants)

**Repository Interfaces:**
- [ ] **AC7.1:** ITenantPricingConfigurationRepository.cs created in `src/Core/Corelio.Domain/Repositories/`
- [ ] **AC7.2:** Method: Task<TenantPricingConfiguration?> GetByTenantIdAsync(Guid tenantId, CancellationToken ct)
- [ ] **AC7.3:** Method: Task<TenantPricingConfiguration?> GetWithTierDefinitionsAsync(Guid tenantId, CancellationToken ct) - includes DiscountTierDefinitions, MarginTierDefinitions
- [ ] **AC7.4:** Method: Task UpdateAsync(TenantPricingConfiguration config, CancellationToken ct)
- [ ] **AC7.5:** IProductPricingRepository.cs created in `src/Core/Corelio.Domain/Repositories/`
- [ ] **AC7.6:** Method: Task<ProductPricingData?> GetProductPricingAsync(Guid productId, CancellationToken ct) - returns product with Discounts and MarginPrices
- [ ] **AC7.7:** Method: Task<List<ProductPricingData>> GetProductsPricingListAsync(ProductPricingFilter filter, CancellationToken ct) - filtered list with pricing
- [ ] **AC7.8:** Method: Task UpdateProductPricingAsync(Guid productId, List<ProductDiscount> discounts, List<ProductMarginPrice> margins, CancellationToken ct)

**EF Core Configuration - TenantPricingConfiguration:**
- [ ] **AC8.1:** TenantPricingConfigurationConfiguration.cs created in `src/Infrastructure/Corelio.Infrastructure/Persistence/Configurations/`
- [ ] **AC8.2:** Table name: "tenant_pricing_configurations"
- [ ] **AC8.3:** Primary key: Id (uuid)
- [ ] **AC8.4:** Property: DiscountTierCount (int, required, CHECK constraint: value BETWEEN 1 AND 6)
- [ ] **AC8.5:** Property: MarginTierCount (int, required, CHECK constraint: value BETWEEN 1 AND 5)
- [ ] **AC8.6:** Property: DefaultIvaEnabled (bool, required, default true)
- [ ] **AC8.7:** Property: IvaPercentage (decimal(5,2), required, default 16.00, CHECK constraint: value BETWEEN 0 AND 100)
- [ ] **AC8.8:** HasMany: DiscountTierDefinitions, OnDelete: Cascade
- [ ] **AC8.9:** HasMany: MarginTierDefinitions, OnDelete: Cascade
- [ ] **AC8.10:** HasQueryFilter: TenantId matching (multi-tenancy isolation)

**EF Core Configuration - DiscountTierDefinition:**
- [ ] **AC9.1:** DiscountTierDefinitionConfiguration.cs created
- [ ] **AC9.2:** Table name: "discount_tier_definitions"
- [ ] **AC9.3:** Primary key: Id (uuid)
- [ ] **AC9.4:** Foreign key: TenantPricingConfigurationId
- [ ] **AC9.5:** Property: TierNumber (int, required, CHECK constraint: value BETWEEN 1 AND 6)
- [ ] **AC9.6:** Property: TierName (nvarchar(50), required)
- [ ] **AC9.7:** Property: IsActive (bool, required, default true)
- [ ] **AC9.8:** Unique index: (TenantId, TierNumber)
- [ ] **AC9.9:** HasQueryFilter: TenantId matching

**EF Core Configuration - MarginTierDefinition:**
- [ ] **AC10.1:** MarginTierDefinitionConfiguration.cs created
- [ ] **AC10.2:** Table name: "margin_tier_definitions"
- [ ] **AC10.3:** Primary key: Id (uuid)
- [ ] **AC10.4:** Foreign key: TenantPricingConfigurationId
- [ ] **AC10.5:** Property: TierNumber (int, required, CHECK constraint: value BETWEEN 1 AND 5)
- [ ] **AC10.6:** Property: TierName (nvarchar(50), required)
- [ ] **AC10.7:** Property: IsActive (bool, required, default true)
- [ ] **AC10.8:** Unique index: (TenantId, TierNumber)
- [ ] **AC10.9:** HasQueryFilter: TenantId matching

**EF Core Configuration - ProductDiscount:**
- [ ] **AC11.1:** ProductDiscountConfiguration.cs created
- [ ] **AC11.2:** Table name: "product_discounts"
- [ ] **AC11.3:** Primary key: Id (uuid)
- [ ] **AC11.4:** Foreign key: ProductId, OnDelete: Cascade
- [ ] **AC11.5:** Property: TierNumber (int, required, CHECK constraint: value BETWEEN 1 AND 6)
- [ ] **AC11.6:** Property: DiscountPercentage (decimal(5,2), required, CHECK constraint: value BETWEEN 0 AND 100)
- [ ] **AC11.7:** Unique index: (ProductId, TierNumber)
- [ ] **AC11.8:** HasQueryFilter: TenantId matching

**EF Core Configuration - ProductMarginPrice:**
- [ ] **AC12.1:** ProductMarginPriceConfiguration.cs created
- [ ] **AC12.2:** Table name: "product_margin_prices"
- [ ] **AC12.3:** Primary key: Id (uuid)
- [ ] **AC12.4:** Foreign key: ProductId, OnDelete: Cascade
- [ ] **AC12.5:** Property: TierNumber (int, required, CHECK constraint: value BETWEEN 1 AND 5)
- [ ] **AC12.6:** Property: MarginPercentage (decimal(5,2), nullable, CHECK constraint: value BETWEEN 0 AND 100 OR value IS NULL)
- [ ] **AC12.7:** Property: SalePrice (decimal(18,2), nullable)
- [ ] **AC12.8:** Property: PriceWithIva (decimal(18,2), nullable)
- [ ] **AC12.9:** Unique index: (ProductId, TierNumber)
- [ ] **AC12.10:** HasQueryFilter: TenantId matching

**ProductConfiguration Modifications:**
- [ ] **AC13.1:** ProductConfiguration.cs modified to add Property: ListPrice (decimal(18,2), nullable)
- [ ] **AC13.2:** Property: NetCost (decimal(18,2), nullable)
- [ ] **AC13.3:** Property: IvaEnabled (bool, required, default false)
- [ ] **AC13.4:** HasMany: Discounts (ProductDiscount), OnDelete: Cascade
- [ ] **AC13.5:** HasMany: MarginPrices (ProductMarginPrice), OnDelete: Cascade

**ApplicationDbContext Updates:**
- [ ] **AC14.1:** ApplicationDbContext.cs updated with DbSet: TenantPricingConfigurations
- [ ] **AC14.2:** DbSet: DiscountTierDefinitions
- [ ] **AC14.3:** DbSet: MarginTierDefinitions
- [ ] **AC14.4:** DbSet: ProductDiscounts
- [ ] **AC14.5:** DbSet: ProductMarginPrices
- [ ] **AC14.6:** OnModelCreating: Apply all 5 new configurations
- [ ] **AC14.7:** Query filters configured for all 5 new entities (multi-tenancy isolation)

**Repository Implementations:**
- [ ] **AC15.1:** TenantPricingConfigurationRepository.cs created in `src/Infrastructure/Corelio.Infrastructure/Persistence/Repositories/`
- [ ] **AC15.2:** Implements ITenantPricingConfigurationRepository
- [ ] **AC15.3:** GetByTenantIdAsync: Returns config for current tenant (uses query filter)
- [ ] **AC15.4:** GetWithTierDefinitionsAsync: Includes DiscountTierDefinitions and MarginTierDefinitions via .Include()
- [ ] **AC15.5:** UpdateAsync: Saves changes with EF Core change tracking
- [ ] **AC15.6:** ProductPricingRepository.cs created
- [ ] **AC15.7:** Implements IProductPricingRepository
- [ ] **AC15.8:** GetProductPricingAsync: Returns product with .Include(p => p.Discounts).Include(p => p.MarginPrices)
- [ ] **AC15.9:** GetProductsPricingListAsync: Supports filtering by category, search term, pagination
- [ ] **AC15.10:** UpdateProductPricingAsync: Removes existing discounts/margins, adds new ones, updates product ListPrice/NetCost

**Database Migration:**
- [ ] **AC16.1:** Migration file created: YYYYMMDDHHMMSS_AddPricingModuleSchema.cs
- [ ] **AC16.2:** Creates tenant_pricing_configurations table
- [ ] **AC16.3:** Creates discount_tier_definitions table
- [ ] **AC16.4:** Creates margin_tier_definitions table
- [ ] **AC16.5:** Creates product_discounts table
- [ ] **AC16.6:** Creates product_margin_prices table
- [ ] **AC16.7:** Alters products table: Adds ListPrice, NetCost, IvaEnabled columns
- [ ] **AC16.8:** All tables have proper indexes (primary keys, foreign keys, unique constraints)
- [ ] **AC16.9:** All tables have tenant_id column with index
- [ ] **AC16.10:** Down migration reverses all changes correctly

**Dependency Injection:**
- [ ] **AC17.1:** DependencyInjection.cs in Infrastructure updated
- [ ] **AC17.2:** Registered: services.AddScoped<ITenantPricingConfigurationRepository, TenantPricingConfigurationRepository>()
- [ ] **AC17.3:** Registered: services.AddScoped<IProductPricingRepository, ProductPricingRepository>()

**Testing & Validation:**
- [ ] **AC18.1:** Migration applies successfully: `dotnet ef database update` succeeds
- [ ] **AC18.2:** All 5 new tables exist in database with correct schema
- [ ] **AC18.3:** Products table has new columns (ListPrice, NetCost, IvaEnabled)
- [ ] **AC18.4:** All indexes and constraints created correctly (verify with pgAdmin or \d commands)
- [ ] **AC18.5:** Multi-tenancy query filters active: Manual SQL query shows WHERE tenant_id filter in generated SQL
- [ ] **AC18.6:** Repository methods work: Can create TenantPricingConfiguration via repository
- [ ] **AC18.7:** Cascade delete works: Deleting TenantPricingConfiguration deletes related tier definitions
- [ ] **AC18.8:** Cascade delete works: Deleting Product deletes related discounts and margin prices
- [ ] **AC18.9:** Unique constraints enforced: Attempting duplicate (TenantId, TierNumber) throws exception
- [ ] **AC18.10:** No compilation errors: Solution builds successfully

---

#### Definition of Done

- [ ] All 5 new entities created and configured (TenantPricingConfiguration, DiscountTierDefinition, MarginTierDefinition, ProductDiscount, ProductMarginPrice)
- [ ] Product entity modified with pricing fields (ListPrice, NetCost, IvaEnabled)
- [ ] All 5 EF Core configurations created with proper constraints
- [ ] ApplicationDbContext updated with DbSets and query filters
- [ ] 2 repository interfaces created (ITenantPricingConfigurationRepository, IProductPricingRepository)
- [ ] 2 repository implementations created and registered in DI
- [ ] Database migration created and applied successfully
- [ ] All tables, indexes, and constraints verified in database
- [ ] Multi-tenancy isolation verified (query filters active)
- [ ] Code reviewed and approved
- [ ] No compilation errors or warnings
- [ ] Documentation updated (CLAUDE.md if schema patterns changed)
- [ ] Committed to feature branch: `feature/US-6.1-pricing-domain-infrastructure`
- [ ] Pull request created with database schema diagram

---

#### Technical Notes

**Files Created:**
- `src/Core/Corelio.Domain/Entities/TenantPricingConfiguration.cs` (NEW)
- `src/Core/Corelio.Domain/Entities/DiscountTierDefinition.cs` (NEW)
- `src/Core/Corelio.Domain/Entities/MarginTierDefinition.cs` (NEW)
- `src/Core/Corelio.Domain/Entities/ProductDiscount.cs` (NEW)
- `src/Core/Corelio.Domain/Entities/ProductMarginPrice.cs` (NEW)
- `src/Core/Corelio.Domain/Repositories/ITenantPricingConfigurationRepository.cs` (NEW)
- `src/Core/Corelio.Domain/Repositories/IProductPricingRepository.cs` (NEW)
- `src/Infrastructure/Corelio.Infrastructure/Persistence/Configurations/TenantPricingConfigurationConfiguration.cs` (NEW)
- `src/Infrastructure/Corelio.Infrastructure/Persistence/Configurations/DiscountTierDefinitionConfiguration.cs` (NEW)
- `src/Infrastructure/Corelio.Infrastructure/Persistence/Configurations/MarginTierDefinitionConfiguration.cs` (NEW)
- `src/Infrastructure/Corelio.Infrastructure/Persistence/Configurations/ProductDiscountConfiguration.cs` (NEW)
- `src/Infrastructure/Corelio.Infrastructure/Persistence/Configurations/ProductMarginPriceConfiguration.cs` (NEW)
- `src/Infrastructure/Corelio.Infrastructure/Persistence/Repositories/TenantPricingConfigurationRepository.cs` (NEW)
- `src/Infrastructure/Corelio.Infrastructure/Persistence/Repositories/ProductPricingRepository.cs` (NEW)
- Database migration file: `YYYYMMDDHHMMSS_AddPricingModuleSchema.cs` (NEW)

**Files Modified:**
- `src/Core/Corelio.Domain/Entities/Product.cs` (add ListPrice, NetCost, IvaEnabled, navigation properties)
- `src/Infrastructure/Corelio.Infrastructure/Persistence/Configurations/ProductConfiguration.cs` (add new property mappings)
- `src/Infrastructure/Corelio.Infrastructure/Persistence/ApplicationDbContext.cs` (add DbSets, query filters)
- `src/Infrastructure/Corelio.Infrastructure/DependencyInjection.cs` (register repositories)

**Dependencies:**
- EF Core 10.0 (already installed)
- PostgreSQL 16 (already configured)
- Multi-tenancy infrastructure (already implemented in Sprint 2)

**Breaking Changes:**
- None - New tables and columns are additive
- Existing Product.SalePrice field retained for backward compatibility

**Performance Impact:**
- 5 new tables with indexes: Minimal impact on existing queries
- Query filters automatically applied: ~5-10ms per query overhead (already present for other entities)
- Cascade deletes: May take longer to delete products with many discounts/margins (acceptable for admin operations)

**Risks:**
- **Medium Risk:** Migration failure in production - Mitigated by testing on staging environment first, rollback plan documented
- **Low Risk:** Unique constraint violations - Mitigated by application-layer validation before DB save

---

---

### User Story 6.2: Pricing Calculation Engine & CQRS

**Story ID:** US-6.2
**Story Title:** Implement Pricing Calculation Service and CQRS Handlers

**Priority:** P0 (Critical - Business Logic)
**Story Points:** 13
**Effort Estimate:** 16-18 hours
**Sprint:** Sprint 6

---

#### User Story

**As a** system,
**I want** a pricing calculation service that applies cascading discounts, calculates margins, and handles IVA,
**So that** product prices are computed accurately following FERRESYS business rules and can be retrieved/updated via CQRS commands and queries.

---

#### Description

Implement the core pricing calculation logic (static pure functions) and CQRS handlers for tenant pricing configuration and product pricing operations. This includes calculation service with unit tests, DTOs, queries, commands, validators, and handlers for all pricing operations.

**Context:**
- FERRESYS uses cascading discount formula: NetCost = ListPrice × (1-D1/100) × (1-D2/100) × ... × (1-Dn/100)
- Margin calculation: SalePrice = NetCost / (1 - MarginPercent/100)
- IVA (VAT): PriceWithIva = SalePrice × 1.16 (16% tax)
- Need static pure functions for testability and reusability
- CQRS pattern for all read/write operations

**Related Documents:**
- `CLAUDE.md` (CQRS pattern, validation requirements)

---

#### Acceptance Criteria

**PricingCalculationService (Static Pure Functions):**
- [ ] **AC1.1:** PricingCalculationService.cs created in `src/Core/Corelio.Application/Services/` (static class)
- [ ] **AC1.2:** Method: `public static decimal CalculateNetCost(decimal listPrice, List<decimal> discountPercentages)` - applies cascading discounts
- [ ] **AC1.3:** CalculateNetCost: Returns listPrice × (1 - D1/100) × (1 - D2/100) × ... × (1 - Dn/100)
- [ ] **AC1.4:** CalculateNetCost: Handles empty discount list (returns listPrice unchanged)
- [ ] **AC1.5:** CalculateNetCost: Handles zero discounts (0.00% = no change)
- [ ] **AC1.6:** CalculateNetCost: Handles 100% discount (should return 0)
- [ ] **AC1.7:** CalculateNetCost: Rounds result to 2 decimal places (Math.Round(value, 2, MidpointRounding.AwayFromZero))
- [ ] **AC1.8:** Method: `public static decimal CalculateSalePriceFromMargin(decimal netCost, decimal marginPercentage)` - calculates sale price from margin
- [ ] **AC1.9:** CalculateSalePriceFromMargin: Returns netCost / (1 - marginPercentage/100)
- [ ] **AC1.10:** CalculateSalePriceFromMargin: Throws ArgumentException if marginPercentage >= 100 (would cause division by zero or negative price)
- [ ] **AC1.11:** CalculateSalePriceFromMargin: Rounds result to 2 decimal places
- [ ] **AC1.12:** Method: `public static decimal CalculateMarginFromSalePrice(decimal netCost, decimal salePrice)` - reverse calculation
- [ ] **AC1.13:** CalculateMarginFromSalePrice: Returns ((salePrice - netCost) / salePrice) × 100
- [ ] **AC1.14:** CalculateMarginFromSalePrice: Handles zero netCost (returns 100% margin)
- [ ] **AC1.15:** CalculateMarginFromSalePrice: Throws ArgumentException if salePrice = 0 (division by zero)
- [ ] **AC1.16:** CalculateMarginFromSalePrice: Rounds result to 2 decimal places
- [ ] **AC1.17:** Method: `public static decimal ApplyIva(decimal salePrice, decimal ivaPercentage)` - applies IVA
- [ ] **AC1.18:** ApplyIva: Returns salePrice × (1 + ivaPercentage/100)
- [ ] **AC1.19:** ApplyIva: Default ivaPercentage parameter = 16.00m
- [ ] **AC1.20:** ApplyIva: Rounds result to 2 decimal places
- [ ] **AC1.21:** Method: `public static decimal RemoveIva(decimal priceWithIva, decimal ivaPercentage)` - reverse calculation
- [ ] **AC1.22:** RemoveIva: Returns priceWithIva / (1 + ivaPercentage/100)
- [ ] **AC1.23:** RemoveIva: Rounds result to 2 decimal places

**PricingCalculationService Unit Tests:**
- [ ] **AC2.1:** PricingCalculationServiceTests.cs created in `tests/Corelio.Application.Tests/Services/`
- [ ] **AC2.2:** Test: CalculateNetCost_WithSingleDiscount_ReturnsCorrectValue (1000 × (1-0.10) = 900)
- [ ] **AC2.3:** Test: CalculateNetCost_WithMultipleCascadingDiscounts_ReturnsCorrectValue (1000 × (1-0.10) × (1-0.05) × (1-0.02) = 1000 × 0.9 × 0.95 × 0.98 = 838.62)
- [ ] **AC2.4:** Test: CalculateNetCost_WithEmptyDiscounts_ReturnsListPrice (1000 → 1000)
- [ ] **AC2.5:** Test: CalculateNetCost_WithZeroDiscounts_ReturnsListPrice ([0, 0, 0] → 1000)
- [ ] **AC2.6:** Test: CalculateNetCost_With100PercentDiscount_ReturnsZero (1000 × (1-1.00) = 0)
- [ ] **AC2.7:** Test: CalculateSalePriceFromMargin_With30PercentMargin_ReturnsCorrectValue (700 / (1-0.30) = 700 / 0.70 = 1000.00)
- [ ] **AC2.8:** Test: CalculateSalePriceFromMargin_With50PercentMargin_ReturnsCorrectValue (500 / (1-0.50) = 500 / 0.50 = 1000.00)
- [ ] **AC2.9:** Test: CalculateSalePriceFromMargin_With100PercentMargin_ThrowsException
- [ ] **AC2.10:** Test: CalculateMarginFromSalePrice_Returns30Percent (netCost=700, salePrice=1000 → 30%)
- [ ] **AC2.11:** Test: ApplyIva_With16Percent_ReturnsCorrectValue (1000 × 1.16 = 1160.00)
- [ ] **AC2.12:** Test: RemoveIva_From1160_Returns1000 (1160 / 1.16 = 1000.00)
- [ ] **AC2.13:** All tests passing with >90% code coverage on PricingCalculationService

**DTOs - Tenant Pricing Configuration:**
- [ ] **AC3.1:** TenantPricingConfigDto.cs created in `src/Core/Corelio.Application/DTOs/Pricing/`
- [ ] **AC3.2:** Properties: TenantId (Guid), DiscountTierCount (int), MarginTierCount (int), DefaultIvaEnabled (bool), IvaPercentage (decimal)
- [ ] **AC3.3:** Properties: DiscountTiers (List<DiscountTierDto>), MarginTiers (List<MarginTierDto>)
- [ ] **AC3.4:** DiscountTierDto.cs created with properties: TierNumber (int), TierName (string), IsActive (bool)
- [ ] **AC3.5:** MarginTierDto.cs created with properties: TierNumber (int), TierName (string), IsActive (bool)
- [ ] **AC3.6:** UpdateTenantPricingConfigDto.cs created with same properties for command input

**DTOs - Product Pricing:**
- [ ] **AC4.1:** ProductPricingDto.cs created in `src/Core/Corelio.Application/DTOs/Pricing/`
- [ ] **AC4.2:** Properties: ProductId (Guid), ProductName (string), Sku (string)
- [ ] **AC4.3:** Properties: ListPrice (decimal?), NetCost (decimal?), IvaEnabled (bool)
- [ ] **AC4.4:** Properties: Discounts (List<ProductDiscountDto>), MarginPrices (List<ProductMarginPriceDto>)
- [ ] **AC4.5:** ProductDiscountDto.cs created with properties: TierNumber (int), TierName (string), DiscountPercentage (decimal)
- [ ] **AC4.6:** ProductMarginPriceDto.cs created with properties: TierNumber (int), TierName (string), MarginPercentage (decimal?), SalePrice (decimal?), PriceWithIva (decimal?)
- [ ] **AC4.7:** UpdateProductPricingDto.cs created for command input
- [ ] **AC4.8:** BulkUpdatePricingDto.cs created with properties: ProductIds (List<Guid>), UpdateType (enum: PercentageIncrease, PercentageDecrease, FixedAmount, NewMargin), Value (decimal)

**CQRS - GetTenantPricingConfigQuery:**
- [ ] **AC5.1:** GetTenantPricingConfigQuery.cs created in `src/Core/Corelio.Application/Pricing/Queries/GetTenantPricingConfig/`
- [ ] **AC5.2:** Implements IRequest<Result<TenantPricingConfigDto>>
- [ ] **AC5.3:** GetTenantPricingConfigQueryHandler.cs created
- [ ] **AC5.4:** Handler injects ITenantPricingConfigurationRepository and ITenantService
- [ ] **AC5.5:** Handler calls repository.GetWithTierDefinitionsAsync(tenantId)
- [ ] **AC5.6:** Handler maps entity to TenantPricingConfigDto (includes tier definitions sorted by TierNumber)
- [ ] **AC5.7:** Handler returns Success(dto) if config found
- [ ] **AC5.8:** Handler returns Failure(NotFound) if config not found for tenant
- [ ] **AC5.9:** Handler returns Failure(Error) if exception occurs

**CQRS - UpdateTenantPricingConfigCommand:**
- [ ] **AC6.1:** UpdateTenantPricingConfigCommand.cs created in `src/Core/Corelio.Application/Pricing/Commands/UpdateTenantPricingConfig/`
- [ ] **AC6.2:** Implements IRequest<Result<Guid>> (returns config ID)
- [ ] **AC6.3:** Properties match UpdateTenantPricingConfigDto
- [ ] **AC6.4:** UpdateTenantPricingConfigCommandValidator.cs created (FluentValidation)
- [ ] **AC6.5:** Validator: DiscountTierCount must be 1-6
- [ ] **AC6.6:** Validator: MarginTierCount must be 1-5
- [ ] **AC6.7:** Validator: IvaPercentage must be 0-100
- [ ] **AC6.8:** Validator: DiscountTiers.Count must equal DiscountTierCount
- [ ] **AC6.9:** Validator: MarginTiers.Count must equal MarginTierCount
- [ ] **AC6.10:** Validator: All TierNumbers must be unique within DiscountTiers (1-6)
- [ ] **AC6.11:** Validator: All TierNumbers must be unique within MarginTiers (1-5)
- [ ] **AC6.12:** Validator: All TierNames must be non-empty and ≤50 chars
- [ ] **AC6.13:** UpdateTenantPricingConfigCommandHandler.cs created
- [ ] **AC6.14:** Handler injects ITenantPricingConfigurationRepository, ITenantService
- [ ] **AC6.15:** Handler gets existing config or creates new one
- [ ] **AC6.16:** Handler removes old tier definitions (cascade delete handles this)
- [ ] **AC6.17:** Handler creates new tier definitions from command
- [ ] **AC6.18:** Handler calls repository.UpdateAsync(config)
- [ ] **AC6.19:** Handler returns Success(config.Id)
- [ ] **AC6.20:** Handler returns Failure(ValidationError) if validation fails

**CQRS - GetProductPricingQuery:**
- [ ] **AC7.1:** GetProductPricingQuery.cs created in `src/Core/Corelio.Application/Pricing/Queries/GetProductPricing/`
- [ ] **AC7.2:** Implements IRequest<Result<ProductPricingDto>>
- [ ] **AC7.3:** Property: ProductId (Guid)
- [ ] **AC7.4:** GetProductPricingQueryHandler.cs created
- [ ] **AC7.5:** Handler injects IProductPricingRepository, ITenantPricingConfigurationRepository
- [ ] **AC7.6:** Handler calls productRepo.GetProductPricingAsync(productId)
- [ ] **AC7.7:** Handler calls configRepo.GetWithTierDefinitionsAsync(tenantId) to get tier names
- [ ] **AC7.8:** Handler maps product + discounts + margins to ProductPricingDto (joins tier names)
- [ ] **AC7.9:** Handler returns Success(dto) if product found
- [ ] **AC7.10:** Handler returns Failure(NotFound) if product not found

**CQRS - GetProductsPricingListQuery:**
- [ ] **AC8.1:** GetProductsPricingListQuery.cs created in `src/Core/Corelio.Application/Pricing/Queries/GetProductsPricingList/`
- [ ] **AC8.2:** Implements IRequest<Result<PaginatedList<ProductPricingDto>>>
- [ ] **AC8.3:** Properties: CategoryId (Guid?), SearchTerm (string?), PageNumber (int), PageSize (int)
- [ ] **AC8.4:** GetProductsPricingListQueryHandler.cs created
- [ ] **AC8.5:** Handler injects IProductPricingRepository, ITenantPricingConfigurationRepository
- [ ] **AC8.6:** Handler builds filter from query parameters
- [ ] **AC8.7:** Handler calls productRepo.GetProductsPricingListAsync(filter)
- [ ] **AC8.8:** Handler gets tenant config for tier names
- [ ] **AC8.9:** Handler maps list of products to ProductPricingDto list
- [ ] **AC8.10:** Handler returns paginated result

**CQRS - UpdateProductPricingCommand:**
- [ ] **AC9.1:** UpdateProductPricingCommand.cs created in `src/Core/Corelio.Application/Pricing/Commands/UpdateProductPricing/`
- [ ] **AC9.2:** Implements IRequest<Result<Guid>>
- [ ] **AC9.3:** Properties: ProductId (Guid), ListPrice (decimal?), IvaEnabled (bool), Discounts (List<ProductDiscountDto>), MarginPrices (List<ProductMarginPriceDto>)
- [ ] **AC9.4:** UpdateProductPricingCommandValidator.cs created
- [ ] **AC9.5:** Validator: Discount percentages must be 0-100
- [ ] **AC9.6:** Validator: Margin percentages must be 0-100 or null
- [ ] **AC9.7:** Validator: Sale prices must be ≥ 0 or null
- [ ] **AC9.8:** Validator: Each MarginPrice must have MarginPercentage OR SalePrice (not both null)
- [ ] **AC9.9:** UpdateProductPricingCommandHandler.cs created
- [ ] **AC9.10:** Handler injects IProductPricingRepository
- [ ] **AC9.11:** Handler calculates NetCost using PricingCalculationService.CalculateNetCost(listPrice, discounts)
- [ ] **AC9.12:** Handler calculates SalePrice for each margin tier (if MarginPercentage provided)
- [ ] **AC9.13:** Handler calculates PriceWithIva for each tier (if IvaEnabled)
- [ ] **AC9.14:** Handler maps DTOs to ProductDiscount and ProductMarginPrice entities
- [ ] **AC9.15:** Handler calls repository.UpdateProductPricingAsync(productId, discounts, margins)
- [ ] **AC9.16:** Handler returns Success(productId)

**CQRS - BulkUpdatePricingCommand:**
- [ ] **AC10.1:** BulkUpdatePricingCommand.cs created in `src/Core/Corelio.Application/Pricing/Commands/BulkUpdatePricing/`
- [ ] **AC10.2:** Implements IRequest<Result<int>> (returns count of updated products)
- [ ] **AC10.3:** Properties: ProductIds (List<Guid>), UpdateType (PricingUpdateType enum), Value (decimal), TierNumber (int?) - which tier to update
- [ ] **AC10.4:** PricingUpdateType enum: PercentageIncrease, PercentageDecrease, FixedAmountIncrease, FixedAmountDecrease, SetNewMargin
- [ ] **AC10.5:** BulkUpdatePricingCommandValidator.cs created
- [ ] **AC10.6:** Validator: ProductIds must not be empty
- [ ] **AC10.7:** Validator: Value must be > 0
- [ ] **AC10.8:** Validator: TierNumber required if UpdateType affects specific tier
- [ ] **AC10.9:** BulkUpdatePricingCommandHandler.cs created
- [ ] **AC10.10:** Handler injects IProductPricingRepository
- [ ] **AC10.11:** Handler loops through ProductIds
- [ ] **AC10.12:** Handler gets current pricing for each product
- [ ] **AC10.13:** Handler applies update based on UpdateType (e.g., PercentageIncrease: newPrice = oldPrice × (1 + value/100))
- [ ] **AC10.14:** Handler recalculates dependent values (NetCost, SalePrice, IVA)
- [ ] **AC10.15:** Handler calls repository.UpdateProductPricingAsync for each product
- [ ] **AC10.16:** Handler returns Success(count) with number of products updated
- [ ] **AC10.17:** Handler handles errors gracefully (logs errors, continues with other products)

**CQRS - CalculatePricesCommand (Preview):**
- [ ] **AC11.1:** CalculatePricesCommand.cs created in `src/Core/Corelio.Application/Pricing/Commands/CalculatePrices/`
- [ ] **AC11.2:** Implements IRequest<Result<PricingCalculationResultDto>> (preview only, does NOT save)
- [ ] **AC11.3:** Properties: ListPrice (decimal), Discounts (List<decimal>), IvaEnabled (bool), IvaPercentage (decimal)
- [ ] **AC11.4:** CalculatePricesCommandHandler.cs created
- [ ] **AC11.5:** Handler is static calculation (no repository calls)
- [ ] **AC11.6:** Handler calls PricingCalculationService.CalculateNetCost(listPrice, discounts)
- [ ] **AC11.7:** Handler calculates sale prices for different margin percentages (e.g., 20%, 30%, 40%, 50%)
- [ ] **AC11.8:** Handler calculates prices with IVA if enabled
- [ ] **AC11.9:** Handler returns PricingCalculationResultDto with NetCost, SampleMarginPrices (for UI preview)
- [ ] **AC11.10:** PricingCalculationResultDto.cs created with properties: NetCost, SamplePrices (List<SamplePriceDto>)
- [ ] **AC11.11:** SamplePriceDto.cs created with properties: MarginPercentage, SalePrice, PriceWithIva

**Testing & Validation:**
- [ ] **AC12.1:** All unit tests for PricingCalculationService passing (13 tests minimum)
- [ ] **AC12.2:** Code coverage on PricingCalculationService >90%
- [ ] **AC12.3:** Validator tests: UpdateTenantPricingConfigCommandValidator rejects invalid tier counts (0, 7 for discounts)
- [ ] **AC12.4:** Validator tests: Rejects duplicate tier numbers
- [ ] **AC12.5:** Validator tests: Rejects tier count mismatch (e.g., DiscountTierCount=3 but 4 tiers provided)
- [ ] **AC12.6:** Handler tests: GetTenantPricingConfigQueryHandler returns correct DTO
- [ ] **AC12.7:** Handler tests: UpdateTenantPricingConfigCommandHandler creates/updates config
- [ ] **AC12.8:** Handler tests: GetProductPricingQueryHandler returns product with tier names joined
- [ ] **AC12.9:** Handler tests: UpdateProductPricingCommandHandler calculates NetCost correctly
- [ ] **AC12.10:** Handler tests: BulkUpdatePricingCommandHandler updates multiple products
- [ ] **AC12.11:** Handler tests: CalculatePricesCommandHandler returns correct preview calculations
- [ ] **AC12.12:** Solution builds successfully with zero errors

---

#### Definition of Done

- [ ] PricingCalculationService implemented with 6 static methods (cascading discounts, margin calculations, IVA)
- [ ] 13+ unit tests for PricingCalculationService with >90% coverage
- [ ] All DTOs created (9 total: config, tiers, product pricing, discounts, margins, bulk update, preview)
- [ ] 3 queries implemented (GetTenantPricingConfig, GetProductPricing, GetProductsPricingList)
- [ ] 4 commands implemented (UpdateTenantPricingConfig, UpdateProductPricing, BulkUpdatePricing, CalculatePrices)
- [ ] 3 validators created with comprehensive validation rules
- [ ] All handlers inject correct repositories and services
- [ ] All calculations follow FERRESYS business rules exactly
- [ ] Code reviewed and approved
- [ ] Unit tests for validators and handlers (>70% coverage on application layer)
- [ ] No compilation errors or warnings
- [ ] Committed to feature branch: `feature/US-6.2-pricing-calculation-cqrs`
- [ ] Pull request created with test results

---

#### Technical Notes

**Files Created:**
- `src/Core/Corelio.Application/Services/PricingCalculationService.cs` (NEW - static class)
- `src/Core/Corelio.Application/DTOs/Pricing/TenantPricingConfigDto.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/Pricing/DiscountTierDto.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/Pricing/MarginTierDto.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/Pricing/UpdateTenantPricingConfigDto.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/Pricing/ProductPricingDto.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/Pricing/ProductDiscountDto.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/Pricing/ProductMarginPriceDto.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/Pricing/BulkUpdatePricingDto.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/Pricing/PricingCalculationResultDto.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/Pricing/SamplePriceDto.cs` (NEW)
- `src/Core/Corelio.Application/Common/Enums/PricingUpdateType.cs` (NEW - enum)
- `src/Core/Corelio.Application/Pricing/Queries/GetTenantPricingConfig/GetTenantPricingConfigQuery.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Queries/GetTenantPricingConfig/GetTenantPricingConfigQueryHandler.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Queries/GetProductPricing/GetProductPricingQuery.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Queries/GetProductPricing/GetProductPricingQueryHandler.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Queries/GetProductsPricingList/GetProductsPricingListQuery.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Queries/GetProductsPricingList/GetProductsPricingListQueryHandler.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/UpdateTenantPricingConfig/UpdateTenantPricingConfigCommand.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/UpdateTenantPricingConfig/UpdateTenantPricingConfigCommandValidator.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/UpdateTenantPricingConfig/UpdateTenantPricingConfigCommandHandler.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/UpdateProductPricing/UpdateProductPricingCommand.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/UpdateProductPricing/UpdateProductPricingCommandValidator.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/UpdateProductPricing/UpdateProductPricingCommandHandler.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/BulkUpdatePricing/BulkUpdatePricingCommand.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/BulkUpdatePricing/BulkUpdatePricingCommandValidator.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/BulkUpdatePricing/BulkUpdatePricingCommandHandler.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/CalculatePrices/CalculatePricesCommand.cs` (NEW)
- `src/Core/Corelio.Application/Pricing/Commands/CalculatePrices/CalculatePricesCommandHandler.cs` (NEW)
- `tests/Corelio.Application.Tests/Services/PricingCalculationServiceTests.cs` (NEW - 13+ tests)
- `tests/Corelio.Application.Tests/Pricing/Commands/UpdateTenantPricingConfigCommandValidatorTests.cs` (NEW)
- `tests/Corelio.Application.Tests/Pricing/Handlers/GetTenantPricingConfigQueryHandlerTests.cs` (NEW)
- (Additional handler/validator test files...)

**Dependencies:**
- User Story 6.1 (Pricing Domain Model & Infrastructure) must be completed first
- MediatR (already installed)
- FluentValidation (already installed)

**Breaking Changes:**
- None - All new code

**Performance Impact:**
- Calculation service is static (no DI overhead)
- Pure functions with no DB calls (fast, <1ms)
- Bulk update command may take 5-10 seconds for 1000 products (acceptable for admin operations)

---

---

### User Story 6.3: Pricing API & Service Layer

**Story ID:** US-6.3
**Story Title:** Implement Pricing API Endpoints and Blazor Service Layer

**Priority:** P0 (Critical - Integration)
**Story Points:** 5
**Effort Estimate:** 6-8 hours
**Sprint:** Sprint 6

---

#### User Story

**As a** frontend developer,
**I want** REST API endpoints for all pricing operations and a Blazor HTTP service,
**So that** the Blazor UI can retrieve and update tenant pricing configuration and product pricing data.

---

#### Description

Implement Minimal API endpoints for tenant pricing configuration and product pricing operations, following the project's Minimal API pattern. Create corresponding Blazor HTTP service and models for frontend integration.

**Context:**
- Corelio uses Minimal APIs (no controllers) with endpoint classes
- All endpoints in `src/Presentation/Corelio.WebAPI/Endpoints/` directory
- Blazor services in `src/Presentation/Corelio.BlazorApp/Services/` directory
- Follow existing patterns from ProductEndpoints.cs and ProductService.cs

**Related Documents:**
- `CLAUDE.md` (Minimal API guidelines, endpoint organization)
- `src/Presentation/Corelio.WebAPI/Endpoints/ProductEndpoints.cs` (reference pattern)

---

#### Acceptance Criteria

**PricingEndpoints.cs:**
- [ ] **AC1.1:** PricingEndpoints.cs created in `src/Presentation/Corelio.WebAPI/Endpoints/`
- [ ] **AC1.2:** Static class with `MapPricingEndpoints(this IEndpointRouteBuilder app)` extension method
- [ ] **AC1.3:** Endpoint group: `/api/v1/pricing` with tag "Pricing"
- [ ] **AC1.4:** All endpoints require authorization
- [ ] **AC1.5:** Endpoint: GET /api/v1/pricing/tenant-config → calls GetTenantPricingConfigQuery → returns TenantPricingConfigDto
- [ ] **AC1.6:** GET /tenant-config: Returns 200 OK with config if found, 404 Not Found if no config exists
- [ ] **AC1.7:** GET /tenant-config: Requires permission "pricing.view" or "tenants.manage"
- [ ] **AC1.8:** Endpoint: PUT /api/v1/pricing/tenant-config → accepts UpdateTenantPricingConfigRequest → calls UpdateTenantPricingConfigCommand → returns 200 OK with ID
- [ ] **AC1.9:** PUT /tenant-config: Returns 400 Bad Request if validation fails
- [ ] **AC1.10:** PUT /tenant-config: Requires permission "pricing.manage" or "tenants.manage"
- [ ] **AC1.11:** Endpoint: GET /api/v1/pricing/products → calls GetProductsPricingListQuery → returns paginated list
- [ ] **AC1.12:** GET /products: Query params: categoryId, searchTerm, pageNumber, pageSize
- [ ] **AC1.13:** GET /products: Returns 200 OK with PaginatedList<ProductPricingDto>
- [ ] **AC1.14:** GET /products: Requires permission "pricing.view" or "products.view"
- [ ] **AC1.15:** Endpoint: GET /api/v1/pricing/products/{id:guid} → calls GetProductPricingQuery → returns ProductPricingDto
- [ ] **AC1.16:** GET /products/{id}: Returns 200 OK if found, 404 Not Found if product not found
- [ ] **AC1.17:** GET /products/{id}: Requires permission "pricing.view" or "products.view"
- [ ] **AC1.18:** Endpoint: PUT /api/v1/pricing/products/{id:guid} → accepts UpdateProductPricingRequest → calls UpdateProductPricingCommand → returns 200 OK
- [ ] **AC1.19:** PUT /products/{id}: Returns 400 Bad Request if validation fails, 404 if product not found
- [ ] **AC1.20:** PUT /products/{id}: Requires permission "pricing.manage" or "products.update"
- [ ] **AC1.21:** Endpoint: POST /api/v1/pricing/calculate → accepts CalculatePricesRequest → calls CalculatePricesCommand → returns PricingCalculationResultDto
- [ ] **AC1.22:** POST /calculate: Returns 200 OK with preview calculations (does NOT save)
- [ ] **AC1.23:** POST /calculate: Requires permission "pricing.view" (lightweight preview operation)
- [ ] **AC1.24:** Endpoint: POST /api/v1/pricing/bulk-update → accepts BulkUpdatePricingRequest → calls BulkUpdatePricingCommand → returns 200 OK with count
- [ ] **AC1.25:** POST /bulk-update: Returns 200 OK with {updatedCount: int}
- [ ] **AC1.26:** POST /bulk-update: Requires permission "pricing.manage"
- [ ] **AC1.27:** All endpoints use ISender mediator to dispatch CQRS commands/queries
- [ ] **AC1.28:** All endpoints return Result<T> pattern mapped to IResult (200/400/404/500)

**Request/Response Contracts:**
- [ ] **AC2.1:** UpdateTenantPricingConfigRequest.cs created in `src/Presentation/Corelio.WebAPI/Contracts/Pricing/`
- [ ] **AC2.2:** Properties match UpdateTenantPricingConfigDto from Application layer
- [ ] **AC2.3:** UpdateProductPricingRequest.cs created
- [ ] **AC2.4:** Properties match UpdateProductPricingDto from Application layer
- [ ] **AC2.5:** BulkUpdatePricingRequest.cs created
- [ ] **AC2.6:** Properties: ProductIds (List<Guid>), UpdateType (string - "PercentageIncrease", etc.), Value (decimal), TierNumber (int?)
- [ ] **AC2.7:** CalculatePricesRequest.cs created
- [ ] **AC2.8:** Properties: ListPrice (decimal), Discounts (List<decimal>), IvaEnabled (bool), IvaPercentage (decimal)

**EndpointExtensions.cs Registration:**
- [ ] **AC3.1:** EndpointExtensions.cs modified to call `app.MapPricingEndpoints();`
- [ ] **AC3.2:** Registered alongside existing MapProductEndpoints(), MapCustomerEndpoints(), etc.

**Blazor Models:**
- [ ] **AC4.1:** TenantPricingConfigModel.cs created in `src/Presentation/Corelio.BlazorApp/Models/Pricing/`
- [ ] **AC4.2:** Properties match TenantPricingConfigDto (for Blazor binding)
- [ ] **AC4.3:** DiscountTierModel.cs created
- [ ] **AC4.4:** MarginTierModel.cs created
- [ ] **AC4.5:** ProductPricingModel.cs created
- [ ] **AC4.6:** ProductDiscountModel.cs created
- [ ] **AC4.7:** ProductMarginPriceModel.cs created
- [ ] **AC4.8:** BulkUpdateRequestModel.cs created
- [ ] **AC4.9:** PricingCalculationResultModel.cs created

**IPricingService Interface:**
- [ ] **AC5.1:** IPricingService.cs created in `src/Presentation/Corelio.BlazorApp/Services/Pricing/`
- [ ] **AC5.2:** Method: Task<TenantPricingConfigModel?> GetTenantConfigAsync()
- [ ] **AC5.3:** Method: Task<bool> UpdateTenantConfigAsync(TenantPricingConfigModel config)
- [ ] **AC5.4:** Method: Task<PaginatedList<ProductPricingModel>> GetProductsPricingAsync(Guid? categoryId, string? searchTerm, int pageNumber, int pageSize)
- [ ] **AC5.5:** Method: Task<ProductPricingModel?> GetProductPricingAsync(Guid productId)
- [ ] **AC5.6:** Method: Task<bool> UpdateProductPricingAsync(Guid productId, ProductPricingModel model)
- [ ] **AC5.7:** Method: Task<PricingCalculationResultModel> CalculatePricesAsync(decimal listPrice, List<decimal> discounts, bool ivaEnabled, decimal ivaPercentage)
- [ ] **AC5.8:** Method: Task<int> BulkUpdatePricingAsync(BulkUpdateRequestModel request)

**PricingService Implementation:**
- [ ] **AC6.1:** PricingService.cs created in `src/Presentation/Corelio.BlazorApp/Services/Pricing/`
- [ ] **AC6.2:** Implements IPricingService
- [ ] **AC6.3:** Constructor injects HttpClient configured with AuthorizationMessageHandler
- [ ] **AC6.4:** GetTenantConfigAsync: GET /api/v1/pricing/tenant-config → deserializes to TenantPricingConfigModel
- [ ] **AC6.5:** GetTenantConfigAsync: Returns null if 404, logs errors if 500
- [ ] **AC6.6:** UpdateTenantConfigAsync: PUT /api/v1/pricing/tenant-config with JSON body → returns true if 200, false otherwise
- [ ] **AC6.7:** GetProductsPricingAsync: GET /api/v1/pricing/products?categoryId={}&searchTerm={}&pageNumber={}&pageSize={} → deserializes to PaginatedList
- [ ] **AC6.8:** GetProductPricingAsync: GET /api/v1/pricing/products/{id} → deserializes to ProductPricingModel
- [ ] **AC6.9:** UpdateProductPricingAsync: PUT /api/v1/pricing/products/{id} with JSON body → returns true if 200, false otherwise
- [ ] **AC6.10:** CalculatePricesAsync: POST /api/v1/pricing/calculate with JSON body → deserializes to PricingCalculationResultModel
- [ ] **AC6.11:** BulkUpdatePricingAsync: POST /api/v1/pricing/bulk-update with JSON body → deserializes response to int (updatedCount)
- [ ] **AC6.12:** All methods use try-catch with error logging (ILogger<PricingService>)
- [ ] **AC6.13:** All methods return appropriate default values on error (null, false, 0, empty list)

**Program.cs Registration:**
- [ ] **AC7.1:** Corelio.BlazorApp Program.cs updated
- [ ] **AC7.2:** Registered: builder.Services.AddScoped<IPricingService, PricingService>();
- [ ] **AC7.3:** HttpClient for PricingService configured with base address and AuthorizationMessageHandler

**Testing & Validation:**
- [ ] **AC8.1:** API endpoints accessible via Swagger/Scalar UI
- [ ] **AC8.2:** GET /api/v1/pricing/tenant-config returns 404 for new tenant (no config yet)
- [ ] **AC8.3:** PUT /api/v1/pricing/tenant-config creates config successfully → returns 200 with ID
- [ ] **AC8.4:** GET /api/v1/pricing/tenant-config returns 200 with created config
- [ ] **AC8.5:** GET /api/v1/pricing/products returns paginated list (empty or with products)
- [ ] **AC8.6:** GET /api/v1/pricing/products/{id} returns 404 for non-existent product
- [ ] **AC8.7:** PUT /api/v1/pricing/products/{id} validates request and returns 400 for invalid data (e.g., discount > 100%)
- [ ] **AC8.8:** POST /api/v1/pricing/calculate returns preview with correct calculations (matches unit tests)
- [ ] **AC8.9:** POST /api/v1/pricing/bulk-update updates multiple products successfully
- [ ] **AC8.10:** All endpoints require authentication (401 Unauthorized if no JWT)
- [ ] **AC8.11:** All endpoints enforce permissions (403 Forbidden if missing required permission)
- [ ] **AC8.12:** PricingService successfully calls API endpoints (integration test or manual test)
- [ ] **AC8.13:** No compilation errors in WebAPI or BlazorApp projects

**HTTP File for Testing:**
- [ ] **AC9.1:** Pricing.http created in `src/Presentation/Corelio.WebAPI/HttpFiles/`
- [ ] **AC9.2:** Includes all 7 endpoint requests with sample JSON bodies
- [ ] **AC9.3:** Variables configured for local testing (localhost:5000, JWT token placeholder)

---

#### Definition of Done

- [ ] PricingEndpoints.cs created with 7 endpoints (GET/PUT tenant config, GET list/single product pricing, PUT single, POST calculate, POST bulk-update)
- [ ] All endpoints registered in EndpointExtensions.cs
- [ ] 4 request contracts created (UpdateTenantPricingConfigRequest, UpdateProductPricingRequest, BulkUpdatePricingRequest, CalculatePricesRequest)
- [ ] 9 Blazor models created (config, tiers, product pricing, discounts, margins, bulk request, calculation result)
- [ ] IPricingService interface created with 8 methods
- [ ] PricingService implementation complete with HttpClient calls
- [ ] PricingService registered in Program.cs with proper DI setup
- [ ] API endpoints tested via Swagger/Scalar (all 7 endpoints functional)
- [ ] Authorization and permission checks verified (401/403 responses)
- [ ] Pricing.http file created for manual API testing
- [ ] Code reviewed and approved
- [ ] No compilation errors or warnings
- [ ] Committed to feature branch: `feature/US-6.3-pricing-api-service`
- [ ] Pull request created with Swagger screenshots

---

#### Technical Notes

**Files Created:**
- `src/Presentation/Corelio.WebAPI/Endpoints/PricingEndpoints.cs` (NEW)
- `src/Presentation/Corelio.WebAPI/Contracts/Pricing/UpdateTenantPricingConfigRequest.cs` (NEW)
- `src/Presentation/Corelio.WebAPI/Contracts/Pricing/UpdateProductPricingRequest.cs` (NEW)
- `src/Presentation/Corelio.WebAPI/Contracts/Pricing/BulkUpdatePricingRequest.cs` (NEW)
- `src/Presentation/Corelio.WebAPI/Contracts/Pricing/CalculatePricesRequest.cs` (NEW)
- `src/Presentation/Corelio.WebAPI/HttpFiles/Pricing.http` (NEW)
- `src/Presentation/Corelio.BlazorApp/Models/Pricing/TenantPricingConfigModel.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Models/Pricing/DiscountTierModel.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Models/Pricing/MarginTierModel.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Models/Pricing/ProductPricingModel.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Models/Pricing/ProductDiscountModel.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Models/Pricing/ProductMarginPriceModel.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Models/Pricing/BulkUpdateRequestModel.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Models/Pricing/PricingCalculationResultModel.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Services/Pricing/IPricingService.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Services/Pricing/PricingService.cs` (NEW)

**Files Modified:**
- `src/Presentation/Corelio.WebAPI/Endpoints/EndpointExtensions.cs` (register PricingEndpoints)
- `src/Presentation/Corelio.BlazorApp/Program.cs` (register IPricingService)

**Dependencies:**
- User Story 6.2 (Pricing Calculation Engine & CQRS) must be completed first
- MediatR (already used in endpoints)
- System.Net.Http.Json (already used in Blazor services)

**Breaking Changes:**
- None - All new endpoints

**Performance Impact:**
- API endpoints delegate to CQRS handlers (existing pattern)
- HTTP calls from Blazor add ~50-200ms latency (acceptable for admin operations)

---

---

### User Story 6.4: Pricing Configuration UI

**Story ID:** US-6.4
**Story Title:** Create Tenant Pricing Configuration Page

**Priority:** P1 (High - Admin Feature)
**Story Points:** 5
**Effort Estimate:** 6-8 hours
**Sprint:** Sprint 6

---

#### User Story

**As a** tenant administrator,
**I want** a settings page where I can configure how many discount and margin tiers my business uses, name each tier, and set VAT defaults,
**So that** the pricing module matches my specific business needs instead of forcing me into a rigid 6-tier structure.

---

#### Description

Create a Blazor page at `/settings/pricing` where tenant administrators can configure their pricing structure: choose 1-6 discount tiers, 1-5 margin tiers, name each tier (e.g., "Mayoreo", "Distribuidor"), and set IVA defaults. All UI text in Spanish (es-MX) with proper localization.

**Context:**
- FERRESYS forces all users into 6-tier discount system (inflexible)
- Hardware stores may only need 2-3 pricing tiers (small business) or 5-6 (distributors)
- Tier names should be meaningful (not "Tier 1, Tier 2")
- IVA default varies by business (some products always taxed, others not)

**Related Documents:**
- `CLAUDE.md` (Blazor UI guidelines, Spanish localization requirements)
- Existing ProductForm.razor and ProductList.razor for MudBlazor patterns

---

#### Acceptance Criteria

(Due to length constraints, providing summary - full AC would include all field details, validation, etc.)

**PricingSettings.razor Page:**
- [ ] **AC1.1-1.10:** Page created at `/settings/pricing`, uses MainLayout, PageHeader component ("Configuración de Precios" title, description, breadcrumbs Inicio→Configuración→Precios), requires "pricing.manage" permission, loads tenant config via IPricingService on OnInitializedAsync
- [ ] **AC1.11-1.20:** Discount Tiers Section: MudNumericField for DiscountTierCount (1-6 range, label "Número de Niveles de Descuento"), dynamically shows N MudTextField inputs for tier names (e.g., if count=3, show 3 name fields), each tier has checkbox "Activo" (IsActive), validation: names required and ≤50 chars
- [ ] **AC1.21-1.30:** Margin Tiers Section: Similar to discounts, MudNumericField for MarginTierCount (1-5 range, label "Número de Niveles de Margen/Precio"), dynamically shows N name fields, active checkboxes
- [ ] **AC1.31-1.40:** IVA Defaults Section: MudSwitch for DefaultIvaEnabled (label "IVA Activado por Defecto"), MudNumericField for IvaPercentage (label "Porcentaje de IVA %", default 16.00, format N2), help text: "El IVA se aplicará automáticamente en los productos nuevos si está activado"
- [ ] **AC1.41-1.50:** Action Buttons: Cancelar (Outlined, navigates back), Guardar Cambios (Filled Primary, calls SaveAsync), loading state on save, success MudSnackbar on save, error MudSnackbar if validation fails
- [ ] **AC1.51-1.60:** Dynamic Tier Fields: When DiscountTierCount changes from 3→4, 4th field appears; from 4→3, 4th field disappears (list truncated)
- [ ] **AC1.61-1.70:** Mobile responsive: Two-column grid desktop (>= 960px), stacked mobile (<960px), tested at 375px/768px/1920px

**Spanish Localization (SharedResource.es-MX.resx):**
- [ ] **AC2.1-2.60:** ~60 new localization keys:
  - PricingConfiguration = Configuración de Precios
  - DiscountTiers = Niveles de Descuento
  - MarginTiers = Niveles de Margen/Precio
  - NumberOfDiscountTiers = Número de Niveles de Descuento
  - NumberOfMarginTiers = Número de Niveles de Margen/Precio
  - TierName = Nombre del Nivel
  - TierActive = Activo
  - DefaultIvaEnabled = IVA Activado por Defecto
  - IvaPercentage = Porcentaje de IVA (%)
  - IvaHelpText = El IVA se aplicará automáticamente en los productos nuevos si está activado
  - DiscountTier1 = Nivel de Descuento 1
  - DiscountTier2 = Nivel de Descuento 2
  - (... up to Tier 6)
  - MarginTier1 = Nivel de Margen 1
  - MarginTier2 = Nivel de Margen 2
  - (... up to Tier 5)
  - TierNameRequired = El nombre del nivel es requerido
  - TierNameTooLong = El nombre del nivel no puede exceder 50 caracteres
  - InvalidDiscountTierCount = El número de niveles debe estar entre 1 y 6
  - InvalidMarginTierCount = El número de niveles debe estar entre 1 y 5
  - SaveSuccessful = Configuración guardada exitosamente
  - SaveFailed = Error al guardar la configuración
  - (Additional keys for field labels, help text, error messages)

**Navigation Menu Item:**
- [ ] **AC3.1:** NavMenu.razor updated with "Configuración" section
- [ ] **AC3.2:** Menu item: "Precios" (Settings icon, navigates to /settings/pricing)
- [ ] **AC3.3:** Requires "pricing.manage" or "tenants.manage" permission to display
- [ ] **AC3.4:** Localized link text: @L["PricingConfiguration"]

**Form Validation:**
- [ ] **AC4.1:** MudForm with @ref and validation
- [ ] **AC4.2:** DiscountTierCount validates: 1-6 range, required
- [ ] **AC4.3:** MarginTierCount validates: 1-5 range, required
- [ ] **AC4.4:** Each tier name validates: required, ≤50 chars
- [ ] **AC4.5:** IvaPercentage validates: 0-100 range
- [ ] **AC4.6:** Form disables Save button when invalid
- [ ] **AC4.7:** Shows Spanish error messages from localization

**Testing & Validation:**
- [ ] **AC5.1:** Page loads without errors, shows loading state
- [ ] **AC5.2:** If no config exists (new tenant), shows default values (3 discount tiers, 3 margin tiers, IVA 16%)
- [ ] **AC5.3:** If config exists, loads and displays current values
- [ ] **AC5.4:** Changing DiscountTierCount from 3→5 adds 2 new tier name fields
- [ ] **AC5.5:** Changing DiscountTierCount from 5→3 removes 2 tier name fields
- [ ] **AC5.6:** Saving valid config calls IPricingService.UpdateTenantConfigAsync
- [ ] **AC5.7:** Success Snackbar displays "Configuración guardada exitosamente"
- [ ] **AC5.8:** Validation errors display in Spanish (e.g., "El nombre del nivel es requerido")
- [ ] **AC5.9:** Unauthorized users (missing permission) redirected to AccessDenied
- [ ] **AC5.10:** Mobile responsive: Fields stack vertically at <960px, touch targets ≥44px

---

#### Definition of Done

- [ ] PricingSettings.razor created with full configuration UI
- [ ] ~60 Spanish localization keys added to SharedResource.es-MX.resx
- [ ] NavMenu.razor updated with "Precios" link under "Configuración" section
- [ ] Form validation implemented with Spanish error messages
- [ ] Dynamic tier fields work correctly (add/remove on count change)
- [ ] Mobile responsive (tested at 375px, 768px, 1920px)
- [ ] Authorization checks enforced (permission "pricing.manage")
- [ ] Code reviewed and approved
- [ ] Manual testing complete (create config, update config, validation errors)
- [ ] No console errors or warnings
- [ ] Committed to feature branch: `feature/US-6.4-pricing-config-ui`
- [ ] Pull request created with screenshots (desktop + mobile)

---

#### Technical Notes

**Files Created:**
- `src/Presentation/Corelio.BlazorApp/Components/Pages/Settings/PricingSettings.razor` (NEW)
- `src/Presentation/Corelio.BlazorApp/Components/Pages/Settings/PricingSettings.razor.cs` (NEW - code-behind)

**Files Modified:**
- `src/Presentation/Corelio.BlazorApp/Components/Layout/NavMenu.razor` (add menu item)
- `src/Presentation/Corelio.BlazorApp/Resources/SharedResource.es-MX.resx` (add ~60 keys)

**Dependencies:**
- User Story 6.3 (Pricing API & Service Layer) must be completed first
- IPricingService (already created in 6.3)
- MudBlazor components (already installed)

**Breaking Changes:**
- None - New page

**Performance Impact:**
- Page load: ~100-200ms (single API call to get config)
- Save operation: ~200-500ms (PUT request + DB update)

---

---

### User Story 6.5: Product Pricing Management UI

**Story ID:** US-6.5
**Story Title:** Implement Product Pricing UI (Costos Tab, Price Change Screen, Bulk Update Dialog)

**Priority:** P0 (Critical - User-Facing Feature)
**Story Points:** 13
**Effort Estimate:** 16-18 hours
**Sprint:** Sprint 6

---

#### User Story

**As a** product manager,
**I want** a dedicated Costos tab in product detail, a price change screen to manage all product prices, and a bulk update dialog,
**So that** I can efficiently manage product pricing across discount tiers and margin tiers without switching between multiple screens.

---

#### Description

Implement comprehensive product pricing UI: (1) Modify ProductForm.razor with tabbed layout (Datos/Costos), (2) Create ProductCostos reusable component for discount/margin inputs, (3) Create PriceChange.razor screen with filterable product list and inline pricing, (4) Create BulkPriceChangeDialog.razor for mass price updates.

**Context:**
- Current ProductForm.razor has all fields on one page (cluttered)
- No dedicated pricing management screen in current UI
- FERRESYS has separate "Costos" screen (Corelio improves with tabs + dedicated page)
- Bulk price changes critical for seasonal pricing, promotions, vendor cost changes

**Related Documents:**
- `CLAUDE.md` (Blazor UI guidelines, MudBlazor component usage)
- Existing ProductForm.razor (to be modified)

---

#### Acceptance Criteria

(Providing high-level summary due to complexity - full AC would detail every field, validation, event handler)

**TASK 10: ProductCostos.razor Component:**
- [ ] **AC1.1-1.50:** Reusable component created in Components/Shared/, parameters: ProductId (Guid), ListPrice (decimal?), NetCost (decimal?), IvaEnabled (bool), Discounts (List<ProductDiscountModel>), MarginPrices (List<ProductMarginPriceModel>), OnPricingChanged (EventCallback<ProductPricingModel>), loads tenant config via IPricingService to get tier names, displays section "Precio Lista y Descuentos" with MudNumericField for ListPrice (es-MX currency format), shows N discount tier inputs (MudNumericField 0-100%, suffix "%", TierName as label), auto-calculates NetCost via PricingCalculationService when discounts change, displays NetCost as read-only field, section "Márgenes y Precios de Venta" with M margin tier inputs, each tier: MarginPercentage (editable) OR SalePrice (editable) - exclusive (one disables other), auto-calculates SalePrice from MarginPercentage when changed, auto-calculates PriceWithIva if IvaEnabled, IvaEnabled toggle switch (MudSwitch), all calculations use PricingCalculationService (or call API /pricing/calculate for preview), fires OnPricingChanged when any value changes (parent updates model), Spanish localization for all labels (~30 keys), mobile responsive (stacked <960px)

**TASK 11: ProductForm.razor Tabbed Layout:**
- [ ] **AC2.1-2.30:** ProductForm.razor modified with MudTabs component, Tab 1: "Datos Generales" (label @L["GeneralData"]) - contains Nombre, SKU, Código de Barras, Categoría, Descripción, Unidad de Medida, Stock Mínimo, fields unchanged from current implementation, Tab 2: "Costos y Precios" (label @L["CostsAndPrices"]) - contains ProductCostos component, ProductCostos bound to product model pricing fields, OnPricingChanged updates product model (ListPrice, NetCost, Discounts, MarginPrices, IvaEnabled), Save button saves entire product including pricing (calls IProductService.UpdateAsync with new pricing fields), validation: Datos tab validates general fields, Costos tab validates pricing (discount ≤100%, margin OR price set), tabs have icons (Tab 1: Info, Tab 2: AttachMoney), active tab styled with primary color, mobile: tabs scroll horizontally if needed

**TASK 12: PriceChange.razor Screen:**
- [ ] **AC3.1-3.60:** Page created at /pricing, PageHeader "Cambio de Precios", filters section (MudCard) with Category selector (dropdown), Search field (name/SKU), MudTable with expandable rows, columns: Expand icon, Nombre, SKU, Precio Lista, Costo Neto, actions (Edit inline), expandable detail shows ProductCostos component (read-only or editable inline), pagination (MudPagination), loads product pricing list via IPricingService.GetProductsPricingAsync, filter triggers re-load (category/search), "Cambio Masivo" button opens BulkPriceChangeDialog, inline edit: Click Edit icon → ProductCostos component becomes editable → Save/Cancel buttons → calls IPricingService.UpdateProductPricingAsync, loading state while fetching data, empty state if no products, Spanish localization (~40 keys: column headers, filter labels, buttons), mobile: table horizontal scroll, expandable rows stack fields vertically, filters wrap/stack, action buttons reduce to icons only

**TASK 13: BulkPriceChangeDialog.razor:**
- [ ] **AC4.1-4.50:** Dialog component (MudDialog), title "Cambio Masivo de Precios", selection section: "Productos Seleccionados: {count}" display, option to select all/deselect all (from parent PriceChange page), update type section (MudRadioGroup): "Aumento Porcentual" (PercentageIncrease), "Disminución Porcentual" (PercentageDecrease), "Aumento Fijo" (FixedAmountIncrease), "Disminución Fija" (FixedAmountDecrease), "Nuevo Margen" (SetNewMargin), value input (MudNumericField, label changes based on type: "Porcentaje (%)" or "Monto ($)" or "Nuevo Margen (%)"), tier selection if type affects specific tier (e.g., "¿Qué nivel de margen actualizar?" dropdown with tier names), preview section: Shows sample calculation (before/after for first product), calculated using PricingCalculationService or API /pricing/calculate, action buttons: Cancelar (closes dialog), Aplicar Cambios (calls IPricingService.BulkUpdatePricingAsync, shows loading spinner, closes on success, shows error Snackbar if fails), confirmation: "¿Está seguro de aplicar estos cambios a {count} productos?" (MudDialog before submit), Spanish localization (~25 keys), mobile: stacked layout, radio buttons stack vertically

**Spanish Localization Keys:**
- [ ] **AC5.1-5.100:** ~100 total new keys added:
  - GeneralData = Datos Generales
  - CostsAndPrices = Costos y Precios
  - PriceChange = Cambio de Precios
  - BulkPriceChange = Cambio Masivo de Precios
  - ListPrice = Precio Lista
  - NetCost = Costo Neto
  - Discounts = Descuentos
  - MarginsAndPrices = Márgenes y Precios de Venta
  - DiscountPercentage = Porcentaje de Descuento
  - MarginPercentage = Porcentaje de Margen
  - SalePrice = Precio de Venta
  - PriceWithIva = Precio con IVA
  - IvaEnabled = IVA Activado
  - ProductsSelected = Productos Seleccionados
  - UpdateType = Tipo de Actualización
  - PercentageIncrease = Aumento Porcentual
  - PercentageDecrease = Disminución Porcentual
  - FixedAmountIncrease = Aumento Fijo
  - FixedAmountDecrease = Disminución Fija
  - SetNewMargin = Establecer Nuevo Margen
  - ApplyChanges = Aplicar Cambios
  - ConfirmBulkUpdate = ¿Está seguro de aplicar estos cambios a {0} productos?
  - BulkUpdateSuccessful = Cambios aplicados exitosamente a {0} productos
  - BulkUpdateFailed = Error al aplicar cambios masivos
  - (Additional keys for all labels, buttons, validation messages, column headers)

**Testing & Validation:**
- [ ] **AC6.1:** ProductForm tabs render correctly, switching between Datos/Costos works
- [ ] **AC6.2:** ProductCostos component calculates NetCost correctly when discounts change (matches unit tests)
- [ ] **AC6.3:** ProductCostos calculates SalePrice from MarginPercentage correctly
- [ ] **AC6.4:** ProductCostos calculates PriceWithIva when IvaEnabled toggled
- [ ] **AC6.5:** ProductForm save includes pricing data (Discounts, MarginPrices arrays)
- [ ] **AC6.6:** PriceChange page loads product list with pagination
- [ ] **AC6.7:** Category filter works (filters products by category)
- [ ] **AC6.8:** Search filter works (filters by name/SKU)
- [ ] **AC6.9:** Expandable rows show ProductCostos component
- [ ] **AC6.10:** Inline edit saves pricing changes successfully
- [ ] **AC6.11:** BulkPriceChangeDialog opens from "Cambio Masivo" button
- [ ] **AC6.12:** Bulk update preview shows correct before/after calculations
- [ ] **AC6.13:** Bulk update applies changes to selected products successfully
- [ ] **AC6.14:** Success Snackbar shows "{count} productos actualizados exitosamente"
- [ ] **AC6.15:** All components tested at 375px (mobile), 768px (tablet), 1920px (desktop)
- [ ] **AC6.16:** No console errors or warnings

---

#### Definition of Done

- [ ] ProductCostos.razor component created and functional
- [ ] ProductForm.razor modified with MudTabs (Datos/Costos)
- [ ] PriceChange.razor page created with filters, table, expandable rows
- [ ] BulkPriceChangeDialog.razor created with all update types
- [ ] ~100 Spanish localization keys added
- [ ] All pricing calculations match PricingCalculationService logic
- [ ] Mobile responsive (all components tested at 375px, 768px, 1920px)
- [ ] Code reviewed and approved
- [ ] Manual end-to-end testing complete (create product with pricing, update pricing, bulk update)
- [ ] No console errors or warnings
- [ ] Committed to feature branch: `feature/US-6.5-product-pricing-ui`
- [ ] Pull request created with demo video/screenshots

---

#### Technical Notes

**Files Created:**
- `src/Presentation/Corelio.BlazorApp/Components/Shared/ProductCostos.razor` (NEW)
- `src/Presentation/Corelio.BlazorApp/Components/Shared/ProductCostos.razor.cs` (NEW - code-behind)
- `src/Presentation/Corelio.BlazorApp/Components/Pages/Pricing/PriceChange.razor` (NEW)
- `src/Presentation/Corelio.BlazorApp/Components/Pages/Pricing/PriceChange.razor.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Components/Dialogs/BulkPriceChangeDialog.razor` (NEW)
- `src/Presentation/Corelio.BlazorApp/Components/Dialogs/BulkPriceChangeDialog.razor.cs` (NEW)

**Files Modified:**
- `src/Presentation/Corelio.BlazorApp/Components/Pages/Products/ProductForm.razor` (add MudTabs, integrate ProductCostos)
- `src/Presentation/Corelio.BlazorApp/Components/Layout/NavMenu.razor` (add "Cambio de Precios" menu item)
- `src/Presentation/Corelio.BlazorApp/Resources/SharedResource.es-MX.resx` (add ~100 keys)

**Dependencies:**
- User Story 6.3 (Pricing API & Service Layer) must be completed first
- User Story 6.4 (Pricing Configuration UI) recommended (tenant config exists before managing product pricing)

**Breaking Changes:**
- ProductForm.razor layout changes (tabs instead of single form) - visual change only, no API changes

**Performance Impact:**
- PriceChange page: Loads paginated list (~50-100ms for 50 products)
- ProductCostos component: Client-side calculations (<5ms, no API calls for preview)
- Bulk update: May take 5-10 seconds for 500+ products (shows loading spinner, acceptable)

---

---

### User Story 6.6: Pricing Module Testing

**Story ID:** US-6.6
**Story Title:** Comprehensive Testing for Pricing Module

**Priority:** P1 (High - Quality Assurance)
**Story Points:** 5
**Effort Estimate:** 6-8 hours
**Sprint:** Sprint 6

---

#### User Story

**As a** QA engineer / developer,
**I want** comprehensive unit tests and integration tests for the pricing module,
**So that** pricing calculations are verified accurate, multi-tenancy isolation is enforced, and all handlers/validators work correctly.

---

#### Description

Create unit tests for all pricing CQRS handlers, validators, and repositories. Create integration tests for multi-tenancy isolation and end-to-end pricing workflows. Achieve >70% code coverage on Application layer pricing code.

**Context:**
- Critical business logic (pricing calculations) requires high test coverage
- Multi-tenancy isolation must be verified (tenant A cannot see tenant B's pricing config)
- FERRESYS pricing logic accuracy must be proven via tests

**Related Documents:**
- `CLAUDE.md` (Testing strategy, coverage requirements)

---

#### Acceptance Criteria

(Summary format - full test suite would have 50+ test methods)

**Unit Tests - Handlers:**
- [ ] **AC1.1-1.30:** Handler tests created in `tests/Corelio.Application.Tests/Pricing/Handlers/`:
  - GetTenantPricingConfigQueryHandlerTests.cs (tests: returns config, returns NotFound if missing, handles errors)
  - UpdateTenantPricingConfigCommandHandlerTests.cs (tests: creates new config, updates existing, cascade creates tier definitions, validates tier count)
  - GetProductPricingQueryHandlerTests.cs (tests: returns product with pricing, joins tier names correctly, returns NotFound if product missing)
  - GetProductsPricingListQueryHandlerTests.cs (tests: returns paginated list, filters by category, filters by search, pagination works)
  - UpdateProductPricingCommandHandlerTests.cs (tests: calculates NetCost correctly, calculates SalePrice from margin, calculates IVA, saves discounts/margins)
  - BulkUpdatePricingCommandHandlerTests.cs (tests: updates multiple products, PercentageIncrease type works, SetNewMargin type works, error handling for partial failures)
  - CalculatePricesCommandHandlerTests.cs (tests: returns preview with correct calculations, handles edge cases like 100% discount)

**Unit Tests - Validators:**
- [ ] **AC2.1-2.20:** Validator tests created:
  - UpdateTenantPricingConfigCommandValidatorTests.cs (tests: rejects DiscountTierCount <1 or >6, rejects MarginTierCount <1 or >5, rejects tier count mismatch, rejects duplicate TierNumbers, rejects empty TierNames, rejects TierName >50 chars, rejects IvaPercentage <0 or >100)
  - UpdateProductPricingCommandValidatorTests.cs (tests: rejects discount <0 or >100, rejects margin <0 or >100, rejects null margin AND null price, accepts valid data)
  - BulkUpdatePricingCommandValidatorTests.cs (tests: rejects empty ProductIds, rejects Value ≤0, accepts valid requests)

**Integration Tests - Multi-Tenancy Isolation:**
- [ ] **AC3.1-3.20:** Integration tests created in `tests/Corelio.Integration.Tests/Pricing/`:
  - TenantPricingConfigurationIsolationTests.cs (tests: Tenant A cannot read Tenant B's config, Tenant A cannot update Tenant B's config, query filters automatically applied, repository methods respect tenancy)
  - ProductPricingIsolationTests.cs (tests: Tenant A cannot read Tenant B's product pricing, Tenant A cannot update Tenant B's products, bulk update only affects current tenant's products)
  - Uses Testcontainers for PostgreSQL, seeds 2 tenants with separate configs/products, verifies isolation

**Integration Tests - End-to-End Workflows:**
- [ ] **AC4.1-4.20:** E2E workflow tests:
  - CreateTenantConfigAndProductPricingWorkflowTests.cs (test: Create tenant config → Create product with pricing → Update pricing → Verify calculations correct → Verify data persisted)
  - BulkUpdateWorkflowTests.cs (test: Create 10 products → Apply 10% price increase via bulk update → Verify all 10 products updated correctly)
  - IvaCalculationWorkflowTests.cs (test: Product with IVA enabled → Verify PriceWithIva calculated correctly → Toggle IVA off → Verify PriceWithIva null)

**Code Coverage:**
- [ ] **AC5.1:** Overall Application layer pricing code coverage >70% (measured via `dotnet test --collect:"XPlat Code Coverage"`)
- [ ] **AC5.2:** PricingCalculationService coverage >90% (already achieved in US-6.2)
- [ ] **AC5.3:** All handlers coverage >70%
- [ ] **AC5.4:** All validators coverage 100% (easy to achieve with validator tests)

**Test Execution:**
- [ ] **AC6.1:** All unit tests pass (green in CI/CD pipeline)
- [ ] **AC6.2:** All integration tests pass (green in CI/CD pipeline)
- [ ] **AC6.3:** No test failures or flaky tests
- [ ] **AC6.4:** Tests run in <2 minutes total (unit tests <30 seconds, integration <90 seconds)

---

#### Definition of Done

- [ ] 7 handler test files created with 20+ test methods total
- [ ] 3 validator test files created with 15+ test methods total
- [ ] 2 integration test files for multi-tenancy isolation
- [ ] 3 E2E workflow test files
- [ ] Code coverage >70% on Application layer pricing code
- [ ] All tests passing in CI/CD pipeline
- [ ] Code reviewed and approved
- [ ] Test results documented (coverage report generated)
- [ ] Committed to feature branch: `feature/US-6.6-pricing-module-testing`
- [ ] Pull request created with coverage report

---

#### Technical Notes

**Files Created:**
- `tests/Corelio.Application.Tests/Pricing/Handlers/GetTenantPricingConfigQueryHandlerTests.cs` (NEW)
- `tests/Corelio.Application.Tests/Pricing/Handlers/UpdateTenantPricingConfigCommandHandlerTests.cs` (NEW)
- (5 more handler test files...)
- `tests/Corelio.Application.Tests/Pricing/Validators/UpdateTenantPricingConfigCommandValidatorTests.cs` (NEW)
- (2 more validator test files...)
- `tests/Corelio.Integration.Tests/Pricing/TenantPricingConfigurationIsolationTests.cs` (NEW)
- `tests/Corelio.Integration.Tests/Pricing/ProductPricingIsolationTests.cs` (NEW)
- `tests/Corelio.Integration.Tests/Pricing/CreateTenantConfigAndProductPricingWorkflowTests.cs` (NEW)
- (2 more E2E workflow test files...)

**Dependencies:**
- User Stories 6.1, 6.2, 6.3 must be completed (code to test must exist)
- xUnit v3 (already installed)
- FluentAssertions (already installed)
- Moq (already installed)
- Testcontainers (already installed)

**Performance Impact:**
- Test execution adds ~2 minutes to CI/CD pipeline (acceptable)

---

---

## Sprint 6 Summary

**Total Story Points:** 49 SP
- US-6.1: Pricing Domain Model & Infrastructure - 8 SP
- US-6.2: Pricing Calculation Engine & CQRS - 13 SP
- US-6.3: Pricing API & Service Layer - 5 SP
- US-6.4: Pricing Configuration UI - 5 SP
- US-6.5: Product Pricing Management UI - 13 SP
- US-6.6: Pricing Module Testing - 5 SP

**Estimated Duration:** 2-3 weeks (based on average velocity 27.5 SP/sprint, 49 SP ≈ 1.8 sprints)

**Recommended Sequencing:**
1. **Week 1:** US-6.1 (Days 1-2) → US-6.2 (Days 3-5)
2. **Week 2:** US-6.3 (Days 6-7) → US-6.4 (Days 8-9) → US-6.5 (Days 10-14)
3. **Week 3:** US-6.6 (Days 15-16) + Buffer for bug fixes

**Demo-Ready Deliverables:**
- Tenant admin can configure pricing structure (discount/margin tier counts and names)
- Product form has Costos tab with full pricing UI
- Price change screen allows filtering and inline editing
- Bulk price change dialog updates 100s of products in seconds
- All pricing calculations match FERRESYS logic exactly
- Multi-tenancy isolation verified via tests

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-02-06 | Development Team | Initial backlog creation - 6 user stories for Sprint 6 |

---

## Appendix: Quick Reference

### Story Point Summary
- 5 points: 6-8 hours (US-6.3, US-6.4, US-6.6)
- 8 points: 10-12 hours (US-6.1)
- 13 points: 16-18 hours (US-6.2, US-6.5)

### Priority Levels
- **P0 (Critical):** Must have for pricing module - US-6.1, US-6.2, US-6.3, US-6.5
- **P1 (High):** Should have for complete feature - US-6.4, US-6.6

### Key Contacts
- **Epic Owner:** Product Owner
- **Technical Lead:** Development Team Lead
- **Domain Expert:** FERRESYS pricing SME (for validation)

### Related Documentation
1. **Project Guide:** `CLAUDE.md`
2. **Database Schema:** `docs/planning/01-database-schema-design.md`
3. **Sprint Status:** `docs/SPRINT_STATUS.md`

---

**Document Status:** ✅ Ready for Sprint 6
**Last Updated:** 2026-02-06
**Prepared By:** Development Team (Product Owner Mode)
**For:** Sprint 6 Planning
