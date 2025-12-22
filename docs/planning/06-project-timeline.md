
**Project:** Corelio Multi-Tenant SaaS ERP
**Timeline:** 12 Weeks (60 Business Days)
**Start Date:** TBD (Upon Plan Approval)
**Target MVP Launch:** Week 12 End

---

## Timeline Overview

| Phase | Weeks | Duration | Deliverables |
|-------|-------|----------|--------------|
| Phase 1 | 1-2 | 10 days | Foundation, Aspire Setup, Multi-Tenancy, Auth |
| Phase 2 | 3-4 | 10 days | Product & Inventory Management |
| Phase 3 | 5 | 5 days | Customer Management |
| Phase 4 | 6-7 | 10 days | POS System |
| Phase 5 | 8 | 5 days | Sales Management (Quotes, Credit Notes) |
| Phase 6 | 9-10 | 10 days | CFDI Integration |
| Phase 7 | 11 | 5 days | Testing & Refinement |
| Phase 8 | 12 | 5 days | Deployment & Launch |
| **Total** | **12** | **60 days** | **Production-Ready MVP** |

---

## Phase 1: Foundation & Aspire Setup (Weeks 1-2)

**Duration:** 10 business days
**Team:** 2 Backend Developers, 1 DevOps Engineer

### Week 1 (Days 1-5)

**Day 1-2: Project Setup**
- [ ] Initialize Git repository
- [ ] Create .NET 10 Aspire solution structure
- [ ] Configure AppHost with PostgreSQL and Redis
- [ ] Set up CI/CD pipeline (GitHub Actions)
- [ ] Configure SonarQube for code quality
- [ ] Set up development environment documentation

**Deliverables:**
- ✅ Solution structure with 7 projects
- ✅ Aspire dashboard running at localhost:15888
- ✅ PostgreSQL and Redis containers orchestrated
- ✅ CI/CD pipeline executing on push

**Day 3-5: Domain & Database Foundation**
- [ ] Create base entities (BaseEntity, AuditableEntity, ITenantEntity)
- [ ] Implement core domain entities (Tenant, User, Role, Permission)
- [ ] Configure EF Core with PostgreSQL
- [ ] Create initial database migration (core tables)
- [ ] Seed system roles and permissions
- [ ] Unit tests for domain entities

**Deliverables:**
- ✅ 7 core tables created (tenants, users, roles, permissions, user_roles, role_permissions, tenant_configurations)
- ✅ Database seeded with system data
- ✅ >80% code coverage on domain layer

### Week 2 (Days 6-10)

**Day 6-8: Multi-Tenancy Implementation**
- [ ] Implement TenantService with resolution logic
- [ ] Create TenantMiddleware for HTTP pipeline
- [ ] Configure EF Core query filters (FIXED security vulnerability)
- [ ] Implement TenantInterceptor for save operations
- [ ] Distributed caching with Redis for tenant data
- [ ] Multi-tenancy isolation tests

**Deliverables:**
- ✅ Tenant resolution working (JWT → Header → Subdomain)
- ✅ Query filters auto-applied to all queries
- ✅ Save interceptor auto-sets TenantId
- ✅ 100% multi-tenancy isolation verified via tests

**Day 9-10: Authentication & Authorization**
- [ ] Implement JWT token generation service
- [ ] Create AuthController (login, register, refresh)
- [ ] Implement password hashing (bcrypt)
- [ ] Create tenant registration endpoint
- [ ] Implement role-based authorization
- [ ] Integration tests for auth flow

**Deliverables:**
- ✅ User registration working with email verification
- ✅ Login returns JWT with tenant_id claim
- ✅ Refresh token mechanism functional
- ✅ Role-based access control operational

**Phase 1 Milestone:**
- 🎯 Multi-tenant architecture complete and secure
- 🎯 User authentication functional
- 🎯 Aspire dashboard showing health metrics
- 🎯 Database schema foundation established

---

## Phase 2: Product & Inventory Management (Weeks 3-4)

**Duration:** 10 business days
**Team:** 2 Backend Developers, 1 Frontend Developer

### Week 3 (Days 11-15)

