# US-2.1.1: Multi-Tenancy Frontend (Tenant and User Display)

**Epic:** Multi-Tenancy
**Sprint:** Sprint 2 (Frontend Debt) or Sprint 3 (High Priority)
**Story Points:** 5 SP
**Priority:** CRITICAL (Part of Sprint 2 completion)
**Status:** 🔴 Not Started
**Created:** 2026-01-13
**Dependencies:** US-2.2.1 (Authentication Frontend) ❌ Not Started

---

## User Story

**As a** tenant administrator using the Corelio ERP system,
**I want** to see my current tenant name and user name displayed in the application header,
**So that** I can confirm which organization and account I'm logged into, ensuring data security and preventing accidental cross-tenant actions.

---

## Business Value

- **Multi-Tenant Awareness:** Users can visually confirm their tenant context at all times
- **Security:** Reduces risk of accidental cross-tenant data access
- **User Experience:** Clear indication of current account/organization
- **Definition of Done:** Completes US-2.1 properly (backend was only 50% of the feature)

---

## Acceptance Criteria

### ✅ Scenario 1: Tenant Display (Happy Path)
**Given** I am logged in as a user for "Ferretería López" (tenant_id: abc-123)
**And** I am viewing any page in the application
**When** I look at the main layout header
**Then** I should see a tenant display component showing "Ferretería López"
**And** the tenant name should be styled as a MudChip with a distinct color (e.g., primary color)
**And** the tenant name should be fetched using the tenant_id claim from my JWT

### ✅ Scenario 2: User Display (Happy Path)
**Given** I am logged in as "Juan López" (user_id: def-456, email: juan@ferreterialopez.mx)
**And** I am viewing any page in the application
**When** I look at the main layout header (top-right corner)
**Then** I should see a user display component showing "Juan López"
**And** I should see a logout button/link next to my name
**And** the user name should be extracted from JWT claims (first name + last name)

### ✅ Scenario 3: Tenant Context Verification (Multi-Tenant Isolation)
**Given** I am logged in as a user for Tenant A ("Ferretería López")
**And** I view the tenant display in the header
**Then** I should see "Ferretería López"
**When** I log out and log in as a user for Tenant B ("Ferretería García")
**And** I view the tenant display in the header
**Then** I should see "Ferretería García"
**And** no data from Tenant A should be visible

### ✅ Scenario 4: User Display Logout Action
**Given** I am logged in and viewing the home page
**When** I click the logout button in the user display component
**Then** I should be redirected to `/auth/logout`
**And** I should see a confirmation dialog (handled by Logout.razor from US-2.2.1)
**And** upon confirmation, I should be logged out and redirected to `/auth/login`

### ✅ Scenario 5: Unauthenticated State (No Display)
**Given** I am NOT logged in (unauthenticated)
**When** I view the login page or any public page
**Then** the tenant display component should NOT render
**And** the user display component should NOT render
**And** the header should only show the application logo/name

### ✅ Scenario 6: Tenant Name Loading (API Call)
**Given** I am logged in with a valid JWT containing tenant_id
**When** the TenantDisplay component initializes
**Then** it should call the backend API to fetch the tenant name using tenant_id
**And** while loading, it should display a skeleton/placeholder (e.g., "Cargando...")
**And** once loaded, it should cache the tenant name to avoid repeated API calls

### ✅ Scenario 7: Tenant Not Found (Error Handling)
**Given** I am logged in with a JWT containing an invalid tenant_id
**When** the TenantDisplay component attempts to fetch the tenant name
**Then** the API call should fail (404 Not Found or 401 Unauthorized)
**And** the component should display "Inquilino Desconocido" (Unknown Tenant)
**And** an error should be logged to the browser console

### ✅ Scenario 8: User Display Name Fallback
**Given** I am logged in with a JWT that contains only email (no first/last name)
**When** the UserDisplay component initializes
**Then** it should fall back to displaying the email address
**And** the logout button should still be visible and functional

