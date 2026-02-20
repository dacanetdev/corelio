# Sprint 7: POS Backend & UI

**Goal:** A functional POS terminal — cashiers can search products, build a cart, accept payment, and complete a sale entirely within the Blazor UI.

**Duration:** ~4 days
**Status:** 🟢 Completed (100%)
**Started:** 2026-02-18
**Total Story Points:** 34 pts (US-7.1: 8, US-7.2: 8, US-7.3: 8, US-7.4: 5, US-7.5: 5)
**Completed:** 34/34 tasks (100%)
**PR:** #55 (merged to main)

---

## User Story 7.1: Customer Entity & UI
**As a cashier, I want to manage customer records and link them to sales so that invoices can be generated for the correct customer.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-7.1.1 | Create `Customer` entity (individual: name/CURP; business: RFC, tax regime) with CFDI preferences | `feature/US-7.1-customer-entity` | 🟢 | Inherits `TenantAuditableEntity` |
| TASK-7.1.2 | Create CQRS: `CreateCustomer`, `UpdateCustomer`, `DeleteCustomer`, `GetCustomers`, `GetCustomerById`, `SearchCustomers` | `feature/US-7.1-customer-entity` | 🟢 | FluentValidation on RFC format |
| TASK-7.1.3 | Create EF Core config for `Customer` (snake_case, query filter) | `feature/US-7.1-customer-entity` | 🟢 | |
| TASK-7.1.4 | Create `CustomerList.razor` — paginated table with search, add/edit/delete | `feature/US-7.1-customer-entity` | 🟢 | Design system components |
| TASK-7.1.5 | Create `CustomerForm.razor` — create/edit form with RFC validation and CFDI fields | `feature/US-7.1-customer-entity` | 🟢 | |

**Acceptance Criteria:**
- [x] Customer CRUD via API and Blazor UI
- [x] RFC format validated (regex: `^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$`)
- [x] Customer search by name, RFC, phone, email
- [x] Multi-tenancy enforced

---

## User Story 7.2: Warehouse, Inventory & Sale Entities
**As a developer, I want Warehouse, InventoryItem, Sale, SaleItem, and Payment entities so that POS transactions can be persisted and inventory tracked.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-7.2.1 | Create `Warehouse` entity with address and default flag | `feature/US-7.2-warehouse-inventory-sale` | 🟢 | |
| TASK-7.2.2 | Create `InventoryItem` entity — links `Product` + `Warehouse`, tracks `QuantityOnHand`, `MinStockLevel` | `feature/US-7.2-warehouse-inventory-sale` | 🟢 | |
| TASK-7.2.3 | Create `Sale` entity — `SaleType` enum (Sale, Invoice, Quote), `SaleStatus` enum | `feature/US-7.2-warehouse-inventory-sale` | 🟢 | |
| TASK-7.2.4 | Create `SaleItem` entity — links Sale + Product, stores quantity, unit price, discount | `feature/US-7.2-warehouse-inventory-sale` | 🟢 | |
| TASK-7.2.5 | Create `Payment` entity — `PaymentMethod` enum (Cash, Card, Transfer, Check), amount, change | `feature/US-7.2-warehouse-inventory-sale` | 🟢 | |
| TASK-7.2.6 | Create 5 EF Core configurations | `feature/US-7.2-warehouse-inventory-sale` | 🟢 | |
| TASK-7.2.7 | Generate `AddSalesInventorySchema` migration | `feature/US-7.2-warehouse-inventory-sale` | 🟢 | |
| TASK-7.2.8 | Seed "Almacén Principal" warehouse (tenantId: `b000...0001`, warehouseId: `e000...0001`) | `feature/US-7.2-warehouse-inventory-sale` | 🟢 | |

**Acceptance Criteria:**
- [x] All 5 entities in database with proper relationships
- [x] "Almacén Principal" seeded for demo tenant
- [x] `Sale.SaleType` supports Sale, Invoice, Quote
- [x] Multi-tenancy enforced on all entities

---

