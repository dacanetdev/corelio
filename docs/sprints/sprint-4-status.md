# Sprint 4: UI/UX Design System (Phase 1-2)

**Goal:** A distinctive "Industrial Terracotta" design system that differentiates Corelio from generic MudBlazor apps and supports multi-tenant branding.

**Duration:** 3 days
**Status:** 🟢 Completed (100%)
**Started:** 2026-01-27
**Total Story Points:** 26 pts (US-4.1: 5, US-4.2: 8, US-4.3: 5, US-4.4: 8)
**Completed:** 26/26 tasks (100%)

---

## User Story 4.1: Core Theme Infrastructure
**As a developer, I want a consistent MudBlazor theme with Corelio's brand colors and typography so that all pages have a professional, cohesive look from day one.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-4.1.1 | Configure `MudTheme` "Industrial Terracotta" — primary `#E74C3C`, secondary `#2C3E50` | `feature/US-4.1-core-theme` | 🟢 | |
| TASK-4.1.2 | Add CSS custom properties for design tokens (spacing, shadows, border-radius) | `feature/US-4.1-core-theme` | 🟢 | |
| TASK-4.1.3 | Load Inter font via Google Fonts CDN | `feature/US-4.1-core-theme` | 🟢 | |
| TASK-4.1.4 | Remove Bootstrap CSS (replaced entirely by MudBlazor + custom CSS) | `feature/US-4.1-core-theme` | 🟢 | |
| TASK-4.1.5 | Apply theme in `MainLayout.razor` via `MudThemeProvider` | `feature/US-4.1-core-theme` | 🟢 | |

**Acceptance Criteria:**
- [x] Primary color `#E74C3C` (Terracotta) applied globally
- [x] Inter font loads successfully on all pages
- [x] Bootstrap CSS completely removed — no conflicts
- [x] Zero compilation errors

---

## User Story 4.2: Authentication Pages Redesign
**As a new user, I want visually professional and distinctive login/register pages so that my first impression of Corelio is one of quality and trust.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-4.2.1 | Create `AuthLayout.razor` — split-screen layout with branded left panel | `feature/US-4.2-auth-redesign` | 🟢 | Gradient background, logo placeholder |
| TASK-4.2.2 | Redesign `Login.razor` with hero section, card form, fade-in animation | `feature/US-4.2-auth-redesign` | 🟢 | |
| TASK-4.2.3 | Redesign `Register.razor` with matching design system | `feature/US-4.2-auth-redesign` | 🟢 | |
| TASK-4.2.4 | Redesign `ForgotPassword.razor` and `ResetPassword.razor` | `feature/US-4.2-auth-redesign` | 🟢 | |
| TASK-4.2.5 | Add CSS animations (fade-in, slide-up on page load) | `feature/US-4.2-auth-redesign` | 🟢 | |
| TASK-4.2.6 | Test mobile responsiveness at 375px (iPhone SE) | `feature/US-4.2-auth-redesign` | 🟢 | Verified |

**Acceptance Criteria:**
- [x] Login page looks distinctive — not generic MudBlazor
- [x] Mobile-responsive at 375px width
- [x] Fade-in animation on page load
- [x] All auth pages use consistent `AuthLayout`

---

## User Story 4.3: Core Reusable Components
**As a developer, I want `PageHeader`, `LoadingState`, and `EmptyState` components so that all future Blazor pages have consistent headers, loading indicators, and empty states.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-4.3.1 | Create `PageHeader.razor` — title, description, breadcrumbs (`MudBreadcrumbs`), and actions slot | `feature/US-4.3-reusable-components` | 🟢 | |
| TASK-4.3.2 | Create `LoadingState.razor` — centered `MudProgressCircular` with localized message | `feature/US-4.3-reusable-components` | 🟢 | |
| TASK-4.3.3 | Create `EmptyState.razor` — icon, title, description, optional action button | `feature/US-4.3-reusable-components` | 🟢 | |
| TASK-4.3.4 | Enhance `TenantDisplay.razor` — pill-shaped badge with gradient, hover effects | `feature/US-4.3-reusable-components` | 🟢 | |
| TASK-4.3.5 | Enhance `UserDisplay.razor` — avatar with initials, dropdown menu (Profile/Settings/Logout) | `feature/US-4.3-reusable-components` | 🟢 | |
| TASK-4.3.6 | Add localization keys: `MyProfile`, `Settings`, `Home`, `Loading`, `NoItemsFound`, `NoDataAvailable`, `CreateFirst` | `feature/US-4.3-reusable-components` | 🟢 | |

**Acceptance Criteria:**
- [x] `PageHeader` supports breadcrumbs and custom action buttons
- [x] `LoadingState` and `EmptyState` usable in any page
- [x] `TenantDisplay` and `UserDisplay` look polished with hover effects
- [x] Zero compilation errors

---

## User Story 4.4: Multi-Tenant Theming Infrastructure
**As a tenant administrator, I want to customize the primary color of Corelio so that the app reflects my hardware store's brand.**

**Status:** 🟢 Completed

| Task ID | Task | Branch | Status | Notes |
|---------|------|--------|--------|-------|
| TASK-4.4.1 | Add branding fields to `TenantConfiguration`: `PrimaryColor`, `LogoUrl`, `UseCustomTheme` | `feature/US-4.4-tenant-theming` | 🟢 | |
| TASK-4.4.2 | Create `AddTenantBrandingFields` EF Core migration | `feature/US-4.4-tenant-theming` | 🟢 | |
| TASK-4.4.3 | Create `ITenantThemeService` interface and `TenantThemeDto` record | `feature/US-4.4-tenant-theming` | 🟢 | |
| TASK-4.4.4 | Implement `TenantThemeService` in Infrastructure with Redis caching (2-hour TTL) | `feature/US-4.4-tenant-theming` | 🟢 | |
| TASK-4.4.5 | Create `DynamicThemeService` in BlazorApp — generates `MudTheme` from tenant color with shade functions | `feature/US-4.4-tenant-theming` | 🟢 | |
| TASK-4.4.6 | Create `TenantThemeHttpService` in BlazorApp for API communication | `feature/US-4.4-tenant-theming` | 🟢 | |
| TASK-4.4.7 | Create `TenantThemeEndpoints.cs` — `GET /api/v1/tenants/theme` and `PUT /api/v1/tenants/theme` | `feature/US-4.4-tenant-theming` | 🟢 | |
| TASK-4.4.8 | Update `MainLayout.razor` — load and apply tenant theme dynamically on page load | `feature/US-4.4-tenant-theming` | 🟢 | |
| TASK-4.4.9 | Register `ITenantThemeService` and `DynamicThemeService` in DI (both methods) | `feature/US-4.4-tenant-theming` | 🟢 | |

**Acceptance Criteria:**
- [x] Tenant admin can change primary color via `PUT /api/v1/tenants/theme`
- [x] Theme change reflected immediately in Blazor UI
- [x] Theme cached in Redis — no DB hit on every page load
- [x] Default "Industrial Terracotta" (#E74C3C) applied for tenants without custom theme

---

**Sprint 4 Total: 26/26 SP delivered | All tasks complete**
