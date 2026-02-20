# Sprint 2: Multi-Tenancy & Authentication

**Goal:** Full end-to-end authentication and multi-tenancy — users can log in via Blazor UI, not just Postman.

**Duration:** 2 days
**Status:** 🟢 Completed (100%)
**Started:** 2026-01-12
**Total Story Points:** 34 pts (US-2.1: 13, US-2.1.1: 5, US-2.2: 8, US-2.2.1: 8)
**Completed:** 34/34 tasks (100%)

---

## User Story 2.1: Multi-Tenancy Services Implementation (Backend)
**As a developer, I want a robust multi-tenancy service that resolves the current tenant from JWT, headers, or subdomain, with Redis caching, so that all downstream services automatically operate in the correct tenant context.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-2.1.1 | Create `ITenantService` interface in Application layer | `feature/US-2.1-multi-tenancy-backend` | 🟢 | |
| TASK-2.1.2 | Implement `TenantService` — resolves tenant from JWT `tenant_id` claim → `X-Tenant-Id` header → subdomain | `feature/US-2.1-multi-tenancy-backend` | 🟢 | |
| TASK-2.1.3 | Create `TenantMiddleware` — resolves and caches tenant per HTTP request | `feature/US-2.1-multi-tenancy-backend` | 🟢 | |
| TASK-2.1.4 | Integrate Redis distributed cache for tenant data (2-hour TTL) | `feature/US-2.1-multi-tenancy-backend` | 🟢 | `Aspire.StackExchange.Redis` |
| TASK-2.1.5 | Register `ITenantService` in `DependencyInjection.cs` (both `IServiceCollection` and `IHostApplicationBuilder` methods) | `feature/US-2.1-multi-tenancy-backend` | 🟢 | |
| TASK-2.1.6 | Write 12 unit tests for tenant resolution logic | `feature/US-2.1-multi-tenancy-backend` | 🟢 | All passing |

**Acceptance Criteria:**
- [x] Tenant resolved from JWT `tenant_id` claim (primary)
- [x] Fallback to `X-Tenant-Id` header then subdomain
- [x] Tenant data cached in Redis — no DB hit on every request
- [x] 12 unit tests passing with 100% success rate

---

## User Story 2.1.1: Multi-Tenancy Frontend (Tenant Display)
**As a user, I want to see my current tenant and user information in the app header so that I know which hardware store account I'm operating in.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-2.1.1.1 | Create `TenantDisplay.razor` — reads `tenant_name` from JWT claims and renders pill badge | `feature/US-2.1.1-tenant-display` | 🟢 | |
| TASK-2.1.1.2 | Create `UserDisplay.razor` — reads full name and email from JWT claims | `feature/US-2.1.1-tenant-display` | 🟢 | |
| TASK-2.1.1.3 | Integrate both components into `MainLayout.razor` header | `feature/US-2.1.1-tenant-display` | 🟢 | |

**Acceptance Criteria:**
- [x] Tenant name visible in header after login
- [x] User name and email visible in header
- [x] Components read JWT claims without additional API calls
- [x] Header displayed on all authenticated pages

---

## User Story 2.2: Authentication & Authorization (Backend)
**As a user, I want to securely log in with email and password and receive a JWT token so that I can access my tenant's data with proper role-based permissions.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-2.2.1 | Create `IJwtService` interface and `JwtService` implementation (generate + validate tokens) | `feature/US-2.2-auth-backend` | 🟢 | RS256 signing |
| TASK-2.2.2 | Create `AuthEndpoints.cs` — POST /auth/login, /auth/register, /auth/refresh, /auth/logout, /auth/forgot-password, /auth/reset-password | `feature/US-2.2-auth-backend` | 🟢 | 6 endpoints |
| TASK-2.2.3 | Implement BCrypt password hashing with work factor 12 | `feature/US-2.2-auth-backend` | 🟢 | `BCrypt.Net-Next` |
| TASK-2.2.4 | Implement refresh token mechanism with SHA256 hashing and token rotation | `feature/US-2.2-auth-backend` | 🟢 | 7-day expiry |
| TASK-2.2.5 | Create 24+ RBAC authorization policies (e.g., `products.view`, `sales.create`) | `feature/US-2.2-auth-backend` | 🟢 | |
| TASK-2.2.6 | Replace Swagger with Scalar for API documentation | `feature/US-2.2-auth-backend` | 🟢 | Accessible at `/scalar` |
| TASK-2.2.7 | Write 35 authentication unit tests (JWT generation, BCrypt, refresh tokens) | `feature/US-2.2-auth-backend` | 🟢 | All passing |

**Acceptance Criteria:**
- [x] Login returns JWT with 1-hour expiry and refresh token with 7-day expiry
- [x] JWT contains: `sub`, `tenant_id`, `email`, `roles`, `permissions` claims
- [x] Password hashed with BCrypt work factor 12
- [x] Refresh token rotates on use (old token invalidated)
- [x] 35 authentication tests passing

---

## User Story 2.2.1: Authentication Frontend (Blazor UI)
**As a user, I want Blazor login, register, and password-reset pages so that I can access Corelio without using Postman or Swagger.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-2.2.1.1 | Add `Blazored.LocalStorage` and `System.IdentityModel.Tokens.Jwt` NuGet packages | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.2 | Create `ITokenService` + `TokenService` (store/retrieve/validate JWT in localStorage) | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.3 | Create `IAuthService` + `AuthService` (HTTP calls to backend auth endpoints) | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.4 | Create `CustomAuthenticationStateProvider` (parse JWT claims for Blazor auth state) | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.5 | Create `AuthorizationMessageHandler` — auto-attach JWT to requests, auto-refresh on 401 | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.6 | Create `Login.razor` (email/password form with Spanish validation messages) | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.7 | Create `Register.razor` (admin-only user creation form) | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.8 | Create `ForgotPassword.razor` and `ResetPassword.razor` | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.9 | Create `Logout.razor` and `AccessDenied.razor` | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.10 | Update `App.razor` — add MudBlazor CSS/JS and `CascadingAuthenticationState` | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.11 | Update `Routes.razor` — add `AuthorizeRouteView` with redirect to `/login` for unauthenticated | `feature/US-2.2.1-auth-frontend` | 🟢 | |
| TASK-2.2.1.12 | Add 47 es-MX localization keys to `SharedResource.es-MX.resx` | `feature/US-2.2.1-auth-frontend` | 🟢 | Login, Register, validation messages |

**Acceptance Criteria:**
- [x] User can log in via Blazor UI (no Postman required)
- [x] Unauthenticated users redirected to `/login`
- [x] JWT token stored in localStorage and auto-attached to API requests
- [x] Token auto-refreshes before expiry
- [x] All UI text in Spanish (es-MX) via `IStringLocalizer`
- [x] Zero compilation errors

---

**Sprint 2 Total: 34/34 SP delivered | 100 tests passing (44 + 12 + 35 + 9 new)**