**Day 11-13: Product Management**
- [ ] Create Product domain entity with all fields
- [ ] Implement ProductCategory hierarchy (5 levels)
- [ ] Create product CQRS commands (Create, Update, Delete)
- [ ] Create product queries (List, GetById, Search)
- [ ] Add SAT product code validation
- [ ] Barcode generation support
- [ ] Unit tests for product logic

**Deliverables:**
- ✅ Product CRUD operations complete
- ✅ Category hierarchy working
- ✅ Product search functional (name, SKU, barcode)
- ✅ Barcode support integrated

**Day 14-15: Product UI (Blazor)**
- [ ] Create ProductList page with MudDataGrid
- [ ] Create ProductForm component (create/edit)
- [ ] Implement product search with autocomplete
- [ ] Add image upload component
- [ ] Product category selector (hierarchical)

**Deliverables:**
- ✅ Product management UI complete
- ✅ Search functional with <500ms response time
- ✅ Image upload working

### Week 4 (Days 16-20)

**Day 16-18: Inventory Management**
- [ ] Create Warehouse entity
- [ ] Implement InventoryItem tracking
- [ ] Create InventoryTransaction audit log
- [ ] Implement stock adjustment commands
- [ ] Create low-stock alert query
- [ ] Inventory movement reports

**Deliverables:**
- ✅ Multi-warehouse inventory tracking
- ✅ Stock adjustments with reason codes
- ✅ Inventory transaction history
- ✅ Low stock alerts functional

**Day 19-20: Inventory UI**
- [ ] Create inventory dashboard
- [ ] Stock adjustment form
- [ ] Inventory movement history
- [ ] Low stock alerts display

**Deliverables:**
- ✅ Inventory UI complete
- ✅ Real-time stock levels displayed

**Phase 2 Milestone:**
- 🎯 Product catalog functional with categories
- 🎯 Multi-warehouse inventory tracking operational
- 🎯 Stock adjustments working with audit trail

---

## Phase 3: Customer Management (Week 5)

**Duration:** 5 business days
**Team:** 1 Backend Developer, 1 Frontend Developer

### Week 5 (Days 21-25)

**Day 21-22: Customer Domain & API**
- [ ] Create Customer entity (individuals & businesses)
- [ ] Implement RFC and CURP validation
- [ ] Create customer CQRS commands/queries
- [ ] Customer search implementation
- [ ] CFDI preferences storage

**Deliverables:**
- ✅ Customer CRUD operations
- ✅ RFC/CURP validation working
- ✅ Customer search functional

**Day 23-25: Customer UI**
- [ ] Customer list page
- [ ] Customer form (create/edit)
- [ ] Customer search component
- [ ] Customer addresses management
- [ ] Quick customer creation modal (for POS)

**Deliverables:**
- ✅ Customer management UI complete
- ✅ Quick customer creation modal ready for POS integration

**Phase 3 Milestone:**
- 🎯 Customer profiles with CFDI data complete
- 🎯 RFC validation functional
- 🎯 Customer search working

---

## Phase 4: POS System (Weeks 6-7)

**Duration:** 10 business days
**Team:** 2 Backend Developers, 2 Frontend Developers

### Week 6 (Days 26-30)

**Day 26-28: POS Backend**
- [ ] Create Sale entity (POS, Invoice, Quote types)
- [ ] Implement SaleItem line items
- [ ] Create Payment entity
- [ ] Implement CreateSaleCommand with inventory reservation
- [ ] Tax calculation logic (IVA 16%)
- [ ] Payment processing (multiple methods)
- [ ] Receipt generation (PDF)

**Deliverables:**
- ✅ Sale creation with inventory deduction
- ✅ Payment processing (cash, card, transfer)
- ✅ Receipt PDF generation

**Day 29-30: POS UI Foundation**
- [ ] Create POS layout (product search + cart + payment)
- [ ] Implement product search component
- [ ] Shopping cart state management
- [ ] Cart item display with quantities

**Deliverables:**
- ✅ POS UI layout complete
- ✅ Product search integrated
- ✅ Cart functionality working

### Week 7 (Days 31-35)

**Day 31-33: POS Features**
- [ ] Barcode scanner integration
- [ ] Keyboard shortcuts implementation
- [ ] Discount application (item & sale level)
- [ ] Payment panel (all methods)
- [ ] Change calculation
- [ ] Quick customer creation integration

