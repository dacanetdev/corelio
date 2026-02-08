# Sprint 6: Tenant-Configurable Pricing Module - Execution Plan

## Sprint Metadata

**Sprint Number:** Sprint 6
**Sprint Goal:** Replace FERRESYS's rigid 6-tier pricing structure with a tenant-configurable pricing module
**Start Date:** 2026-02-06
**Estimated End Date:** 2026-02-24 (2-3 weeks, 49 Story Points)
**Team Capacity:** ~27.5 SP/week average velocity (Sprint 6 = 49 SP ≈ 1.8 sprints)

**Epic:** EPIC-PRICING-001 - Tenant-Configurable Pricing Module
**Total Story Points:** 49 SP (60-74 estimated hours)

---

## Sprint Overview

### The Problem

**Current State:**
- Corelio stores only a single `SalePrice` field on Product entity (no pricing tiers)
- FERRESYS uses a rigid 6-tier cascading discount system + 5-tier margin/price structure
- All FERRESYS users forced into 6-tier model even if they only need 2-3 tiers
- No bulk price update feature in FERRESYS (tedious manual updates)
- Product form cluttered with all fields on one page

**Desired Future State:**
- **Flexible tier configuration:** Tenants choose 1-6 discount tiers and 1-5 margin tiers per business needs
- **Named tiers:** "Mayoreo", "Distribuidor", "Público General" instead of "Tier 1, Tier 2"
- **Accurate FERRESYS pricing math:** Cascading discounts, margin calculations, IVA (16% VAT) support
- **Efficient bulk updates:** Change 100s of product prices in seconds (%, fixed amount, new margin)
- **Organized UI:** Tabbed product form (Datos/Costos), dedicated price change screen
- **Multi-tenancy security:** Tenant A cannot see/modify Tenant B's pricing configuration

### Success Metrics

**Technical Metrics:**
- [ ] Database migration applies successfully with 5 new tables + 3 new product columns
- [ ] >90% code coverage on PricingCalculationService (cascading discount math)
- [ ] >70% code coverage on Application layer pricing code (handlers, validators)
- [ ] Multi-tenancy isolation verified via integration tests (100% pass rate)
- [ ] Zero compilation errors, solution builds successfully
- [ ] All API endpoints documented in Swagger/Scalar

**Business Metrics:**
- [ ] Tenant can configure 1-6 discount tiers and 1-5 margin tiers with custom names
- [ ] Pricing calculations match FERRESYS logic with 100% accuracy:
  - NetCost = ListPrice × (1-D1/100) × (1-D2/100) × ... × (1-Dn/100) ✅
  - SalePrice = NetCost / (1 - MarginPercent/100) ✅
  - PriceWithIva = SalePrice × 1.16 ✅
- [ ] Bulk price change updates 100 products in < 10 seconds
- [ ] Spanish (es-MX) localization complete (~160 keys)
- [ ] Mobile-responsive UI (tested at 375px width - iPhone SE)

**Demo-Ready Criteria:**
- [ ] Stakeholder can configure pricing tiers at /settings/pricing
- [ ] Stakeholder can create a product with multi-tier pricing in ProductForm (Costos tab)
- [ ] Stakeholder can filter products and update prices inline at /pricing
- [ ] Stakeholder can apply bulk price change to 50 products in < 5 seconds
- [ ] All UI text displays in Spanish with proper localization
- [ ] No backend-only workarounds required (full Blazor UI implemented)

---

## User Story Breakdown & Sequencing

### Week 1: Foundation & Business Logic (Days 1-5)

**Day 1-2: US-6.1 - Pricing Domain Model & Infrastructure (8 SP)**
- **Tasks:**
  - Create 5 new domain entities (TenantPricingConfiguration, DiscountTierDefinition, MarginTierDefinition, ProductDiscount, ProductMarginPrice)
  - Modify Product entity (add ListPrice, NetCost, IvaEnabled, navigation properties)
  - Create 2 repository interfaces + implementations
  - Create 5 EF Core configurations with constraints, indexes, query filters
  - Update ApplicationDbContext (5 DbSets, query filters)
  - Create database migration AddPricingModuleSchema
  - Register repositories in DI
- **Acceptance Criteria:** Migration applies successfully, multi-tenancy query filters active, repositories functional
- **Dependencies:** None (foundational work)
- **Risks:** Migration failure in existing database - Mitigated by testing on dev environment first
- **Deliverables:** 5 tables created, Product entity enhanced, repositories ready

