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

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-12.2.1 | `PurchaseOrder` + `PurchaseOrderItem` domain entities; `PurchaseOrderStatus` enum (`Draft`, `Submitted`, `Approved`, `PartiallyReceived`, `Received`, `Cancelled`) | feature/US-12.2-TASK-1-to-6-purchase-order-backend | 🟢 | `PurchaseOrder` inherits `TenantAuditableEntity` + `ISoftDeletable`; `OrderNumber` auto-generated `PO-{year}-{seq:D4}` |
| TASK-12.2.2 | Commands: `CreatePurchaseOrderCommand`, `UpdatePurchaseOrderCommand`, `SubmitPurchaseOrderCommand`, `ApprovePurchaseOrderCommand`, `CancelPurchaseOrderCommand` + handlers | feature/US-12.2-TASK-1-to-6-purchase-order-backend | 🟢 | Status transition guards; FluentValidation validators for Create/Update; `Result<T>` pattern throughout |
| TASK-12.2.3 | Queries: `GetPurchaseOrdersQuery` (paged, filterable by status/supplier/date), `GetPurchaseOrderByIdQuery` | feature/US-12.2-TASK-1-to-6-purchase-order-backend | 🟢 | `AsNoTracking()`, includes items + supplier; DTOs: `PurchaseOrderListDto`, `PurchaseOrderDto`, `PurchaseOrderItemDto` |
| TASK-12.2.4 | EF Core config (`PurchaseOrderConfiguration.cs`, `PurchaseOrderItemConfiguration.cs`) + migrations `AddPurchaseOrderSchema` + `AddPurchaseOrderPermissionsSeed` | feature/US-12.2-TASK-1-to-6-purchase-order-backend | 🟢 | `purchase_orders` + `purchase_order_items` tables; 3 indexes; 5 permissions (Administrator+Manager get all, Cashier gets view) |
| TASK-12.2.5 | `PurchaseOrderEndpoints.cs` — GET list (paged), GET by ID, POST create, PUT update, POST submit, POST approve, POST cancel | feature/US-12.2-TASK-1-to-6-purchase-order-backend | 🟢 | 7 endpoints at `/api/v1/purchase-orders`; separate action routes for state transitions |
| TASK-12.2.6 | Unit tests for all PO command/query handlers | feature/US-12.2-TASK-1-to-6-purchase-order-backend | 🟢 | 24 tests passing — Create(6), Update(4), Submit(3), Approve(3), Cancel(4), GetList(2), GetById(2) |

**Acceptance Criteria:**
- [x] Purchase order creation with at least one line item enforced
- [x] `OrderNumber` auto-generated as `PO-{YYYY}-{sequence}` per tenant
- [x] Status transitions enforced: Draft→Submitted→Approved, Draft/Submitted→Cancelled
- [x] Total calculated server-side (Subtotal + IVA 16% = Total)
- [x] `purchases.view`, `purchases.create`, `purchases.submit`, `purchases.approve`, `purchases.cancel` permissions seeded
- [x] Unit tests passing for all state transitions

---