**Deliverables:**
- ✅ Barcode scanning functional
- ✅ All keyboard shortcuts working
- ✅ Discount features complete

**Day 34-35: POS Performance Optimization**
- [ ] Implement Redis caching for product search
- [ ] Optimize database queries (compiled queries)
- [ ] Load testing (target <3s checkout time)
- [ ] UI responsiveness improvements

**Deliverables:**
- ✅ Product search <500ms
- ✅ Complete sale <3 seconds
- ✅ Load test passed (100 concurrent users)

**Phase 4 Milestone:**
- 🎯 POS system functional with all payment methods
- 🎯 Sub-3-second checkout achieved
- 🎯 Barcode scanner working
- 🎯 Keyboard-driven interface complete

---

## Phase 5: Sales Management (Week 8)

**Duration:** 5 business days
**Team:** 1 Backend Developer, 1 Frontend Developer

### Week 8 (Days 36-40)

**Day 36-37: Quote Management**
- [ ] Create quote functionality (sale_type = 'quote')
- [ ] Quote to sale conversion command
- [ ] Quote expiration handling
- [ ] Stock reservation for quotes

**Deliverables:**
- ✅ Quote creation working
- ✅ Quote conversion to sale functional

**Day 38-40: Credit Notes & Sales History**
- [ ] Credit note creation (sale_type = 'credit_note')
- [ ] Reference to original sale
- [ ] Sales history with filters
- [ ] Sales reports (daily, weekly, monthly)

**Deliverables:**
- ✅ Credit notes functional
- ✅ Sales history with search/filters
- ✅ Basic reports available

**Phase 5 Milestone:**
- 🎯 Quote system operational
- 🎯 Credit notes working
- 🎯 Sales reporting functional

---

## Phase 6: CFDI Integration (Weeks 9-10)

**Duration:** 10 business days
**Team:** 2 Backend Developers, 1 Frontend Developer, 1 QA Engineer

### Week 9 (Days 41-45)

**Day 41-43: CFDI XML Generation**
- [ ] Implement CFDI 4.0 XML generator
- [ ] SAT schema validation
- [ ] Digital signature with CSD
- [ ] UUID generation
- [ ] QR code generation

**Deliverables:**
- ✅ CFDI XML generation functional
- ✅ SAT validation passing
- ✅ Digital signature working

**Day 44-45: Azure Key Vault Integration**
- [ ] Implement AzureKeyVaultCertificateService
- [ ] CSD certificate upload functionality
- [ ] Certificate loading at runtime
- [ ] Certificate expiration monitoring

**Deliverables:**
- ✅ Certificates stored in Azure Key Vault
- ✅ Certificate loading functional
- ✅ Expiration alerts configured

### Week 10 (Days 46-50)

**Day 46-48: PAC Integration**
- [ ] Implement Finkel PAC provider
- [ ] Stamping workflow
- [ ] Retry logic for failures
- [ ] Invoice cancellation
- [ ] PDF generation with stamp

**Deliverables:**
- ✅ PAC integration complete
- ✅ Invoice stamping functional
- ✅ Cancellation working

**Day 49-50: CFDI UI**
- [ ] Invoice generation form
- [ ] Invoice list with filters
- [ ] Invoice detail view (PDF preview)
- [ ] Cancellation interface
- [ ] Email delivery

**Deliverables:**
- ✅ CFDI UI complete
- ✅ Email delivery functional

**Phase 6 Milestone:**
- 🎯 CFDI 4.0 compliant invoices generated
- 🎯 PAC integration operational
- 🎯 Certificate security implemented
- 🎯 Invoice PDF with QR code

---

## Phase 7: Testing & Refinement (Week 11)

**Duration:** 5 business days
**Team:** 2 Developers, 2 QA Engineers

### Week 11 (Days 51-55)

**Day 51-52: Automated Testing**
- [ ] Increase unit test coverage to >70%
- [ ] Integration tests for all critical paths
- [ ] Multi-tenancy isolation tests
- [ ] Performance testing (load tests)
- [ ] Security testing (OWASP Top 10)

**Deliverables:**
- ✅ Test coverage >70%
- ✅ All critical paths tested
- ✅ Security vulnerabilities fixed