### ✅ Scenario 9: Responsive Design (Mobile View)
**Given** I am logged in and viewing the application on a mobile device (< 768px width)
**When** I view the main layout header
**Then** the tenant display and user display components should adapt responsively
**And** the tenant name may be abbreviated or hidden on very small screens (e.g., < 480px)
**And** the user display should show an icon (avatar) instead of full name on small screens
**And** the logout button should remain accessible

### ✅ Scenario 10: Localization (Spanish UI)
**Given** I am viewing the tenant display and user display components
**Then** all tooltips, labels, and error messages should be in Spanish (es-MX)
**And** if the tenant name is "Unknown Tenant", it should display as "Inquilino Desconocido"
**And** the logout button should display "Cerrar Sesión"

---

## Technical Scope

### Files to Create (2 components)
1. `Components/Shared/TenantDisplay.razor` - Tenant name display component
2. `Components/Shared/UserDisplay.razor` - User name + logout button component

### Files to Modify (2 files)
1. `Components/Layout/MainLayout.razor` - Add TenantDisplay and UserDisplay to header
2. `Resources/SharedResource.es-MX.resx` - Add 5 new Spanish translation keys

### Backend Work Required (⚠️ NEW ENDPOINT NEEDED)
**Status:** Not yet implemented
**Effort:** 1.5 hours additional work

Need to create:
- `Application/Tenants/Queries/GetTenantName/GetTenantNameQuery.cs`
- `Application/Tenants/Queries/GetTenantName/GetTenantNameQueryHandler.cs`
- `WebAPI/Endpoints/TenantEndpoints.cs` - Add `GET /api/v1/tenants/{tenantId}/name`
- Endpoint should return: `{ "tenantId": "guid", "name": "Tenant Name" }`
- Should cache result in Redis (30 min TTL)

### New Localization Keys (5 keys)
```xml
<data name="Loading"><value>Cargando...</value></data>
<data name="UnknownTenant"><value>Inquilino Desconocido</value></data>
<data name="UnknownUser"><value>Usuario Desconocido</value></data>
<data name="CurrentTenant"><value>Inquilino Actual</value></data>
<data name="CurrentUser"><value>Usuario Actual</value></data>
```

### JWT Claims Required (From US-2.2.1)
CustomAuthenticationStateProvider must parse these claims:
```json
{
    "tenant_id": "abc-123-def-456",  // Used by TenantDisplay
    "first_name": "Juan",            // Used by UserDisplay
    "last_name": "López",            // Used by UserDisplay
    "email": "juan@example.mx"       // Fallback for UserDisplay
}
```

---

## Component Design

### TenantDisplay.razor
```razor
@inject CustomAuthenticationStateProvider AuthStateProvider
@inject ITenantService TenantService
@inject IStringLocalizer<SharedResource> L

<AuthorizeView>
    <Authorized>
        @if (isLoading)
        {
            <MudChip Color="Color.Default">@L["Loading"]</MudChip>
        }
        else if (!string.IsNullOrEmpty(tenantName))
        {
            <MudChip Color="Color.Primary" Icon="@Icons.Material.Filled.Business">
                @tenantName
            </MudChip>
        }
        else
        {
            <MudChip Color="Color.Error">@L["UnknownTenant"]</MudChip>
        }
    </Authorized>
</AuthorizeView>

@code {
    private string? tenantName;
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = user.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                tenantName = await TenantService.GetTenantNameAsync(tenantId);
            }
        }

        isLoading = false;
    }
}
```

