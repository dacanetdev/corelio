# UI/UX Design System - Suggested User Stories

## Document Purpose

This document provides ready-to-use user story templates for the Product Owner to create backlog items. Each user story follows the standard format with acceptance criteria, effort estimates, and dependencies.

**Related Documents:**
- `UI-UX-Design-System-Recommendation.md` (business case)
- `UI-UX-Implementation-Guide.md` (technical details)

**Target Audience:** Product Owner, Scrum Master

**Last Updated:** 2026-01-27

---

## Epic

**Epic Name:** UI/UX Design System Implementation

**Epic Description:**
As a **product owner**, I want to implement a professional, distinctive design system for Corelio so that the application stands out in the market, provides excellent user experience on mobile devices, and enables basic multi-tenant branding capabilities.

**Business Value:**
- Market differentiation through distinctive visual identity
- Improved user experience, especially on mobile devices (tablets/phones in-store)
- Tenant engagement through simple branding customization
- Faster development velocity with reusable component library

**Success Metrics:**
- Professional appearance that differs from generic MudBlazor applications
- Mobile-optimized UI working at 375px width (iPhone SE)
- Tenant theming adoption rate > 30% within 3 months
- Development time reduction of 30% for new pages (via component reuse)

**Estimated Total Effort:** 32-42 hours (~3-4 sprints)

---

## Sprint 1: Foundation & First Impression (14-18 hours)

### User Story 1.1: Core Theme Infrastructure

**As a** developer
**I want** a centralized design system with custom colors, typography, and spacing
**So that** all pages have consistent visual appearance and styling decisions are standardized

**Description:**
Implement the core theme infrastructure including MudTheme configuration with "Industrial Terracotta" color palette, CSS custom properties for design tokens, Inter font integration, and removal of Bootstrap CSS. This provides the foundation for all subsequent UI work.

**Acceptance Criteria:**