**Day 53-54: User Acceptance Testing (UAT)**
- [ ] Create demo tenant with sample data
- [ ] UAT with internal stakeholders
- [ ] Bug fixes from UAT
- [ ] UI/UX polish

**Deliverables:**
- ✅ Demo tenant ready
- ✅ UAT feedback addressed
- ✅ UI polished

**Day 55: Documentation**
- [ ] User manual (basic operations)
- [ ] Admin guide (tenant setup)
- [ ] API documentation (Swagger)
- [ ] Deployment runbook

**Deliverables:**
- ✅ Documentation complete

**Phase 7 Milestone:**
- 🎯 All tests passing
- 🎯 Performance targets met
- 🎯 Security audit passed
- 🎯 Documentation published

---

## Phase 8: Deployment & Launch (Week 12)

**Duration:** 5 business days
**Team:** 1 Developer, 1 DevOps Engineer, 1 QA Engineer

### Week 12 (Days 56-60)

**Day 56-57: Production Infrastructure**
- [ ] Provision Azure/DigitalOcean resources
- [ ] Configure PostgreSQL managed database
- [ ] Set up Redis cache
- [ ] Configure Azure Key Vault
- [ ] SSL certificates (Let's Encrypt or Azure)
- [ ] Domain configuration (corelio.com.mx)

**Deliverables:**
- ✅ Production environment ready
- ✅ SSL configured
- ✅ Monitoring configured (Aspire + Application Insights)

**Day 58: Database Migration & Seed**
- [ ] Run EF Core migrations on production DB
- [ ] Seed system data (roles, permissions, SAT catalogs)
- [ ] Backup strategy verified

**Deliverables:**
- ✅ Production database ready
- ✅ Seed data loaded

**Day 59: Deployment & Smoke Testing**
- [ ] Deploy API to production
- [ ] Deploy Blazor app to production
- [ ] Smoke tests on production
- [ ] Create first real tenant (pilot customer)
- [ ] Process test sale with CFDI

**Deliverables:**
- ✅ Application deployed
- ✅ Smoke tests passed
- ✅ Pilot tenant operational

**Day 60: MVP Launch**
- [ ] Final go/no-go decision
- [ ] Launch announcement
- [ ] Onboard first paying customers
- [ ] Monitor system health
- [ ] Support team ready

**Deliverables:**
- ✅ MVP launched to production
- ✅ First customers onboarded
- ✅ Monitoring active

**Phase 8 Milestone:**
- 🎯 Production deployment complete
- 🎯 MVP launched and accessible
- 🎯 First customers using system
- 🎯 Support processes in place

---

## Critical Path

The following tasks are on the critical path (blocking subsequent work):

1. **Days 1-2:** Aspire solution setup → Blocks all development
2. **Days 3-5:** Domain foundation → Blocks all business logic
3. **Days 6-8:** Multi-tenancy → Blocks all tenant-scoped features
4. **Days 26-30:** POS backend → Blocks POS UI and sales
5. **Days 41-43:** CFDI XML → Blocks PAC integration
6. **Days 56-57:** Infrastructure → Blocks deployment

**Risk Mitigation:**
- Daily standups to identify blockers
- Parallel work streams where possible (e.g., UI while backend in progress)
- Buffer built into each phase for unexpected issues

---

## Milestones Summary

| Milestone | Target Date | Success Criteria |
|-----------|-------------|------------------|
| M1: Foundation Complete | End Week 2 | Multi-tenancy and auth working |
| M2: Product & Inventory | End Week 4 | Product catalog and stock tracking operational |
| M3: Customer Management | End Week 5 | Customer profiles with CFDI data |
| M4: POS System | End Week 7 | Sub-3-second checkout achieved |
| M5: Sales Management | End Week 8 | Quotes and credit notes working |
| M6: CFDI Integration | End Week 10 | Invoices generated and stamped |
| M7: Testing Complete | End Week 11 | All tests passing, security audit passed |
| M8: MVP Launch | End Week 12 | Production deployment, first customers live |

---

## Assumptions

1. Team available full-time throughout 12 weeks
2. No major scope changes during implementation
3. .NET 10 and Aspire stable (no breaking changes)
4. Azure/DigitalOcean infrastructure available
5. PAC provider API stable and responsive
6. Stakeholder reviews completed within 48 hours

---

**Last Updated:** 2025-12-21
