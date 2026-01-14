# US-2.2.1: Blazor Authentication UI Implementation

**Epic:** Authentication & Authorization
**Sprint:** Sprint 2 (Frontend Debt) or Sprint 3 (High Priority)
**Story Points:** 13 SP
**Priority:** CRITICAL (Blocks Sprint 3)
**Status:** 🔴 Not Started
**Created:** 2026-01-13
**Dependencies:** US-2.2 (Backend Authentication) ✅ Complete

---

## User Story

**As a** tenant administrator managing my hardware store,
**I want** a complete Spanish-language authentication UI in Blazor,
**So that** I can securely log in to access my store's data and manage my account without relying on API testing tools.

---

## Business Value

- **Stakeholder Impact:** Enables stakeholder demos (system is currently unusable without UI)
- **Sprint 3 Blocker:** Product Management cannot be tested without login UI
- **User Experience:** Eliminates need for Postman/Swagger workarounds
- **Definition of Done:** Completes US-2.2 properly (backend was only 50% of the feature)

---

## Acceptance Criteria

### ✅ Scenario 1: User Login (Happy Path)
**Given** I am a registered tenant administrator with valid credentials
**And** I navigate to the Blazor application at `http://localhost:5000`
**When** I am redirected to `/auth/login` because I am unauthenticated
**And** I enter my email "admin@ferreterialopez.mx" and password in Spanish-labeled fields
**And** I click "Iniciar Sesión"
**Then** I should see a success notification "Inicio de sesión exitoso"
**And** I should be redirected to the home page (`/`)
**And** my JWT access token and refresh token should be stored in localStorage
**And** the header should display my tenant name "Ferretería López" and my user name "Juan López"

### ✅ Scenario 2: User Login (Invalid Credentials)
**Given** I am on the login page
**When** I enter an invalid email or incorrect password
**And** I click "Iniciar Sesión"
**Then** I should see an error message "Inicio de sesión fallido. Verifica tus credenciales."
**And** I should remain on the login page
**And** no tokens should be stored in localStorage

### ✅ Scenario 3: Validation Errors
**Given** I am on the login page
**When** I submit the form without entering an email
**Then** I should see a validation message "Este campo es requerido" under the email field
**When** I enter an invalid email format (e.g., "notanemail")
**Then** I should see a validation message "Formato de correo inválido"
**When** I enter a password with fewer than 8 characters
**Then** I should see a validation message "La contraseña debe tener al menos 8 caracteres"

### ✅ Scenario 4: Automatic Token Refresh
**Given** I am logged in with a valid JWT token that expires in 1 minute
**When** I navigate to a protected page requiring authentication
**Then** the AuthorizationMessageHandler should automatically refresh my token using the refresh token
**And** the new JWT access token should be stored in localStorage
**And** the page should load successfully without requiring me to log in again

### ✅ Scenario 5: Forgot Password Flow
**Given** I am on the login page
**When** I click the "¿Olvidaste tu contraseña?" link
**Then** I should be redirected to `/auth/forgot-password`
**When** I enter my email "admin@ferreterialopez.mx"
**And** I click "Enviar Enlace"
**Then** I should see a message "Si el correo existe, se ha enviado un enlace de restablecimiento"
**And** this message should appear regardless of whether the email exists (security best practice)

### ✅ Scenario 6: Reset Password Flow
**Given** I received a password reset email with a token
**When** I click the reset link in the email (e.g., `/auth/reset-password?token=abc123&email=admin@ferreterialopez.mx`)
**Then** the reset password page should pre-populate the email field
**And** the token should be captured from the query string
**When** I enter a new password "NewSecure123!" and confirm it
**And** I click "Restablecer Contraseña"
**Then** I should see a success message "Contraseña restablecida exitosamente"
**And** I should be redirected to `/auth/login`
**And** I should be able to log in with the new password

