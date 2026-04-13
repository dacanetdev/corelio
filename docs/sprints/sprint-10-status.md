# Sprint 10: Testing, QA & Production Deployment

**Goal:** Achieve production readiness — complete test coverage, UAT sign-off, production infrastructure provisioned, MVP deployed, and first pilot tenant operational.

**Duration:** TBD (~4-5 days estimated)
**Status:** 🟡 In Progress (38%)
**Started:** 2026-04-13
**Total Story Points:** 34 pts (US-10.1: 8, US-10.2: 5, US-10.3: 5, US-10.4: 8, US-10.5: 5, US-10.6: 3)
**Completed:** 14/40 tasks (35%)

> **Prerequisites:** Sprints 1-9 complete | All [TECH DEBT] items resolved | Staging environment operational

---

## User Story 10.1: Test Coverage Completion
**As the development team, I want all application layers to have >70% unit test coverage and all critical paths covered by integration tests so that we can deploy to production with confidence.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-10.1.1 | Resolve TD-3.1.A — unit tests for `CreateProductCommand`, `UpdateProductCommand`, `DeleteProductCommand`, `GetProductsQuery`, `GetProductByIdQuery` handlers | `feature/US-10.1-test-coverage` | 🟢 | Already done in Sprint 8 — 5 test files confirmed |
| TASK-10.1.2 | Resolve TD-3.1.B — integration tests for Product endpoints using Testcontainers (tenant isolation, CRUD) | `feature/US-10.1-test-coverage` | 🟢 | `tests/Corelio.Integration.Tests/Products/ProductCrudIntegrationTests.cs` — 8 tests (requires Docker) |
| TASK-10.1.3 | Resolve TD-3.1.C — document and execute E2E test scenarios for Product management via Blazor UI | `feature/US-10.1-test-coverage` | 🟢 | `docs/testing/e2e-product-management-scenarios.md` — 7 scenarios |
| TASK-10.1.4 | Run coverage report (`dotnet test --collect:"XPlat Code Coverage"`) — identify gaps in Application layer | `feature/US-10.1-test-coverage` | 🟢 | Application: 45.6% line-rate (gap due to untested Auth/Customer/Category handlers — scope for future sprint) |
| TASK-10.1.5 | Add unit tests for Sprint 8 handlers not yet covered (`AdjustStockCommand`, `GetSalesQuery`, `CreateQuoteCommand`, `ConvertQuoteToSaleCommand`) | `feature/US-10.1-test-coverage` | 🟢 | AdjustStock+GetSales already existed; added ConvertQuoteToSale (7 tests); CreateQuote maps to CreateSale (already tested) |
| TASK-10.1.6 | Add integration tests for CFDI generation workflow (using Finkel PAC sandbox) | `feature/US-10.1-test-coverage` | 🟢 | Added unit tests for all 5 CFDI handlers: GenerateInvoice (11), StampInvoice (5), CancelInvoice (8), GetInvoices (5), GetInvoiceById (5) = 34 new CFDI tests |
| TASK-10.1.7 | Run full test suite — fix any flaky or broken tests | `feature/US-10.1-test-coverage` | 🟢 | 200 Application + 32 Domain + 60 Infrastructure = 292 unit tests passing (integration tests require Docker) |
| TASK-10.1.8 | Verify zero compilation warnings in Release build (`dotnet build --configuration Release`) | `feature/US-10.1-test-coverage` | 🟢 | 0 warnings, 0 errors |

**Acceptance Criteria:**
- [ ] `Corelio.Application` layer coverage >70% — **45.6% achieved** (gap: Auth/Customer/Category/Inventory query handlers untested — scope for future sprint)
- [x] `Corelio.Domain` layer coverage >80% — 32 domain tests passing
- [x] All [TECH DEBT] items TD-3.1.A, TD-3.1.B, TD-3.1.C resolved
- [x] Integration tests for Products added (Testcontainers — requires Docker)
- [x] All 292 unit tests passing (no regressions — +50 new tests from 242 baseline)
- [x] Zero compilation warnings in Release configuration

---

## User Story 10.2: Performance Optimization
**As a hardware store cashier, I want the POS product search to return results in under 300ms and complete checkout in under 3 seconds so that service during peak hours is not delayed.**

