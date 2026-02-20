# Corelio Product Backlog

**Project:** Corelio Multi-Tenant SaaS ERP for Mexican Hardware Stores
**Tech Stack:** .NET 10 + Blazor Server + PostgreSQL 16 + .NET Aspire
**Last Updated:** 2026-02-19
**Maintainer:** Development Team

---

## How to Use This Document

This is the single source of truth for all sprint planning and backlog tracking. It supersedes `docs/SPRINT_STATUS.md` for backlog purposes.

- **Completed sprints (1-7):** Concise summaries of what was delivered.
- **Next sprint (Sprint 8):** Full detail — stories, acceptance criteria, task breakdowns.
- **Future sprints (9-10):** Full detail for planning purposes.
- **Pending Sprint 3 items:** Flagged with `[TECH DEBT]` — must be addressed before Sprint 10.

When starting a new sprint, expand the "Next Sprint" section with full story detail and update the status table below.

---

## Status Legend

| Icon | Meaning |
|------|---------|
| ✅ | Completed — all DoD criteria met |
| 🎯 | NEXT SPRINT — current sprint to execute |
| 🟡 | In Progress / Partially Complete |
| ⚠️ | Partially Complete — some tasks remain |
| 🔵 | Not Started |
| [TECH DEBT] | Pending work from a prior sprint |

---

## Epics Overview

| Epic ID | Name | Sprints | Status |
|---------|------|---------|--------|
| EPIC-FOUNDATION-001 | Foundation & Infrastructure | Sprint 1-2 | ✅ Complete |
| EPIC-PRODUCTS-001 | Product & Category Management | Sprint 3 | ⚠️ Tests pending |
| EPIC-DESIGN-001 | UI/UX Design System | Sprint 4-5 | ✅ Complete |
| EPIC-PRICING-001 | Tenant-Configurable Pricing Module | Sprint 6 | ✅ Complete |
| EPIC-POS-001 | Point of Sale System | Sprint 7-8 | 🟡 Sprint 7 done, Sprint 8 next |
| EPIC-CFDI-001 | CFDI 4.0 Tax Compliance | Sprint 9 | 🔵 Not Started |
| EPIC-QA-001 | Testing, QA & Production Deployment | Sprint 10 | 🔵 Not Started |

---

## Sprint Overview Table

| Sprint | Focus | Status | SP | Dates |
|--------|-------|--------|----|-------|
| Sprint 1 | Foundation & Project Setup | ✅ Completed | 21 | Jan 8-10, 2026 |
| Sprint 2 | Multi-Tenancy & Authentication | ✅ Completed | 34 | Jan 12-13, 2026 |
| Sprint 3 | Products & Categories | ⚠️ Partially Complete | 21 | Jan 13-17, 2026 |
| Sprint 4 | UI/UX Design System Phase 1-2 | ✅ Completed | 26 | Jan 27-29, 2026 |
| Sprint 5 | UI/UX Design System Phase 3 | ✅ Completed | 3 | Feb 3, 2026 |
| Sprint 6 | Tenant-Configurable Pricing Module | ✅ Completed | 49 | Feb 6-17, 2026 |
| Sprint 7 | POS Backend & UI | ✅ Completed | 34 | Feb 2026 |
| **Sprint 8** | **POS Features & Sales Management** | **🎯 NEXT SPRINT** | **21** | **TBD** |
| Sprint 9 | CFDI Integration | 🔵 Not Started | 34 | TBD |
| Sprint 10 | Testing, QA & Deployment | 🔵 Not Started | 34 | TBD |

**Total planned:** 277 SP across 10 sprints
**Completed to date:** 188 SP (Sprints 1-7, with 3 test tasks pending from Sprint 3)
**Average team velocity:** ~26.6 SP/sprint (based on 5 completed sprints)
**Current test suite:** 254/254 passing (100%)

---

## Pending Technical Debt from Sprint 3

> [TECH DEBT] These three tasks from US-3.1 (Product Management) were not completed. They must be addressed no later than Sprint 10. Recommend slotting the unit tests into Sprint 8 since the product domain is active, and deferring integration/E2E tests to Sprint 10.

| ID | Task | Priority | Suggested Sprint |
|----|------|----------|-----------------|
| TD-3.1.A | Unit tests for Product and ProductCategory CQRS handlers (>70% coverage target) | High | Sprint 8 |
| TD-3.1.B | Integration tests with multi-tenancy isolation verification for Product endpoints | High | Sprint 10 |
| TD-3.1.C | E2E manual test scenarios documented and executed via Blazor UI | Medium | Sprint 10 |

---

## Completed Sprints (Summaries)

### Sprint 1 — Foundation & Project Setup ✅
**Dates:** Jan 8-10, 2026 | **Delivered:** 21 SP | **Velocity:** 21 SP in 3 days

| Story | SP | What Was Built |
|-------|----|----------------|
| US-1.1: Create .NET 10 Solution Structure | 5 | 11-project solution (7 src + 4 test), CI/CD pipeline, `.editorconfig`, `.gitattributes` |
| US-1.2: Configure Aspire Orchestration | 5 | AppHost with PostgreSQL + Redis, ServiceDefaults, Aspire dashboard at localhost:15888 |
| US-1.3: Implement Base Domain Entities | 5 | `BaseEntity`, `AuditableEntity`, `TenantAuditableEntity`; 12 domain enums; core entities (Tenant, User, Role, Permission, UserRole, RolePermission, RefreshToken, AuditLog); 32 unit tests |
| US-1.4: Configure EF Core with PostgreSQL | 6 | `ApplicationDbContext`, 9 Fluent API configs, multi-tenancy query filters, `TenantInterceptor`, `AuditInterceptor`, `DesignTimeDbContextFactory`, `InitialSchemaWithSeedData` migration, DataSeeder (1 tenant, 3 roles, 17 permissions, 3 test users), 12 unit tests |

**Key deliverable:** Solid Clean Architecture foundation with automated tenant isolation.

---

### Sprint 2 — Multi-Tenancy & Authentication ✅
**Dates:** Jan 12-13, 2026 | **Delivered:** 34 SP | **Velocity:** 34 SP in 2 days

| Story | SP | What Was Built |
|-------|----|----------------|
| US-2.1: Multi-Tenancy Services Backend | 13 | `ITenantService`, `TenantService` (JWT/header/subdomain resolution), `TenantMiddleware`, Redis distributed cache for tenant data, 12 unit tests |
| US-2.1.1: Multi-Tenancy Frontend | 5 | `TenantDisplay` component (reads JWT claims), `UserDisplay` component, integrated into `MainLayout` header |
| US-2.2: Authentication & Authorization Backend | 8 | `IJwtService`/`JwtService`, `AuthEndpoints` (login/register/refresh/logout/forgot/reset password), BCrypt work factor 12, 24+ RBAC authorization policies, 35 authentication tests, Scalar replacing Swagger |
| US-2.2.1: Authentication Frontend | 8 | `ITokenService`/`TokenService` (localStorage), `IAuthService`/`AuthService`, `CustomAuthenticationStateProvider`, `AuthorizationMessageHandler`, Login/Register/ForgotPassword/ResetPassword/Logout/AccessDenied pages, 47 es-MX localization keys |

**Key deliverable:** Full end-to-end authentication flow in Blazor. Users can log in via UI instead of Postman.

---

### Sprint 3 — Products & Categories ⚠️ Partially Complete
**Dates:** Jan 13-17, 2026 | **Delivered:** ~18/21 SP (7/10 tasks) | **Tests pending**

| Story | SP | What Was Built | Gap |
|-------|----|----------------|-----|
| US-3.1: Product Management API & UI | 21 | Product + ProductCategory entities (5-level hierarchy), CQRS handlers for full CRUD + search on both resources, `ProductEndpoints.cs` (6 endpoints), `ProductCategoryEndpoints.cs` (5 endpoints), `ProductList.razor`, `ProductForm.razor` (tabbed Datos/Costos, design system applied in Sprint 5) | Unit tests, integration tests, and E2E test scenarios are pending — see [TECH DEBT] section |

