# Sprint 8: POS Features & Sales Management

**Goal:** Extend the POS system with sales history, inventory management UI, receipt generation, and quote management — giving hardware store staff complete visibility and control over all sales operations.

**Duration:** TBD (~3-4 days estimated at 8 SP/day velocity)
**Status:** 🔴 Not Started (0%)
**Started:** TBD
**Total Story Points:** 21 pts (US-8.1: 5, US-8.2: 8, US-8.3: 5, US-8.4: 3) + ~2 SP tech debt (TD-3.1.A)
**Completed:** 0/37 tasks (0%)

> 🎯 **This is the NEXT SPRINT — ready to begin.**
> Prerequisites: Sprint 7 complete ✅ | Sale, InventoryItem, Payment entities exist ✅ | PosEndpoints, SaleEndpoints scaffolded ✅

---

## ⚠️ Tech Debt: TD-3.1.A — Product Handler Unit Tests
Recommended to fold into Sprint 8 since the product domain is active.

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-TD-3.1.A | Unit tests for `CreateProduct`, `UpdateProduct`, `DeleteProduct`, `GetProductsQuery`, `GetProductByIdQuery` handlers (>70% coverage) | `feature/US-8.0-product-handler-tests` | 🔴 | Deferred from Sprint 3 |

---

## User Story 8.1: Sales History & Management UI
**As a store manager, I want to view, search, and manage completed and cancelled sales so that I can review transactions and resolve disputes without needing database access.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-8.1.1 | Add `GetSalesQuery` + handler with pagination and filter support (dateFrom, dateTo, searchTerm, status, page, pageSize) | `feature/US-8.1-sales-history-ui` | 🔴 | Use `AsNoTracking()` |
| TASK-8.1.2 | Add `GetSaleByIdQuery` + handler (includes `SaleItems` and `Payments`) | `feature/US-8.1-sales-history-ui` | 🔴 | |
| TASK-8.1.3 | Update `SaleEndpoints.cs` — add `GET /api/v1/sales` (paginated list with query params) and `GET /api/v1/sales/{id}` (detail) | `feature/US-8.1-sales-history-ui` | 🔴 | |
| TASK-8.1.4 | Create `SaleList.razor` — paginated table with filter bar (date range `dd/MM/yyyy`, search, status dropdown) | `feature/US-8.1-sales-history-ui` | 🔴 | |
| TASK-8.1.5 | Create `SaleDetail.razor` (or modal) — items table, payment summary, cancel button | `feature/US-8.1-sales-history-ui` | 🔴 | |
| TASK-8.1.6 | Add cancel sale flow to UI with `MudDialog` confirmation and Snackbar feedback | `feature/US-8.1-sales-history-ui` | 🔴 | Reuse existing `CancelSaleCommand` |
| TASK-8.1.7 | Add "Historial de Ventas" navigation link to `NavMenu.razor` | `feature/US-8.1-sales-history-ui` | 🔴 | |
| TASK-8.1.8 | Add ~25 es-MX localization keys (`SalesHistory`, `SaleDetail`, `CancelSale`, `SaleStatus`, `SaleItems`, `PaymentSummary`, etc.) | `feature/US-8.1-sales-history-ui` | 🔴 | |
| TASK-8.1.9 | Unit tests for `GetSalesQuery` handler and filter logic (>70% coverage) | `feature/US-8.1-sales-history-ui` | 🔴 | |

**Acceptance Criteria:**
- [ ] Manager navigates to `/ventas` and sees paginated table (20/page) with: Folio, Date (dd/MM/yyyy), Customer, Total ($MXN), Payment Method, Status
- [ ] Filter by date range, search by folio or customer name (case-insensitive, ILike)
- [ ] Sale detail shows all items, quantities, unit prices, payment breakdown, and IVA
- [ ] Manager can cancel a completed sale — inventory restored, status updated, Snackbar shown
- [ ] Multi-tenancy: only Tenant A's sales visible to Tenant A's manager
- [ ] Feature demo-able via Blazor — no Postman required