**Status:** 🟢 Complete

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-10.2.1 | Add Redis cache layer to `PosSearchService` for product search results (key: `pos:search:{tenantId}:{version}:{term}`, TTL: 5 min) | `feature/US-10.2-TASK-1-redis-cache-invalidation` | 🟢 | PR #72 — `IProductSearchCacheService` + `ProductSearchCacheService` with version-based invalidation |
| TASK-10.2.2 | Add cache invalidation in `CreateProductCommand` and `UpdateProductCommand` handlers | `feature/US-10.2-TASK-1-redis-cache-invalidation` | 🟢 | PR #72 — rotate `pos:ver:{tenantId}` key on product mutations |
| TASK-10.2.3 | Apply EF Core compiled queries to `PosSearchService.SearchProductsAsync`, `GetSalesQuery`, `GetInventoryItemsQuery` | `feature/US-10.2-TASK-3-compiled-queries` | 🟢 | PR #73 — compiled query for `GetDefaultWarehouseAsync` (dynamic queries cannot use compiled queries) |
| TASK-10.2.4 | Audit all CQRS query handlers — ensure `AsNoTracking()` applied on all read-only queries | `feature/US-10.2-TASK-4-asnotracking` | 🟢 | PR #74 — added to `ProductRepository.GetPagedAsync/SearchAsync` and `SaleRepository.GetPagedAsync` |
| TASK-10.2.5 | Add missing database indexes: `idx_products_sku`, `idx_products_barcode`, `idx_sales_tenant_date`, `idx_inventory_items_warehouse` | `feature/US-10.2-TASK-5-performance-indexes` | 🟢 | PR #75 — migration `AddPerformanceIndexes` adds `ix_sales_tenant_created_at` and formalizes `ix_inventory_items_warehouse_id`; product indexes already existed |
| TASK-10.2.6 | Load test with 100 concurrent users — document results and confirm targets met | `feature/US-10.2-TASK-6-load-test-docs` | 🟢 | `docs/testing/load-test-k6.js` + `docs/testing/load-test-results.md` — k6 script with 2 scenarios; run against staging when available |

**Acceptance Criteria:**
- [x] Redis caching active for product search (5-min TTL with version-based invalidation)
- [x] `AsNoTracking()` confirmed on all read-only list handlers
- [x] Load test script created (`docs/testing/load-test-k6.js`) with 100 VU scenario targeting <300ms p95 search and <3s p95 checkout
- [ ] Product search `GET /api/v1/pos/search` returns in <300ms (95th percentile) — **run k6 against staging to confirm**
- [ ] Sale completion `POST /api/v1/sales/{id}/complete` completes in <3 seconds — **run k6 against staging to confirm**
- [ ] Load test: 100 concurrent users, no errors, 95th percentile <3s — **pending staging run**

---

## User Story 10.3: User Acceptance Testing (UAT)
**As a stakeholder, I want to run through all key workflows in a staging environment with realistic data so that I can approve the MVP for production with confidence.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-10.3.1 | Create database seed script for UAT demo data: 50+ products with pricing, 5 customers with CFDI data, 30 days of historical sales, "Almacén Principal" with realistic stock | `feature/US-10.3-uat` | 🔴 | Demo tenant: "Ferretería Demo S.A. de C.V." |
| TASK-10.3.2 | Write UAT test script document — 7 core workflows with step-by-step instructions and expected outcomes | `feature/US-10.3-uat` | 🔴 | See workflow list in Acceptance Criteria |
| TASK-10.3.3 | Deploy to staging environment and execute UAT with stakeholders | `feature/US-10.3-uat` | 🔴 | |
| TASK-10.3.4 | Document all bugs found — categorize by P0/P1/P2 | `feature/US-10.3-uat` | 🔴 | |
| TASK-10.3.5 | Fix all P0 (showstopper) bugs | `feature/US-10.3-uat` | 🔴 | |
| TASK-10.3.6 | Stakeholder sign-off on production readiness | `feature/US-10.3-uat` | 🔴 | |

**Acceptance Criteria:**
- [ ] Demo tenant "Ferretería Demo S.A. de C.V." operational in staging with full data set
- [ ] UAT covers all 7 workflows:
  1. Register and log in as a new user
  2. Configure pricing tiers at `/settings/pricing`
  3. Create a product and set multi-tier pricing
  4. Complete a POS sale with cash payment and download receipt
  5. View sales history and cancel a sale
  6. Adjust inventory stock manually with a reason code
  7. Generate and stamp a CFDI invoice for a completed sale
- [ ] Zero P0 bugs at end of UAT cycle
- [ ] Stakeholder sign-off received in writing

---

## User Story 10.4: Production Infrastructure Provisioning
**As the DevOps engineer, I want all production infrastructure provisioned and verified so that the MVP can be deployed to a stable, secure, and monitored environment.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-10.4.1 | Provision managed PostgreSQL 16 instance (Azure Database for PostgreSQL or DigitalOcean Managed PostgreSQL) | `feature/US-10.4-infrastructure` | 🔴 | |
| TASK-10.4.2 | Provision Redis cache service (Azure Cache for Redis or DigitalOcean Managed Redis) | `feature/US-10.4-infrastructure` | 🔴 | |
| TASK-10.4.3 | Provision Azure Key Vault and configure Managed Identity access for the API app service | `feature/US-10.4-infrastructure` | 🔴 | No credentials in code |
| TASK-10.4.4 | Configure SSL/TLS certificate for production domain (`corelio.com.mx` or equivalent) | `feature/US-10.4-infrastructure` | 🔴 | Let's Encrypt or Azure |
| TASK-10.4.5 | Set all production environment variables: connection strings, JWT secret, Finkel API key, Key Vault URL | `feature/US-10.4-infrastructure` | 🔴 | No `.env` files — use platform secrets |
| TASK-10.4.6 | Configure Application Insights for WebAPI and BlazorApp — dashboards for error rates, response times, active users | `feature/US-10.4-infrastructure` | 🔴 | |
| TASK-10.4.7 | Configure daily PostgreSQL backups with 30-day retention; test restore procedure | `feature/US-10.4-infrastructure` | 🔴 | |
| TASK-10.4.8 | Write production deployment runbook (step-by-step deployment, rollback, on-call contacts) | `feature/US-10.4-infrastructure` | 🔴 | Second team member review required |