**Day 3-5: US-6.2 - Pricing Calculation Engine & CQRS (13 SP)**
- **Tasks:**
  - Implement PricingCalculationService (static class with 6 pure functions)
  - Write 13+ unit tests for calculation service (>90% coverage)
  - Create 10 DTOs (config, tiers, product pricing, discounts, margins, bulk update, preview)
  - Implement 3 queries (GetTenantPricingConfig, GetProductPricing, GetProductsPricingList)
  - Implement 4 commands (UpdateTenantPricingConfig, UpdateProductPricing, BulkUpdatePricing, CalculatePrices)
  - Create 3 validators (UpdateTenantPricingConfig, UpdateProductPricing, BulkUpdatePricing)
  - Write unit tests for all handlers and validators
- **Acceptance Criteria:** All unit tests passing, calculation service matches FERRESYS math exactly, >70% coverage on handlers
- **Dependencies:** US-6.1 (domain entities and repositories)
- **Risks:** Calculation accuracy - Mitigated by extensive unit tests with edge cases (0%, 100% discounts, division by zero)
- **Deliverables:** Complete business logic layer, all calculations verified

**Week 1 Checkpoint:**
- [ ] Database schema complete and deployed to dev environment
- [ ] All business logic implemented and tested
- [ ] Backend foundation ready for API layer
- [ ] Team velocity tracking: 21 SP completed (43% of sprint)

---

### Week 2: API, Configuration UI, and Product UI (Days 6-14)

**Day 6-7: US-6.3 - Pricing API & Service Layer (5 SP)**
- **Tasks:**
  - Create PricingEndpoints.cs with 7 Minimal API endpoints
  - Create 4 request/response contracts
  - Create 9 Blazor models (mirroring DTOs)
  - Implement IPricingService interface + PricingService (HttpClient wrapper)
  - Register service in Blazor Program.cs with DI
  - Create Pricing.http file for manual API testing
- **Acceptance Criteria:** All 7 endpoints functional in Swagger, authorization enforced, PricingService successfully calls APIs
- **Dependencies:** US-6.2 (CQRS handlers)
- **Risks:** Permission checks missing - Mitigated by testing with different user roles (admin, manager, cashier)
- **Deliverables:** Complete API layer, Blazor service ready for UI integration

**Day 8-9: US-6.4 - Pricing Configuration UI (5 SP)**
- **Tasks:**
  - Create PricingSettings.razor at /settings/pricing
  - Implement dynamic tier fields (discount count 1-6, margin count 1-5)
  - Add ~60 Spanish localization keys to SharedResource.es-MX.resx
  - Implement form validation (tier counts, names, IVA percentage)
  - Add "Precios" nav menu item under "Configuración" section
  - Test mobile responsive design (375px, 768px, 1920px)
- **Acceptance Criteria:** Tenant admin can configure pricing tiers, dynamic fields appear/disappear based on counts, all text in Spanish
- **Dependencies:** US-6.3 (IPricingService)
- **Risks:** Dynamic UI complexity - Mitigated by testing tier count changes thoroughly (3→5→2→6 sequence)
- **Deliverables:** Functional tenant pricing configuration page

**Day 10-14: US-6.5 - Product Pricing Management UI (13 SP)**
- **Tasks:**
  - Create ProductCostos.razor reusable component (discount tier inputs, margin tier displays, ListPrice, NetCost, IVA toggle)
  - Modify ProductForm.razor with MudTabs (Datos/Costos tabs)
  - Create PriceChange.razor page at /pricing (filterable product list, expandable rows, inline editing)
  - Create BulkPriceChangeDialog.razor (update types: %, fixed amount, new margin; preview calculations)
  - Add ~100 Spanish localization keys
  - Test mobile responsive design for all components
  - Add "Cambio de Precios" nav menu item
- **Acceptance Criteria:** ProductForm has tabbed layout, Costos tab shows all pricing tiers, PriceChange page allows inline editing, bulk dialog updates 50+ products successfully
- **Dependencies:** US-6.3 (IPricingService), US-6.4 (tenant config must exist before managing product pricing)
- **Risks:** UI complexity (4 components, ~100 localization keys) - Mitigated by breaking work into subtasks (ProductCostos Day 10-11, ProductForm tabs Day 12, PriceChange Day 13, BulkDialog Day 14)
- **Deliverables:** Complete product pricing UI, demo-ready feature

**Week 2 Checkpoint:**
- [ ] All UI components implemented and tested
- [ ] Spanish localization complete (~160 keys total)
- [ ] Demo-ready: Stakeholder can configure pricing and manage product prices
- [ ] Team velocity tracking: 46 SP completed (94% of sprint)

---

### Week 3: Testing, Polish, and Completion (Days 15-16 + Buffer)

**Day 15-16: US-6.6 - Pricing Module Testing (5 SP)**
- **Tasks:**
  - Create handler tests (7 test files, 20+ test methods)
  - Create validator tests (3 test files, 15+ test methods)
  - Create integration tests for multi-tenancy isolation (2 test files)
  - Create E2E workflow tests (3 test files: create config → product pricing, bulk update, IVA calculation)
  - Generate code coverage report (target >70%)
  - Fix any test failures or coverage gaps