**Key deliverable:** Full product catalog management with hierarchical categories, available via Blazor UI.

---

### Sprint 4 — UI/UX Design System Phase 1-2 ✅
**Dates:** Jan 27-29, 2026 | **Delivered:** 26 SP | **Velocity:** 26 SP in 3 days

| Story | SP | What Was Built |
|-------|----|----------------|
| US-4.1: Core Theme Infrastructure | 5 | `MudTheme` "Industrial Terracotta" (#E74C3C primary), CSS variables, Inter font, Bootstrap removed |
| US-4.2: Authentication Pages Redesign | 8 | `AuthLayout` component, Login/Register/ForgotPassword/ResetPassword redesigned with hero sections, gradient backgrounds, fade-in animations, mobile responsive (375px tested) |
| US-4.3: Core Reusable Components | 5 | `PageHeader`, `LoadingState`, `EmptyState` components; enhanced `TenantDisplay`/`UserDisplay` with pill badges, avatars, dropdown menus |
| US-4.4: Multi-Tenant Theming Infrastructure | 8 | `TenantThemeService` (Redis 2h TTL), `DynamicThemeService`, `TenantThemeEndpoints` (GET/PUT `/api/v1/tenants/theme`), dynamic theme loading in `MainLayout` |

**Key deliverable:** Distinctive "Industrial Terracotta" design system distinguishing Corelio from generic MudBlazor apps.

---

### Sprint 5 — UI/UX Design System Phase 3 ✅
**Date:** Feb 3, 2026 | **Delivered:** 3 SP | **Velocity:** 3 SP in 1 day

| Story | SP | What Was Built |
|-------|----|----------------|
| US-5.1: Apply Design System to Existing Pages | 3 | `ProductList.razor` and `ProductForm.razor` updated to use `PageHeader`, `LoadingState`, `EmptyState`; responsive filter bar; form-section cards with icons; all hardcoded Spanish replaced with localization keys; currency formatted with es-MX culture |

---

### Sprint 6 — Tenant-Configurable Pricing Module ✅
**Dates:** Feb 6-17, 2026 | **Delivered:** 49 SP | **Velocity:** 49 SP in 6 days (45% ahead of schedule)

| Story | SP | What Was Built |
|-------|----|----------------|
| US-6.1: Pricing Domain Model & Infrastructure | 8 | 5 new entities (`TenantPricingConfiguration`, `DiscountTierDefinition`, `MarginTierDefinition`, `ProductDiscount`, `ProductMarginPrice`), 5 EF Core configs, `AddPricingModuleSchema` migration, 2 repositories |
| US-6.2: Pricing Calculation Engine & CQRS | 13 | `PricingCalculationService` (cascading discount math matching FERRESYS), 7 CQRS handlers, 3 validators, 75 unit tests (>90% coverage) |
| US-6.3: Pricing API & Service Layer | 5 | 7 Minimal API endpoints (GET/PUT tenant config, GET list/single product pricing, PUT single, POST calculate, POST bulk-update), Blazor HTTP service |
| US-6.4: Pricing Configuration UI | 5 | `PricingSettings.razor` at `/settings/pricing`, 1-6 discount tiers + 1-5 margin tiers with custom names, IVA configuration, 20 l10n keys |
| US-6.5: Product Pricing Management UI | 13 | `ProductCostos.razor` (reusable pricing editor with live recalculation), `PriceChange.razor` at `/pricing`, `BulkPriceChangeDialog.razor` (5 update types), ~80 l10n keys |
| US-6.6: Pricing Module Integration Testing | 5 | 29 integration tests using Testcontainers (PostgreSQL), 100% pass rate, multi-tenancy isolation verified |

**Key deliverable:** Hardware store owners can configure custom pricing tiers, manage per-product costs, and perform bulk price changes — all via Blazor UI.

---

### Sprint 7 — POS Backend & UI ✅
**Date:** Feb 2026 | **Delivered:** 34 SP | **PR:** #55 (merged to main)

| Story | SP | What Was Built |
|-------|----|----------------|
| US-7.1: Customer Entity & UI | 8 | `Customer` entity (individual + business), CQRS handlers, EF Core config, `CustomerList.razor`, `CustomerForm.razor` |
| US-7.2: Warehouse/Inventory/Sale Entities | 8 | `Warehouse`, `InventoryItem`, `Sale`, `SaleItem`, `Payment` entities; `AddSalesInventorySchema` migration; seed "Almacén Principal" (warehouseId: `e000...0001`, tenantId: `b000...0001`) |
| US-7.3: POS Search & Sale Operations | 8 | `IPosSearchService` with `EF.Functions.ILike` for case-insensitive PostgreSQL search, `CreateSaleCommand`, `CompleteSaleCommand`, `CancelSaleCommand` |
| US-7.4: POS & Sale API Endpoints | 5 | `PosEndpoints.cs`, `SaleEndpoints.cs`, `CustomerEndpoints.cs`, `IPosService`/`PosService` |
| US-7.5: POS Blazor UI | 5 | `Pos.razor` 3-panel layout (search / cart / payment), `ProductSearch` component, `ShoppingCart` component, `PaymentPanel` component, ~80 l10n keys |

**Key deliverable:** Functional POS terminal. Cashiers can search products, build a cart, accept payment, and complete a sale — entirely within the Blazor UI.
**Test suite after Sprint 7:** 254/254 passing (100%).

---

---

# 🎯 NEXT SPRINT — Sprint 8: POS Features & Sales Management

**Sprint Goal:** Extend the POS system with sales history and management capabilities, add inventory management UI, enable quote creation, and implement receipt/ticket generation — giving hardware store staff complete visibility and control over sales operations.

**Estimated SP:** 21
**Status:** Not Started
**Dependencies:** Sprint 7 completed (Sale, InventoryItem, Payment entities all exist)

---

## US-8.1 — Sales History & Management UI

**Priority:** P0 Critical | **Story Points:** 5 SP
**Epic:** EPIC-POS-001

### User Story
As a store manager,
I want to view, search, and manage completed and cancelled sales,
So that I can review transactions, resolve disputes, and generate daily reports without needing direct database access.

### Acceptance Criteria

**Scenario 1: View sales list**
Given the manager navigates to `/ventas`
When the page loads
Then a paginated table displays all sales for the current tenant with columns: Folio, Date, Customer (or "Público General"), Total, Payment Method, and Status (completed/cancelled)

**Scenario 2: Filter sales by date range**
Given the manager is on the sales history page
When they select a start and end date and click "Buscar"
Then only sales within that date range are shown, formatted as `dd/MM/yyyy`

**Scenario 3: Search by folio or customer name**
Given the manager enters a folio number or customer name in the search field
When they press Enter or click the search button
Then results are filtered in real-time (case-insensitive, PostgreSQL ILike)

**Scenario 4: View sale detail**
Given the manager clicks on a sale row
When the detail panel or modal opens
Then it shows all sale items, quantities, unit prices, the payment breakdown (methods and amounts), and the sale total including IVA

**Scenario 5: Cancel a completed sale**
Given the manager selects a completed sale
When they click "Cancelar Venta" and confirm the confirmation dialog
Then the sale status changes to `Cancelled`, inventory items are restored to stock, and a success snackbar is shown

**Scenario 6: Pagination**
Given there are more than 20 sales
When the page loads
Then results are paginated with 20 items per page and navigation controls (Previous/Next/page numbers)

**Scenario 7: Multi-tenancy isolation**
Given Manager A is logged into Tenant A
When they view sales history
Then they see only Tenant A's sales — never Tenant B's sales

### Technical Notes
- Affected components: `Corelio.Application`, `Corelio.WebAPI`, `Corelio.BlazorApp`
- New query: `GetSalesQuery` (with filters: dateFrom, dateTo, searchTerm, status, page, pageSize)
- New query: `GetSaleByIdQuery` (includes SaleItems and Payments)
- New command: reuse existing `CancelSaleCommand` from Sprint 7
- New endpoint: `GET /api/v1/sales?dateFrom=&dateTo=&search=&status=&page=&pageSize=` (already scaffolded in `SaleEndpoints.cs` — add query parameters)
- New endpoint: `GET /api/v1/sales/{id}` (sale detail with items and payments)
- New Blazor pages: `Pages/Sales/SaleList.razor`, `Pages/Sales/SaleDetail.razor`
- Multi-tenancy: `Sale` inherits `TenantAuditableEntity` — query filters auto-applied
- Use `AsNoTracking()` for all read queries (performance)

### Task Breakdown
- [ ] **TASK-1:** Add `GetSalesQuery` + handler with pagination and filter support
- [ ] **TASK-2:** Add `GetSaleByIdQuery` + handler (includes SaleItems, Payments)
- [ ] **TASK-3:** Update `SaleEndpoints.cs` — add `GET /api/v1/sales` (list) and `GET /api/v1/sales/{id}` (detail) endpoints
- [ ] **TASK-4:** Create `SaleList.razor` — paginated table with filter bar (date range, search, status dropdown)
- [ ] **TASK-5:** Create `SaleDetail.razor` (or modal component) — items table, payment summary, cancel button
- [ ] **TASK-6:** Add cancel sale flow to UI with confirmation dialog and snackbar feedback
- [ ] **TASK-7:** Add navigation link "Historial de Ventas" to `NavMenu.razor`
- [ ] **TASK-8:** Add ~25 es-MX localization keys (`SalesHistory`, `SaleDetail`, `CancelSale`, `SaleStatus`, `SaleItems`, `PaymentSummary`, etc.)
- [ ] **TASK-9:** Unit tests for `GetSalesQuery` handler and filter logic (>70% coverage)

### Dependencies
- [x] US-7.2: `Sale`, `SaleItem`, `Payment` entities exist
- [x] US-7.3: `CancelSaleCommand` exists
- [x] US-7.4: `SaleEndpoints.cs` scaffolded

### Definition of Done
- [ ] Backend implemented (queries, handlers, API endpoints)
- [ ] Frontend implemented (SaleList.razor, SaleDetail.razor/modal)
- [ ] All UI text in Spanish via `IStringLocalizer`
- [ ] Feature is demo-able without Postman (stakeholder can browse sales in Blazor)
- [ ] Unit tests passing (>70% coverage on new handlers)
- [ ] Multi-tenancy isolation verified
- [ ] Zero compilation errors

### Out of Scope
- Sales reports / analytics (deferred to Sprint 10)
- Export to PDF/Excel (deferred to Sprint 10)
- Void vs. cancel distinction (all cancellations treated as void for MVP)

---

## US-8.2 — Inventory Management UI

**Priority:** P0 Critical | **Story Points:** 8 SP
**Epic:** EPIC-POS-001

### User Story
As an inventory manager,
I want to view current stock levels per warehouse, make manual adjustments, and see the transaction history,
So that I can keep the product catalog accurate and respond to low-stock situations before they affect sales.

### Acceptance Criteria

**Scenario 1: View stock dashboard**
Given the inventory manager navigates to `/inventario`
When the page loads
Then a product table shows each product's current stock, minimum stock level, and a color-coded status indicator (green = adequate, yellow = low stock, red = out of stock)

**Scenario 2: Low-stock alert visual**
Given a product has stock quantity at or below its `MinStockLevel`
When the inventory list is displayed
Then that row is highlighted in amber and a "Stock Bajo" chip is shown next to the product name

**Scenario 3: Create stock adjustment — increase**
Given the inventory manager selects a product and clicks "Ajustar Stock"
When they choose "Incremento", enter a quantity (e.g., 10), select reason "Corrección de conteo", and confirm
Then the product's stock increases by 10, an `InventoryTransaction` record is created with the user ID, timestamp, before/after quantities, and reason code

**Scenario 4: Create stock adjustment — decrease**
Given the inventory manager selects a product and clicks "Ajustar Stock"
When they choose "Disminución" and enter a quantity greater than current stock
Then an error message "La cantidad no puede ser mayor al stock actual" is shown and the adjustment is rejected

**Scenario 5: View inventory transaction history**
Given the inventory manager clicks "Ver Historial" for a product
When the history panel opens
Then all `InventoryTransaction` records for that product are listed chronologically with: date, type (sale/adjustment/return), quantity change, reason, and user name

**Scenario 6: Warehouse filter**
Given the tenant has multiple warehouses (future state, for now single "Almacén Principal")
When the inventory page loads
Then a warehouse selector dropdown is shown (even if only one warehouse exists) for forward compatibility

**Scenario 7: Multi-tenancy isolation**
Given inventory manager is logged into Tenant A
When viewing or adjusting inventory
Then only Tenant A's `InventoryItem` and `Warehouse` records are accessible

### Technical Notes
- New entities to consider: `InventoryTransaction` entity (if not already created in Sprint 7 — confirm in codebase)
  - Fields: `Id`, `TenantId`, `InventoryItemId`, `TransactionType` (enum: Sale, Adjustment, Return, Correction), `QuantityBefore`, `QuantityAfter`, `QuantityChange`, `ReasonCode`, `Notes`, `CreatedBy`, `CreatedAt`
- New CQRS: `GetInventoryItemsQuery` (with warehouse and low-stock filters), `AdjustStockCommand`, `GetInventoryTransactionsQuery`
- New endpoints: `GET /api/v1/inventory`, `POST /api/v1/inventory/adjustments`, `GET /api/v1/inventory/{productId}/transactions`
- New Blazor pages: `Pages/Inventory/InventoryList.razor`, components: `StockAdjustmentDialog.razor`, `InventoryTransactionHistory.razor`
- Reason codes: `Damaged`, `Lost`, `Stolen`, `Found`, `CountCorrection`, `Other`
- Minimum migration: only if `InventoryTransaction` table was not created in Sprint 7

### Task Breakdown
- [ ] **TASK-1:** Verify Sprint 7 migration — confirm whether `inventory_transactions` table exists; if not, create `InventoryTransaction` entity + EF config + migration `AddInventoryTransactions`
- [ ] **TASK-2:** Implement `GetInventoryItemsQuery` + handler (filters: warehouseId, lowStockOnly, search)
- [ ] **TASK-3:** Implement `AdjustStockCommand` + handler + `AdjustStockCommandValidator` (validates: positive quantity, sufficient stock for decreases, reason required)
- [ ] **TASK-4:** Implement `GetInventoryTransactionsQuery` + handler (filter by productId, paginated)
- [ ] **TASK-5:** Add endpoints: `GET /api/v1/inventory`, `POST /api/v1/inventory/adjustments`, `GET /api/v1/inventory/{productId}/transactions` in `InventoryEndpoints.cs`
- [ ] **TASK-6:** Create `InventoryList.razor` — table with color-coded stock status, filter bar (warehouse, low-stock toggle, search), "Ajustar Stock" action per row
- [ ] **TASK-7:** Create `StockAdjustmentDialog.razor` — MudDialog with increase/decrease toggle, quantity input, reason dropdown, notes field
- [ ] **TASK-8:** Create `InventoryTransactionHistory.razor` (panel or page) — chronological transaction list per product
- [ ] **TASK-9:** Add "Inventario" nav link to `NavMenu.razor`
- [ ] **TASK-10:** Add ~35 es-MX localization keys (`Inventory`, `StockAdjustment`, `AdjustmentReason`, `LowStock`, `OutOfStock`, `TransactionHistory`, reason code labels, etc.)
- [ ] **TASK-11:** Unit tests for `AdjustStockCommand` handler — validates decrease-below-zero guard, validates reason required (>70% coverage)

### Dependencies
- [x] US-7.2: `Warehouse`, `InventoryItem` entities exist with `QuantityOnHand` and `MinStockLevel`
- [x] US-7.3: `PosSearchService` already decrements inventory on `CompleteSaleCommand`

### Definition of Done
- [ ] Backend implemented (CQRS handlers, API endpoints, migration if needed)
- [ ] Frontend implemented (InventoryList.razor, StockAdjustmentDialog.razor, history view)
- [ ] All UI text in Spanish via `IStringLocalizer`
- [ ] Feature is demo-able (manager can view stock, adjust, and see audit trail)
- [ ] Unit tests passing (>70% coverage on AdjustStockCommand)
- [ ] Zero compilation errors

### Out of Scope
- Inventory import from Excel/CSV
- Barcode-based stock count (physical inventory count workflow)
- Multi-warehouse transfers (single warehouse assumed for MVP)

---

## US-8.3 — Receipt and Ticket Generation

**Priority:** P1 High | **Story Points:** 5 SP
**Epic:** EPIC-POS-001

### User Story
As a cashier,
I want the system to generate a printable PDF receipt after each completed sale,
So that customers have proof of purchase and the business has a professional paper trail without needing external tools.

### Acceptance Criteria

**Scenario 1: Generate PDF receipt from POS**
Given the cashier has just completed a sale in the POS
When the payment is confirmed and the sale is marked as `Completed`
Then a "Imprimir Ticket" button appears in the payment confirmation screen that triggers a PDF download

**Scenario 2: PDF receipt content**
Given a receipt is generated for a completed sale
When the PDF is opened
Then it contains: business name and RFC (from tenant), sale date (dd/MM/yyyy HH:mm), folio number, list of items (name, quantity, unit price, line total), subtotal, IVA amount, total, payment method, and change (for cash payments)

**Scenario 3: Generate receipt from sales history**
Given the manager is on the sales history page
When they click the "Ticket" icon on any completed sale row
Then a PDF is downloaded for that sale

**Scenario 4: Fallback when browser printing unavailable**
Given the PDF cannot be rendered in the browser
When the download fails
Then an error snackbar "No se pudo generar el ticket. Intente de nuevo." is shown

**Scenario 5: Receipt currency formatting**
Given amounts are displayed on the receipt
When the PDF is rendered
Then all amounts use Mexican peso format `$1,234.56 MXN` with `CultureInfo("es-MX")`

### Technical Notes
- Library options: `QuestPDF` (recommended — MIT licensed, .NET native) or `iText7`
- New command: `GenerateSaleReceiptQuery` returns `byte[]` (PDF binary)
- New endpoint: `GET /api/v1/sales/{id}/receipt` returns `application/pdf`
- Blazor: use `IJSRuntime` to trigger browser download of PDF bytes
- PDF template should be simple (no logo for MVP) but styled consistently
- Thermal printer support (58mm/80mm ESC/POS) is out of scope for this story — deferred to future sprint
- Add `QuestPDF` NuGet package to `Corelio.Infrastructure`

### Task Breakdown
- [ ] **TASK-1:** Add `QuestPDF` package to `Corelio.Infrastructure` (`dotnet add package QuestPDF`)
- [ ] **TASK-2:** Create `SaleReceiptDocument.cs` in `Infrastructure/CFDI/` (or `Infrastructure/Documents/`) — QuestPDF document class with receipt layout
- [ ] **TASK-3:** Create `ISaleReceiptService` interface in Application layer + `SaleReceiptService` implementation in Infrastructure
- [ ] **TASK-4:** Add `GenerateSaleReceiptQuery` + handler that returns `byte[]`
- [ ] **TASK-5:** Add endpoint `GET /api/v1/sales/{id}/receipt` to `SaleEndpoints.cs` returning `application/pdf` file result
- [ ] **TASK-6:** Add Blazor HTTP service method `DownloadReceiptAsync(Guid saleId)` using `IJSRuntime` for browser file download
- [ ] **TASK-7:** Add "Imprimir Ticket" button to `PaymentPanel.razor` (shown after successful sale completion)
- [ ] **TASK-8:** Add "Ticket" icon button to `SaleList.razor` table rows
- [ ] **TASK-9:** Add ~10 es-MX localization keys (`PrintTicket`, `DownloadTicket`, `TicketGenerationError`, `GeneratingTicket`)
- [ ] **TASK-10:** Register `ISaleReceiptService` in `DependencyInjection.cs`

### Dependencies
- [x] US-7.4: `SaleEndpoints.cs` exists
- [x] US-7.5: `PaymentPanel.razor` exists
- [ ] US-8.1: `SaleList.razor` must exist for the history ticket button

### Definition of Done
- [ ] Backend implemented (service, query handler, API endpoint)
- [ ] Frontend implemented (download triggered from POS and from history)
- [ ] PDF contains all required fields with es-MX formatting
- [ ] All UI text in Spanish via `IStringLocalizer`
- [ ] Feature is demo-able (cashier can complete sale and download receipt)
- [ ] Zero compilation errors

### Out of Scope
- Thermal/ESC-POS printer direct output
- Logo on receipt
- Email delivery of receipt (deferred to Sprint 9 CFDI email delivery)

---

## US-8.4 — Quote Management

**Priority:** P1 High | **Story Points:** 3 SP
**Epic:** EPIC-POS-001

### User Story
As a seller,
I want to create quotes for customers from the POS and convert them to sales when the customer decides to purchase,
So that I can provide formal estimates without immediately reducing inventory or processing payment.

### Acceptance Criteria

**Scenario 1: Create a quote from POS**
Given the seller has items in the POS cart
When they click "Guardar como Cotización" instead of proceeding to payment
Then a sale with `SaleType = Quote` is saved with status `Draft`, no inventory is deducted, and the folio is shown in a confirmation message

**Scenario 2: View open quotes list**
Given the seller navigates to `/cotizaciones`
When the page loads
Then all open (non-expired, non-converted) quotes for the tenant are listed with columns: Folio, Date, Customer, Total, Expiry Date, Status

**Scenario 3: Convert quote to sale**
Given the seller opens a quote and clicks "Convertir a Venta"
When the confirmation is accepted
Then the POS screen pre-loads the quote items in the cart, `SaleType` changes to `Sale`, inventory is reserved, and the cashier can proceed to payment

**Scenario 4: Quote expiration**
Given a quote was created more than 30 days ago (default expiry — tenant-configurable in future)
When the quote is displayed in the list
Then it shows status "Vencida" and the "Convertir a Venta" button is disabled

**Scenario 5: Cancel a quote**
Given the seller selects an open quote
When they click "Cancelar Cotización" and confirm
Then the quote status changes to `Cancelled` and it no longer appears in the open quotes list

### Technical Notes
- `Sale` entity already has `SaleType` enum (Sale, Invoice, Quote) from Sprint 7
- New command: `CreateQuoteCommand` (same as `CreateSaleCommand` but `SaleType = Quote`, no inventory deduction)
- New command: `ConvertQuoteToSaleCommand` (changes SaleType, triggers inventory reservation)
- New query: `GetQuotesQuery` (filter by status: Open, Expired, Cancelled, Converted)
- New Blazor page: `Pages/Sales/QuoteList.razor`
- Endpoint additions to `SaleEndpoints.cs`: `GET /api/v1/sales?type=quote`, `POST /api/v1/sales/{id}/convert`
- Quote expiry: add `ExpiresAt` nullable field to `Sale` entity if not already present; migration if needed

### Task Breakdown
- [ ] **TASK-1:** Verify `Sale.SaleType` enum includes `Quote` and `Sale.ExpiresAt` nullable field exists; create migration `AddSaleExpiresAt` if needed
- [ ] **TASK-2:** Create `CreateQuoteCommand` + handler (no inventory deduction, sets `SaleType = Quote`, sets `ExpiresAt = now + 30 days`)
- [ ] **TASK-3:** Create `ConvertQuoteToSaleCommand` + handler (validates quote is open and not expired, deducts inventory, changes type to Sale)
- [ ] **TASK-4:** Update `GetSalesQuery` to filter by `SaleType` and expose open quotes
- [ ] **TASK-5:** Add "Guardar como Cotización" button to `PaymentPanel.razor` or POS cart toolbar
- [ ] **TASK-6:** Create `QuoteList.razor` page at `/cotizaciones` — table with expiry indicator, convert and cancel actions
- [ ] **TASK-7:** Add "Cotizaciones" nav link to `NavMenu.razor`
- [ ] **TASK-8:** Add ~15 es-MX localization keys (`Quotes`, `QuoteList`, `CreateQuote`, `ConvertToSale`, `QuoteExpired`, `SaveAsQuote`, etc.)

### Dependencies
- [x] US-7.2: `Sale` entity with `SaleType` enum exists
- [x] US-7.5: POS cart and `PaymentPanel.razor` exist
- [ ] US-8.1: `SaleList.razor` pattern to follow for `QuoteList.razor`

### Definition of Done
- [ ] Backend implemented (commands, query, API endpoint additions)
- [ ] Frontend implemented (Quote button in POS, QuoteList.razor)
- [ ] All UI text in Spanish via `IStringLocalizer`
- [ ] Feature is demo-able (seller can save quote, view list, and convert to sale)
- [ ] Zero compilation errors

### Out of Scope
- Quote PDF generation (separate from receipt; deferred)
- Stock reservation on quote creation (only reserved on conversion)
- Quote templates

---

## Sprint 8 — Backlog Summary

| Story | Priority | SP | Status |
|-------|----------|----|--------|
| US-8.1: Sales History & Management UI | P0 Critical | 5 | 🔵 Not Started |
| US-8.2: Inventory Management UI | P0 Critical | 8 | 🔵 Not Started |
| US-8.3: Receipt & Ticket Generation | P1 High | 5 | 🔵 Not Started |
| US-8.4: Quote Management | P1 High | 3 | 🔵 Not Started |
| **[TECH DEBT] TD-3.1.A: Product handler unit tests** | P1 High | **~2** | 🔵 Pending |
| **Total** | | **~23** | |

> Note: TD-3.1.A is recommended to be folded into Sprint 8 given active product domain work. It adds approximately 2 SP of effort. Adjust sprint capacity accordingly.

---

---

# Sprint 9 — CFDI Integration 🔵

**Sprint Goal:** Implement SAT-compliant CFDI 4.0 invoice generation, stamping via PAC (Finkel), and invoice management UI — allowing accountants to generate, view, and cancel electronic invoices directly within Corelio.

**Estimated SP:** 34
**Status:** Not Started
**Prerequisites:** Sprint 8 complete (receipt generation via QuestPDF already in place), Customer entity has RFC/CFDI fields (Sprint 7)

---

## US-9.1 — CFDI Domain Model & Infrastructure

**Priority:** P0 Critical | **Story Points:** 8 SP

### User Story
As a developer,
I want the CFDI invoice domain model, database schema, and repository infrastructure in place,
So that all subsequent CFDI features build on a solid, SAT-compliant data foundation.

### Acceptance Criteria
- `Invoice` and `InvoiceItem` entities created in `Corelio.Domain/Entities/CFDI/`
- `InvoiceStatus` enum: `Draft`, `Stamped`, `Cancelled`
- `cfdi_invoices` and `cfdi_invoice_items` tables created via EF Core migration
- `TenantId` on both tables — multi-tenancy query filters applied
- `IInvoiceRepository` interface in Application layer
- `InvoiceRepository` implementation in Infrastructure
- `TenantConfiguration` entity updated with: `CfdiCertificateId` (Key Vault ref), `CfdiKeyVaultUrl` (optional), `CfdiCertificateExpiresAt`, `IssuerRfc`, `IssuerName`, `IssuerTaxRegime`, `IssuerPostalCode`, `DefaultCfdiSerie`
- Migration applies successfully to existing database
- Registered in both DI methods in `DependencyInjection.cs`

### Technical Notes
- Follow schema from `docs/planning/04-cfdi-integration-specification.md` exactly
- `CONSTRAINT uk_cfdi_tenant_serie_folio UNIQUE (tenant_id, serie, folio)` must be enforced
- `taxes` on `InvoiceItem` stored as `JSONB` (EF Core value conversion to `string`)
- Add to `ApplicationDbContext.ApplyTenantQueryFilters()`: `modelBuilder.Entity<Invoice>().HasQueryFilter(...)` and same for `InvoiceItem`
- `folio` is auto-incremented per series per tenant — implement sequence or `MAX(folio) + 1` in handler

### Task Breakdown
- [ ] **TASK-1:** Create `Invoice` entity with all fields from spec
- [ ] **TASK-2:** Create `InvoiceItem` entity with JSONB taxes field
- [ ] **TASK-3:** Create `InvoiceStatus`, `InvoiceType` enums
- [ ] **TASK-4:** Update `TenantConfiguration` entity with CFDI fields (CfdiCertificateId, IssuerRfc, IssuerName, IssuerTaxRegime, IssuerPostalCode, DefaultCfdiSerie)
- [ ] **TASK-5:** Create EF Core configurations for `Invoice`, `InvoiceItem`, `TenantConfiguration` (update)
- [ ] **TASK-6:** Update `ApplicationDbContext` — add `DbSet<Invoice>` and `DbSet<InvoiceItem>`, add query filters
- [ ] **TASK-7:** Create `IInvoiceRepository` interface and `InvoiceRepository` implementation
- [ ] **TASK-8:** Create migration `AddCfdiInvoiceSchema` and verify it applies without errors
- [ ] **TASK-9:** Register repository in `DependencyInjection.cs` (both methods)

### Dependencies
- [x] Sprint 7: Customer entity with RFC/CFDI fields exists
- [x] Sprint 7: Sale entity exists for linking invoices to sales

---

## US-9.2 — CFDI XML Generation & Digital Signature

**Priority:** P0 Critical | **Story Points:** 8 SP

### User Story
As an accountant,
I want the system to generate a valid CFDI 4.0 XML document from a completed sale and sign it with the tenant's CSD certificate,
So that the invoice is ready for PAC stamping with no manual XML editing.

### Acceptance Criteria
- `CFDIXMLGenerator` service generates valid CFDI 4.0 XML matching the SAT schema (`cfdv40.xsd`)
- Generated XML validates against the official SAT XSD without errors
- `ICertificateService` / `AzureKeyVaultCertificateService` loads CSD from Azure Key Vault using `DefaultAzureCredential` (Managed Identity — no credentials in code)
- Certificate upload endpoint creates a properly named secret in Key Vault (`csd-tenant-{tenantId:N}`)
- Digital signature (cadena original + sello) is applied correctly per SAT specification
- `GenerateInvoiceCommand` creates an `Invoice` with status `Draft` from a completed `Sale`
- All monetary amounts in XML have exactly 2 decimal places
- RFC format validated against regex `^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$` before XML generation
- Unit tests verify correct XML structure for at least 3 invoice scenarios (standard, with discount, cash/card payment)

### Technical Notes
- File: `Infrastructure/CFDI/CFDIXMLGenerator.cs` — follow spec in `docs/planning/04-cfdi-integration-specification.md`
- File: `Infrastructure/CFDI/AzureKeyVaultCertificateService.cs` — follow spec exactly
- NuGet packages needed: `Azure.Identity`, `Azure.Security.KeyVault.Certificates`, `Azure.Security.KeyVault.Secrets`
- Configure `Azure:KeyVault:Url` in `appsettings.json`; use user secrets for local dev
- Certificate upload: `POST /api/v1/tenants/cfdi/certificate` — accepts multipart form with `.pfx` file
- SAT namespace: `http://www.sat.gob.mx/cfd/4`
- CFDI fields auto-populated from Sale: Folio (auto-increment per series), Fecha (now), FormaPago (from Payment method), MetodoPago (PUE), Moneda (MXN), LugarExpedicion (from TenantConfiguration.IssuerPostalCode)

### Task Breakdown
- [ ] **TASK-1:** Add NuGet packages `Azure.Identity`, `Azure.Security.KeyVault.Certificates`, `Azure.Security.KeyVault.Secrets` to `Corelio.Infrastructure`
- [ ] **TASK-2:** Create `ICertificateService` interface in Application layer
- [ ] **TASK-3:** Implement `AzureKeyVaultCertificateService` with `LoadCertificateAsync`, `UploadCertificateAsync`, `GetCertificateMetadataAsync`
- [ ] **TASK-4:** Create `CFDIXMLGenerator` — generates CFDI 4.0 XML using `System.Xml.Linq`
- [ ] **TASK-5:** Add XSD validation step in generator (download `cfdv40.xsd` from SAT and embed as resource)
- [ ] **TASK-6:** Create `GenerateInvoiceCommand` + handler — creates `Invoice` draft from `SaleId`, calculates folio (MAX + 1 within tenant/series), maps sale items to invoice items with SAT product/unit codes
- [ ] **TASK-7:** Add certificate upload endpoint `POST /api/v1/tenants/cfdi/certificate` to `TenantThemeEndpoints.cs` or a new `TenantEndpoints.cs`
- [ ] **TASK-8:** Register `ICertificateService` → `AzureKeyVaultCertificateService` in `DependencyInjection.cs`
- [ ] **TASK-9:** Unit tests for `CFDIXMLGenerator` — 3+ test scenarios verifying XML node structure and attribute values

### Dependencies
- [ ] US-9.1: Invoice domain model must be complete
- [x] Sprint 7: Customer entity with RFC fields exists

---

## US-9.3 — PAC Integration & Invoice Stamping

**Priority:** P0 Critical | **Story Points:** 8 SP

### User Story
As an accountant,
I want the system to send generated CFDI XML to the PAC (Finkel) for stamping and store the UUID and fiscal seal,
So that invoices are legally valid per SAT and customers receive a stamped copy.

### Acceptance Criteria
- `IPACProvider` interface with `StampAsync` and `CancelAsync` methods
- `FinkelPACProvider` implementation connects to Finkel sandbox (`https://sandbox.finkel.com.mx/api`) in development
- Stamping workflow: Generate XML → Load cert from Key Vault → Sign → POST to PAC → Store UUID + seals on Invoice
- `StampInvoiceCommand` changes `Invoice.Status` to `Stamped` and persists all stamp fields (Uuid, StampDate, SatCertificateNumber, PacStampSignature, SatStampSignature, QrCodeData)
- PAC failures: retry up to 3 times with exponential backoff (100ms, 400ms, 1600ms)
- `CancelInvoiceCommand` handles SAT cancellation reasons 01-04 and enforces 72-hour cancellation window
- `ICFDIService` orchestrates the full generate → sign → stamp workflow
- Integration test uses PAC sandbox to stamp a test invoice with RFC `XAXX010101000`

### Technical Notes
- File: `Infrastructure/CFDI/FinkelPACProvider.cs` — follow spec in `docs/planning/04-cfdi-integration-specification.md`
- File: `Infrastructure/CFDI/CFDIService.cs` — orchestrates full workflow
- Configuration (user secrets): `Finkel:ApiUrl`, `Finkel:ApiKey`, `Finkel:TestMode`
- Retry logic: use `Polly` NuGet package (`dotnet add package Polly.Extensions.Http`)
- The `using var certificate = ...` pattern MUST be used to ensure certificate disposal after stamping
- PDF generation of stamped invoice (with QR code) is included in this story

### Task Breakdown
- [ ] **TASK-1:** Add `Polly.Extensions.Http` NuGet to `Corelio.Infrastructure`
- [ ] **TASK-2:** Create `IPACProvider` interface in Application layer with `StampAsync` and `CancelAsync`
- [ ] **TASK-3:** Implement `FinkelPACProvider` with retry policy (Polly) and error mapping
- [ ] **TASK-4:** Create `ICFDIService` interface in Application layer
- [ ] **TASK-5:** Implement `CFDIService` — orchestrates: GenerateDraft → LoadCert → GenerateXML → Sign → Stamp → Persist
- [ ] **TASK-6:** Create `StampInvoiceCommand` + handler
- [ ] **TASK-7:** Create `CancelInvoiceCommand` + handler (validates 72-hour window, reason code 01-04)
- [ ] **TASK-8:** Implement PDF generation with QR code for stamped invoice (extend `SaleReceiptService` or create `InvoicePdfService`)
- [ ] **TASK-9:** Register `IPACProvider`, `ICFDIService` in `DependencyInjection.cs`
- [ ] **TASK-10:** Add user secrets config: `Finkel:ApiUrl`, `Finkel:ApiKey`
- [ ] **TASK-11:** Integration test: stamp a test invoice using Finkel sandbox and assert UUID is returned

### Dependencies
- [ ] US-9.1: Invoice domain model
- [ ] US-9.2: CFDIXMLGenerator and AzureKeyVaultCertificateService

---

## US-9.4 — CFDI Invoice UI

**Priority:** P0 Critical | **Story Points:** 10 SP

### User Story
As an accountant,
I want a Blazor UI to generate invoices from completed sales, view invoice status, download XML/PDF, cancel invoices, and trigger email delivery to customers,
So that the entire CFDI workflow is self-service and accessible without technical assistance.

### Acceptance Criteria
- Invoice generation form: select sale, verify customer RFC/CFDI data, adjust CFDI use code if needed, click "Generar Factura"
- Invoice list at `/facturas`: paginated table with columns Folio, Serie, UUID (partial), Customer RFC, Total, Status (borrador/timbrado/cancelado), Date
- Invoice detail: shows all stamped fields, download XML button, download PDF button, email delivery button, cancel button (only if stamped and within 72 hours)
- Email delivery: triggers email to customer's invoice email with XML and PDF attached
- Tenant CFDI settings page at `/settings/cfdi`: upload CSD certificate form, show certificate expiry date, configure issuer RFC/name/tax regime/postal code/default serie
- All status labels, error messages, and form labels in Spanish (es-MX)
- Certificate upload shows validation error if file is not a valid `.pfx` with correct password
- Invoice cancellation requires selecting reason code (01-04) from a labeled dropdown

### Technical Notes
- New Blazor pages: `Pages/CFDI/InvoiceList.razor`, `Pages/CFDI/InvoiceDetail.razor`, `Pages/CFDI/GenerateInvoiceForm.razor`, `Pages/Settings/CfdiSettings.razor`
- New HTTP services in BlazorApp: `ICfdiHttpService`, `CfdiHttpService`
- New endpoints in `CfdiEndpoints.cs`: `POST /api/v1/cfdi/invoices` (generate), `GET /api/v1/cfdi/invoices` (list), `GET /api/v1/cfdi/invoices/{id}` (detail), `POST /api/v1/cfdi/invoices/{id}/stamp`, `POST /api/v1/cfdi/invoices/{id}/cancel`, `GET /api/v1/cfdi/invoices/{id}/xml`, `GET /api/v1/cfdi/invoices/{id}/pdf`, `POST /api/v1/cfdi/invoices/{id}/send-email`
- Certificate settings: `GET /api/v1/tenants/cfdi/settings`, `PUT /api/v1/tenants/cfdi/settings`, `POST /api/v1/tenants/cfdi/certificate` (upload)
- Required permissions: `cfdi.generate`, `cfdi.cancel`, `settings.cfdi` (add these to seed data)

### Task Breakdown
- [ ] **TASK-1:** Create `CfdiEndpoints.cs` with all 8 invoice endpoints
- [ ] **TASK-2:** Add CFDI settings endpoints to tenant endpoints file
- [ ] **TASK-3:** Register `CfdiEndpoints` in `EndpointExtensions.cs`
- [ ] **TASK-4:** Create `ICfdiHttpService` + `CfdiHttpService` in BlazorApp
- [ ] **TASK-5:** Create `InvoiceList.razor` — paginated table with status color chips, action buttons
- [ ] **TASK-6:** Create `GenerateInvoiceForm.razor` — sale selector, customer RFC verification, CFDI use dropdown (SAT catalog), submit
- [ ] **TASK-7:** Create `InvoiceDetail.razor` — full stamped invoice view, download XML/PDF, send email, cancel with reason dropdown
- [ ] **TASK-8:** Create `CfdiSettings.razor` — certificate upload with drag-and-drop, certificate expiry display, issuer data form
- [ ] **TASK-9:** Add "Facturas" and "Configuración CFDI" links to `NavMenu.razor`
- [ ] **TASK-10:** Add ~50 es-MX localization keys (CFDI-specific terminology: `Invoice`, `Stamp`, `CancellationReason`, `TaxRegime`, `CfdiUse`, `FiscalFolio`, etc.)

### Dependencies
- [ ] US-9.1: Invoice domain model
- [ ] US-9.2: XML generator + certificate service
- [ ] US-9.3: PAC integration + CFDIService

---

## Sprint 9 — Backlog Summary

| Story | Priority | SP | Status |
|-------|----------|----|--------|
| US-9.1: CFDI Domain Model & Infrastructure | P0 Critical | 8 | 🔵 Not Started |
| US-9.2: CFDI XML Generation & Digital Signature | P0 Critical | 8 | 🔵 Not Started |
| US-9.3: PAC Integration & Invoice Stamping | P0 Critical | 8 | 🔵 Not Started |
| US-9.4: CFDI Invoice UI | P0 Critical | 10 | 🔵 Not Started |
| **Total** | | **34** | |

> Risk: US-9.2 and US-9.3 carry the highest technical risk — Azure Key Vault and PAC sandbox access must be configured before work begins. Recommend spiking on Key Vault connectivity on Sprint 8 Day 1 of Sprint 9 planning.

---

---

# Sprint 10 — Testing, QA & Production Deployment 🔵

**Sprint Goal:** Achieve production readiness by completing test coverage across all modules, conducting user acceptance testing, optimizing performance, provisioning production infrastructure, and deploying the MVP to production with the first pilot tenant operational.

**Estimated SP:** 34
**Status:** Not Started
**Prerequisites:** Sprints 1-9 complete, all [TECH DEBT] items resolved

---

## US-10.1 — Test Coverage Completion

**Priority:** P0 Critical | **Story Points:** 8 SP

### User Story
As the development team,
I want all application layers to have >70% unit test coverage and all critical paths covered by integration tests,
So that we can deploy to production with confidence and quickly detect regressions.

### Acceptance Criteria
- Unit test coverage for `Corelio.Application` layer is >70% (measured by `dotnet test --collect:"XPlat Code Coverage"`)
- Unit test coverage for `Corelio.Domain` layer is >80%
- All [TECH DEBT] items from Sprint 3 are resolved:
  - TD-3.1.A: Product CQRS handler unit tests complete
  - TD-3.1.B: Product management integration tests with multi-tenancy isolation
  - TD-3.1.C: E2E manual test scenarios for Product CRUD documented and executed
- Integration tests exist for: Authentication endpoints, Product endpoints, Pricing module, Sales/POS endpoints, CFDI invoice generation (end-to-end with PAC sandbox)
- All 254+ existing tests continue to pass (no regressions)
- Zero compilation warnings in Release configuration

### Task Breakdown
- [ ] **TASK-1:** Resolve TD-3.1.A — unit tests for `CreateProductCommand`, `UpdateProductCommand`, `DeleteProductCommand`, `GetProductsQuery`, `GetProductByIdQuery` handlers
- [ ] **TASK-2:** Resolve TD-3.1.B — integration tests for Product endpoints using Testcontainers (tenant isolation, CRUD operations)
- [ ] **TASK-3:** Resolve TD-3.1.C — document and execute E2E test scenarios for Product management via Blazor UI
- [ ] **TASK-4:** Run coverage report — identify gaps in Application layer coverage
- [ ] **TASK-5:** Add unit tests for Sprint 8 handlers not yet covered (AdjustStockCommand, GetSalesQuery, etc.)
- [ ] **TASK-6:** Add integration tests for CFDI generation workflow (using PAC sandbox)
- [ ] **TASK-7:** Run full test suite — fix any flaky or broken tests
- [ ] **TASK-8:** Verify zero warnings in Release build (`dotnet build --configuration Release`)

### Dependencies
- All prior sprints complete

---

## US-10.2 — Performance Optimization

**Priority:** P1 High | **Story Points:** 5 SP

### User Story
As a hardware store cashier,
I want the POS product search to return results in under 300ms and a complete sale to process in under 3 seconds,
So that checkout is fast enough to serve customers during peak hours without frustrating delays.

### Acceptance Criteria
- POS product search (`GET /api/v1/pos/search`) returns results in <300ms (95th percentile) under normal load
- Sale completion (`POST /api/v1/sales/{id}/complete`) completes in <3 seconds
- Redis caching implemented for:
  - Tenant pricing configuration (TTL: 30 minutes — already in place from Sprint 6)
  - Product search results (TTL: 5 minutes with cache invalidation on product update)
- `AsNoTracking()` verified on all read-only queries
- EF Core compiled queries applied to the 3 most frequent hot-path queries (product search, get inventory items, get sale list)
- Load test: 100 concurrent users completing checkout — no errors, 95th percentile <3s

### Task Breakdown
- [ ] **TASK-1:** Add Redis cache layer to `PosSearchService` for product search results (key: `{tenantId}:search:{query}`, TTL 5 min)
- [ ] **TASK-2:** Add cache invalidation in `CreateProductCommand` and `UpdateProductCommand` handlers
- [ ] **TASK-3:** Apply EF Core compiled queries to `PosSearchService.SearchProductsAsync`, `GetSalesQuery`, `GetInventoryItemsQuery`
- [ ] **TASK-4:** Audit all CQRS query handlers — ensure `AsNoTracking()` is applied everywhere reads don't need tracking
- [ ] **TASK-5:** Add database indexes (review missing indexes against query patterns): `idx_products_sku`, `idx_products_barcode`, `idx_sales_tenant_date`, `idx_inventory_items_warehouse`
- [ ] **TASK-6:** Run load test with 100 concurrent users — document results and confirm targets met

### Dependencies
- All feature sprints complete

---

## US-10.3 — User Acceptance Testing (UAT)

**Priority:** P0 Critical | **Story Points:** 5 SP

### User Story
As a stakeholder,
I want to run through all key workflows in a staging environment with realistic sample data,
So that I can approve the MVP for production launch with confidence.

### Acceptance Criteria
- Demo tenant "Ferretería Demo S.A. de C.V." created in staging with:
  - 50+ sample products with pricing tiers configured
  - 5 sample customers with CFDI data
  - 30 days of historical sales data
  - "Almacén Principal" warehouse with realistic inventory levels
- UAT test script covers all 7 core workflows:
  1. Register and log in as a new user
  2. Configure pricing tiers at /settings/pricing
  3. Create a product and set multi-tier pricing
  4. Complete a POS sale with cash payment and download receipt
  5. View sales history and cancel a sale
  6. Adjust inventory stock manually with a reason code
  7. Generate and stamp a CFDI invoice for a completed sale
- All UAT feedback items documented, prioritized, and critical bugs fixed before go-live
- Zero P0 (showstopper) bugs at end of UAT cycle

### Task Breakdown
- [ ] **TASK-1:** Create database seed script for UAT demo data (50 products, 5 customers, 30 days sales history)
- [ ] **TASK-2:** Write UAT test script document covering the 7 workflows above with step-by-step instructions and expected outcomes
- [ ] **TASK-3:** Deploy to staging environment and execute UAT
- [ ] **TASK-4:** Document all bugs found — categorize by P0/P1/P2
- [ ] **TASK-5:** Fix all P0 bugs
- [ ] **TASK-6:** Stakeholder sign-off on production readiness

---

## US-10.4 — Production Infrastructure Provisioning

**Priority:** P0 Critical | **Story Points:** 8 SP

### User Story
As the DevOps engineer,
I want all production infrastructure provisioned, configured, and verified,
So that the MVP can be deployed to a stable, secure, and monitored environment.

### Acceptance Criteria
- Azure or DigitalOcean resources provisioned:
  - Managed PostgreSQL 16 instance (production database)
  - Redis cache service
  - App Service or container hosting for WebAPI and BlazorApp
  - Azure Key Vault for CSD certificates
- SSL/TLS certificate configured for the production domain (`corelio.com.mx` or equivalent)
- Environment variables configured for all secrets (no hardcoded values)
- Automatic database migration on startup verified in production
- Application Insights or equivalent monitoring configured — dashboards for error rates, response times, active users
- Backup strategy: daily database backups with 30-day retention verified
- Production deployment runbook document written and reviewed

### Task Breakdown
- [ ] **TASK-1:** Provision managed PostgreSQL 16 instance (dev → staging → production pathway)
- [ ] **TASK-2:** Provision Redis cache service
- [ ] **TASK-3:** Provision Azure Key Vault and configure Managed Identity access for the API app service
- [ ] **TASK-4:** Configure SSL certificate for production domain
- [ ] **TASK-5:** Set all production environment variables/secrets (Connection strings, JWT secret, Finkel API key, Key Vault URL)
- [ ] **TASK-6:** Configure Application Insights — connect to WebAPI and BlazorApp
- [ ] **TASK-7:** Configure PostgreSQL backup strategy and test restore procedure
- [ ] **TASK-8:** Write and review deployment runbook (step-by-step deployment, rollback procedure, on-call contacts)

---

## US-10.5 — Production Deployment & MVP Launch

**Priority:** P0 Critical | **Story Points:** 5 SP

### User Story
As the product team,
I want the MVP deployed to production and the first pilot tenant onboarded,
So that Corelio is live and generating real business value for its first customer.

### Acceptance Criteria
- CI/CD pipeline deploys `main` branch to production automatically on merge
- Smoke tests run automatically post-deployment (health check endpoints return 200, login works, POS product search returns results)
- First pilot tenant ("Ferretería López" or equivalent) onboarded:
  - Admin user created
  - Pricing tiers configured
  - Initial product catalog imported (if applicable)
  - CSD certificate uploaded to Key Vault
- MVP announcement ready (internal)
- Support process documented (who to contact for P0 bugs in production)

### Task Breakdown
- [ ] **TASK-1:** Configure GitHub Actions production deployment workflow (build → test → deploy on push to `main`)
- [ ] **TASK-2:** Add health check endpoints: `GET /health` (API) and `GET /health` (BlazorApp)
- [ ] **TASK-3:** Write automated smoke test script (bash or Playwright) for post-deployment verification
- [ ] **TASK-4:** Deploy WebAPI and BlazorApp to production
- [ ] **TASK-5:** Run full smoke test suite in production — verify all green
- [ ] **TASK-6:** Onboard first pilot tenant — create admin user, configure tenant settings
- [ ] **TASK-7:** Document support process for production incidents
- [ ] **TASK-8:** Update `CLAUDE.md` and `README.md` with production URLs and deployment notes

### Dependencies
- [ ] US-10.3: UAT sign-off complete
- [ ] US-10.4: Infrastructure provisioned and verified

---

## US-10.6 — User and Technical Documentation

**Priority:** P1 High | **Story Points:** 3 SP

### User Story
As a new tenant administrator,
I want clear documentation on how to set up and use Corelio,
So that I can onboard my team without requiring support from the development team.

### Acceptance Criteria
- User manual covers: login, product management, POS checkout, sales history, inventory adjustment, CFDI invoice generation
- Admin guide covers: tenant setup, user and role management, pricing tier configuration, CSD certificate upload
- API documentation (Scalar) is complete and accessible at `/scalar` in staging
- CFDI certificate setup guide documents the SAT process for obtaining a CSD, uploading to Key Vault, and verifying in Corelio
- Deployment runbook reviewed by a second team member and approved

### Task Breakdown
- [ ] **TASK-1:** Write user manual (Markdown or Word) — 7 core workflow walkthroughs with screenshots
- [ ] **TASK-2:** Write admin guide — tenant setup, user management, settings walkthrough
- [ ] **TASK-3:** Verify Scalar OpenAPI documentation is complete — all endpoints documented, request/response examples present
- [ ] **TASK-4:** Write CFDI certificate setup guide (SAT portal → download CSD → upload to Corelio)
- [ ] **TASK-5:** Final review of `CLAUDE.md` — update with any post-Sprint-10 architectural changes

---

## Sprint 10 — Backlog Summary

| Story | Priority | SP | Status |
|-------|----------|----|--------|
| US-10.1: Test Coverage Completion | P0 Critical | 8 | 🔵 Not Started |
| US-10.2: Performance Optimization | P1 High | 5 | 🔵 Not Started |
| US-10.3: User Acceptance Testing | P0 Critical | 5 | 🔵 Not Started |
| US-10.4: Production Infrastructure | P0 Critical | 8 | 🔵 Not Started |
| US-10.5: Production Deployment & MVP Launch | P0 Critical | 5 | 🔵 Not Started |
| US-10.6: User & Technical Documentation | P1 High | 3 | 🔵 Not Started |
| **Total** | | **34** | |

---

---

## Global Definition of Done

A user story is **NOT** complete until ALL of the following criteria are met. Backend-only implementations do not satisfy the DoD for user-facing features.

### Code Implementation
- [ ] Backend implemented (API endpoints, CQRS handlers, domain logic)
- [ ] Frontend implemented for all user-facing features (Blazor pages/components)
- [ ] Clean Architecture followed (no layer dependency violations)
- [ ] C# 14 features used (primary constructors, collection expressions, file-scoped namespaces)
- [ ] CQRS pattern followed (commands vs queries)
- [ ] Proper error handling with `Result<T>` pattern
- [ ] Input validation with FluentValidation

### User Experience
- [ ] Feature is demo-able via Blazor UI (no Postman/Swagger required)
- [ ] All UI text in Spanish (es-MX) via `IStringLocalizer`
- [ ] Resource files updated (`*.es-MX.resx`)
- [ ] Forms have validation with Spanish error messages
- [ ] Date format: `dd/MM/yyyy` | Currency: `$1,234.56 MXN`

### Testing
- [ ] Unit tests with >70% coverage on new Application layer code
- [ ] Integration tests for multi-tenancy isolation (Testcontainers)
- [ ] All existing tests continue to pass (254/254 baseline)

### Security
- [ ] Multi-tenancy enforced (`ITenantEntity` / query filters)
- [ ] Authorization checks on all API endpoints
- [ ] No hardcoded secrets

### Documentation & Deployment
- [ ] API documentation updated (Scalar/OpenAPI)
- [ ] This backlog file (`PRODUCT_BACKLOG.md`) updated with new status
- [ ] Code reviewed and approved
- [ ] Merged to `main` and deployed to staging

---

## Velocity Reference

| Sprint | SP Delivered | Duration | SP/Day |
|--------|-------------|----------|--------|
| Sprint 1 | 21 | 3 days | 7.0 |
| Sprint 2 | 34 | 2 days | 17.0 |
| Sprint 4 | 26 | 3 days | 8.7 |
| Sprint 5 | 3 | 1 day | 3.0 |
| Sprint 6 | 49 | 6 days | 8.2 |
| Sprint 7 | 34 | ~4 days | 8.5 |
| **Average** | | | **8.4 SP/day** |

> Sprint 2 outlier (17 SP/day) excluded from planning estimates — it consisted of pure backend work without frontend. Typical velocity with frontend included is **8-9 SP/day**.

---

## Related Documents

| Document | Location | Purpose |
|----------|----------|---------|
| Architecture Specification | `docs/planning/00-architecture-specification.md` | Clean Architecture + patterns |
| Database Schema Design | `docs/planning/01-database-schema-design.md` | PostgreSQL schema reference |
| API Specification | `docs/planning/02-api-specification.md` | Endpoint design standards |
| Multi-Tenancy Guide | `docs/planning/03-multi-tenancy-implementation-guide.md` | Tenant isolation patterns |
| CFDI Integration Spec | `docs/planning/04-cfdi-integration-specification.md` | CFDI 4.0 + PAC + Key Vault |
| Functional Specifications | `docs/planning/05-functional-specifications.md` | Module-by-module requirements |
| Project Timeline | `docs/planning/06-project-timeline.md` | Phase breakdown and milestones |
| Pricing Module Stories | `docs/backlog/Pricing-Module-User-Stories.md` | Sprint 6 detailed stories |
| Design System Stories | `docs/backlog/UI-UX-Design-System-User-Stories.md` | Sprint 4-5 detailed stories |
| Developer Guide | `CLAUDE.md` | Project conventions, commands, standards |