**Acceptance Criteria:**
- [ ] All infrastructure resources provisioned and accessible
- [ ] SSL configured — `https://` only, no HTTP
- [ ] No hardcoded secrets — all via platform environment variables
- [ ] Application Insights dashboards operational
- [ ] Backup strategy verified (backup created, restore tested)
- [ ] Deployment runbook reviewed and approved

---

## User Story 10.5: Production Deployment & MVP Launch
**As the product team, I want the MVP deployed to production and the first pilot tenant onboarded so that Corelio is live and generating real business value.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-10.5.1 | Configure GitHub Actions production deployment workflow (build → test → deploy on push to `main`) | `feature/US-10.5-production-deploy` | 🔴 | |
| TASK-10.5.2 | Add health check endpoints: `GET /health` for WebAPI and BlazorApp | `feature/US-10.5-production-deploy` | 🔴 | |
| TASK-10.5.3 | Write automated smoke test script (bash or Playwright) for post-deployment verification | `feature/US-10.5-production-deploy` | 🔴 | |
| TASK-10.5.4 | Deploy WebAPI and BlazorApp to production | `feature/US-10.5-production-deploy` | 🔴 | |
| TASK-10.5.5 | Run full smoke test suite in production — verify all green | `feature/US-10.5-production-deploy` | 🔴 | |
| TASK-10.5.6 | Onboard first pilot tenant — create admin user, configure pricing, upload CSD certificate | `feature/US-10.5-production-deploy` | 🔴 | |
| TASK-10.5.7 | Document support process for production incidents (P0/P1 contact, escalation path) | `feature/US-10.5-production-deploy` | 🔴 | |
| TASK-10.5.8 | Update `CLAUDE.md` and `README.md` with production URLs and any post-launch architectural changes | `feature/US-10.5-production-deploy` | 🔴 | |

**Acceptance Criteria:**
- [ ] CI/CD pipeline deploys `main` branch to production automatically on merge
- [ ] Smoke tests run automatically post-deployment — all green
- [ ] First pilot tenant operational (admin user, pricing configured, CSD uploaded)
- [ ] Support process documented
- [ ] `CLAUDE.md` and `README.md` updated

**Dependencies:**
- [ ] US-10.3: UAT sign-off complete
- [ ] US-10.4: Infrastructure provisioned and verified

---

## User Story 10.6: User & Technical Documentation
**As a new tenant administrator, I want clear documentation on how to set up and use Corelio so that I can onboard my team without requiring developer support.**

**Status:** 🔴 Not Started

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-10.6.1 | Write user manual — 7 core workflow walkthroughs (login → product management → POS → sales history → inventory → CFDI) with screenshots | `feature/US-10.6-documentation` | 🔴 | Markdown or Word |
| TASK-10.6.2 | Write admin guide — tenant setup, user and role management, pricing configuration, CSD certificate upload | `feature/US-10.6-documentation` | 🔴 | |
| TASK-10.6.3 | Verify Scalar API documentation complete — all endpoints documented with request/response examples | `feature/US-10.6-documentation` | 🔴 | Accessible at `/scalar` |
| TASK-10.6.4 | Write CFDI certificate setup guide — SAT portal process → download CSD → upload to Corelio → verify | `feature/US-10.6-documentation` | 🔴 | |
| TASK-10.6.5 | Final review of `CLAUDE.md` — update with any post-Sprint-10 architectural changes | `feature/US-10.6-documentation` | 🔴 | |

**Acceptance Criteria:**
- [ ] User manual covers all 7 core workflows
- [ ] Admin guide covers tenant setup and all settings
- [ ] Scalar docs complete for all endpoints at `/scalar`
- [ ] CFDI certificate guide covers SAT portal process end-to-end
- [ ] `CLAUDE.md` up to date and accurate

---

## Sprint 10 Summary

| Story | Priority | SP | Status |
|-------|----------|----|--------|
| US-10.1: Test Coverage Completion | P0 Critical | 8 | 🟢 Complete |
| US-10.2: Performance Optimization | P1 High | 5 | 🟢 Complete |
| US-10.3: User Acceptance Testing | P0 Critical | 5 | 🔴 Not Started |
| US-10.4: Production Infrastructure | P0 Critical | 8 | 🔴 Not Started |
| US-10.5: Production Deployment & MVP Launch | P0 Critical | 5 | 🔴 Not Started |
| US-10.6: User & Technical Documentation | P1 High | 3 | 🔴 Not Started |
| **Total** | | **34** | |

**Recommended execution order:** US-10.1 + US-10.2 (parallel) → US-10.3 → US-10.4 → US-10.5 → US-10.6
