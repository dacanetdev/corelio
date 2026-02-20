# Sprint 1: Foundation & Project Setup

**Goal:** A running .NET 10 Aspire solution with Clean Architecture, EF Core configured, multi-tenancy foundation, and seed data.

**Duration:** 3 days
**Status:** 🟢 Completed (100%)
**Started:** 2026-01-08
**Total Story Points:** 21 pts (US-1.1: 5, US-1.2: 5, US-1.3: 5, US-1.4: 6)
**Completed:** 34/34 tasks (100%)

---

## User Story 1.1: Create .NET 10 Solution Structure
**As a developer, I want a properly structured .NET 10 solution with Clean Architecture so that all future development has a solid, maintainable foundation.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-1.1.1 | Initialize Git repository with `.gitignore` and `.gitattributes` | `feature/US-1.1-solution-structure` | 🟢 | LF line endings enforced |
| TASK-1.1.2 | Create .NET 10 solution with 7 source projects (Domain, Application, Infrastructure, WebAPI, BlazorApp, AppHost, ServiceDefaults) | `feature/US-1.1-solution-structure` | 🟢 | |
| TASK-1.1.3 | Create 4 test projects (Domain.Tests, Application.Tests, Infrastructure.Tests, Integration.Tests) | `feature/US-1.1-solution-structure` | 🟢 | |
| TASK-1.1.4 | Configure project references following Clean Architecture dependency flow | `feature/US-1.1-solution-structure` | 🟢 | Domain → Application → Infrastructure → Presentation |
| TASK-1.1.5 | Configure `.editorconfig` with C# 14 style rules (file-scoped namespaces, primary constructors) | `feature/US-1.1-solution-structure` | 🟢 | |
| TASK-1.1.6 | Set up GitHub Actions CI/CD pipeline (build + test on PR) | `feature/US-1.1-solution-structure` | 🟢 | |

**Acceptance Criteria:**
- [x] `dotnet build` succeeds on all 11 projects
- [x] CI/CD pipeline triggers on push and runs all tests
- [x] Clean Architecture dependency flow enforced (no circular references)
- [x] `.editorconfig` enforces file-scoped namespaces and primary constructors

---

## User Story 1.2: Configure Aspire Orchestration
**As a developer, I want .NET Aspire to orchestrate PostgreSQL and Redis so that the development environment is reproducible and requires no manual setup.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-1.2.1 | Configure `Corelio.AppHost` with PostgreSQL 16 and Redis containers | `feature/US-1.2-aspire-orchestration` | 🟢 | |
| TASK-1.2.2 | Configure `Corelio.ServiceDefaults` with health checks, OpenTelemetry, service discovery | `feature/US-1.2-aspire-orchestration` | 🟢 | |
| TASK-1.2.3 | Wire WebAPI and BlazorApp to AppHost with service references | `feature/US-1.2-aspire-orchestration` | 🟢 | |
| TASK-1.2.4 | Verify Aspire dashboard at `http://localhost:15888` shows all services healthy | `feature/US-1.2-aspire-orchestration` | 🟢 | |

**Acceptance Criteria:**
- [x] `dotnet run --project src/Aspire/Corelio.AppHost` starts all services
- [x] Aspire dashboard accessible at `localhost:15888`
- [x] PostgreSQL and Redis containers start automatically
- [x] WebAPI and BlazorApp visible in Aspire service discovery

---