**Core Theme:**
- [ ] ThemeConfiguration.cs created with CorelioDefaultTheme
- [ ] Primary color is #E74C3C (Terracotta Red) throughout application
- [ ] Secondary color is #6C757D (Concrete Gray) for text/borders
- [ ] Semantic colors defined: Success (#28A745), Warning (#FFC107), Error (#DC3545), Info (#17A2B8)

**Typography:**
- [ ] Inter font family loads from Google Fonts (300, 400, 500, 600, 700 weights)
- [ ] Typography scale implemented: H1=40px, H2=32px, H3=28px, H4=24px, H5=20px, H6=18px, Body1=16px, Body2=14px, Button=14px, Caption=12px
- [ ] Button text transformation set to "none" (no uppercase)
- [ ] Fallback fonts configured: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto

**CSS Variables:**
- [ ] variables.css created with color palette (--color-primary-*, --color-secondary-*)
- [ ] Spacing system defined (--space-0 through --space-24, 8px grid)
- [ ] Shadow utilities defined (--shadow-sm, --shadow-md, --shadow-lg, --shadow-xl)
- [ ] Border radius utilities defined (--radius-sm through --radius-xl, --radius-full)
- [ ] CSS variables accessible throughout application

**Application Files:**
- [ ] App.razor updated with Inter font preconnect and import
- [ ] Bootstrap CSS completely removed from App.razor
- [ ] CSS load order corrected: MudBlazor → variables.css → app.css → scoped CSS
- [ ] MainLayout.razor applies CorelioDefaultTheme
- [ ] AppBar styled with white background and subtle border (#DEE2E6)
- [ ] Main content area has light gray background (#F8F9FA)
- [ ] Consistent padding applied (24px via pa-6)

**MudBlazor Overrides:**
- [ ] app.css updated with component overrides
- [ ] All buttons have 8px border-radius (not default 4px)
- [ ] All cards have 12px border-radius (softer corners)
- [ ] Focus states styled with primary color and subtle shadow
- [ ] Shadows are subtle (not harsh Material Design defaults)

**Testing:**
- [ ] No compilation errors
- [ ] Inter font loads successfully (verify in DevTools Network tab)
- [ ] No Bootstrap CSS loaded (verify in DevTools Sources)
- [ ] Primary color renders correctly in UI (#E74C3C)
- [ ] Border radius is visibly softer on cards/buttons
- [ ] Mobile: Base font size reduces to 14px at 960px width

**Definition of Done:**
- [ ] Code reviewed and approved
- [ ] Visual QA performed (color, typography, spacing checklist)
- [ ] Works on Chrome, Firefox, Safari, Edge (latest versions)
- [ ] Mobile-responsive (tested at 375px, 768px, 1024px, 1920px widths)
- [ ] No console errors or warnings
- [ ] Committed to feature branch

**Effort Estimate:** 6-8 hours

**Priority:** P0 (Critical - Foundation for all other work)

**Dependencies:** None

**Technical Notes:**
- This story must be completed before other UI work
- No breaking changes to existing functionality
- Changes are purely visual/stylistic

---

### User Story 1.2: Authentication Pages Redesign

**As a** potential customer evaluating Corelio
**I want** a professional, welcoming login experience
**So that** I feel confident the product is high-quality and trustworthy

**Description:**
Redesign all authentication pages (Login, Register, ForgotPassword, ResetPassword) with the new design system. Create a dedicated AuthLayout for minimal, full-screen auth experiences. Focus on generous whitespace, large touch targets, clear visual hierarchy, and smooth animations to create an excellent first impression.

**Acceptance Criteria:**

**AuthLayout Component:**
- [ ] AuthLayout.razor created as minimal layout (no AppBar, no Drawer)
- [ ] Full-screen centered content with subtle gradient background
- [ ] Background pattern overlay (repeating diagonal lines, 30% opacity)
- [ ] Footer with copyright text
- [ ] Responsive padding (24px mobile, 32px tablet, 48px desktop)

**Login Page:**
- [ ] Uses AuthLayout instead of MainLayout
- [ ] Large logo section with Store icon (4rem size) in circular gradient background (96px diameter)
- [ ] App title "Corelio ERP" (H3, 28px, 700 weight, primary color #B03A2E)
- [ ] Subtitle "Sistema de Gestión para Ferreterías" (Body1, secondary color)
- [ ] Auth card has 16px border-radius (softer, hero card)
- [ ] Auth card has Elevation="8" (prominent shadow)
- [ ] Auth card padding: 48px desktop, 32px mobile
- [ ] Email field with email icon (Adornment.Start)
- [ ] Password field with visibility toggle icon (eye/eye-off)
- [ ] Helper text "ejemplo@ferreteria.mx" below email field
- [ ] Submit button is 56px tall (prominent)
- [ ] Submit button full-width with primary color
- [ ] Loading state: spinner + "Cargando..." text
- [ ] Error alerts styled with border-radius and error icon
- [ ] Session expired warning displays when ?expired=true query parameter present
- [ ] Forgot password link styled with primary color (#CB4335)
- [ ] "No tienes una cuenta?" text with "Contacta a tu administrador" message
- [ ] Smooth fade-in animation on page load (0.4s ease-out)

**Register Page:**
- [ ] Uses AuthLayout
- [ ] Same card styling as Login (Elevation="8", 16px radius, generous padding)
- [ ] Person Add icon in header (large, primary color)
- [ ] Page title "Registrar Usuario" (H4, primary color)
- [ ] Two-column grid layout on desktop (>= 960px)
- [ ] Stacked layout on mobile (< 960px)
- [ ] Section headers: "Información Personal" and "Seguridad" (H6, primary color #B03A2E)
- [ ] Fields: FirstName, LastName, Email (in Personal section)
- [ ] Fields: Password, ConfirmPassword (in Security section)
- [ ] Password visibility toggle on both password fields
- [ ] All fields use Variant.Outlined
- [ ] Horizontal divider between sections
- [ ] Action buttons (Cancel, Register) aligned right
- [ ] Cancel button: Outlined, Secondary color
- [ ] Register button: Filled, Primary color, min-width 160px
- [ ] Register button icon: PersonAdd
- [ ] Loading state on submit button
- [ ] Success alert on registration complete
- [ ] Auto-redirect to login after 2 seconds on success

**ForgotPassword & ResetPassword Pages:**
- [ ] Same card styling as Login/Register
- [ ] Consistent icon usage (Email icon for ForgotPassword, Key icon for ResetPassword)
- [ ] Consistent button styling (56px height, full-width)
- [ ] Consistent error/success alert styling
- [ ] All Spanish text uses IStringLocalizer (no hardcoded strings)

**Localization:**
- [ ] All user-facing text uses IStringLocalizer<SharedResource>
- [ ] Resource keys created for all labels, buttons, messages
- [ ] No hardcoded Spanish strings in Razor markup

**Mobile Optimization:**
- [ ] Auth card max-width: 480px desktop, 100% mobile
- [ ] Padding reduces on mobile (32px instead of 48px)
- [ ] Font size appropriate for mobile (14px base at < 960px)
- [ ] Touch targets at least 44px height
- [ ] Form fields stack vertically on mobile
- [ ] Logo icon reduces to 64px on mobile (from 80px desktop)

**Testing:**
- [ ] Login page loads without errors
- [ ] Login form submission works (both success and error cases)
- [ ] Password visibility toggle works
- [ ] Session expired warning displays correctly
- [ ] Register page renders correctly on desktop and mobile
- [ ] Register form validation works (required fields, password match)
- [ ] Form submission shows loading state
- [ ] Success message displays and auto-redirects
- [ ] Forgot password and reset password pages styled consistently
- [ ] All pages tested at 375px (iPhone SE), 768px (iPad), 1920px (desktop)

**Definition of Done:**
- [ ] All acceptance criteria met
- [ ] Code reviewed and approved
- [ ] Visual QA performed (compare to design specs)
- [ ] Spanish localization complete (no hardcoded strings)
- [ ] Mobile-tested on real device or Chrome DevTools
- [ ] No accessibility issues (color contrast, keyboard navigation)
- [ ] Committed to feature branch

**Effort Estimate:** 8-10 hours

**Priority:** P0 (Critical - First impression)

**Dependencies:** User Story 1.1 (Core Theme Infrastructure)

**Technical Notes:**
- AuthLayout.razor is a new file, no breaking changes
- Existing auth logic unchanged, only visual redesign
- Consider animation performance on low-end devices

---

## Sprint 2: Component Library & Theming (16-18 hours)

### User Story 2.1: Core Reusable Components

**As a** developer
**I want** a library of polished, reusable UI components
**So that** I can build new pages faster with consistent patterns and eliminate duplicate code

**Description:**
Create a library of core reusable components following the design system that address common UI patterns: page headers with breadcrumbs, loading states, empty states, and improve existing TenantDisplay and UserDisplay components. These components eliminate inconsistency and accelerate development of new features.

**Acceptance Criteria:**

**PageHeader Component:**
- [ ] PageHeader.razor created in Components/Shared
- [ ] Required parameter: Title (string)
- [ ] Optional parameter: Description (string)
- [ ] Optional parameter: Breadcrumbs (List<BreadcrumbItem>)
- [ ] Optional parameter: Actions (RenderFragment for action buttons)
- [ ] Renders breadcrumbs with ">" separator if provided
- [ ] Title displayed as H4 (24px, 700 weight, secondary-900 color)
- [ ] Description displayed as Body2 (14px, secondary color, max-width 600px)
- [ ] Title and actions row uses flexbox with space-between
- [ ] Actions section wraps on mobile
- [ ] Bottom border: 1px solid secondary-200
- [ ] Margin bottom: 32px (var(--space-8))
- [ ] Padding bottom: 24px (var(--space-6))
- [ ] Responsive: margin reduces to 24px on mobile

**LoadingState Component:**
- [ ] LoadingState.razor created in Components/Shared
- [ ] Parameter: Message (string, default: "Cargando...")
- [ ] Displays centered 64px primary-colored spinner
- [ ] Message displayed below spinner with 16px gap
- [ ] Message uses Body1 typography, secondary color
- [ ] Container: centered flexbox column with min-height 320px
- [ ] Padding: 48px (var(--space-12))
- [ ] Responsive: padding reduces to 32px on mobile

**EmptyState Component:**
- [ ] EmptyState.razor created in Components/Shared
- [ ] Required parameter: Icon (string, default: Icons.Material.Filled.Inbox)
- [ ] Required parameter: Title (string)
- [ ] Required parameter: Description (string)
- [ ] Optional parameter: ActionText (string)
- [ ] Optional parameter: ActionIcon (string, default: Icons.Material.Filled.Add)
- [ ] Optional parameter: OnAction (EventCallback)
- [ ] Icon displayed at 4rem size in 96px circular background (secondary-100 color)
- [ ] Title displayed as H6 (18px, 600 weight, secondary-800 color)
- [ ] Description displayed as Body2 (14px, secondary color, max-width 400px, centered)
- [ ] Action button only displayed if ActionText provided and OnAction has delegate
- [ ] Action button: Filled variant, Primary color, with StartIcon
- [ ] Container: centered flexbox column with min-height 400px
- [ ] Padding: 48px (var(--space-12))
- [ ] Responsive: padding reduces to 32px on mobile

**TenantDisplay Component (Enhanced):**
- [ ] TenantDisplay.razor updated
- [ ] Pill-shaped badge: 20px border-radius, 8px vertical padding, 16px horizontal padding
- [ ] Background: linear gradient from primary-400/10% to primary-400/5%
- [ ] Border: 2px solid primary-400
- [ ] Store icon: Small size, primary-700 color
- [ ] Tenant name: Body2 typography, 600 weight, primary-700 color
- [ ] Row layout with 8px gap (var(--space-2))
- [ ] Responsive: Hide tenant name on mobile (< 600px), show icon only
- [ ] Smooth hover effect (0.2s ease transition)

**UserDisplay Component (Enhanced):**
- [ ] UserDisplay.razor updated
- [ ] Avatar with user initials (first letter of first name + first letter of last name)
- [ ] Avatar: Primary color background, Small size, 600 weight text
- [ ] User info section: name (Body2, 600 weight) and email (Caption, secondary color)
- [ ] Background: secondary-100 color
- [ ] Padding: 8px vertical, 16px horizontal
- [ ] Border radius: 24px (pill shape)
- [ ] Dropdown icon: KeyboardArrowDown, Small size, secondary color
- [ ] Hover effect: background changes to secondary-200, shadow-md appears
- [ ] Dropdown menu items: "Mi Perfil", "Configuración", "Cerrar Sesión"
- [ ] Menu items have icons (Person, Settings, Logout)
- [ ] Logout item styled with error color
- [ ] Divider before logout item
- [ ] Responsive: Hide user info on mobile (< 600px), show avatar + dropdown icon only

**Component CSS:**
- [ ] page-header CSS added to app.css
- [ ] loading-state CSS added to app.css
- [ ] empty-state CSS added to app.css
- [ ] empty-state-icon CSS added to app.css
- [ ] user-display-activator hover CSS added to app.css
- [ ] Responsive media queries for all components

**Usage Examples:**
- [ ] Usage examples documented in implementation guide
- [ ] Example code snippets provided for each component

**Testing:**
- [ ] PageHeader renders correctly with all parameter combinations
- [ ] PageHeader breadcrumbs display and navigate correctly
- [ ] PageHeader actions section renders custom buttons
- [ ] LoadingState displays spinner and message
- [ ] EmptyState displays icon, title, description
- [ ] EmptyState action button triggers OnAction callback
- [ ] EmptyState handles missing ActionText gracefully (no button)
- [ ] TenantDisplay shows store icon + tenant name
- [ ] TenantDisplay hides text on mobile, shows icon only
- [ ] UserDisplay shows avatar with correct initials
- [ ] UserDisplay dropdown menu works correctly
- [ ] UserDisplay navigation to profile/settings/logout works
- [ ] UserDisplay hides user info on mobile, shows avatar only
- [ ] All components responsive at 375px, 768px, 1024px, 1920px widths
- [ ] All hover effects smooth (0.2s transition)
- [ ] CSS variables used correctly for colors/spacing

**Definition of Done:**
- [ ] All 5 components created/updated
- [ ] Code reviewed and approved
- [ ] Component documentation added to implementation guide
- [ ] Visual QA performed
- [ ] Mobile-tested on multiple screen sizes
- [ ] No accessibility issues
- [ ] Committed to feature branch

**Effort Estimate:** 6-8 hours

**Priority:** P1 (High - Enables faster development)

**Dependencies:** User Story 1.1 (Core Theme Infrastructure)

**Technical Notes:**
- Components are new files, no breaking changes
- TenantDisplay and UserDisplay are modified but maintain same interface
- Consider adding Storybook/component showcase in future

---

### User Story 2.2: Multi-Tenant Theming Infrastructure

**As a** tenant administrator
**I want** to customize my company's primary color and logo
**So that** the ERP system feels personalized to my business and reinforces my brand

**Description:**
Implement multi-tenant theming infrastructure that allows tenants to customize their primary color and upload a logo. This includes database schema changes, backend services with Redis caching, frontend dynamic theming, and API endpoints for theme management.

**Acceptance Criteria:**

**Database Schema:**
- [ ] Migration "AddTenantBrandingFields" created
- [ ] TenantConfiguration entity has PrimaryColor property (string, nullable)
- [ ] TenantConfiguration entity has LogoUrl property (string, nullable)
- [ ] TenantConfiguration entity has UseCustomTheme property (bool, default: false)
- [ ] Migration applied to database successfully
- [ ] Database columns created: primary_color, logo_url, use_custom_theme

**Application Layer:**
- [ ] ITenantThemeService interface created in Application/Common/Interfaces
- [ ] TenantThemeDto record created with properties: TenantId, PrimaryColor, LogoUrl, UseCustomTheme
- [ ] GetTenantThemeAsync method signature defined
- [ ] InvalidateThemeCacheAsync method signature defined
- [ ] IsValidHexColor method signature defined

**Infrastructure Layer:**
- [ ] TenantThemeService implemented in Infrastructure/Services
- [ ] GetTenantThemeAsync retrieves theme from database
- [ ] GetTenantThemeAsync caches result in Redis with key: "tenant-theme:{tenantId}"
- [ ] Cache TTL: 2 hours (AbsoluteExpirationRelativeToNow)
- [ ] GetTenantThemeAsync returns cached value if available (cache hit)
- [ ] GetTenantThemeAsync returns null if UseCustomTheme is false (use default theme)
- [ ] InvalidateThemeCacheAsync removes cache entry on theme update
- [ ] IsValidHexColor validates format: ^#[0-9A-Fa-f]{6}$ regex
- [ ] TenantThemeService registered in DependencyInjection.cs

**Blazor App Layer:**
- [ ] DynamicThemeService created in BlazorApp/Services
- [ ] GetThemeForTenantAsync builds MudTheme from TenantThemeDto
- [ ] DynamicThemeService clones CorelioDefaultTheme and overrides primary color
- [ ] Color shade generation: DarkenColor method (15% darker for PrimaryDarken)
- [ ] Color shade generation: LightenColor method (15% lighter for PrimaryLighten)
- [ ] HexToRgb helper method converts hex to RGB tuple
- [ ] RgbToHex helper method converts RGB to hex string
- [ ] DynamicThemeService registered in BlazorApp Program.cs

**MainLayout Integration:**
- [ ] MainLayout.razor injects DynamicThemeService
- [ ] MainLayout.razor injects AuthenticationStateProvider
- [ ] LoadTenantTheme method reads tenant_id claim from authenticated user
- [ ] LoadTenantTheme calls GetThemeForTenantAsync with tenant ID
- [ ] Theme applied via MudThemeProvider Theme parameter
- [ ] StateHasChanged called after theme load to re-render UI
- [ ] Theme loads on initial authentication (OnInitializedAsync)

**API Endpoints:**
- [ ] TenantThemeEndpoints.cs created in WebAPI/Endpoints
- [ ] GET /api/v1/tenants/theme/current endpoint implemented
- [ ] GET endpoint returns current tenant's theme (from ITenantThemeService)
- [ ] GET endpoint returns default theme if no custom theme configured
- [ ] GET endpoint requires authorization
- [ ] PUT /api/v1/tenants/theme endpoint implemented
- [ ] PUT endpoint accepts UpdateTenantThemeRequest (PrimaryColor, UseCustomTheme)
- [ ] PUT endpoint validates hex color format (returns 400 if invalid)
- [ ] PUT endpoint retrieves TenantConfiguration from repository
- [ ] PUT endpoint updates PrimaryColor and UseCustomTheme properties
- [ ] PUT endpoint saves changes to database
- [ ] PUT endpoint invalidates cache via InvalidateThemeCacheAsync
- [ ] PUT endpoint requires "tenants.manage" permission
- [ ] PUT endpoint returns success message on completion
- [ ] Endpoints registered in EndpointExtensions.MapAllEndpoints
- [ ] Endpoints tagged with "Tenant Theme" in Swagger/Scalar

**Error Handling:**
- [ ] Invalid hex color returns 400 Bad Request with descriptive message
- [ ] Tenant configuration not found returns 404 Not Found
- [ ] Database errors logged and return 500 Internal Server Error
- [ ] Cache failures gracefully degrade (load from database)

**Testing:**
- [ ] Migration applies successfully (dotnet ef database update)
- [ ] TenantConfiguration table has new columns
- [ ] TenantThemeService.GetTenantThemeAsync retrieves from database
- [ ] TenantThemeService caches result in Redis (verify with Redis CLI)
- [ ] Cache TTL is 2 hours (verify expiration time)
- [ ] Cache hit returns cached value (faster response)
- [ ] IsValidHexColor validates correctly (#RRGGBB format)
- [ ] IsValidHexColor rejects invalid formats (#RGB, #RRGGBBAA, rgb(), etc.)
- [ ] DynamicThemeService generates correct color shades
- [ ] MainLayout loads tenant theme after login
- [ ] UI re-renders with new primary color
- [ ] GET /api/v1/tenants/theme/current returns theme
- [ ] PUT /api/v1/tenants/theme updates theme successfully
- [ ] PUT endpoint validates hex color
- [ ] Cache invalidates after PUT request
- [ ] Theme changes persist across page navigations
- [ ] Theme changes visible in buttons, links, icons throughout app

**Manual Testing Scenario:**
1. Login as tenant admin
2. Open browser DevTools → Application → Local Storage
3. Call GET /api/v1/tenants/theme/current → verify default theme
4. Call PUT /api/v1/tenants/theme with PrimaryColor: "#2E7D32" (green), UseCustomTheme: true
5. Verify response: { Success: true, Message: "Theme updated successfully" }
6. Refresh page → verify UI changes to green primary color
7. Check Redis CLI: GET tenant-theme:{tenantId} → verify cached JSON
8. Wait 2 hours or manually delete cache key
9. Refresh page → verify theme re-loads from database and re-caches

**Definition of Done:**
- [ ] All acceptance criteria met
- [ ] Database migration applied successfully
- [ ] Code reviewed and approved
- [ ] Unit tests for TenantThemeService caching logic
- [ ] Unit tests for DynamicThemeService color generation
- [ ] Unit tests for hex color validation
- [ ] Integration test for API endpoints
- [ ] Manual testing scenario completed
- [ ] Redis caching verified (hit rate, TTL)
- [ ] Performance tested (theme load time < 100ms)
- [ ] Security reviewed (authorization checks)
- [ ] Documentation updated (API docs, implementation guide)
- [ ] Committed to feature branch

**Effort Estimate:** 10-12 hours

**Priority:** P1 (High - Competitive differentiation)

**Dependencies:**
- User Story 1.1 (Core Theme Infrastructure)
- Redis running in Aspire environment

**Technical Notes:**
- Logo upload functionality is out of scope (future enhancement)
- Only primary color customization in this story
- Secondary/accent colors remain default
- Dark mode out of scope
- Cache invalidation critical for theme updates to take effect

---

## Sprint 3: Consistency & Polish (4-6 hours)

### User Story 3.1: Apply Design System to Existing Pages

**As a** user
**I want** all pages to have consistent styling and visual quality
**So that** the application feels polished and professional throughout

**Description:**
Apply the design system to existing pages (ProductList, ProductForm) using the new reusable components and styling patterns. Update table styling, form layouts, button consistency, and ensure proper localization and mobile responsiveness.

**Acceptance Criteria:**

**ProductList Page:**
- [ ] PageHeader component used with title, description, breadcrumbs
- [ ] Title: "Gestión de Productos" (or localized equivalent)
- [ ] Description: "Administre el catálogo de productos de su ferretería"
- [ ] Breadcrumbs: Inicio → Productos
- [ ] Action button: "Nuevo Producto" with Add icon
- [ ] LoadingState component used during data fetch
- [ ] LoadingState message: "Cargando productos..."
- [ ] EmptyState component used when no products exist
- [ ] EmptyState icon: Inventory icon
- [ ] EmptyState title: "No hay productos"
- [ ] EmptyState description: "No hay productos en el catálogo. Comience agregando su primer producto."
- [ ] EmptyState action: "Nuevo Producto" button that navigates to /products/new
- [ ] Filters card: MudPaper with Elevation="1", 12px border-radius, 24px padding
- [ ] Search field: MudTextField with Search icon, Outlined variant
- [ ] Category select: MudSelect, Outlined variant, "Todas las categorías" option
- [ ] Search button: Filled variant, Primary color, Search icon
- [ ] Table card: MudPaper with Elevation="1", 12px border-radius
- [ ] Table: MudTable with Hover="true", Striped="true"
- [ ] Table headers: Light gray background (#E9ECEF), 600 font weight
- [ ] Table columns: Imagen, SKU, Nombre, Categoría, Precio, Estado, Acciones
- [ ] Image: 40x40px with 4px border-radius, object-fit: cover
- [ ] SKU: Displayed in monospace chip (Consolas font, secondary-100 background)
- [ ] Price: Formatted with es-MX culture (e.g., "$1,234.56")
- [ ] Estado: MudChip with Success color if active, Default color if inactive
- [ ] Actions: Edit button (Primary color, Edit icon) and Delete button (Error color, Delete icon)
- [ ] Pagination: MudPagination centered below table, Primary color

**ProductForm Page:**
- [ ] PageHeader component used
- [ ] Section headers in primary color: "Información General", "Precios e Inventario"
- [ ] Section headers: H6 typography, primary-600 color, 600 weight
- [ ] Two-column grid on desktop (>= 960px): MudGrid with Spacing="4"
- [ ] Stacked layout on mobile (< 960px)
- [ ] Horizontal divider between sections
- [ ] All fields use Variant.Outlined
- [ ] Numeric fields formatted with es-MX culture where applicable
- [ ] Action buttons aligned right: Cancel (Outlined, Secondary) and Save (Filled, Primary)
- [ ] Save button has Save icon
- [ ] Loading state on save button during submission

**Styling Consistency:**
- [ ] All buttons use design system border-radius (8px)
- [ ] All cards use 12px border-radius
- [ ] All spacing uses design system variables (8px grid)
- [ ] All colors use design system palette (CSS variables)
- [ ] Shadows are subtle (elevation 1 for cards, elevation 2 for elevated elements)

**Localization:**
- [ ] All Spanish text uses IStringLocalizer
- [ ] Currency formatted with es-MX culture: `value.ToString("C", new CultureInfo("es-MX"))`
- [ ] Dates formatted with es-MX culture: dd/MM/yyyy format
- [ ] No hardcoded Spanish strings in markup

**Mobile Responsiveness:**
- [ ] ProductList tested at 375px width (iPhone SE)
- [ ] Table scrolls horizontally on mobile (horizontal overflow)
- [ ] Filters card wraps on mobile (vertical stacking)
- [ ] Action buttons stack vertically on mobile
- [ ] ProductForm fields stack vertically on mobile (< 960px)
- [ ] Padding reduces appropriately on mobile
- [ ] Touch targets at least 44px height

**Testing:**
- [ ] ProductList displays correctly with data
- [ ] ProductList shows LoadingState during fetch
- [ ] ProductList shows EmptyState when no products
- [ ] Search and filter functionality works
- [ ] Table headers styled correctly (light gray background)
- [ ] SKU displays in monospace chip
- [ ] Price formats correctly (es-MX currency)
- [ ] Edit and Delete buttons work
- [ ] Pagination works
- [ ] ProductForm renders correctly
- [ ] ProductForm sections have primary-colored headers
- [ ] ProductForm uses two-column grid on desktop
- [ ] ProductForm stacks vertically on mobile
- [ ] Form submission works
- [ ] All pages tested at 375px, 768px, 1024px, 1920px widths
- [ ] No console errors

**Definition of Done:**
- [ ] All acceptance criteria met
- [ ] Code reviewed and approved
- [ ] Visual QA performed (compare to design specs)
- [ ] Spanish localization complete
- [ ] Mobile-tested on real device or Chrome DevTools
- [ ] No accessibility issues
- [ ] Performance tested (page load < 2 seconds)
- [ ] Committed to feature branch

**Effort Estimate:** 4-6 hours

**Priority:** P2 (Medium - Consistency across app)

**Dependencies:**
- User Story 1.1 (Core Theme Infrastructure)
- User Story 2.1 (Core Reusable Components)

**Technical Notes:**
- ProductList and ProductForm already exist, modifications only
- No changes to business logic, only visual updates
- Existing functionality must continue working

---

## Additional Notes for Product Owner

### Backlog Prioritization Recommendations

**Must Have (Sprint 1):**
- User Story 1.1: Core Theme Infrastructure
- User Story 1.2: Authentication Pages Redesign

These stories create immediate visual impact and establish the foundation for all other work.

**Should Have (Sprint 2):**
- User Story 2.1: Core Reusable Components
- User Story 2.2: Multi-Tenant Theming Infrastructure

These stories enable faster development and provide competitive differentiation through tenant branding.

**Nice to Have (Sprint 3):**
- User Story 3.1: Apply Design System to Existing Pages

This story improves consistency but can be deferred if higher-priority features emerge.

### Story Point Estimates (if using Fibonacci scale)

| User Story | Hours | Story Points |
|------------|-------|--------------|
| 1.1: Core Theme Infrastructure | 6-8 | 5 |
| 1.2: Authentication Pages Redesign | 8-10 | 8 |
| 2.1: Core Reusable Components | 6-8 | 5 |
| 2.2: Multi-Tenant Theming Infrastructure | 10-12 | 8 |
| 3.1: Apply Design System to Existing Pages | 4-6 | 3 |
| **Total** | **34-44 hours** | **29 points** |

### Success Metrics to Track

After implementation, track these metrics:

1. **Visual Differentiation:** Stakeholder feedback on "Does Corelio look different from competitors?"
2. **Mobile Usability:** Mobile user engagement metrics (time on site, task completion rate)
3. **Tenant Theming Adoption:** % of tenants who customize their primary color within 3 months
4. **Development Velocity:** Time to build new pages (should reduce by ~30% with component library)
5. **User Satisfaction:** CSAT/NPS scores before and after design system implementation

### Risks & Mitigation

**Risk 1:** Font loading failure (Google Fonts CDN down)
**Mitigation:** Font-display: swap, fallback to system fonts

**Risk 2:** Tenant chooses low-contrast color
**Mitigation:** Validate color contrast ratio in future enhancement

**Risk 3:** Migration fails in production
**Mitigation:** Test migration on staging environment first, have rollback plan

**Risk 4:** Performance degradation
**Mitigation:** Monitor page load times, optimize if > 2 seconds

### Future Enhancements (Out of Scope)

These can be separate user stories in future sprints:

- Logo upload functionality (Azure Blob Storage integration)
- Advanced theming (secondary/accent color customization)
- Dark mode support
- Theme templates (Hardware Green, Safety Orange, Tool Steel Blue)
- Dashboard page with stat cards and charts
- Additional module pages (Sales, Customers, Inventory)

---

**Document Version:** 1.0
**Last Updated:** 2026-01-27
**Prepared By:** Development Team
**For:** Product Owner