### ✅ Scenario 7: User Registration (Admin Only)
**Given** I am logged in as a system administrator with "users.create" permission
**When** I navigate to `/auth/register`
**Then** I should see a registration form with fields for email, password, first name, last name, and roles
**When** I fill out the form and click "Registrar"
**Then** a new user should be created
**And** I should see a success message "Usuario creado exitosamente"
**And** I should be redirected to the user list page

### ✅ Scenario 8: Unauthorized Access to Registration
**Given** I am logged in as a tenant administrator WITHOUT "users.create" permission
**When** I attempt to navigate to `/auth/register`
**Then** I should be redirected to `/access-denied`
**And** I should see a message "Acceso Denegado - No tienes permiso para acceder a esta página."

### ✅ Scenario 9: Logout Flow
**Given** I am logged in and viewing the home page
**When** I click "Cerrar Sesión" in the user display component
**Then** I should see a confirmation dialog "¿Estás seguro de que deseas cerrar sesión?"
**When** I click "Confirmar"
**Then** my tokens should be cleared from localStorage
**And** the logout API endpoint should be called to invalidate my refresh token
**And** I should be redirected to `/auth/login`
**And** attempting to access protected pages should redirect me to login

### ✅ Scenario 10: Session Expiry Handling
**Given** I am logged in with a JWT token that has expired
**And** my refresh token has also expired (beyond 7 days)
**When** I attempt to navigate to a protected page
**Then** the AuthenticationStateProvider should detect the expired session
**And** I should be redirected to `/auth/login`
**And** I should see a message "Tu sesión ha expirado. Por favor, inicia sesión nuevamente."

### ✅ Scenario 11: Spanish Localization Verification
**Given** I am viewing any authentication page
**Then** all labels, buttons, error messages, and validation messages should be in Spanish (es-MX)
**And** date fields should use Mexican date format (dd/MM/yyyy)
**And** currency fields should use Mexican peso format ($1,234.56 MXN)
**And** no hardcoded Spanish strings should exist in Razor files (all must use IStringLocalizer)

### ✅ Scenario 12: MudBlazor Component Rendering
**Given** I am viewing any authentication page
**Then** all form fields should render using MudBlazor components (MudTextField, MudButton, MudCard)
**And** the design should be consistent with the MudBlazor theme
**And** validation errors should display using MudAlert components
**And** success messages should display using MudSnackbar or MudAlert components

### ✅ Scenario 13: Responsive Design
**Given** I am viewing any authentication page
**When** I resize my browser window to mobile width (< 768px)
**Then** the layout should adapt responsively
**And** all form fields should remain usable
**And** no horizontal scrolling should be required

### ✅ Scenario 14: Multi-Tenant Display Verification
**Given** I am logged in as a user for Tenant A ("Ferretería López")
**When** I view the main layout header
**Then** I should see "Ferretería López" displayed in the tenant display component
**When** I log out and log in as a user for Tenant B ("Ferretería García")
**Then** I should see "Ferretería García" displayed in the tenant display component
**And** the tenant name should match the tenant_id claim from my JWT

---

## Technical Scope

### Files to Create (18 files)

**Localization:**
1. `Resources/SharedResource.cs` - Empty marker class for localization
2. `Resources/SharedResource.es-MX.resx` - 47 Spanish translation keys
3. `Resources/SharedResource.resx` - English fallback

**Service Layer:**
4. `Services/Authentication/ITokenService.cs` - Token storage interface
5. `Services/Authentication/TokenService.cs` - LocalStorage token management
6. `Services/Authentication/IAuthService.cs` - Authentication API interface
7. `Services/Authentication/AuthService.cs` - HTTP API calls to backend
8. `Services/Authentication/CustomAuthenticationStateProvider.cs` - JWT claims parsing
9. `Services/Authentication/AuthorizationMessageHandler.cs` - Auto-attach JWT to requests

**Models/DTOs:**
10. `Models/Authentication/LoginRequest.cs`
11. `Models/Authentication/LoginResponse.cs`
12. `Models/Authentication/RegisterRequest.cs`
13. `Models/Authentication/ForgotPasswordRequest.cs`
14. `Models/Authentication/ResetPasswordRequest.cs`
15. `Models/Authentication/TokenResponse.cs`
16. `Models/Authentication/UserInfo.cs`
17. `Models/Common/Result.cs`