### UserDisplay.razor
```razor
@inject CustomAuthenticationStateProvider AuthStateProvider
@inject NavigationManager Navigation
@inject IStringLocalizer<SharedResource> L

<AuthorizeView>
    <Authorized>
        <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
            <MudAvatar Color="Color.Secondary" Size="Size.Small">
                @GetInitials(context.User)
            </MudAvatar>
            <MudText Typo="Typo.body2">@GetDisplayName(context.User)</MudText>
            <MudIconButton Icon="@Icons.Material.Filled.Logout"
                           Color="Color.Error"
                           Size="Size.Small"
                           OnClick="HandleLogout"
                           Title="@L["Logout"]" />
        </MudStack>
    </Authorized>
</AuthorizeView>

@code {
    private string GetDisplayName(ClaimsPrincipal user)
    {
        var firstName = user.FindFirst("first_name")?.Value;
        var lastName = user.FindFirst("last_name")?.Value;
        var email = user.FindFirst("email")?.Value;

        if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            return $"{firstName} {lastName}";

        return email ?? L["UnknownUser"];
    }

    private string GetInitials(ClaimsPrincipal user)
    {
        var firstName = user.FindFirst("first_name")?.Value;
        var lastName = user.FindFirst("last_name")?.Value;

        if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            return $"{firstName[0]}{lastName[0]}".ToUpper();

        var email = user.FindFirst("email")?.Value;
        return email?[0].ToString().ToUpper() ?? "?";
    }

    private void HandleLogout()
    {
        Navigation.NavigateTo("/auth/logout");
    }
}
```

### MainLayout.razor Updates
```razor
<MudAppBar Elevation="1">
    <MudIconButton Icon="@Icons.Material.Filled.Menu" ... />
    <MudText Typo="Typo.h6">Corelio ERP</MudText>
    <MudSpacer />

    <!-- Tenant Display (left of user) -->
    <TenantDisplay />

    <MudSpacer />

    <!-- User Display (right side) -->
    <UserDisplay />
</MudAppBar>
```

---

## Implementation Tasks

### Task 1: Backend - GetTenantName Endpoint (1.5 hours)
- [ ] Create `GetTenantNameQuery.cs` and `GetTenantNameQueryHandler.cs`
- [ ] Create `GET /api/v1/tenants/{tenantId}/name` endpoint
- [ ] Add Redis caching (30 min TTL)
- [ ] Write 3 unit tests (valid tenant, invalid tenant, caching)

### Task 2: TenantDisplay Component (1.5 hours)
- [ ] Create `TenantDisplay.razor`
- [ ] Integrate with ITenantService
- [ ] Add loading state
- [ ] Add error handling (unknown tenant)
- [ ] Write 2 bUnit component tests

### Task 3: UserDisplay Component (1 hour)
- [ ] Create `UserDisplay.razor`
- [ ] Extract user name from JWT claims
- [ ] Add avatar with initials
- [ ] Add logout button
- [ ] Write 2 bUnit component tests

### Task 4: MainLayout Updates (0.5 hours)
- [ ] Add `<TenantDisplay />` to header
- [ ] Add `<UserDisplay />` to header
- [ ] Ensure responsive layout

### Task 5: Localization (0.5 hours)
- [ ] Add 5 Spanish translation keys to SharedResource.es-MX.resx

### Task 6: Testing (2 hours)
- [ ] Write 8+ unit tests (components + backend query)
- [ ] Manual testing of all 10 acceptance criteria scenarios

**Total Effort:** 5-7 hours (1 day)

---

## Dependencies

### Blocking Dependencies
- ❌ **US-2.2.1 (Authentication Frontend)** - REQUIRED
  - Provides CustomAuthenticationStateProvider
  - Provides JWT claims parsing (tenant_id, first_name, last_name, email)
  - Provides authentication state management
  - **Cannot start US-2.1.1 until US-2.2.1 is complete**

### New Backend Work Required
- ⏸️ **GET /api/v1/tenants/{tenantId}/name** endpoint
  - Not yet implemented
  - 1.5 hours additional work
  - Required before component can fetch tenant name

### Non-Blocking Dependencies
- MudBlazor (already installed in US-2.2.1)
- Localization (already configured in US-2.2.1)

