# Sprint 13: Reporting & Analytics

**Goal:** Deliver a complete Reporting & Analytics module so store owners can view daily sales performance, inventory valuation, and purchase summaries — with chart visualizations and PDF/CSV export — without leaving the Blazor UI.

**Duration:** ~4-5 days estimated
**Status:** 🟡 In Progress
**Started:** 2026-06-17
**Total Story Points:** 26 pts

> **Prerequisites:** Sprints 1-12 complete | Sales, InventoryItem, PurchaseOrder, and Supplier entities exist | QuestPDF already integrated (Sprint 9)

---

## User Story 13.1: Daily Sales Report
**As a store owner or cashier supervisor, I want to view a daily sales report with KPIs, payment method breakdown, top-selling products, and hourly distribution so that I can monitor store performance and export the data for accounting.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-13.1.1 | `GetSalesReportQuery` + `GetSalesReportQueryHandler` — aggregate `Sales` table: total amount, transaction count, average ticket, breakdown by `PaymentMethod`, breakdown by cashier (`CreatedBy`), top 10 products by quantity sold (join `SaleItems`), hourly distribution (group by hour of `CreatedAt`) | feature/US-13.1-TASK-1-6-sales-report | 🟢 | Query-only CQRS; `AsNoTracking()`; `SalesReportQueryService` delegates from handler; `ISalesReportQueryService` interface |
| TASK-13.1.2 | `SalesReportEndpoints.cs` — `GET /api/v1/reports/sales` + `GET /api/v1/reports/sales/export?format=pdf\|csv`; seeds `reports.view` + `reports.export` permissions via `DataSeeder.cs` | feature/US-13.1-TASK-1-6-sales-report | 🟢 | `reports.view` permission gates GET; `reports.export` gates export; OpenAPI docs with `WithDescription` + `Produces<SalesReportDto>` |
| TASK-13.1.3 | `SalesReportPdfExporter` (QuestPDF A4: KPI header, payment method table, top-10 products, hourly distribution) + CSV export via `StringBuilder` | feature/US-13.1-TASK-1-6-sales-report | 🟢 | `SalesReportDocument.cs` + `SalesReportExportService.cs`; reuses QuestPDF patterns from InvoicePdfService |
| TASK-13.1.4 | `ISalesReportHttpService` + `SalesReportHttpService` in BlazorApp (`Services/Reports/`) | feature/US-13.1-TASK-1-6-sales-report | 🟢 | `SalesReportModels.cs`; calls `/api/v1/reports/sales` and `/api/v1/reports/sales/export`; uses `AuthenticatedHttpClient` |
| TASK-13.1.5 | `SalesReport.razor` at `/reportes/ventas` — date range picker, payment method filter, KPI cards, MudChart bar (hourly), MudChart donut (payment), top-10 products MudTable, cashier breakdown, PDF/CSV export buttons | feature/US-13.1-TASK-1-6-sales-report | 🟢 | `ReportsDashboard.razor` at `/reportes` also created; REPORTES nav section in NavMenu |
| TASK-13.1.6 | Unit tests for `SalesReportQueryService` — 6 tests: empty range, single sale KPIs, excludes non-completed, payment breakdown, top products sorted, hourly distribution 24 slots, warehouse filter | feature/US-13.1-TASK-1-6-sales-report | 🟢 | `tests/Corelio.Infrastructure.Tests/Reports/SalesReportQueryServiceTests.cs` |

**Acceptance Criteria:**
- [ ] Report returns total amount, transaction count, and average ticket for the selected date range
- [ ] Payment method breakdown shows each method (Efectivo, Tarjeta, Transferencia) with amount and percentage
- [ ] Top 10 selling products sorted by quantity sold descending
- [ ] Hourly distribution shows 24 slots (00–23h), zero-filled for hours with no sales
- [ ] Filters for date range, warehouse, and payment method applied server-side
- [ ] PDF export renders KPI summary, payment method table, and top-10 products table
- [ ] CSV export contains one row per transaction with all columns
- [ ] `reports.sales.view` permission gates the endpoint and the nav link
- [ ] All UI text in Spanish (es-MX) via `IStringLocalizer`
- [ ] Unit tests passing for all aggregation scenarios

---

