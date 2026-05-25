# Sprint 12: Supplier & Purchase Order Management

**Goal:** Add supplier profiles and the full purchase order lifecycle (Draft → Approved → Received) so store owners can reorder inventory from suppliers and automatically update stock on goods receipt.

**Duration:** ~4-5 days estimated
**Status:** 🟡 In Progress
**Started:** 2026-05-25
**Total Story Points:** 26 pts

> **Prerequisites:** Sprints 1-11 complete | Local Aspire environment running | Warehouse entity exists (Sprint 7)

---

## User Story 12.1: Supplier Management
**As an inventory manager, I want to create and manage supplier profiles so that I can associate purchase orders with the correct vendor and contact information.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-12.1.1 | Create `Supplier` domain entity (`Name`, `RFC`, `ContactName`, `Email`, `Phone`, `Address`, `PaymentTermsDays`, `TaxRegime`, `IsActive`) + `ISupplierRepository` | feature/US-12.1-TASK-1-supplier-domain-cqrs | 🟢 | Inherits `TenantAuditableEntity`, `ISoftDeletable` |
| TASK-12.1.2 | CQRS: `CreateSupplierCommand`, `UpdateSupplierCommand`, `DeleteSupplierCommand`, `GetSuppliersQuery`, `GetSupplierByIdQuery` + handlers + validators | feature/US-12.1-TASK-1-supplier-domain-cqrs | 🟢 | FluentValidation; RFC regex `^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$` |
| TASK-12.1.3 | EF Core config + migrations `AddSupplierSchema` + `AddSupplierPermissionsSeed` + DI registration | feature/US-12.1-TASK-1-supplier-domain-cqrs | 🟢 | `suppliers` table; unique `ix_suppliers_tenant_rfc`; 4 permissions seeded |
| TASK-12.1.4 | `SupplierEndpoints.cs` — GET list, GET by ID, POST, PUT, DELETE + auth policies | feature/US-12.1-TASK-1-supplier-domain-cqrs | 🟢 | `WithDescription` + `Produces<T>`; 4 policies in `AuthorizationExtensions` |
| TASK-12.1.5 | Unit tests: 20 tests covering Create/Update/Delete/GetAll/GetById | feature/US-12.1-TASK-1-supplier-domain-cqrs | 🟢 | All 20 passing — Moq + xUnit + FluentAssertions |

**Acceptance Criteria:**
- [x] Supplier CRUD operations functional via API
- [x] RFC format validated (regex `^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$`)
- [x] RFC unique per tenant (duplicate returns 409 Conflict)
- [x] Soft-delete: sets `IsDeleted=true`, `DeletedAt` (no hard deletes)
- [x] `suppliers.view`, `suppliers.create`, `suppliers.update`, `suppliers.delete` permissions seeded
- [x] Unit tests passing (20/20)

---

## User Story 12.2: Purchase Order Backend
**As a store owner, I want to create purchase orders for products I need to restock so that I can track what I've ordered from each supplier.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-12.2.1 | `PurchaseOrder` + `PurchaseOrderItem` domain entities; `PurchaseOrderStatus` enum (`Draft`, `Submitted`, `Approved`, `PartiallyReceived`, `Received`, `Cancelled`) | — | 🔴 | `PurchaseOrder` inherits `TenantAuditableEntity`; `OrderNumber` auto-generated `PO-{year}-{seq}` |
| TASK-12.2.2 | Commands: `CreatePurchaseOrderCommand`, `UpdatePurchaseOrderCommand`, `SubmitPurchaseOrderCommand`, `ApprovePurchaseOrderCommand`, `CancelPurchaseOrderCommand` + handlers | — | 🔴 | Status transition guards (e.g., cannot approve a Cancelled PO); `Result<T>` pattern |
| TASK-12.2.3 | Queries: `GetPurchaseOrdersQuery` (paged, filterable by status/supplier/date), `GetPurchaseOrderByIdQuery` | — | 🔴 | `AsNoTracking()`, include items + product names |
| TASK-12.2.4 | EF Core config (`PurchaseOrderConfiguration.cs`, `PurchaseOrderItemConfiguration.cs`) + migration `AddPurchaseOrderSchema` | — | 🔴 | `purchase_orders` + `purchase_order_items` tables; `ix_purchase_orders_tenant_status` index |
| TASK-12.2.5 | `PurchaseOrderEndpoints.cs` — GET list (paged), GET by ID, POST create, PUT update, POST submit, POST approve, POST cancel | — | 🔴 | Separate action endpoints for state transitions |
| TASK-12.2.6 | Unit tests for all PO command/query handlers | — | 🔴 | Status transition edge cases; invalid transitions return `Result.Failure` |

**Acceptance Criteria:**
- [ ] Purchase order creation with at least one line item enforced
- [ ] `OrderNumber` auto-generated as `PO-{YYYY}-{sequence}` per tenant
- [ ] Status transitions enforced: Draft→Submitted→Approved, Draft/Submitted→Cancelled
- [ ] Total calculated server-side (Subtotal + IVA 16% = Total)
- [ ] `purchases.view`, `purchases.create`, `purchases.submit`, `purchases.approve`, `purchases.cancel` permissions seeded
- [ ] Unit tests passing for all state transitions

---