**Technical Notes:**
- Affected components: `Corelio.Application`, `Corelio.WebAPI`, `Corelio.BlazorApp`
- `Sale` inherits `TenantAuditableEntity` — query filters auto-applied
- Use `AsNoTracking()` on all read queries

**Dependencies:**
- [x] US-7.2: `Sale`, `SaleItem`, `Payment` entities exist
- [x] US-7.3: `CancelSaleCommand` exists
- [x] US-7.4: `SaleEndpoints.cs` scaffolded

**Out of Scope:**
- Sales reports/analytics (Sprint 10)
- Export to PDF/Excel (Sprint 10)

---

## User Story 8.2: Inventory Management UI
**As an inventory manager, I want to view current stock levels per warehouse, make manual adjustments, and see the transaction history so that I can keep the product catalog accurate and respond to low-stock situations.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-8.2.1 | Verify Sprint 7 migration — confirm whether `inventory_transactions` table exists; if not, create `InventoryTransaction` entity + EF config + migration `AddInventoryTransactions` | `feature/US-8.2-inventory-management-ui` | 🔴 | Fields: Id, TenantId, InventoryItemId, TransactionType enum, QuantityBefore, QuantityAfter, QuantityChange, ReasonCode, Notes, CreatedBy, CreatedAt |
| TASK-8.2.2 | Implement `GetInventoryItemsQuery` + handler (filters: warehouseId, lowStockOnly, search) | `feature/US-8.2-inventory-management-ui` | 🔴 | |
| TASK-8.2.3 | Implement `AdjustStockCommand` + handler + `AdjustStockCommandValidator` (validates: positive quantity, sufficient stock for decreases, reason required) | `feature/US-8.2-inventory-management-ui` | 🔴 | |
| TASK-8.2.4 | Implement `GetInventoryTransactionsQuery` + handler (filter by productId, paginated) | `feature/US-8.2-inventory-management-ui` | 🔴 | |
| TASK-8.2.5 | Add endpoints in `InventoryEndpoints.cs`: `GET /api/v1/inventory`, `POST /api/v1/inventory/adjustments`, `GET /api/v1/inventory/{productId}/transactions` | `feature/US-8.2-inventory-management-ui` | 🔴 | |
| TASK-8.2.6 | Create `InventoryList.razor` — table with color-coded stock status (🟢 adequate / 🟡 low / 🔴 out), filter bar, "Ajustar Stock" per row | `feature/US-8.2-inventory-management-ui` | 🔴 | |
| TASK-8.2.7 | Create `StockAdjustmentDialog.razor` — `MudDialog` with increase/decrease toggle, quantity input, reason dropdown (Damaged/Lost/Stolen/Found/CountCorrection/Other), notes | `feature/US-8.2-inventory-management-ui` | 🔴 | |
| TASK-8.2.8 | Create `InventoryTransactionHistory.razor` (panel or page) — chronological transaction list per product | `feature/US-8.2-inventory-management-ui` | 🔴 | |
| TASK-8.2.9 | Add "Inventario" navigation link to `NavMenu.razor` | `feature/US-8.2-inventory-management-ui` | 🔴 | |
| TASK-8.2.10 | Add ~35 es-MX localization keys (`Inventory`, `StockAdjustment`, `AdjustmentReason`, `LowStock`, `OutOfStock`, `TransactionHistory`, reason code labels, etc.) | `feature/US-8.2-inventory-management-ui` | 🔴 | |
| TASK-8.2.11 | Unit tests for `AdjustStockCommand` — decrease-below-zero guard, reason required (>70% coverage) | `feature/US-8.2-inventory-management-ui` | 🔴 | |