- **Acceptance Criteria:** All tests passing, >70% coverage on Application layer pricing code, multi-tenancy isolation verified
- **Dependencies:** US-6.1 through US-6.5 (all code to test must exist)
- **Risks:** Test coverage gaps - Mitigated by prioritizing critical business logic (calculation service, handlers)
- **Deliverables:** Comprehensive test suite, coverage report

**Buffer Days (Day 17-18):**
- Bug fixes from testing
- UI polish (spacing, alignment, responsiveness)
- Performance optimization if bulk update too slow
- Documentation updates (CLAUDE.md, API docs)
- Demo preparation (sample tenant data, test scenarios)

**Week 3 Checkpoint:**
- [ ] All 6 user stories completed (49 SP)
- [ ] Zero critical bugs
- [ ] Demo script prepared
- [ ] Sprint retrospective conducted

---

## Story Sequencing Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│ Week 1: Foundation & Business Logic (21 SP)                     │
├─────────────────────────────────────────────────────────────────┤
│ Day 1-2: US-6.1 (8 SP)                                          │
│   └─> Domain Entities → Repositories → Migration               │
│                                                                  │
│ Day 3-5: US-6.2 (13 SP)                                         │
│   └─> Calculation Service → CQRS → Validators → Unit Tests     │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ Week 2: API & UI (23 SP)                                        │
├─────────────────────────────────────────────────────────────────┤
│ Day 6-7: US-6.3 (5 SP)                                          │
│   └─> API Endpoints → Blazor Service                           │
│                                                                  │
│ Day 8-9: US-6.4 (5 SP)      ← Can start in parallel with 6.3   │
│   └─> PricingSettings.razor                                    │
│                                                                  │
│ Day 10-14: US-6.5 (13 SP)                                       │
│   └─> ProductCostos → ProductForm Tabs → PriceChange → Bulk    │
└─────────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────────┐
│ Week 3: Testing & Polish (5 SP + Buffer)                        │
├─────────────────────────────────────────────────────────────────┤
│ Day 15-16: US-6.6 (5 SP)                                        │
│   └─> Handler Tests → Validator Tests → Integration Tests      │
│                                                                  │
│ Day 17-18: Buffer                                               │
│   └─> Bug Fixes → UI Polish → Demo Prep                        │
└─────────────────────────────────────────────────────────────────┘
```

**Parallelization Opportunities:**
- US-6.3 and US-6.4 can be worked on simultaneously (different developers)
- US-6.2 unit tests can be written while implementing handlers (TDD approach)
- Integration tests (US-6.6) can start during Week 2 (don't wait for all UI)

**Critical Path:**
US-6.1 → US-6.2 → US-6.3 → US-6.5 (13+13+5 = 31 SP on critical path)

---

## Risk Assessment & Mitigation

### High Risks

| Risk | Likelihood | Impact | Mitigation Strategy | Owner |
|------|------------|--------|---------------------|-------|
| **Database migration fails in existing database** | Medium | High | Test migration on fresh dev environment first, document rollback plan, schedule during low-traffic window | Backend Developer |
| **Calculation accuracy mismatch with FERRESYS** | Medium | High | Extensive unit tests with FERRESYS test data, validate with domain expert, document formula in code comments | Backend Developer |
| **Scope creep (e.g., requests for promotional pricing, quantity-based discounts)** | Medium | High | Clearly document out-of-scope items, create future backlog items, refer stakeholders to Epic scope | Product Owner |

### Medium Risks

| Risk | Likelihood | Impact | Mitigation Strategy | Owner |
|------|------------|--------|---------------------|-------|
| **Bulk update performance too slow (>10s for 500 products)** | Medium | Medium | Implement in batches with async processing, show progress indicator, optimize DB queries with indexes | Backend Developer |
| **UI complexity causes timeline slippage** | Medium | Medium | Break US-6.5 into 4 daily subtasks (ProductCostos, Tabs, PriceChange, Bulk), prioritize critical components first | Frontend Developer |
| **Spanish localization incomplete or incorrect** | Low | Medium | Use localization checklist, review with native Spanish speaker, test with Spanish UI tester | Frontend Developer |

### Low Risks

| Risk | Likelihood | Impact | Mitigation Strategy | Owner |
|------|------------|--------|---------------------|-------|
| **Multi-tenancy isolation bug (tenant A sees tenant B's pricing)** | Low | High | Comprehensive integration tests, code review focusing on query filters, manual security testing | Backend Developer |
| **Test coverage <70%** | Low | Low | Write tests incrementally during implementation (TDD), prioritize critical business logic, use code coverage tools | QA/Developer |

**Risk Heatmap:**

```
Impact
High    │  🔴Migration  🔴Accuracy  🔴Scope
        │