## User Story 12.3: Goods Receipt & Inventory Integration
**As a warehouse manager, I want to record goods received against a purchase order so that inventory levels are automatically updated when stock arrives.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-12.3.1 | `GoodsReceipt` + `GoodsReceiptItem` domain entities | — | 🔴 | `GoodsReceipt` inherits `TenantAuditableEntity`; links to `PurchaseOrder` |
| TASK-12.3.2 | `ReceiveGoodsCommand` handler: validate PO is Approved/PartiallyReceived, update `InventoryItem.Quantity` per warehouse, update `PurchaseOrderItem.ReceivedQuantity`, update PO status (PartiallyReceived vs Received) | — | 🔴 | Transactional — all updates in one `SaveChangesAsync`; reuses existing `InventoryItem` entity from Sprint 7 |
| TASK-12.3.3 | `GetGoodsReceiptsQuery` (list by PO or by date range) + `GetGoodsReceiptByIdQuery` | — | 🔴 | Include receipt items + product names + warehouse names |
| TASK-12.3.4 | EF Core config (`GoodsReceiptConfiguration.cs`, `GoodsReceiptItemConfiguration.cs`) + migration `AddGoodsReceiptSchema` | — | 🔴 | `goods_receipts` + `goods_receipt_items` tables |
| TASK-12.3.5 | `GoodsReceiptEndpoints.cs` — POST receive (creates receipt + updates inventory), GET receipts by PO, GET receipt by ID | — | 🔴 | — |
| TASK-12.3.6 | Unit tests for `ReceiveGoodsCommand` (full receipt, partial receipt, over-receipt rejected, wrong PO status rejected) | — | 🔴 | — |

**Acceptance Criteria:**
- [ ] Receiving goods against an Approved PO updates `InventoryItem.Quantity` in the specified warehouse
- [ ] Partial receipt (some items received, not all) → PO status = `PartiallyReceived`
- [ ] Full receipt (all items received) → PO status = `Received`
- [ ] Over-receiving (more than ordered) returns validation error
- [ ] Receipt against non-Approved PO returns error
- [ ] Inventory update and receipt creation are atomic (single transaction)
- [ ] `receipts.create`, `receipts.view` permissions seeded
- [ ] Unit tests for all edge cases passing

---

## User Story 12.4: Purchase Management UI (Blazor)
**As a store owner, I want a complete Blazor UI for managing suppliers and purchase orders so that I can perform all purchasing workflows without using Postman or Swagger.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-12.4.1 | `ISupplierHttpService` + `SupplierHttpService` in BlazorApp (`Services/Suppliers/`) | — | 🔴 | Follows pattern of `ICfdiHttpService`; JWT bearer injected via `HttpClient` |
| TASK-12.4.2 | `IPurchaseOrderHttpService` + `PurchaseOrderHttpService` in BlazorApp (`Services/PurchaseOrders/`) | — | 🔴 | Includes `SubmitAsync`, `ApproveAsync`, `CancelAsync`, `ReceiveGoodsAsync` actions |
| TASK-12.4.3 | `SupplierList.razor` at `/proveedores` — MudDataGrid with search, add/edit/delete actions | — | 🔴 | Follows `CustomerList.razor` pattern |
| TASK-12.4.4 | `SupplierForm.razor` component — create/edit supplier modal with RFC validation | — | 🔴 | MudDialog, inline RFC format validation |
| TASK-12.4.5 | `PurchaseOrderList.razor` at `/compras` — MudDataGrid with status filter chips, date range filter | — | 🔴 | Status badges with color coding (Draft=grey, Submitted=blue, Approved=green, etc.) |
| TASK-12.4.6 | `PurchaseOrderDetail.razor` at `/compras/{id}` — order header + line items grid + action buttons (Submit, Approve, Cancel, Receive) | — | 🔴 | Action buttons shown/hidden by PO status and user permissions |
| TASK-12.4.7 | `ReceiveGoodsDialog.razor` — dialog to enter received quantities per line item + warehouse selector | — | 🔴 | Opens from PurchaseOrderDetail; partial receipt supported |
| TASK-12.4.8 | es-MX localization resource file (`Purchases.es-MX.resx`) + nav menu entries (COMPRAS section) + new permissions registered in `AuthorizationPolicies.cs` | — | 🔴 | ~40 localization keys; nav under sidebar COMPRAS section |

**Acceptance Criteria:**
- [ ] Supplier list visible at `/proveedores` with search and CRUD
- [ ] Purchase order list visible at `/compras` with status filter and date range
- [ ] Purchase order detail shows header, line items, and correct action buttons per status
- [ ] Receive Goods dialog allows entering quantities per line item, with warehouse selection
- [ ] After receiving, inventory levels updated and PO status changes visually
- [ ] All UI text in Spanish (es-MX) via `IStringLocalizer`
- [ ] New nav section "COMPRAS" with Proveedores + Órdenes de Compra entries
- [ ] Feature works end-to-end without Postman: create supplier → create PO → approve → receive goods → verify inventory

---

## Sprint 12 Summary

| Story | Priority | SP | Status |
|-------|----------|----|--------|
| US-12.1: Supplier Management | P1 High | 5 | 🟢 Complete |
| US-12.2: Purchase Order Backend | P0 Critical | 8 | 🔴 Not Started |
| US-12.3: Goods Receipt & Inventory Integration | P0 Critical | 5 | 🔴 Not Started |
| US-12.4: Purchase Management UI | P1 High | 8 | 🔴 Not Started |
| **Total** | | **26** | |

**Recommended execution order:** US-12.1 → US-12.2 → US-12.3 (parallel with US-12.1 after TASK-12.1.3) → US-12.4