## User Story 1.3: Implement Base Domain Entities
**As a developer, I want base entity classes and core domain entities so that all future business entities follow consistent patterns.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-1.3.1 | Create `BaseEntity` with `Id` (Guid, `ValueGeneratedNever`) | `feature/US-1.3-domain-entities` | 🟢 | |
| TASK-1.3.2 | Create `AuditableEntity` extending `BaseEntity` with `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` | `feature/US-1.3-domain-entities` | 🟢 | |
| TASK-1.3.3 | Create `TenantAuditableEntity` extending `AuditableEntity` implementing `ITenantEntity` with `TenantId` | `feature/US-1.3-domain-entities` | 🟢 | |
| TASK-1.3.4 | Create 12 domain enums (`SubscriptionPlan`, `PaymentMethod`, `SaleType`, `UnitOfMeasure`, etc.) | `feature/US-1.3-domain-entities` | 🟢 | |
| TASK-1.3.5 | Create core entities: `Tenant`, `TenantConfiguration`, `User`, `Role`, `Permission` | `feature/US-1.3-domain-entities` | 🟢 | |
| TASK-1.3.6 | Create join entities: `UserRole`, `RolePermission`, `RefreshToken`, `AuditLog` | `feature/US-1.3-domain-entities` | 🟢 | |
| TASK-1.3.7 | Write 32 unit tests for entity constructors, properties, and invariants | `feature/US-1.3-domain-entities` | 🟢 | All passing |

**Acceptance Criteria:**
- [x] All entities inherit from appropriate base class
- [x] `ITenantEntity.TenantId` present on all business entities
- [x] 32 unit tests passing with 100% success rate
- [x] No circular dependencies in Domain project

---

## User Story 1.4: Configure EF Core with PostgreSQL
**As a developer, I want EF Core configured with multi-tenancy query filters and audit interceptors so that tenant isolation is enforced automatically at the database level.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-1.4.1 | Create `ApplicationDbContext` with `DbSet<T>` for all 9 domain entities | `feature/US-1.4-efcore-postgresql` | 🟢 | |
| TASK-1.4.2 | Create 9 Fluent API entity configurations with snake_case tables/columns | `feature/US-1.4-efcore-postgresql` | 🟢 | `HasColumnName` on all props |
| TASK-1.4.3 | Implement `ApplyTenantQueryFilters()` in `OnModelCreating` | `feature/US-1.4-efcore-postgresql` | 🟢 | Auto-applies `WHERE tenant_id = ?` |
| TASK-1.4.4 | Create `TenantInterceptor` — auto-sets `TenantId` on INSERT via `SaveChangesInterceptor` | `feature/US-1.4-efcore-postgresql` | 🟢 | |
| TASK-1.4.5 | Create `AuditInterceptor` — auto-sets `CreatedAt`, `UpdatedAt`, `CreatedBy` | `feature/US-1.4-efcore-postgresql` | 🟢 | |
| TASK-1.4.6 | Create `DesignTimeDbContextFactory` for EF Core migrations | `feature/US-1.4-efcore-postgresql` | 🟢 | |
| TASK-1.4.7 | Create `DependencyInjection.cs` in Infrastructure with both `IServiceCollection` and `IHostApplicationBuilder` registration methods | `feature/US-1.4-efcore-postgresql` | 🟢 | Registered in both methods |
| TASK-1.4.8 | Generate `InitialSchemaWithSeedData` migration (all tables + seed data) | `feature/US-1.4-efcore-postgresql` | 🟢 | Applied automatically on startup |
| TASK-1.4.9 | Create `DataSeeder` — 1 demo tenant, 3 roles, 17 permissions, 3 test users | `feature/US-1.4-efcore-postgresql` | 🟢 | admin/manager/cashier test users |
| TASK-1.4.10 | Write 12 unit tests for interceptors | `feature/US-1.4-efcore-postgresql` | 🟢 | All passing |

**Acceptance Criteria:**
- [x] EF Core migrations run successfully against PostgreSQL 16
- [x] Query filters auto-apply `WHERE tenant_id = ?` on all tenant entities
- [x] `TenantInterceptor` sets `TenantId` automatically on INSERT
- [x] `AuditInterceptor` sets audit fields automatically
- [x] Seed data creates demo tenant, 3 roles, 17 permissions, 3 test users on startup
- [x] 12 interceptor unit tests passing

---

**Sprint 1 Total: 21/21 SP delivered | 34/34 tasks complete | 44 tests passing**
