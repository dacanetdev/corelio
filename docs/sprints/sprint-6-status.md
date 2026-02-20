# Sprint 6: Tenant-Configurable Pricing Module

**Goal:** Replace FERRESYS's rigid 6-tier pricing with a tenant-configurable module — hardware store owners can define their own discount tiers (1-6), margin tiers (1-5), manage list prices, and perform bulk price changes.

**Duration:** 6 days (11 planned — 45% ahead of schedule)
**Status:** 🟢 Completed (100%)
**Started:** 2026-02-06
**Total Story Points:** 49 pts (US-6.1: 8, US-6.2: 13, US-6.3: 5, US-6.4: 5, US-6.5: 13, US-6.6: 5)
**Completed:** 49/49 tasks (100%)

---

## User Story 6.1: Pricing Domain Model & Infrastructure
**As a developer, I want a complete pricing domain model with tenant configuration and product pricing entities so that the application can store tenant-specific pricing structures with multi-tenancy isolation.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-6.1.1 | Create `TenantPricingConfiguration` entity (discount tier count 1-6, margin tier count 1-5, IVA settings) | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.2 | Create `DiscountTierDefinition` entity (custom tier name, tier number, percentage) | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.3 | Create `MarginTierDefinition` entity (custom tier name, tier number) | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.4 | Create `ProductDiscount` entity (per-product discount % per tier) | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.5 | Create `ProductMarginPrice` entity (per-product margin %, sale price, IVA price per tier) | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.6 | Update `Product` entity — add `ListPrice`, `NetCost`, `IvaEnabled` properties and navigation properties | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.7 | Create 5 EF Core configurations with CHECK constraints, unique indexes, cascade deletes | `feature/US-6.1-pricing-domain` | 🟢 | 8 CHECK constraints |
| TASK-6.1.8 | Create `ITenantPricingConfigurationRepository` and `IProductPricingRepository` interfaces | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.9 | Implement both repositories with `Include`/`ThenInclude` patterns | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.10 | Update `ApplicationDbContext` — 5 new `DbSet<T>` and 5 new tenant query filters | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.11 | Generate `AddPricingModuleSchema` migration (5 tables, 3 product columns, 7 indexes, 8 CHECK constraints) | `feature/US-6.1-pricing-domain` | 🟢 | |
| TASK-6.1.12 | Register both repositories in `DependencyInjection.cs` (both `IServiceCollection` and `IHostApplicationBuilder` methods) | `feature/US-6.1-pricing-domain` | 🟢 | |

**Acceptance Criteria:**
- [x] 5 new tables in database with proper tenant isolation
- [x] CHECK constraints enforce valid tier numbers (1-6 for discounts, 1-5 for margins)
- [x] Cascade deletes work correctly (delete config → delete tier definitions)
- [x] All query filters applied — tenant A cannot see tenant B's pricing

---

## User Story 6.2: Pricing Calculation Engine & CQRS
**As a system, I want a stateless pricing calculation service that exactly replicates FERRESYS's cascading discount math so that pricing is consistent and verifiable.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-6.2.1 | Implement `PricingCalculationService` — cascading discount: `NetCost = ListPrice × (1-D1/100) × ... × (1-Dn/100)` | `feature/US-6.2-pricing-calculation` | 🟢 | |
| TASK-6.2.2 | Implement margin calculation: `SalePrice = NetCost / (1 - MarginPercent/100)` | `feature/US-6.2-pricing-calculation` | 🟢 | |
| TASK-6.2.3 | Implement IVA calculation: `PriceWithIva = SalePrice × 1.16` | `feature/US-6.2-pricing-calculation` | 🟢 | |
| TASK-6.2.4 | Create CQRS commands: `SaveProductPricingCommand`, `BulkUpdatePricingCommand`, `SaveTenantPricingConfigCommand` | `feature/US-6.2-pricing-calculation` | 🟢 | |
| TASK-6.2.5 | Create CQRS queries: `GetTenantPricingConfigQuery`, `GetProductPricingQuery`, `GetProductsPricingListQuery`, `CalculatePricingQuery` | `feature/US-6.2-pricing-calculation` | 🟢 | |
| TASK-6.2.6 | Create FluentValidation validators for 3 commands | `feature/US-6.2-pricing-calculation` | 🟢 | |
| TASK-6.2.7 | Write 75 unit tests (>90% coverage) — cascading discount math, margin calculation, IVA, edge cases | `feature/US-6.2-pricing-calculation` | 🟢 | All passing |

**Acceptance Criteria:**
- [x] `NetCost = ListPrice × (1-D1/100) × (1-D2/100) × ... × (1-Dn/100)` verified
- [x] `SalePrice = NetCost / (1 - MarginPercent/100)` verified
- [x] `PriceWithIva = SalePrice × 1.16` verified
- [x] 75 unit tests passing with >90% coverage

---

## User Story 6.3: Pricing API & Service Layer
**As a developer, I want Minimal API endpoints for all pricing operations so that the Blazor UI and external integrations can manage pricing.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-6.3.1 | Create `PricingEndpoints.cs` — 7 endpoints: GET/PUT tenant config, GET product list, GET single product, PUT single, POST calculate, POST bulk-update | `feature/US-6.3-pricing-api` | 🟢 | |
| TASK-6.3.2 | Create `IPricingHttpService` + `PricingHttpService` in BlazorApp for all API calls | `feature/US-6.3-pricing-api` | 🟢 | |
| TASK-6.3.3 | Create 9 request/response model records | `feature/US-6.3-pricing-api` | 🟢 | |
| TASK-6.3.4 | Register `PricingEndpoints` in `EndpointExtensions.cs` | `feature/US-6.3-pricing-api` | 🟢 | |