**Authentication Pages:**
18. `Components/Pages/Auth/Login.razor + Login.razor.cs`
19. `Components/Pages/Auth/Register.razor + Register.razor.cs`
20. `Components/Pages/Auth/ForgotPassword.razor + ForgotPassword.razor.cs`
21. `Components/Pages/Auth/ResetPassword.razor + ResetPassword.razor.cs`
22. `Components/Pages/Auth/Logout.razor + Logout.razor.cs`
23. `Components/Pages/Auth/AccessDenied.razor`

### Files to Modify (5 files)
1. `Program.cs` - Add MudBlazor, localization, authentication services
2. `Components/App.razor` - Add MudBlazor CSS/JS, CascadingAuthenticationState
3. `Components/Routes.razor` - Add AuthorizeRouteView
4. `Components/Layout/MainLayout.razor` - Add UserDisplay, TenantDisplay
5. `appsettings.json` - Add API settings

### NuGet Packages Required
```xml
<PackageReference Include="Blazored.LocalStorage" Version="4.5.0" />
<PackageReference Include="Microsoft.Extensions.Localization" Version="10.0.0" />
<PackageReference Include="MudBlazor" Version="8.0.0" />
```

### Backend Endpoints (Already Implemented)
- ✅ `POST /api/v1/auth/login`
- ✅ `POST /api/v1/auth/register`
- ✅ `POST /api/v1/auth/refresh`
- ✅ `POST /api/v1/auth/logout`
- ✅ `POST /api/v1/auth/forgot-password`
- ✅ `POST /api/v1/auth/reset-password`

---

## Implementation Plan

**Detailed plan available at:** `C:\Users\Usuario\.claude\plans\radiant-riding-lantern.md`

### Phase 1: Foundation (1-2 hours)
- Add NuGet packages
- Create localization files (47 Spanish keys)
- Create Models/DTOs

### Phase 2: Service Layer (2-3 hours)
- ITokenService + TokenService (localStorage)
- IAuthService + AuthService (HTTP API calls)
- CustomAuthenticationStateProvider (JWT parsing)
- AuthorizationMessageHandler (auto-attach tokens)

### Phase 3: Configuration (1 hour)
- Update Program.cs (MudBlazor, localization, auth services)
- Update App.razor (CascadingAuthenticationState)
- Update Routes.razor (AuthorizeRouteView)
- Update appsettings.json

### Phase 4: Authentication Pages (4-5 hours)
- Login.razor (email, password, optional tenant subdomain)
- Register.razor (requires users.create permission)
- ForgotPassword.razor (always shows success message)
- ResetPassword.razor (token from query string)
- Logout.razor (confirmation dialog)
- AccessDenied.razor (403 page)

### Phase 5: Layout Updates (2 hours)
- UserDisplay.razor (current user + logout button)
- TenantDisplay.razor (current tenant name)
- MainLayout.razor updates

### Phase 6: Testing (3-4 hours)
- Create Corelio.BlazorApp.Tests project
- AuthServiceTests.cs (12 tests)
- TokenServiceTests.cs (8 tests)
- CustomAuthenticationStateProviderTests.cs (6 tests)
- LoginPageTests.cs (5 tests using bUnit)
- **Target:** 31+ tests, >70% coverage

### Phase 7: Verification (2-3 hours)
- Run all 14 acceptance criteria scenarios
- Verify localStorage token storage
- Test token auto-refresh
- Verify Spanish localization
- Test responsive design

**Total Estimated Effort:** 15-20 hours (2-3 days)

---

## Dependencies

### Blocking Dependencies
- ✅ **Backend Authentication Endpoints** (Complete) - All 6 endpoints functional
- ✅ **JWT Token Generation** (Complete) - JWT with proper claims
- ✅ **Multi-Tenancy Backend** (Complete) - TenantService, TenantMiddleware