## User Story 13.2: Inventory Valuation Report
**As a store owner or inventory manager, I want to view the current inventory valuation using weighted average cost so that I know the total book value of my stock, which categories hold the most value, and which products are below their minimum stock level.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-13.2.1 | Add `AverageCost decimal(18,4)` to `InventoryItem`; `UnitCost decimal(18,4)` to `GoodsReceiptItem`; WAC recalculation in `ReceiveGoodsCommandHandler`; migration `AddInventoryAverageCost` | feature/US-13.2-TASK-1-6-inventory-valuation-report | 🟢 | WAC: `(OldQty×OldCost + RecvQty×UnitCost)/(OldQty+RecvQty)`; new items: `AverageCost = poItem.UnitPrice` |
| TASK-13.2.2 | `GetInventoryValuationReportQuery` + handler delegates to `IInventoryValuationQueryService` | feature/US-13.2-TASK-1-6-inventory-valuation-report | 🟢 | Returns `InventoryValuationReportDto` with valuations, category groups, low-stock alerts |
| TASK-13.2.3 | `InventoryReportEndpoints.cs` — `GET /api/v1/reports/inventory` + `/export?format=pdf\|csv`; uses existing `reports.view`/`reports.export` permissions | feature/US-13.2-TASK-1-6-inventory-valuation-report | 🟢 | OpenAPI docs with `WithDescription` + `Produces<T>` |
| TASK-13.2.4 | `InventoryReportDocument.cs` (QuestPDF A4) + `InventoryReportExportService.cs` (PDF + CSV) | feature/US-13.2-TASK-1-6-inventory-valuation-report | 🟢 | KPI header, category table, product detail table, low-stock alerts section |
| TASK-13.2.5 | `InventoryValuationReport.razor` at `/reportes/inventario` — KPI cards, MudChart donut (value-by-category), product valuation MudTable, low-stock alerts table, export buttons | feature/US-13.2-TASK-1-6-inventory-valuation-report | 🟢 | Dashboard card now clickable; INVENTARIO nav link added to REPORTES section |
| TASK-13.2.6 | 6 unit tests in `InventoryValuationReportTests.cs`: empty inventory, single product, category grouping, zero-qty excluded, low-stock alert, warehouse filter, WAC calculation verification | feature/US-13.2-TASK-1-6-inventory-valuation-report | 🟢 | InMemoryDatabase; WAC math test in isolation |

**Acceptance Criteria:**
- [ ] `AverageCost` on `InventoryItem` updated on every goods receipt using WAC formula
- [ ] Report shows current quantity, average cost, and extended value per product
- [ ] Total inventory value displayed as sum of all extended values
- [ ] Value-by-category breakdown shows each category's total value and percentage
- [ ] Low-stock alerts list products where `Quantity <= MinStockLevel` (only show `Quantity = 0` products if MinStockLevel not set)
- [ ] Warehouse and category filters applied server-side
- [ ] PDF export includes all sections: summary, category breakdown, full product list, low-stock alerts
- [ ] `reports.inventory.view` permission gates the endpoint
- [ ] WAC unit tests passing (first receipt, partial receipt recalculation)

---

## User Story 13.3: Purchase Summary Report
**As a store owner, I want a purchase summary report showing spending by supplier, order status breakdown, and received-vs-pending quantities per period so that I can evaluate supplier relationships and track outstanding deliveries.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-13.3.1 | `GetPurchaseSummaryReportQuery` + handler — aggregate `PurchaseOrders` + `PurchaseOrderItems` + `Suppliers`; compute: total spent (Approved+PartiallyReceived+Received orders), order count by status, top suppliers by amount, received vs ordered quantities per product; filter params: `DateFrom`, `DateTo`, `SupplierId?`, `Status?` | - | 🔴 | Returns `PurchaseSummaryReportDto`; `AsNoTracking()` |
| TASK-13.3.2 | `PurchaseReportEndpoints.cs` — `GET /api/v1/reports/purchases` + `GET /api/v1/reports/purchases/export?format=pdf\|csv` | - | 🔴 | `reports.purchases.view` permission; reuses migration from TASK-13.1.2 |
| TASK-13.3.3 | `PurchaseReportPdfExporter` — QuestPDF: period header, KPI row (total orders, total amount, pending deliveries), spending-by-supplier table, order status breakdown table, received-vs-pending table per product | - | 🔴 | Follows InvoicePdfService layout patterns |
| TASK-13.3.4 | `PurchaseSummaryReport.razor` at `/reportes/compras` — date range + supplier filters, KPI cards (total orders, total amount, pending orders), `MudChart` bar for spending by supplier (top 5), `MudDataGrid` for received-vs-pending per product, status breakdown chips with counts, export buttons | - | 🔴 | Top-5 suppliers bar chart; remaining suppliers shown in table below |
| TASK-13.3.5 | Unit tests for `GetPurchaseSummaryReportQuery` handler | - | 🔴 | 5 tests: date range filter, supplier filter, status-only pending orders excluded from total spent, zero orders returns empty, spending by supplier ranking |