Medium  │  🟡BulkPerf   🟡UI        🟡L10n
        │
Low     │  🟠Tenancy    🟢Coverage
        └─────────────────────────
         Low    Medium    High
              Likelihood
```

**Overall Risk Level:** 🟡 **MEDIUM** - Well-defined scope but significant technical complexity (5 new entities, 160 localization keys, complex UI)

---

## Definition of Done (Sprint-Level)

### Code Implementation
- [ ] All 6 user stories marked as "Completed" in SPRINT_STATUS.md
- [ ] Database migration applied successfully to dev, staging environments
- [ ] All 5 new tables created with proper indexes, constraints, query filters
- [ ] Product entity enhanced with ListPrice, NetCost, IvaEnabled fields
- [ ] PricingCalculationService implemented with 6 static methods (cascading discounts, margin calculations, IVA)
- [ ] 7 CQRS handlers implemented (3 queries, 4 commands)
- [ ] 3 validators implemented with comprehensive validation rules
- [ ] 7 API endpoints functional in Swagger/Scalar
- [ ] 3 Blazor pages/components created (PricingSettings, ProductCostos, PriceChange, BulkPriceChangeDialog)
- [ ] ProductForm.razor modified with MudTabs (Datos/Costos)
- [ ] ~160 Spanish localization keys added to SharedResource.es-MX.resx
- [ ] Clean Architecture followed (proper layer separation)
- [ ] C# 14 features used (primary constructors, collection expressions, file-scoped namespaces)
- [ ] CQRS pattern followed (commands vs queries)
- [ ] Multi-tenancy properly enforced (ITenantEntity implemented on all 5 new entities)

### Testing
- [ ] >90% code coverage on PricingCalculationService (13+ unit tests)
- [ ] >70% code coverage on Application layer pricing code (handlers, validators)
- [ ] 7 handler test files created (20+ test methods)
- [ ] 3 validator test files created (15+ test methods)
- [ ] 2 integration test files for multi-tenancy isolation
- [ ] 3 E2E workflow tests (create config, bulk update, IVA calculation)
- [ ] All tests passing in CI/CD pipeline (zero failures)
- [ ] Integration tests verify multi-tenancy isolation (Tenant A cannot see Tenant B's pricing)
- [ ] Zero compilation warnings

### Security
- [ ] All 7 API endpoints require authorization (401 Unauthorized if no JWT)
- [ ] All endpoints enforce permissions (403 Forbidden if missing "pricing.view", "pricing.manage", etc.)
- [ ] Multi-tenancy query filters active on all 5 new entities (verified via SQL inspection)
- [ ] No SQL injection vulnerabilities (parameterized queries via EF Core)
- [ ] Sensitive data encrypted (not applicable - pricing is business data, not PII)
- [ ] No hardcoded secrets (use Key Vault or user secrets)

### User Experience (Demo-Ready)
- [ ] **Feature is demo-able** (stakeholder can configure pricing and manage product prices via Blazor UI)
- [ ] **All UI text in Spanish (es-MX)** via IStringLocalizer (~160 keys)
- [ ] **Forms have validation** with Spanish error messages (tier counts, discount %, margin %, names)
- [ ] **Feature works end-to-end:**
  - Tenant admin configures 3 discount tiers + 3 margin tiers at /settings/pricing → Saved successfully
  - Product manager creates product with pricing in ProductForm → Costos tab shows all tiers → Saved successfully
  - Product manager filters products at /pricing → Expands row → Edits pricing inline → Saved successfully
  - Product manager selects 50 products → Opens BulkPriceChangeDialog → Applies 10% increase → 50 products updated in <5 seconds
- [ ] **Responsive design** (works at 375px iPhone SE, 768px iPad, 1920px desktop)
- [ ] **MudBlazor components** used consistently with project design system
- [ ] Date format: dd/MM/yyyy (Mexican locale) - if applicable to pricing UI
- [ ] Currency format: $1,234.56 MXN (ListPrice, NetCost, SalePrice formatted correctly)

### Documentation
- [ ] **API documentation updated** (Swagger/Scalar annotations on all 7 endpoints)
- [ ] **Pricing.http file created** for manual API testing (7 requests with sample JSON)
- [ ] **SPRINT_STATUS.md updated** with Sprint 6 completion status
- [ ] **CLAUDE.md updated** if new patterns introduced (e.g., static calculation services)
- [ ] **User-facing documentation drafted** (how to configure pricing tiers, how to use bulk update)

### Deployment Readiness
- [ ] Code reviewed and approved (PR merged to main)
- [ ] All CI/CD checks passing (build, tests, SonarQube quality gates)
- [ ] Deployed to staging environment
- [ ] Smoke tested in staging (create tenant config, create product with pricing, bulk update 10 products)
- [ ] Rollback plan documented (migration rollback steps, feature flag if applicable)

### Demo Script Checklist
- [ ] Sample tenant created with 3 discount tiers ("Mayoreo 10%", "Distribuidor 5%", "Minorista 2%")
- [ ] Sample tenant has 3 margin tiers ("Precio Mayoreo 20%", "Precio Distribuidor 30%", "Precio Público 40%")
- [ ] 10 sample products created with full pricing data
- [ ] Demo scenario 1: Configure pricing tiers at /settings/pricing
- [ ] Demo scenario 2: Create new product with pricing in ProductForm Costos tab
- [ ] Demo scenario 3: Update pricing for existing product at /pricing (inline edit)
- [ ] Demo scenario 4: Bulk update 10 products with 10% price increase (show before/after)
- [ ] Demo scenario 5: Verify multi-tenancy (login as Tenant B, cannot see Tenant A's pricing)

---

## Daily Standup Template

### Day 1-2 (US-6.1)
**Yesterday:** Sprint 6 kickoff, planning session
**Today:**
- Create 5 domain entities (TenantPricingConfiguration, tier definitions, product pricing)
- Create repository interfaces and implementations
- Create EF Core configurations with constraints
- Create database migration
**Blockers:** None expected
**Risks:** Migration may need adjustment based on existing schema

### Day 3-5 (US-6.2)
**Yesterday:** Domain entities and migration complete
**Today:**
- Implement PricingCalculationService (cascading discounts, margin calcs, IVA)
- Write unit tests (>90% coverage)
- Create DTOs (10 total)
- Implement CQRS handlers (3 queries, 4 commands)
- Create validators (3 total)
**Blockers:** None expected
**Risks:** Calculation accuracy - validate with FERRESYS test data

### Day 6-7 (US-6.3)
**Yesterday:** Business logic complete, all unit tests passing
**Today:**
- Create PricingEndpoints.cs (7 Minimal API endpoints)
- Create request/response contracts (4 total)
- Implement Blazor PricingService (HttpClient wrapper)
- Create Pricing.http for manual testing
**Blockers:** None expected
**Risks:** Permission checks - test with different user roles

### Day 8-9 (US-6.4)
**Yesterday:** API layer complete
**Today:**
- Create PricingSettings.razor page
- Implement dynamic tier fields (1-6 discounts, 1-5 margins)
- Add ~60 Spanish localization keys
- Test mobile responsive design
**Blockers:** None expected
**Risks:** Dynamic UI complexity - test tier count changes thoroughly

### Day 10-14 (US-6.5)
**Yesterday:** Tenant pricing configuration page complete
**Today:**
- Day 10-11: Create ProductCostos.razor component
- Day 12: Modify ProductForm.razor with MudTabs
- Day 13: Create PriceChange.razor page
- Day 14: Create BulkPriceChangeDialog.razor
- Add ~100 Spanish localization keys
**Blockers:** None expected
**Risks:** UI complexity - break into daily subtasks to track progress

### Day 15-16 (US-6.6)
**Yesterday:** All UI components complete
**Today:**
- Write handler tests (7 files, 20+ tests)
- Write validator tests (3 files, 15+ tests)
- Write integration tests (multi-tenancy isolation, E2E workflows)
- Generate code coverage report
**Blockers:** None expected
**Risks:** Test coverage gaps - prioritize critical business logic

### Day 17-18 (Buffer)
**Yesterday:** All tests passing, coverage >70%
**Today:**
- Bug fixes from testing
- UI polish (spacing, alignment, mobile responsiveness)
- Performance optimization (bulk update <10s)
- Demo preparation
**Blockers:** None expected
**Risks:** None - buffer for unexpected issues

---

## Demo Script for Stakeholders

### Setup (Pre-Demo)
1. Login as tenant admin: admin@demo.corelio.app / Admin123!
2. Ensure tenant has no pricing configuration yet (new tenant or reset)
3. Prepare 10 sample products in "Herramientas" category (SKU: HERR-001 through HERR-010)

### Demo Scenario 1: Configure Pricing Tiers (5 minutes)
**Narrative:** "As a hardware store owner, I want to configure my business's pricing structure with 3 discount tiers and 3 margin tiers."

**Steps:**
1. Navigate to Configuración → Precios (/settings/pricing)
2. Show empty/default configuration form
3. Set "Número de Niveles de Descuento" to 3
4. Name discount tiers:
   - Tier 1: "Descuento Mayoreo" (Wholesale discount)
   - Tier 2: "Descuento Distribuidor" (Distributor discount)
   - Tier 3: "Descuento Minorista" (Retail discount)
5. Set "Número de Niveles de Margen/Precio" to 3
6. Name margin tiers:
   - Tier 1: "Precio Mayoreo" (Wholesale price)
   - Tier 2: "Precio Distribuidor" (Distributor price)
   - Tier 3: "Precio Público" (Retail price)
7. Set "IVA Activado por Defecto" to ON (16%)
8. Click "Guardar Cambios"
9. **Expected Result:** Success Snackbar "Configuración guardada exitosamente", page refreshes with saved values

### Demo Scenario 2: Create Product with Pricing (5 minutes)
**Narrative:** "Now I'll create a new product (hammer) and set up its pricing with all three tiers."

**Steps:**
1. Navigate to Productos → Nuevo Producto (/products/new)
2. **Tab 1: Datos Generales**
   - Nombre: "Martillo de Carpintero 16oz"
   - SKU: "HERR-011"
   - Categoría: "Herramientas"
   - Precio (old field - skip for now)
3. **Tab 2: Costos y Precios** (click tab)
4. Set "Precio Lista": $500.00 (list price before discounts)
5. Set discount percentages:
   - Descuento Mayoreo: 10%
   - Descuento Distribuidor: 5%
   - Descuento Minorista: 2%
6. **Expected Calculation:** NetCost = 500 × 0.90 × 0.95 × 0.98 = $420.21 (auto-calculated, displayed as read-only)
7. Set margin percentages:
   - Precio Mayoreo: 20% → SalePrice = 420.21 / 0.80 = $525.26 → PriceWithIva = $609.30
   - Precio Distribuidor: 30% → SalePrice = 420.21 / 0.70 = $600.30 → PriceWithIva = $696.35
   - Precio Público: 40% → SalePrice = 420.21 / 0.60 = $700.35 → PriceWithIva = $812.41
8. Toggle "IVA Activado" to verify IVA recalculates (ON → PriceWithIva populated, OFF → PriceWithIva null)
9. Click "Guardar"
10. **Expected Result:** Product saved successfully, navigate to product list, product appears with correct data

### Demo Scenario 3: Price Change Screen - Inline Edit (3 minutes)
**Narrative:** "I need to update the pricing for an existing product (screwdriver). I'll use the price change screen."

**Steps:**
1. Navigate to Cambio de Precios (/pricing)
2. Filter by category: "Herramientas"
3. Search: "destornillador" (screwdriver)
4. **Expected Result:** Table shows filtered products with columns: Nombre, SKU, Precio Lista, Costo Neto
5. Click expand icon on first product row
6. **Expected Result:** ProductCostos component expands inline (shows all discount/margin tiers)
7. Click "Editar" (Edit icon)
8. Change "Descuento Mayoreo" from 10% to 12%
9. **Expected Calculation:** NetCost recalculates automatically (cascading effect), all SalePrice tiers recalculate
10. Click "Guardar" (Save inline)
11. **Expected Result:** Success Snackbar "Precios actualizados exitosamente", row collapses, new NetCost displayed in table

### Demo Scenario 4: Bulk Price Change (7 minutes)
**Narrative:** "My supplier increased costs by 8%. I need to update 50 products at once. I'll use the bulk price change feature."

**Steps:**
1. On /pricing page, select 10 products (checkboxes)
2. Click "Cambio Masivo" button
3. **Expected Result:** BulkPriceChangeDialog opens
4. Dialog shows: "Productos Seleccionados: 10"
5. Select update type: "Aumento Porcentual" (Percentage Increase)
6. Enter value: 8 (%)
7. **Expected Result:** Preview section shows sample calculation:
   - Product: "Martillo de Carpintero 16oz"
   - Before: Precio Lista $500.00 → NetCost $420.21 → Precio Público $700.35
   - After: Precio Lista $540.00 → NetCost $453.83 → Precio Público $756.38
8. Click "Aplicar Cambios"
9. **Expected Result:** Confirmation dialog: "¿Está seguro de aplicar estos cambios a 10 productos?"
10. Click "Confirmar"
11. **Expected Result:** Loading spinner, progress indicator (if >100 products)
12. **Expected Result:** Success Snackbar "Cambios aplicados exitosamente a 10 productos"
13. Verify table: All 10 products now show updated Precio Lista and Costo Neto
14. **Performance Metric:** Entire operation <5 seconds for 10 products, <10 seconds for 100 products

### Demo Scenario 5: Multi-Tenancy Security (3 minutes)
**Narrative:** "Let me verify that Tenant A cannot see or modify Tenant B's pricing configuration."

**Steps:**
1. Note current tenant name in header (e.g., "Demo Hardware Store")
2. Logout
3. Login as different tenant: manager@tenant2.corelio.app / Manager123! (Tenant B)
4. Navigate to Configuración → Precios
5. **Expected Result:** Empty configuration form (Tenant B has no config yet) OR Tenant B's own configuration (not Tenant A's)
6. Navigate to Cambio de Precios (/pricing)
7. **Expected Result:** Product list shows only Tenant B's products (not Tenant A's)
8. Attempt to navigate directly to Tenant A's product by URL manipulation (if possible)
9. **Expected Result:** 404 Not Found or 403 Forbidden (multi-tenancy isolation enforced)
10. **Conclusion:** Tenant isolation verified ✅

### Demo Wrap-Up (2 minutes)
**Key Takeaways:**
- ✅ Flexible tier configuration (1-6 discounts, 1-5 margins, not forced into rigid 6-tier model)
- ✅ Named tiers with business-meaningful names ("Mayoreo", "Distribuidor")
- ✅ Accurate FERRESYS pricing math (cascading discounts, margin calculations, IVA)
- ✅ Efficient bulk updates (10% price increase to 50 products in <10 seconds)
- ✅ Organized UI (tabbed product form, dedicated price change screen)
- ✅ Multi-tenancy security (Tenant A cannot see Tenant B's pricing)
- ✅ Spanish (es-MX) localization complete
- ✅ Mobile-responsive (demo on tablet or resize browser to 375px)

**Total Demo Time:** ~25 minutes

---

## Rollback Plan

### Scenario 1: Critical Bug Discovered Post-Deployment
**Symptoms:** Pricing calculations incorrect, data corruption, multi-tenancy breach

**Rollback Steps:**
1. **Immediate:** Deploy previous version of application (before Sprint 6)
   - Rollback WebAPI deployment (previous Docker image or IIS deployment)
   - Rollback BlazorApp deployment
2. **Database:** Rollback migration if data corruption suspected
   ```bash
   dotnet ef database update PreviousMigrationName --project src/Infrastructure/Corelio.Infrastructure --startup-project src/WebAPI/Corelio.WebAPI
   ```
   - **Warning:** This will DROP all 5 pricing tables and 3 product columns (ListPrice, NetCost, IvaEnabled)
   - **Data Loss:** All pricing configurations and product pricing data will be lost
   - **Mitigation:** Take database backup before rollback
3. **Communication:** Notify stakeholders of rollback, estimated fix timeline
4. **Investigation:** Identify root cause, create hotfix branch, re-test, re-deploy

**Rollback Time:** ~30 minutes (application rollback + database rollback)

### Scenario 2: Performance Issue (Bulk Update Too Slow)
**Symptoms:** Bulk update takes >60 seconds for 100 products (unacceptable)

**Mitigation Steps (No Rollback Required):**
1. **Immediate:** Add loading spinner and progress indicator (UX improvement)
2. **Short-term:** Optimize DB queries
   - Add indexes on ProductId, TierNumber (product_discounts, product_margin_prices)
   - Use bulk insert instead of loop (EF Core BulkExtensions library)
3. **Long-term:** Implement background job processing (Hangfire or Azure Functions)
   - Bulk update queued as background job
   - User notified via email/notification when complete
4. **Feature Flag:** Add "BulkUpdateAsync" feature flag to enable background processing

**Fix Time:** ~2-4 hours (optimization) or ~1 day (background job implementation)

### Scenario 3: Migration Fails in Production
**Symptoms:** `dotnet ef database update` fails with constraint violation or schema mismatch

**Recovery Steps:**
1. **Do NOT proceed with deployment** (application deployment depends on migration success)
2. **Investigate:** Check migration logs, PostgreSQL error messages
3. **Common Causes:**
   - Existing data violates new constraints (e.g., Product.ListPrice cannot be null but existing products have null)
   - Schema mismatch (migration generated on different EF Core version)
4. **Fix:**
   - Update migration to handle existing data (set default values, nullable columns)
   - Test migration on staging environment first
   - Re-generate migration if schema mismatch
5. **Re-apply:** Run updated migration on production
6. **Verify:** Check all 5 tables created, 3 columns added to products, all indexes/constraints in place

**Recovery Time:** ~1-2 hours (investigate + fix + re-apply)

---

## Post-Sprint Activities

### Sprint Retrospective Agenda (1 hour)

**What Went Well?** (15 minutes)
- What technical decisions paid off?
- What processes helped us move faster?
- What collaboration patterns worked well?

**What Could Be Improved?** (15 minutes)
- What slowed us down?
- What technical debt did we create?
- What would we do differently next time?

**Action Items for Next Sprint:** (15 minutes)
- Process improvements
- Technical improvements
- Documentation improvements

**Metrics Review:** (15 minutes)
- Actual velocity vs planned (49 SP)
- Test coverage achieved (target >70%)
- Bug count (target <5 critical bugs)
- Code review time (target <24 hours per PR)

### Sprint Review / Demo Preparation

**Stakeholder Demo:** (30 minutes)
- Run through demo script (5 scenarios)
- Prepare sample data (tenant config, 10 products)
- Test demo on staging environment
- Prepare Q&A responses (common questions about FERRESYS pricing)

**Demo Materials:**
- [ ] Slides: Sprint 6 overview, goals, achievements
- [ ] Live demo environment (staging)
- [ ] Backup: Screen recording of demo scenarios (in case live demo fails)
- [ ] Metrics slide: 49 SP delivered, >70% test coverage, 160 localization keys

### Documentation Updates

**CLAUDE.md Updates:**
- [ ] Add pricing module patterns (static calculation services, multi-tier pricing)
- [ ] Update database schema documentation (link to pricing tables)
- [ ] Add pricing API endpoints to API documentation section

**SPRINT_STATUS.md Updates:**
- [ ] Mark Sprint 6 as 🟢 Completed
- [ ] Update velocity metrics (Sprint 6: 49 SP completed)
- [ ] Add Sprint 6 to Completed Work Log

**User Documentation:**
- [ ] Create "Pricing Configuration Guide" (tenant admin audience)
- [ ] Create "Product Pricing Management Guide" (product manager audience)
- [ ] Create "Bulk Price Change Guide" with examples

### Handoff to Next Sprint

**Sprint 7 Planning:**
- [ ] Review Product Backlog (Sprint 6 Inventory & Customers was replaced, restore to Sprint 7?)
- [ ] Identify dependencies on pricing module (e.g., POS system may need pricing tier selection)
- [ ] Estimate Sprint 7 stories (target ~27.5 SP based on average velocity)

**Technical Debt Backlog:**
- [ ] Create future stories for pricing module enhancements:
  - Promotional pricing (time-bound discounts)
  - Quantity-based pricing (buy 10+ get 5% extra discount)
  - Customer-specific pricing overrides
  - Price history and audit trail (beyond standard AuditableEntity)
  - Logo upload functionality for tenant theming (from Sprint 4-5 backlog)

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-02-06 | Development Team | Initial Sprint 6 execution plan created |

---

## Appendix: Useful Resources

### FERRESYS Pricing Reference
- **Cascading Discount Formula:** NetCost = ListPrice × ∏(1 - Di/100) for i=1 to N
- **Margin Formula:** SalePrice = NetCost / (1 - MarginPercent/100)
- **IVA Formula:** PriceWithIva = SalePrice × (1 + IvaPercent/100), typically 1.16 for 16%

### PostgreSQL Query Examples

**Verify Multi-Tenancy Query Filters:**
```sql
-- Should show WHERE tenant_id filter in generated SQL
EXPLAIN SELECT * FROM discount_tier_definitions;
```

**Check Pricing Configuration for Tenant:**
```sql
SELECT tpc.*, dtd.tier_number, dtd.tier_name, mtd.tier_number, mtd.tier_name
FROM tenant_pricing_configurations tpc
LEFT JOIN discount_tier_definitions dtd ON tpc.id = dtd.tenant_pricing_configuration_id
LEFT JOIN margin_tier_definitions mtd ON tpc.id = mtd.tenant_pricing_configuration_id
WHERE tpc.tenant_id = 'TENANT-GUID-HERE'
ORDER BY dtd.tier_number, mtd.tier_number;
```

**Product Pricing Data:**
```sql
SELECT p.name, p.sku, p.list_price, p.net_cost, p.iva_enabled,
       pd.tier_number AS discount_tier, pd.discount_percentage,
       pmp.tier_number AS margin_tier, pmp.margin_percentage, pmp.sale_price, pmp.price_with_iva
FROM products p
LEFT JOIN product_discounts pd ON p.id = pd.product_id
LEFT JOIN product_margin_prices pmp ON p.id = pmp.product_id
WHERE p.tenant_id = 'TENANT-GUID-HERE' AND p.id = 'PRODUCT-GUID-HERE'
ORDER BY pd.tier_number, pmp.tier_number;
```

### Code Coverage Commands

**Generate Coverage Report:**
```bash
dotnet test --collect:"XPlat Code Coverage"
```

**View Coverage in HTML:**
```bash
reportgenerator -reports:"**\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
start coveragereport\index.html
```

---

**Document Status:** ✅ Ready for Sprint 6 Execution
**Last Updated:** 2026-02-06
**Prepared By:** Development Team (Product Owner / Scrum Master Mode)
**For:** Sprint 6 Team