### Non-Blocking Dependencies
- Blazored.LocalStorage package (install in Phase 1)
- Microsoft.Extensions.Localization package (install in Phase 1)

### Downstream Impact
- **Sprint 3 (Product Management)** - BLOCKED until this is complete
- **US-2.1.1 (Tenant Display)** - BLOCKED (depends on JWT claims from this story)
- **All stakeholder demos** - BLOCKED (no UI to show)

---

## Definition of Done

### Functional Requirements
- [ ] User can log in via Blazor UI with email/password
- [ ] JWT + refresh tokens stored securely in localStorage
- [ ] Tokens auto-refresh before expiry (1 min buffer)
- [ ] User can log out (tokens revoked and cleared)
- [ ] User can request password reset
- [ ] User can reset password with token
- [ ] Admin can register users (with users.create permission)
- [ ] Unauthorized users redirected to /access-denied
- [ ] All authentication pages in Spanish (es-MX)

### Technical Requirements
- [ ] Clean Architecture maintained
- [ ] MudBlazor components render correctly
- [ ] IStringLocalizer used (no hardcoded strings)
- [ ] Aspire service discovery configured
- [ ] C# 14 features used
- [ ] AuthenticationStateProvider integrated
- [ ] AuthorizationMessageHandler attaches JWT

### Quality Requirements
- [ ] 31+ unit tests passing
- [ ] >70% code coverage for services
- [ ] Zero compilation warnings
- [ ] All 14 acceptance criteria pass
- [ ] Browser DevTools shows correct token storage
- [ ] No console errors

### Documentation
- [ ] SPRINT_STATUS.md updated
- [ ] XML comments on public interfaces
- [ ] README updated with auth setup

### Security
- [ ] Passwords never logged
- [ ] Tokens cleared on logout
- [ ] Expired tokens handled gracefully
- [ ] Authorization checks on protected pages

---

## Out of Scope

❌ **NOT Included:**
- Two-Factor Authentication (2FA)
- OAuth/Social Login
- Password Strength Meter
- User Profile Management
- Remember Me Checkbox
- Email sending implementation (API returns success, but no actual email sent)
- Mobile app authentication

---

## Success Metrics

**Business Value:**
- Stakeholders can log in and demo the system
- Sprint 3 unblocked

**User Experience:**
- Login completes in < 3 seconds
- Clear Spanish error messages
- 7-day session persistence (refresh token)

**Technical Quality:**
- >70% code coverage
- 100% test pass rate
- Zero critical bugs

**Compliance:**
- 100% UI text in Spanish (es-MX)
- Tenant ID from JWT claims
- Secure password/token storage

---

## Story Points Breakdown

**13 SP = 15-20 hours**

| Phase | Effort | Complexity |
|-------|--------|------------|
| Foundation | 2h | Low |
| Service Layer | 3h | High |
| Configuration | 1h | Medium |
| Pages | 5h | Medium |
| Layout Updates | 2h | Low |
| Testing | 4h | High |
| Verification | 3h | Medium |

**Complexity Factors:**
- 18 new files, 5 files to modify
- Complex JWT parsing and token refresh logic
- 47 translation keys
- Multiple external integrations (localStorage, HttpClient, Aspire)

**Risk Factors:**
- MudBlazor 8.0 potential breaking changes
- Token refresh race conditions
- bUnit testing (limited team experience)

---

## Review Checklist

Before approving this story, confirm:

- [ ] Acceptance criteria are clear and testable
- [ ] Technical scope is feasible
- [ ] 13 SP estimate is reasonable (2-3 days)
- [ ] Dependencies are accurate
- [ ] Definition of Done is achievable
- [ ] Out of scope is acceptable
- [ ] Implementation plan is detailed enough

---

**Ready for Sprint Planning:** ✅ Yes
**Recommended Priority:** 🔴 CRITICAL (Must complete before Sprint 3)
**Implementation Order:** 1st (US-2.1.1 depends on this)