## User Story 7.3: POS Search & Sale Operations
**As a cashier, I want fast case-insensitive product search and the ability to create, complete, and cancel sales so that I can process transactions efficiently.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-7.3.1 | Create `IPosSearchService` interface | `feature/US-7.3-pos-search-sale-ops` | 🟢 | |
| TASK-7.3.2 | Implement `PosSearchService` using `EF.Functions.ILike` for case-insensitive PostgreSQL search | `feature/US-7.3-pos-search-sale-ops` | 🟢 | Searches name, SKU, barcode |
| TASK-7.3.3 | Create `CreateSaleCommand` + handler — creates sale, validates stock availability | `feature/US-7.3-pos-search-sale-ops` | 🟢 | |
| TASK-7.3.4 | Create `CompleteSaleCommand` + handler — deducts inventory, records payments | `feature/US-7.3-pos-search-sale-ops` | 🟢 | |
| TASK-7.3.5 | Create `CancelSaleCommand` + handler — restores inventory, sets status to Cancelled | `feature/US-7.3-pos-search-sale-ops` | 🟢 | |
| TASK-7.3.6 | Register `IPosSearchService` in `DependencyInjection.cs` (both methods) | `feature/US-7.3-pos-search-sale-ops` | 🟢 | |

**Acceptance Criteria:**
- [x] Product search returns results in <300ms (case-insensitive, PostgreSQL ILike)
- [x] `CompleteSaleCommand` deducts `QuantityOnHand` from correct `InventoryItem`
- [x] `CancelSaleCommand` restores inventory on cancellation
- [x] Multi-tenancy enforced on all operations

---

## User Story 7.4: POS & Sale API Endpoints
**As a developer, I want Minimal API endpoints for POS operations and sales management so that the Blazor POS UI can perform all transactions.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-7.4.1 | Create `PosEndpoints.cs` — `GET /api/v1/pos/search`, `POST /api/v1/pos/sales` | `feature/US-7.4-pos-api-endpoints` | 🟢 | |
| TASK-7.4.2 | Create `SaleEndpoints.cs` — `POST /api/v1/sales/{id}/complete`, `POST /api/v1/sales/{id}/cancel` | `feature/US-7.4-pos-api-endpoints` | 🟢 | |
| TASK-7.4.3 | Create `CustomerEndpoints.cs` — full CRUD for customers | `feature/US-7.4-pos-api-endpoints` | 🟢 | |
| TASK-7.4.4 | Create `IPosService` interface and `PosService` implementation for Blazor HTTP calls | `feature/US-7.4-pos-api-endpoints` | 🟢 | |
| TASK-7.4.5 | Register all new endpoints in `EndpointExtensions.cs` | `feature/US-7.4-pos-api-endpoints` | 🟢 | |

**Acceptance Criteria:**
- [x] All POS and sale endpoints require authorization
- [x] `PosService` provides typed methods for Blazor components
- [x] Multi-tenancy enforced at endpoint level

---

## User Story 7.5: POS Blazor UI
**As a cashier, I want a 3-panel POS screen (product search / shopping cart / payment) so that I can complete a sale without leaving the page.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-7.5.1 | Create `Pos.razor` — 3-panel layout: left (search), center (cart), right (payment) | `feature/US-7.5-pos-blazor-ui` | 🟢 | |
| TASK-7.5.2 | Create `ProductSearch.razor` — search input (auto-focus), results list with product details and "Add to Cart" | `feature/US-7.5-pos-blazor-ui` | 🟢 | |
| TASK-7.5.3 | Create `ShoppingCart.razor` — line items with quantity controls, discount field, real-time subtotal/IVA/total | `feature/US-7.5-pos-blazor-ui` | 🟢 | |
| TASK-7.5.4 | Create `PaymentPanel.razor` — payment method selector (Cash/Card/Transfer), amount input, change calculation, "Cobrar" button | `feature/US-7.5-pos-blazor-ui` | 🟢 | |
| TASK-7.5.5 | Add "POS" navigation link to `NavMenu.razor` | `feature/US-7.5-pos-blazor-ui` | 🟢 | |
| TASK-7.5.6 | Add ~80 es-MX localization keys | `feature/US-7.5-pos-blazor-ui` | 🟢 | |

**Acceptance Criteria:**
- [x] Cashier can search products, add to cart, and complete payment — all without leaving POS page
- [x] Change calculated automatically for cash payments
- [x] Cart shows real-time subtotal, IVA (16%), and total
- [x] Sale confirmed with success message and cart cleared
- [x] All UI text in Spanish (es-MX)

---

**Sprint 7 Total: 34/34 SP delivered | Test suite: 254/254 passing (100%) | PR #55 merged**
