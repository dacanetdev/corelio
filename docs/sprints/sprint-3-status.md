# Sprint 3: Products & Categories

**Goal:** A fully functional product catalog with hierarchical categories, CRUD API, and Blazor UI — the first business-domain feature stakeholders can interact with.

**Duration:** 5 days
**Status:** ⚠️ Partially Complete (7/10 tasks — tests pending)
**Started:** 2026-01-13
**Total Story Points:** 21 pts (US-3.1: 21)
**Completed:** 7/10 tasks (70%) — API and UI complete, tests deferred

> ⚠️ **Tech Debt:** 3 tasks remain incomplete (TASK-3.1.8, 3.1.9, 3.1.10). Deferred to Sprint 10 (integration/E2E) and Sprint 8 (unit tests). See [PRODUCT_BACKLOG.md](../backlog/PRODUCT_BACKLOG.md) for tracking.

---

## User Story 3.1: Product Management API & UI
**As an inventory manager, I want to create, update, search, and delete products with hierarchical categories so that the hardware store catalog is fully managed in Corelio.**

**Status:** ⚠️ Partially Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-3.1.1 | Create `Product` entity with all catalog fields (SKU, barcode, name, description, unit, category) | `feature/US-3.1-product-management` | 🟢 | Inherits `TenantAuditableEntity` |
| TASK-3.1.2 | Create `ProductCategory` entity with self-referential parent (up to 5 hierarchy levels) | `feature/US-3.1-product-management` | 🟢 | Soft-delete, sort order |
| TASK-3.1.3 | Create CQRS handlers for products: `CreateProduct`, `UpdateProduct`, `DeleteProduct`, `GetProducts`, `GetProductById`, `SearchProducts` | `feature/US-3.1-product-management` | 🟢 | FluentValidation on commands |
| TASK-3.1.4 | Create CQRS handlers for categories: `CreateCategory`, `UpdateCategory`, `DeleteCategory`, `GetCategories`, `GetCategoryById` | `feature/US-3.1-product-management` | 🟢 | |
| TASK-3.1.5 | Create `ProductEndpoints.cs` — 6 Minimal API endpoints (GET list paginated, GET by id, GET search, POST, PUT, DELETE) | `feature/US-3.1-product-management` | 🟢 | |
| TASK-3.1.6 | Create `ProductCategoryEndpoints.cs` — 5 endpoints (GET list, GET by id, POST, PUT, DELETE) | `feature/US-3.1-product-management` | 🟢 | |
| TASK-3.1.7 | Create `ProductList.razor` and `ProductForm.razor` Blazor pages (design system applied in Sprint 5) | `feature/US-3.1-product-management` | 🟢 | Tabbed form (Datos/Costos) |
| TASK-3.1.8 | Unit tests for Product and ProductCategory CQRS handlers (>70% coverage) | — | 🔴 | **[TECH DEBT TD-3.1.A] → Recommended Sprint 8** |
| TASK-3.1.9 | Integration tests for Product endpoints with multi-tenancy isolation (Testcontainers) | — | 🔴 | **[TECH DEBT TD-3.1.B] → Sprint 10** |
| TASK-3.1.10 | E2E manual test scenarios documented and executed via Blazor UI | — | 🔴 | **[TECH DEBT TD-3.1.C] → Sprint 10** |

**Acceptance Criteria:**
- [x] Product CRUD operations available via API and Blazor UI
- [x] Category hierarchy (up to 5 levels) supported
- [x] Product search by name, SKU, barcode via `GET /api/v1/products/search`
- [x] Soft delete — deleted products not returned in queries
- [x] Multi-tenancy enforced — `Product` inherits `TenantAuditableEntity`
- [ ] Unit test coverage >70% on Application layer handlers — **PENDING**
- [ ] Integration tests verify tenant isolation for Product endpoints — **PENDING**
- [ ] E2E scenarios documented and passed — **PENDING**

---

**Sprint 3 Status: 7/10 tasks complete. 3 test tasks deferred.**
**Tech Debt tickets:** TD-3.1.A (unit tests → Sprint 8), TD-3.1.B (integration tests → Sprint 10), TD-3.1.C (E2E → Sprint 10)