**Acceptance Criteria:**
- [ ] Report shows total orders, total amount spent, and count of pending deliveries for the selected period
- [ ] Spending-by-supplier breakdown with amount and order count per supplier
- [ ] Order status breakdown: count of orders in each `PurchaseOrderStatus` state
- [ ] Received-vs-pending quantities per product (ordered qty vs received qty)
- [ ] Only Approved, PartiallyReceived, and Received orders count toward "total spent"
- [ ] Date range and supplier filters applied server-side
- [ ] PDF export includes all sections
- [ ] `reports.purchases.view` permission gates the endpoint
- [ ] Unit tests passing

---

## User Story 13.4: Reports Dashboard & Navigation
**As a store owner, I want a central reports dashboard at /reportes that shows a snapshot of today's key metrics and provides navigation to all detailed reports so that I can get a quick overview without running each report manually.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-13.4.1 | `GetDashboardSummaryQuery` + handler — returns: today's sales total + transaction count, current low-stock product count, pending purchase orders count | - | 🔴 | Reuses aggregation logic from US-13.1 and US-13.2 handlers; no new DB entities |
| TASK-13.4.2 | `DashboardEndpoints.cs` — `GET /api/v1/reports/dashboard` | - | 🔴 | Requires any of the three report permissions; `Produces<DashboardSummaryDto>` |
| TASK-13.4.3 | `Reports.razor` at `/reportes` (dashboard home) — 3 KPI summary cards (today's sales, low-stock alerts, pending POs), 3 navigation cards linking to each detailed report, last-updated timestamp | - | 🔴 | `MudCard` grid layout; KPI cards refresh on page load |
| TASK-13.4.4 | `Reports.es-MX.resx` localization resource file — all Spanish strings for the 4 report pages and nav entries; add REPORTES section to `NavMenu.razor` with sub-links: Panel, Ventas, Inventario, Compras; register 3 report permissions in `AuthorizationPolicies.cs` | - | 🔴 | ~40 localization keys; REPORTES section follows COMPRAS section pattern in NavMenu |

**Acceptance Criteria:**
- [ ] `/reportes` dashboard loads today's sales total, low-stock count, and pending PO count
- [ ] Three navigation cards link to `/reportes/ventas`, `/reportes/inventario`, `/reportes/compras`
- [ ] REPORTES nav section visible in sidebar with sub-links to each report
- [ ] Nav links respect permissions — hidden if user lacks the required `reports.*.view` permission
- [ ] `Reports.es-MX.resx` contains all UI strings; no hardcoded Spanish in `.razor` files
- [ ] Dashboard summary responds in under 200ms for typical data volumes

---

## Sprint 13 Summary

| Story | Priority | SP | Status |
|-------|----------|----|--------|
| US-13.1: Daily Sales Report | P0 Critical | 8 | 🟢 Complete |
| US-13.2: Inventory Valuation Report | P0 Critical | 8 | 🟢 Complete |
| US-13.3: Purchase Summary Report | P1 High | 6 | 🔴 Not Started |
| US-13.4: Reports Dashboard & Navigation | P1 High | 4 | 🔴 Not Started |
| **Total** | | **26** | |

**Recommended execution order:** US-13.1 → US-13.2 (TASK-13.2.1 schema change first) → US-13.3 → US-13.4

**Key dependencies:**
- TASK-13.2.1 (`AddInventoryAverageCost` migration) must run before `InventoryItem.AverageCost` is readable
- Migration `AddReportPermissionsSeed` (TASK-13.1.2) seeds all three `reports.*.view` permissions — US-13.2 and US-13.3 endpoints depend on it
- TASK-13.4.4 (localization + nav) should be done last to include all report page strings