---

## Definition of Done

### Functional Requirements
- [ ] TenantDisplay shows current tenant name in header
- [ ] Tenant name fetched from backend API using tenant_id from JWT
- [ ] Tenant name cached to avoid repeated API calls
- [ ] UserDisplay shows current user name in header
- [ ] User name extracted from JWT claims (first_name + last_name)
- [ ] Logout button navigates to `/auth/logout`
- [ ] Components only render when authenticated
- [ ] Spanish localization for all labels/errors

### Technical Requirements
- [ ] Clean Architecture maintained
- [ ] MudBlazor components used
- [ ] Backend endpoint: `GET /api/v1/tenants/{tenantId}/name` created
- [ ] Redis caching implemented (30 min TTL)
- [ ] C# 14 features used
- [ ] Components are responsive

### Quality Requirements
- [ ] 8+ unit tests passing
- [ ] >70% code coverage
- [ ] All 10 acceptance criteria pass
- [ ] No console errors

### Documentation
- [ ] SPRINT_STATUS.md updated
- [ ] XML comments on components
- [ ] Localization keys documented

### Security
- [ ] Tenant ID never accepted from client input
- [ ] Tenant name API enforces authorization
- [ ] Multi-tenant query filter applied

---

## Out of Scope

❌ **NOT Included:**
- Tenant Switcher (users cannot switch tenants)
- User Profile Dropdown (logout only)
- Tenant Logo/Icon (text only)
- User Avatar Image (initials only)
- Notification Badge
- User Settings menu
- Tenant Settings menu

---

## Success Metrics

**Business Value:**
- Users can visually confirm tenant/account context
- Reduces cross-tenant data access risk

**User Experience:**
- 100% users can identify their tenant/account
- Tenant name loads in < 500ms (target: < 200ms with caching)

**Technical Quality:**
- >70% code coverage
- 100% test pass rate
- All 10 scenarios pass

**Compliance:**
- 100% UI text in Spanish
- Tenant ID from JWT claims
- Security: Tenant ID never from client

---

## Story Points Breakdown

**5 SP = 5-7 hours**

| Task | Effort | Complexity |
|------|--------|------------|
| Backend Endpoint | 1.5h | Low |
| TenantDisplay | 1.5h | Low |
| UserDisplay | 1h | Low |
| MainLayout Updates | 0.5h | Low |
| Localization | 0.5h | Low |
| Testing | 2h | Medium |

**Complexity Factors:**
- Simple UI components
- Standard Blazor patterns
- MudBlazor components well-documented

**Risk Factors:**
- LOW - No complex state management
- LOW - Simple API integration

---

## Implementation Order

**CRITICAL:** This story MUST be implemented AFTER US-2.2.1

**Sequence:**
1. **US-2.2.1 (Authentication Frontend)** - 13 SP - MUST complete first
2. **US-2.1.1 (Multi-Tenancy Frontend)** - 5 SP - Start after US-2.2.1

**Reason:** US-2.1.1 depends on CustomAuthenticationStateProvider and JWT claims parsing from US-2.2.1.

**Combined Effort:** 18 SP (US-2.2.1: 13 SP + US-2.1.1: 5 SP)
**Combined Timeline:** 3-4 days for 1 developer (sequential)

---

## Review Checklist

Before approving this story, confirm:

- [ ] Acceptance criteria are clear and testable
- [ ] Technical scope is feasible
- [ ] 5 SP estimate is reasonable (1 day)
- [ ] Backend endpoint work is accounted for (1.5h)
- [ ] Dependency on US-2.2.1 is understood
- [ ] Definition of Done is achievable
- [ ] Out of scope is acceptable

---

**Ready for Sprint Planning:** ✅ Yes
**Recommended Priority:** 🔴 CRITICAL (Completes Sprint 2 frontend)
**Implementation Order:** 2nd (After US-2.2.1)