**Acceptance Criteria:**
- [ ] Manager sees stock table with color-coded status: 🟢 above min / 🟡 at or below min ("Stock Bajo" chip) / 🔴 zero ("Sin Stock" chip)
- [ ] Manager creates stock adjustment — increase or decrease with reason code
- [ ] Decrease below 0 rejected: "La cantidad no puede ser mayor al stock actual"
- [ ] `InventoryTransaction` record created for every adjustment (before/after quantities, reason, user)
- [ ] Transaction history per product visible in panel
- [ ] Warehouse selector dropdown shown (even if only 1 warehouse — forward compatible)
- [ ] Multi-tenancy enforced

**Technical Notes:**
- Reason codes: `Damaged`, `Lost`, `Stolen`, `Found`, `CountCorrection`, `Other`
- `TransactionType` enum: `Sale`, `Adjustment`, `Return`, `Correction`

**Dependencies:**
- [x] US-7.2: `Warehouse`, `InventoryItem` entities exist with `QuantityOnHand` and `MinStockLevel`
- [x] US-7.3: `PosSearchService` already decrements inventory on `CompleteSaleCommand`

**Out of Scope:**
- Inventory import from Excel/CSV
- Multi-warehouse transfers

---

## User Story 8.3: Receipt & Ticket Generation
**As a cashier, I want the system to generate a printable PDF receipt after each completed sale so that customers have proof of purchase.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-8.3.1 | Add `QuestPDF` NuGet package to `Corelio.Infrastructure` (`dotnet add package QuestPDF`) | `feature/US-8.3-receipt-generation` | 🔴 | MIT licensed |
| TASK-8.3.2 | Create `SaleReceiptDocument.cs` in `Infrastructure/Documents/` — QuestPDF document class with receipt layout | `feature/US-8.3-receipt-generation` | 🔴 | Simple layout, no logo for MVP |
| TASK-8.3.3 | Create `ISaleReceiptService` interface in Application layer + `SaleReceiptService` implementation in Infrastructure | `feature/US-8.3-receipt-generation` | 🔴 | |
| TASK-8.3.4 | Add `GenerateSaleReceiptQuery` + handler returning `byte[]` | `feature/US-8.3-receipt-generation` | 🔴 | |
| TASK-8.3.5 | Add endpoint `GET /api/v1/sales/{id}/receipt` to `SaleEndpoints.cs` — returns `application/pdf` file result | `feature/US-8.3-receipt-generation` | 🔴 | |
| TASK-8.3.6 | Add Blazor HTTP service method `DownloadReceiptAsync(Guid saleId)` using `IJSRuntime` for browser file download | `feature/US-8.3-receipt-generation` | 🔴 | |
| TASK-8.3.7 | Add "Imprimir Ticket" button to `PaymentPanel.razor` (shown after successful sale completion) | `feature/US-8.3-receipt-generation` | 🔴 | |
| TASK-8.3.8 | Add "Ticket" icon button to `SaleList.razor` table rows | `feature/US-8.3-receipt-generation` | 🔴 | Depends on US-8.1 |
| TASK-8.3.9 | Add ~10 es-MX localization keys (`PrintTicket`, `DownloadTicket`, `TicketGenerationError`, `GeneratingTicket`) | `feature/US-8.3-receipt-generation` | 🔴 | |
| TASK-8.3.10 | Register `ISaleReceiptService` in `DependencyInjection.cs` (both methods) | `feature/US-8.3-receipt-generation` | 🔴 | |

**Acceptance Criteria:**
- [ ] "Imprimir Ticket" button appears after successful POS checkout
- [ ] PDF contains: business name, RFC, date (dd/MM/yyyy HH:mm), folio, items (name, qty, unit price, line total), subtotal, IVA, total, payment method, change (for cash)
- [ ] All amounts in format `$1,234.56 MXN` with `CultureInfo("es-MX")`
- [ ] "Ticket" icon in sales history also triggers PDF download
- [ ] Error Snackbar shown if PDF generation fails

**Technical Notes:**
- Library: `QuestPDF` (MIT, .NET native — preferred over iText7)
- Thermal printer (ESC/POS) is out of scope — deferred

**Dependencies:**
- [x] US-7.4: `SaleEndpoints.cs` exists
- [x] US-7.5: `PaymentPanel.razor` exists
- [ ] US-8.1: `SaleList.razor` (for history ticket button)