**Acceptance Criteria:**
- [x] All 7 endpoints return correct data and enforce authorization
- [x] Blazor HTTP service wraps all endpoints with typed responses
- [x] Multi-tenancy enforced — tenant A cannot update tenant B's pricing

---

## User Story 6.4: Pricing Configuration UI
**As a tenant administrator, I want a Blazor page to configure my pricing tier structure (number of tiers, names, IVA settings) so that pricing reflects my business model.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-6.4.1 | Create `PricingSettings.razor` at `/settings/pricing` | `feature/US-6.4-pricing-config-ui` | 🟢 | PageHeader, form-section pattern |
| TASK-6.4.2 | Discount tiers section — 1-6 configurable tiers with custom names and active toggles | `feature/US-6.4-pricing-config-ui` | 🟢 | Dynamic add/remove |
| TASK-6.4.3 | Margin tiers section — 1-5 configurable tiers with custom names | `feature/US-6.4-pricing-config-ui` | 🟢 | |
| TASK-6.4.4 | IVA configuration section — default IVA enabled toggle and percentage field (default 16%) | `feature/US-6.4-pricing-config-ui` | 🟢 | |
| TASK-6.4.5 | Add "Configuración de Precios" to `NavMenu.razor` | `feature/US-6.4-pricing-config-ui` | 🟢 | |
| TASK-6.4.6 | Add 20 es-MX localization keys | `feature/US-6.4-pricing-config-ui` | 🟢 | |

**Acceptance Criteria:**
- [x] Admin can configure 1-6 discount tiers and 1-5 margin tiers with custom names
- [x] Save with Snackbar feedback (success/error)
- [x] Graceful fallback to defaults for new tenants with no config
- [x] All UI text in Spanish (es-MX)

---

## User Story 6.5: Product Pricing Management UI
**As an inventory manager, I want to view and edit per-product pricing on the Product Form and perform bulk price changes, so that pricing is managed entirely within Corelio.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-6.5.1 | Create `ProductDiscountEditModel.cs` and `ProductMarginPriceEditModel.cs` — mutable form binding models | `feature/US-6.5-pricing-management-ui` | 🟢 | |
| TASK-6.5.2 | Create `PricingCalculator.cs` in BlazorApp — client-side pricing math (mirrors Application layer) | `feature/US-6.5-pricing-management-ui` | 🟢 | |
| TASK-6.5.3 | Create `ProductCostos.razor` — reusable pricing editor with live recalculation and bidirectional margin/price editing | `feature/US-6.5-pricing-management-ui` | 🟢 | `Value`+`ValueChanged` pattern (no `@bind-Value` conflicts) |
| TASK-6.5.4 | Update `ProductForm.razor` — add `MudTabs` with "Datos Generales" and "Costos y Precios" tabs | `feature/US-6.5-pricing-management-ui` | 🟢 | Costos tab uses `ProductCostos` |
| TASK-6.5.5 | Create `PriceChange.razor` at `/pricing` — paginated product table with dynamic margin columns, inline edit expand, multi-select | `feature/US-6.5-pricing-management-ui` | 🟢 | |
| TASK-6.5.6 | Create `BulkPriceChangeDialog.razor` — 5 update types (% increase/decrease, fixed amount, set new margin), preview calculation | `feature/US-6.5-pricing-management-ui` | 🟢 | |
| TASK-6.5.7 | Add "Cambio de Precios" to `NavMenu.razor` | `feature/US-6.5-pricing-management-ui` | 🟢 | |
| TASK-6.5.8 | Add ~80 es-MX localization keys | `feature/US-6.5-pricing-management-ui` | 🟢 | |

**Acceptance Criteria:**
- [x] Product form "Costos" tab shows all pricing tiers with live recalculation
- [x] Editing margin% updates sale price; editing sale price updates margin%
- [x] Bulk price change updates 100 products in <10 seconds
- [x] All UI text in Spanish (es-MX)

---

## User Story 6.6: Pricing Module Integration Testing
**As the development team, I want integration tests verifying multi-tenancy isolation and calculation accuracy for the pricing module so that we can deploy with confidence.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-6.6.1 | Create `PostgreSqlTestContainerFixture` with migration support (Testcontainers PostgreSQL 16) | `feature/US-6.6-pricing-tests` | 🟢 | |
| TASK-6.6.2 | Create `TenantProvider` and `CurrentUserProvider` test context helpers | `feature/US-6.6-pricing-tests` | 🟢 | |
| TASK-6.6.3 | Create `TenantPricingConfigurationIsolationTests` (6 tests) — query filters, auto-TenantId assignment | `feature/US-6.6-pricing-tests` | 🟢 | |
| TASK-6.6.4 | Create `ProductPricingIsolationTests` (8 tests) — tenant A cannot see tenant B's product pricing | `feature/US-6.6-pricing-tests` | 🟢 | |
| TASK-6.6.5 | Create `CreateTenantConfigAndProductPricingWorkflowTests` (4 tests) — full workflow end-to-end | `feature/US-6.6-pricing-tests` | 🟢 | |
| TASK-6.6.6 | Create `BulkUpdateWorkflowTests` (5 tests) | `feature/US-6.6-pricing-tests` | 🟢 | |
| TASK-6.6.7 | Create `DatabaseConstraintsTests` (6 tests) — valid data verification | `feature/US-6.6-pricing-tests` | 🟢 | |

**Acceptance Criteria:**
- [x] 29/29 integration tests passing (100%)
- [x] Multi-tenancy isolation verified — Tenant A cannot access Tenant B's data
- [x] Testcontainers PostgreSQL 16 starts with migrations applied
- [x] Total test suite: 232/232 passing

---

**Sprint 6 Total: 49/49 SP delivered | 29 integration tests + 75 unit tests | 45% ahead of schedule**