## User Story 12.3: Goods Receipt & Inventory Integration
**As a warehouse manager, I want to record goods received against a purchase order so that inventory levels are automatically updated when stock arrives.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-12.3.1 | `GoodsReceipt` + `GoodsReceiptItem` domain entities | feature/US-12.3-TASK-1-goods-receipt-domain | 🟢 | `GoodsReceipt` inherits `TenantAuditableEntity`; links to `PurchaseOrder`; IGoodsReceiptRepository |
| TASK-12.3.2 | `ReceiveGoodsCommand` handler: validate PO is Approved/PartiallyReceived, update `InventoryItem.Quantity` per warehouse, update `PurchaseOrderItem.ReceivedQuantity`, update PO status (PartiallyReceived vs Received) | feature/US-12.3-TASK-1-goods-receipt-domain | 🟢 | Transactional — all updates in one `SaveChangesAsync`; reuses existing `InventoryItem` entity from Sprint 7 |
| TASK-12.3.3 | `GetGoodsReceiptsQuery` (list by PO or by date range) + `GetGoodsReceiptByIdQuery` | feature/US-12.3-TASK-1-goods-receipt-domain | 🟢 | Include receipt items + product names + warehouse names |
| TASK-12.3.4 | EF Core config (`GoodsReceiptConfiguration.cs`, `GoodsReceiptItemConfiguration.cs`) + migration `AddGoodsReceiptSchema` | feature/US-12.3-TASK-4-ef-config-migration | 🟢 | `goods_receipts` + `goods_receipt_items` tables; GoodsReceiptRepository; DbContext + DI updated |
| TASK-12.3.5 | `GoodsReceiptEndpoints.cs` — POST receive (creates receipt + updates inventory), GET receipts by PO, GET receipt by ID | feature/US-12.3-TASK-4-ef-config-migration | 🟢 | 3 endpoints at /api/v1/goods-receipts; receipts.view + receipts.create permissions; AddGoodsReceiptPermissionsSeed migration |
| TASK-12.3.6 | Unit tests for `ReceiveGoodsCommand` (full receipt, partial receipt, over-receipt rejected, wrong PO status rejected) | feature/US-12.3-TASK-6-unit-tests | 🟢 | 9 tests: full→Received, partial→PartiallyReceived, 2nd-partial completes, over-receipt, Draft+Cancelled rejected, PO not found, new/existing InventoryItem |

**Acceptance Criteria:**
- [x] Receiving goods against an Approved PO updates `InventoryItem.Quantity` in the specified warehouse
- [x] Partial receipt (some items received, not all) → PO status = `PartiallyReceived`
- [x] Full receipt (all items received) → PO status = `Received`
- [x] Over-receiving (more than ordered) returns validation error
- [x] Receipt against non-Approved PO returns error
- [x] Inventory update and receipt creation are atomic (single transaction)
- [x] `receipts.create`, `receipts.view` permissions seeded
- [x] Unit tests for all edge cases passing (9 tests)

---

## User Story 12.4: Purchase Management UI (Blazor)
**As a store owner, I want a complete Blazor UI for managing suppliers and purchase orders so that I can perform all purchasing workflows without using Postman or Swagger.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-12.4.1 | `ISupplierHttpService` + `SupplierHttpService` in BlazorApp (`Services/Suppliers/`) | feature/US-12.4-TASK-1-2-http-services | 🟢 | SupplierModels.cs (ListModel, Model, FormModel); CRUD calls to /api/v1/suppliers |
| TASK-12.4.2 | `IPurchaseOrderHttpService` + `PurchaseOrderHttpService` in BlazorApp (`Services/PurchaseOrders/`) | feature/US-12.4-TASK-1-2-http-services | 🟢 | PurchaseOrderModels.cs; Submit/Approve/Cancel/ReceiveGoods; ReceiveGoods posts to /api/v1/goods-receipts |
| TASK-12.4.3 | `SupplierList.razor` at `/proveedores` — MudDataGrid with search, add/edit/delete actions | feature/US-12.4-TASK-3-4-supplier-pages | 🟢 | MudTable + search debounce + pagination + inline delete dialog; follows CustomerList pattern |
| TASK-12.4.4 | `SupplierForm.razor` component — create/edit supplier modal with RFC validation | feature/US-12.4-TASK-3-4-supplier-pages | 🟢 | Pages at /proveedores/nueva + /proveedores/{Id}/editar; all fields + IsActive toggle on edit |
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
| US-12.2: Purchase Order Backend | P0 Critical | 8 | 🟢 Complete |
| US-12.3: Goods Receipt & Inventory Integration | P0 Critical | 5 | 🟢 Complete |
| US-12.4: Purchase Management UI | P1 High | 8 | 🔴 Not Started |
| **Total** | | **26** | |

**Recommended execution order:** US-12.1 → US-12.2 → US-12.3 (parallel with US-12.1 after TASK-12.1.3) → US-12.4