**Out of Scope:**
- Thermal/ESC-POS printer output
- Logo on receipt
- Email delivery of receipt

---

## User Story 8.4: Quote Management
**As a seller, I want to create quotes from the POS and convert them to sales when the customer decides to purchase, so that I can provide formal estimates without deducting inventory.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-8.4.1 | Verify `Sale.SaleType` enum includes `Quote` and `Sale.ExpiresAt` nullable field exists; create migration `AddSaleExpiresAt` if needed | `feature/US-8.4-quote-management` | 🔴 | |
| TASK-8.4.2 | Create `CreateQuoteCommand` + handler (no inventory deduction, sets `SaleType = Quote`, `ExpiresAt = now + 30 days`) | `feature/US-8.4-quote-management` | 🔴 | |
| TASK-8.4.3 | Create `ConvertQuoteToSaleCommand` + handler (validates open + not expired, deducts inventory, changes type to Sale) | `feature/US-8.4-quote-management` | 🔴 | |
| TASK-8.4.4 | Update `GetSalesQuery` to filter by `SaleType` and expose open quotes | `feature/US-8.4-quote-management` | 🔴 | |
| TASK-8.4.5 | Add "Guardar como Cotización" button to POS cart toolbar or `PaymentPanel.razor` | `feature/US-8.4-quote-management` | 🔴 | |
| TASK-8.4.6 | Create `QuoteList.razor` at `/cotizaciones` — table with expiry indicator, convert and cancel actions | `feature/US-8.4-quote-management` | 🔴 | |
| TASK-8.4.7 | Add "Cotizaciones" navigation link to `NavMenu.razor` | `feature/US-8.4-quote-management` | 🔴 | |
| TASK-8.4.8 | Add ~15 es-MX localization keys (`Quotes`, `QuoteList`, `CreateQuote`, `ConvertToSale`, `QuoteExpired`, `SaveAsQuote`, etc.) | `feature/US-8.4-quote-management` | 🔴 | |

**Acceptance Criteria:**
- [ ] Seller clicks "Guardar como Cotización" — sale saved with `SaleType = Quote`, no inventory deducted, folio shown in confirmation
- [ ] Quote list at `/cotizaciones` shows: Folio, Date, Customer, Total, Expiry Date, Status
- [ ] Expired quotes (>30 days) show status "Vencida" and "Convertir a Venta" is disabled
- [ ] Convert to sale — POS pre-loads quote items in cart, seller proceeds to payment
- [ ] Cancel quote — status changes to Cancelled, removed from open list

**Technical Notes:**
- `Sale.SaleType` enum already includes `Quote` from Sprint 7
- Endpoint additions: `GET /api/v1/sales?type=quote`, `POST /api/v1/sales/{id}/convert`

**Dependencies:**
- [x] US-7.2: `Sale` entity with `SaleType` enum exists
- [x] US-7.5: POS cart and `PaymentPanel.razor` exist
- [ ] US-8.1: `SaleList.razor` pattern to follow for `QuoteList.razor`

**Out of Scope:**
- Quote PDF generation
- Stock reservation on quote creation
- Quote templates

---

## Sprint 8 Summary

| Story | Priority | SP | Status |
|-------|----------|----|--------|
| TD-3.1.A: Product handler unit tests | P1 High | ~2 | 🔴 Not Started |
| US-8.1: Sales History & Management UI | P0 Critical | 5 | 🔴 Not Started |
| US-8.2: Inventory Management UI | P0 Critical | 8 | 🔴 Not Started |
| US-8.3: Receipt & Ticket Generation | P1 High | 5 | 🔴 Not Started |
| US-8.4: Quote Management | P1 High | 3 | 🔴 Not Started |
| **Total** | | **~23** | |

**Recommended execution order:** US-8.1 → US-8.2 → US-8.3 (depends on 8.1) → US-8.4 → TD-3.1.A
