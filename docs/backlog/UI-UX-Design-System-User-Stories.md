# UI/UX Design System - Production-Ready User Stories

## Document Purpose

This document contains **backlog-ready user stories** for implementing the UI/UX Design System in Corelio. Each story is ready to be copied directly into JIRA, Azure DevOps, or your backlog management tool.

**Status:** Ready for Backlog
**Date:** 2026-01-27
**Total Effort:** 34-44 hours (29 story points)
**Sprints:** 3-4 sprints

---

## Epic Definition

### Epic: UI/UX Design System Implementation

**Epic ID:** EPIC-DESIGN-001

**Epic Description:**
As a **Product Owner**, I want to implement a professional, distinctive design system for Corelio so that the application stands out in the market, provides excellent user experience on mobile devices, and enables basic multi-tenant branding capabilities.

**Business Value:**
- **Market Differentiation:** Create distinctive visual identity that separates Corelio from generic SaaS competitors
- **Mobile-First Experience:** Enable hardware store owners to use the system on tablets/phones in-store
- **Tenant Engagement:** Allow tenants to customize their primary color and logo for brand personalization
- **Development Velocity:** Provide reusable component library to accelerate feature development by ~30%

**Success Metrics:**
1. **Visual Quality:** Login page achieves "professional and distinctive" rating in stakeholder review
2. **Mobile Usability:** All pages functional at 375px width (iPhone SE) with touch targets ≥ 44px
3. **Tenant Adoption:** 30% of tenants customize their theme within 3 months post-launch
4. **Development Efficiency:** Time to build new pages reduces by 30% using component library
5. **User Satisfaction:** CSAT/NPS scores improve by 10+ points post-implementation

**Out of Scope (Future Enhancements):**
- Logo upload functionality (Azure Blob Storage integration)
- Advanced theming (secondary/accent color customization)
- Dark mode support
- Theme templates (pre-designed color schemes)
- Dashboard page with charts/visualizations

**Epic Owner:** Product Owner
**Epic Status:** Pending Approval
**Target Start:** Next Sprint
**Estimated Completion:** 3-4 sprints from start

---

## Sprint 1: Foundation & First Impression (14-18 hours)

### User Story 1.1: Core Theme Infrastructure

**Story ID:** US-DESIGN-1.1
**Story Title:** Implement Core Design System Infrastructure

**Priority:** P0 (Critical - Foundation)
**Story Points:** 5
**Effort Estimate:** 6-8 hours
**Sprint:** Sprint 1

---

#### User Story

**As a** developer,
**I want** a centralized design system with custom colors, typography, and spacing,
**So that** all pages have consistent visual appearance and styling decisions are standardized.

---

#### Description

Implement the core theme infrastructure including MudTheme configuration with "Industrial Terracotta" color palette, CSS custom properties for design tokens, Inter font integration, and removal of Bootstrap CSS. This provides the foundation for all subsequent UI work.

**Context:**
- Corelio currently uses default MudBlazor styling (Material Design blue)
- Bootstrap CSS causes conflicts and inconsistencies
- No standardized spacing or color system
- This story establishes the visual foundation for the entire application

**Related Documents:**
- `docs/recommendations/UI-UX-Design-System-Recommendation.md` (Section: Phase 1)
- `docs/recommendations/UI-UX-Implementation-Guide.md` (Section: Phase 1)
- `docs/recommendations/Design-System-Visual-Reference.md` (Color Palette section)

---

#### Acceptance Criteria

**Core Theme Configuration:**
- [ ] **AC1.1:** ThemeConfiguration.cs created in `src/Presentation/Corelio.BlazorApp/Services/` with `CorelioDefaultTheme` static property
- [ ] **AC1.2:** Primary color configured as #E74C3C (Terracotta Red) in Palette.Primary
- [ ] **AC1.3:** Primary color shades configured: PrimaryDarken (#CB4335), PrimaryLighten (#EC7063), PrimaryContrastText (#FFFFFF)
- [ ] **AC1.4:** Secondary color configured as #6C757D (Concrete Gray) in Palette.Secondary
- [ ] **AC1.5:** Secondary color shades configured: SecondaryDarken (#495057), SecondaryLighten (#ADB5BD), SecondaryContrastText (#FFFFFF)
- [ ] **AC1.6:** Semantic colors configured: Success (#28A745), Warning (#FFC107), Error (#DC3545), Info (#17A2B8)
- [ ] **AC1.7:** Background colors configured: Background (#FFFFFF), BackgroundGrey (#F8F9FA), Surface (#FFFFFF)
- [ ] **AC1.8:** Text colors configured: TextPrimary (#212529), TextSecondary (#6C757D), TextDisabled (#ADB5BD)
- [ ] **AC1.9:** Border colors configured: Divider (#DEE2E6), DividerLight (#E9ECEF)

**Typography Configuration:**
- [ ] **AC2.1:** Typography.Default configured with Inter font family: `["Inter", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif"]`
- [ ] **AC2.2:** H1 configured: FontSize=2.5rem (40px), FontWeight=700, LineHeight=1.2, LetterSpacing=-0.02em
- [ ] **AC2.3:** H2 configured: FontSize=2rem (32px), FontWeight=700, LineHeight=1.3, LetterSpacing=-0.01em
- [ ] **AC2.4:** H3 configured: FontSize=1.75rem (28px), FontWeight=600, LineHeight=1.3, LetterSpacing=-0.01em
- [ ] **AC2.5:** H4 configured: FontSize=1.5rem (24px), FontWeight=600, LineHeight=1.4, LetterSpacing=normal
- [ ] **AC2.6:** H5 configured: FontSize=1.25rem (20px), FontWeight=600, LineHeight=1.4, LetterSpacing=normal
- [ ] **AC2.7:** H6 configured: FontSize=1.125rem (18px), FontWeight=600, LineHeight=1.5, LetterSpacing=normal
- [ ] **AC2.8:** Body1 configured: FontSize=1rem (16px), FontWeight=400, LineHeight=1.5, LetterSpacing=normal
- [ ] **AC2.9:** Body2 configured: FontSize=0.875rem (14px), FontWeight=400, LineHeight=1.5, LetterSpacing=normal
- [ ] **AC2.10:** Button configured: FontSize=0.875rem (14px), FontWeight=600, LineHeight=1.75, LetterSpacing=0.02em, **TextTransform=none** (CRITICAL: No uppercase)
- [ ] **AC2.11:** Caption configured: FontSize=0.75rem (12px), FontWeight=400, LineHeight=1.66, LetterSpacing=0.01em

**Shadow Configuration:**
- [ ] **AC3.1:** Shadows.Elevation[1] configured: `0 1px 2px 0 rgba(0, 0, 0, 0.05)` (subtle)
- [ ] **AC3.2:** Shadows.Elevation[4] configured: `0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)` (standard cards)
- [ ] **AC3.3:** Shadows.Elevation[8] configured: `0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)` (elevated modals)
- [ ] **AC3.4:** Shadows are noticeably more subtle than default Material Design shadows (visual QA verification)

**Layout Properties:**
- [ ] **AC4.1:** LayoutProperties.DefaultBorderRadius set to "8px"
- [ ] **AC4.2:** LayoutProperties.AppbarHeight set to "64px"
- [ ] **AC4.3:** LayoutProperties.DrawerWidthLeft set to "260px"
- [ ] **AC4.4:** LayoutProperties.DrawerWidthRight set to "300px"

**CSS Variables File:**
- [ ] **AC5.1:** variables.css created in `src/Presentation/Corelio.BlazorApp/wwwroot/css/`
- [ ] **AC5.2:** Primary color shades defined (--color-primary-50 through --color-primary-900)
- [ ] **AC5.3:** Secondary color shades defined (--color-secondary-50 through --color-secondary-900)
- [ ] **AC5.4:** Semantic colors defined (--color-success, --color-warning, --color-error, --color-info)
- [ ] **AC5.5:** Spacing variables defined (--space-0 through --space-24 using 8px grid)
- [ ] **AC5.6:** Shadow variables defined (--shadow-sm, --shadow-md, --shadow-lg, --shadow-xl)
- [ ] **AC5.7:** Border radius variables defined (--radius-sm, --radius-md, --radius-lg, --radius-xl, --radius-full)
- [ ] **AC5.8:** Font family variable defined (--font-family with Inter and fallbacks)
- [ ] **AC5.9:** CSS variables accessible via `var(--color-primary-400)` syntax throughout application

**App.razor Updates:**
- [ ] **AC6.1:** Inter font loaded via Google Fonts CDN: `https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap`
- [ ] **AC6.2:** Preconnect hints added for Google Fonts: `<link rel="preconnect" href="https://fonts.googleapis.com">` and `<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>`
- [ ] **AC6.3:** Bootstrap CSS completely removed from App.razor (no `bootstrap/bootstrap.min.css` link)
- [ ] **AC6.4:** CSS load order correct: MudBlazor CSS → variables.css → app.css → scoped CSS
- [ ] **AC6.5:** HTML lang attribute set to "es-MX"
- [ ] **AC6.6:** Material Design Icons CSS loaded for MudBlazor icon support

**MainLayout.razor Updates:**
- [ ] **AC7.1:** MainLayout imports `Corelio.BlazorApp.Services` namespace
- [ ] **AC7.2:** MudThemeProvider configured with `Theme="@ThemeConfiguration.CorelioDefaultTheme"`
- [ ] **AC7.3:** MudAppBar styled with white background: `Style="background: white; border-bottom: 1px solid var(--color-secondary-200);"`
- [ ] **AC7.4:** MudAppBar has Elevation="1" (subtle shadow)
- [ ] **AC7.5:** Store icon in AppBar uses Color.Primary (terracotta red)
- [ ] **AC7.6:** "Corelio ERP" text in AppBar uses primary color (#B03A2E via inline style or class)
- [ ] **AC7.7:** Main content area background set to light gray: `Style="background: var(--color-secondary-50); min-height: 100vh;"`
- [ ] **AC7.8:** MudContainer has consistent padding: `Class="pa-6"` (24px padding)
- [ ] **AC7.9:** MudDrawer has white background and Elevation="2"

**app.css MudBlazor Overrides:**
- [ ] **AC8.1:** app.css updated in `src/Presentation/Corelio.BlazorApp/wwwroot/`
- [ ] **AC8.2:** Global font-family set to `var(--font-family)` for html/body elements
- [ ] **AC8.3:** Global font-size set to 16px for html/body elements
- [ ] **AC8.4:** Global text color set to `var(--color-secondary-800)` for html/body elements
- [ ] **AC8.5:** Button text-transform overridden to `none !important` (`.mud-button-root` selector)
- [ ] **AC8.6:** Button border-radius overridden to `var(--radius-md) !important` (8px)
- [ ] **AC8.7:** Button font-weight overridden to `600 !important`
- [ ] **AC8.8:** Card border-radius overridden to `var(--radius-lg) !important` (12px) (`.mud-paper`, `.mud-card` selectors)
- [ ] **AC8.9:** Input border-radius overridden to `var(--radius-md) !important` (8px) (`.mud-input` selector)
- [ ] **AC8.10:** Table border-radius overridden to `var(--radius-lg) !important` (12px) (`.mud-table` selector)
- [ ] **AC8.11:** Input focus state styled with primary color border and subtle shadow: `border-color: var(--color-primary-400) !important; box-shadow: 0 0 0 3px rgba(231, 76, 60, 0.1) !important;`
- [ ] **AC8.12:** Utility classes added: `.text-primary`, `.text-secondary`, `.text-success`, `.text-warning`, `.text-error`
- [ ] **AC8.13:** Utility classes added: `.bg-light`, `.bg-white`
- [ ] **AC8.14:** Utility classes added: `.shadow-sm`, `.shadow-md`, `.shadow-lg`
- [ ] **AC8.15:** Spacing utility classes added: `.mt-0` through `.mt-8`, `.mb-0` through `.mb-8`
- [ ] **AC8.16:** Responsive media query added for mobile (< 960px): html/body font-size reduces to 14px

**Testing & Validation:**
- [ ] **AC9.1:** Solution compiles without errors: `dotnet build src/Presentation/Corelio.BlazorApp` succeeds
- [ ] **AC9.2:** Application runs without errors: `dotnet run --project src/Aspire/Corelio.AppHost` succeeds
- [ ] **AC9.3:** Inter font loads successfully (DevTools Network tab shows successful Google Fonts request)
- [ ] **AC9.4:** Bootstrap CSS not loaded (DevTools Sources tab confirms no bootstrap.min.css)
- [ ] **AC9.5:** Primary color renders as #E74C3C in UI (DevTools Elements inspector confirms computed color)
- [ ] **AC9.6:** Button text is NOT uppercase (visual verification: "Guardar Cambios" not "GUARDAR CAMBIOS")
- [ ] **AC9.7:** Border radius visibly softer on cards (12px) and buttons (8px) compared to before (4px)
- [ ] **AC9.8:** Shadows are noticeably more subtle than before (visual QA comparison)
- [ ] **AC9.9:** AppBar has white background with subtle bottom border (visual verification)
- [ ] **AC9.10:** Main content area has light gray background (#F8F9FA) (visual verification)
- [ ] **AC9.11:** CSS variables accessible via browser console: `getComputedStyle(document.documentElement).getPropertyValue('--color-primary-400')` returns "#E74C3C"
- [ ] **AC9.12:** Mobile responsiveness: Base font size reduces to 14px at < 960px width (DevTools Responsive mode verification)
- [ ] **AC9.13:** No console errors or warnings in browser DevTools Console
- [ ] **AC9.14:** No CORS errors for Google Fonts (verify in Network tab)

**Browser Compatibility:**
- [ ] **AC10.1:** Tested on Chrome 120+ (Windows): All visual elements render correctly
- [ ] **AC10.2:** Tested on Firefox 115+ (Windows): All visual elements render correctly
- [ ] **AC10.3:** Tested on Edge 120+ (Windows): All visual elements render correctly
- [ ] **AC10.4:** Tested on mobile Chrome DevTools (375px width): Responsive adjustments work correctly

**Accessibility:**
- [ ] **AC11.1:** Color contrast ratio for #E74C3C (Primary) on #FFFFFF (White) is ≥ 4.5:1 (WCAG AA) - Verify with contrast checker tool
- [ ] **AC11.2:** Color contrast ratio for #212529 (Text Primary) on #FFFFFF (White) is ≥ 7:1 (WCAG AAA) - Verify with contrast checker tool
- [ ] **AC11.3:** Color contrast ratio for #6C757D (Text Secondary) on #FFFFFF (White) is ≥ 4.5:1 (WCAG AA) - Verify with contrast checker tool

---

#### Definition of Done

- [ ] All acceptance criteria met (100% completion)
- [ ] Code reviewed and approved by at least one team member
- [ ] No compilation errors or warnings
- [ ] Visual QA checklist completed (colors, typography, spacing, shadows)
- [ ] Browser compatibility verified on Chrome, Firefox, Edge (latest versions)
- [ ] Mobile responsiveness verified at 375px, 768px, 1024px, 1920px widths
- [ ] Accessibility verified (color contrast ratios meet WCAG AA standards)
- [ ] No console errors or warnings in browser DevTools
- [ ] Performance verified (page load time < 2 seconds, no font loading delays > 500ms)
- [ ] Documentation updated (CLAUDE.md if setup instructions changed)
- [ ] Committed to feature branch: `feature/US-DESIGN-1.1-core-theme-infrastructure`
- [ ] Pull request created with screenshots of before/after comparison

---

#### Technical Notes

**Files Created:**
- `src/Presentation/Corelio.BlazorApp/Services/ThemeConfiguration.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/wwwroot/css/variables.css` (NEW)

**Files Modified:**
- `src/Presentation/Corelio.BlazorApp/Components/App.razor`
- `src/Presentation/Corelio.BlazorApp/Components/Layout/MainLayout.razor`
- `src/Presentation/Corelio.BlazorApp/wwwroot/app.css`

**Dependencies:**
- MudBlazor 7.x (already installed)
- Google Fonts CDN (Inter font family)
- No new NuGet packages required

**Breaking Changes:**
- None - All changes are additive or visual/styling only

**Performance Impact:**
- Inter font loading: ~100KB (5 weights), cached by browser
- CSS size increase: +15KB (variables + overrides), gzipped
- Page load time impact: < 50ms (negligible)

**Risks:**
- **Low Risk:** Google Fonts CDN failure (rare) - Mitigated by font-display: swap and fallback fonts
- **Low Risk:** CSS variable support in older browsers - Mitigated by targeting modern browsers only (Chrome 90+, Firefox 88+, Safari 14+, Edge 90+)

**Testing Strategy:**
- Manual visual QA in browser (primary testing method)
- DevTools inspection for computed styles and CSS variables
- Responsive testing in Chrome DevTools Device Mode
- Contrast checker tool for accessibility verification
- No automated tests required for this story (purely visual changes)

**Follow-up Stories:**
- US-DESIGN-1.2: Authentication Pages Redesign (depends on this story)
- US-DESIGN-2.1: Core Reusable Components (depends on this story)

---

#### Reference Materials

**Design System Specifications:**
- Color Palette: See `docs/recommendations/Design-System-Visual-Reference.md` (Section: Color Palette)
- Typography Scale: See `docs/recommendations/UI-UX-Design-System-Recommendation.md` (Section: Typography)
- Spacing System: See `docs/recommendations/UI-UX-Design-System-Recommendation.md` (Section: Spacing System)

**Implementation Guide:**
- Step-by-step instructions: `docs/recommendations/UI-UX-Implementation-Guide.md` (Phase 1)
- Code snippets provided in implementation guide

**Visual Mockups:**
- Before/After comparison: `docs/recommendations/Design-System-Visual-Reference.md` (Section: Component Comparison)

---

---

## User Story 1.2: Authentication Pages Redesign

**Story ID:** US-DESIGN-1.2
**Story Title:** Redesign Authentication Pages with New Design System

**Priority:** P0 (Critical - First Impression)
**Story Points:** 8
**Effort Estimate:** 8-10 hours
**Sprint:** Sprint 1

---

#### User Story

**As a** potential customer evaluating Corelio,
**I want** a professional, welcoming login experience,
**So that** I feel confident the product is high-quality and trustworthy.

---

#### Description

Redesign all authentication pages (Login, Register, ForgotPassword, ResetPassword) with the new design system. Create a dedicated AuthLayout for minimal, full-screen auth experiences. Focus on generous whitespace, large touch targets, clear visual hierarchy, and smooth animations to create an excellent first impression.

**Context:**
- Current authentication pages use default MudBlazor styling with MainLayout (AppBar, Drawer)
- Login page lacks visual polish and professional appearance
- Poor mobile experience (cramped on small screens)
- First impression is critical for trial users and stakeholder demos

**Impact:**
- This is the FIRST PAGE every user sees
- Professional appearance builds trust and credibility
- Mobile-optimized for hardware store owners evaluating on tablets/phones

**Related Documents:**
- `docs/recommendations/UI-UX-Design-System-Recommendation.md` (Section: Phase 2)
- `docs/recommendations/UI-UX-Implementation-Guide.md` (Section: Phase 2)
- `docs/recommendations/Design-System-Visual-Reference.md` (Section: Login Page Mockup)

---

#### Acceptance Criteria

**AuthLayout Component:**
- [ ] **AC1.1:** AuthLayout.razor created in `src/Presentation/Corelio.BlazorApp/Components/Layout/`
- [ ] **AC1.2:** Layout uses MudThemeProvider with CorelioDefaultTheme
- [ ] **AC1.3:** Layout has NO AppBar (minimal, full-screen experience)
- [ ] **AC1.4:** Layout has NO Drawer/navigation menu
- [ ] **AC1.5:** Background has subtle gradient: `background: linear-gradient(135deg, #F8F9FA 0%, #E9ECEF 100%);`
- [ ] **AC1.6:** Background has repeating diagonal pattern overlay (30% opacity, optional decorative element)
- [ ] **AC1.7:** Content container is centered both horizontally and vertically: `display: flex; align-items: center; justify-content: center; min-height: 100vh;`
- [ ] **AC1.8:** Content max-width: 480px on desktop, 100% on mobile (< 600px)
- [ ] **AC1.9:** Content padding: 48px on desktop, 32px on tablet, 24px on mobile
- [ ] **AC1.10:** Footer with copyright text: "© 2026 Corelio ERP. Todos los derechos reservados."
- [ ] **AC1.11:** Footer uses Caption typography (12px, secondary color)
- [ ] **AC1.12:** Footer positioned at bottom of viewport

**Login Page - Hero Section:**
- [ ] **AC2.1:** Login.razor uses `@layout AuthLayout` directive
- [ ] **AC2.2:** Large logo section at top of card
- [ ] **AC2.3:** Store icon (Material.Filled.Store) size: 4rem (64px) on desktop, 3rem (48px) on mobile
- [ ] **AC2.4:** Icon displayed in circular gradient background: `background: linear-gradient(135deg, var(--color-primary-100) 0%, var(--color-primary-200) 100%);`
- [ ] **AC2.5:** Icon circle diameter: 96px on desktop, 80px on mobile
- [ ] **AC2.6:** Icon color: Primary-700 (#B03A2E)
- [ ] **AC2.7:** App title "Corelio ERP" displayed below icon
- [ ] **AC2.8:** Title typography: H3 (28px), 700 weight, primary-700 color
- [ ] **AC2.9:** Subtitle "Sistema de Gestión para Ferreterías" below title
- [ ] **AC2.10:** Subtitle typography: Body1 (16px), secondary color
- [ ] **AC2.11:** Spacing between icon and title: 16px (var(--space-4))
- [ ] **AC2.12:** Spacing between title and subtitle: 8px (var(--space-2))
- [ ] **AC2.13:** Spacing between hero section and form: 32px (var(--space-8))

**Login Page - Form Card:**
- [ ] **AC3.1:** Form card uses MudPaper with Elevation="8" (prominent shadow)
- [ ] **AC3.2:** Card border-radius: 16px (var(--radius-xl)) - softer, hero card
- [ ] **AC3.3:** Card padding: 48px on desktop (>= 960px), 32px on mobile (< 960px)
- [ ] **AC3.4:** Card max-width: 480px on desktop, 100% on mobile
- [ ] **AC3.5:** Card background: White (#FFFFFF)
- [ ] **AC3.6:** Email field uses MudTextField with Variant.Outlined
- [ ] **AC3.7:** Email field has email icon in Adornment.Start position
- [ ] **AC3.8:** Email field label: "Correo Electrónico" (localized via IStringLocalizer)
- [ ] **AC3.9:** Email field helper text: "ejemplo@ferreteria.mx" (localized)
- [ ] **AC3.10:** Email field required validation configured
- [ ] **AC3.11:** Password field uses MudTextField with Variant.Outlined
- [ ] **AC3.12:** Password field has lock icon in Adornment.Start position
- [ ] **AC3.13:** Password field has visibility toggle icon in Adornment.End position (eye/eye-off)
- [ ] **AC3.14:** Password field label: "Contraseña" (localized)
- [ ] **AC3.15:** Password field InputType toggles between Password and Text on icon click
- [ ] **AC3.16:** Password field required validation configured
- [ ] **AC3.17:** Form fields have 16px vertical spacing (var(--space-4))

**Login Page - Submit Button:**
- [ ] **AC4.1:** Submit button uses MudButton with Variant.Filled, Color.Primary
- [ ] **AC4.2:** Button height: 56px (Class="py-4" or custom style)
- [ ] **AC4.3:** Button full-width: FullWidth="true"
- [ ] **AC4.4:** Button text: "Iniciar Sesión" (localized)
- [ ] **AC4.5:** Button disabled when form invalid or during loading
- [ ] **AC4.6:** Button shows loading state: spinner + "Cargando..." text when form submitting
- [ ] **AC4.7:** Button has smooth transition on hover (0.2s ease)
- [ ] **AC4.8:** Spacing between password field and button: 24px (var(--space-6))

**Login Page - Alerts & Messages:**
- [ ] **AC5.1:** Error alerts use MudAlert with Severity.Error, Variant.Filled
- [ ] **AC5.2:** Error alerts have error icon (Icons.Material.Filled.Error)
- [ ] **AC5.3:** Error alerts have 8px border-radius (var(--radius-md))
- [ ] **AC5.4:** Warning alerts use MudAlert with Severity.Warning, Variant.Outlined
- [ ] **AC5.5:** Warning alert displays when `?expired=true` query parameter present: "Su sesión ha expirado. Por favor, inicie sesión nuevamente." (localized)
- [ ] **AC5.6:** Warning alerts have warning icon (Icons.Material.Filled.Warning)
- [ ] **AC5.7:** Alerts positioned above form with 16px bottom margin

**Login Page - Additional Links:**
- [ ] **AC6.1:** "¿Olvidaste tu contraseña?" link displayed below submit button
- [ ] **AC6.2:** Forgot password link styled with primary color (#CB4335)
- [ ] **AC6.3:** Forgot password link has underline on hover
- [ ] **AC6.4:** Forgot password link navigates to `/auth/forgot-password`
- [ ] **AC6.5:** Spacing between button and link: 16px (var(--space-4))
- [ ] **AC6.6:** Horizontal divider (MudDivider) displayed below forgot password link
- [ ] **AC6.7:** Divider margin: 24px top and bottom (var(--space-6))
- [ ] **AC6.8:** "¿No tienes una cuenta?" text displayed below divider (Body2, secondary color)
- [ ] **AC6.9:** "Contacta a tu administrador" text displayed below (Body2, secondary color)
- [ ] **AC6.10:** Text aligned center

**Login Page - Animations:**
- [ ] **AC7.1:** Page has smooth fade-in animation on load
- [ ] **AC7.2:** Animation duration: 0.4s with ease-out timing
- [ ] **AC7.3:** Animation CSS: `@keyframes fadeIn { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }`
- [ ] **AC7.4:** Animation applied to main card: `animation: fadeIn 0.4s ease-out;`

**Register Page:**
- [ ] **AC8.1:** Register.razor uses `@layout AuthLayout` directive
- [ ] **AC8.2:** Same hero section as Login page (Store icon, title, subtitle)
- [ ] **AC8.3:** Card styling matches Login page (Elevation="8", 16px radius, generous padding)
- [ ] **AC8.4:** Page title "Registrar Usuario" displayed with PersonAdd icon
- [ ] **AC8.5:** Title typography: H4 (24px), primary color (#B03A2E)
- [ ] **AC8.6:** Section header "Información Personal" displayed (H6, primary-600 color, 600 weight)
- [ ] **AC8.7:** First Name field (MudTextField, Variant.Outlined, required, icon: Person)
- [ ] **AC8.8:** Last Name field (MudTextField, Variant.Outlined, required, icon: Person)
- [ ] **AC8.9:** Email field (MudTextField, Variant.Outlined, required, email validation, icon: Email)
- [ ] **AC8.10:** Two-column grid layout for Personal Information section on desktop (>= 960px): MudGrid with Spacing="4"
- [ ] **AC8.11:** Stacked layout for Personal Information section on mobile (< 960px)
- [ ] **AC8.12:** Horizontal divider (MudDivider) between sections with 24px margin
- [ ] **AC8.13:** Section header "Seguridad" displayed (H6, primary-600 color, 600 weight)
- [ ] **AC8.14:** Password field (MudTextField, Variant.Outlined, required, visibility toggle, icon: Lock)
- [ ] **AC8.15:** Confirm Password field (MudTextField, Variant.Outlined, required, visibility toggle, icon: Lock)
- [ ] **AC8.16:** Password match validation: Error message if passwords don't match
- [ ] **AC8.17:** Action buttons aligned right with 16px gap
- [ ] **AC8.18:** Cancel button (Variant.Outlined, Color.Secondary, navigates to /auth/login)
- [ ] **AC8.19:** Register button (Variant.Filled, Color.Primary, min-width: 160px, icon: PersonAdd)
- [ ] **AC8.20:** Register button shows loading state during submission
- [ ] **AC8.21:** Success alert (MudAlert, Severity.Success) on registration complete
- [ ] **AC8.22:** Auto-redirect to /auth/login after 2 seconds on success
- [ ] **AC8.23:** Smooth fade-in animation matches Login page

**ForgotPassword Page:**
- [ ] **AC9.1:** ForgotPassword.razor uses `@layout AuthLayout` directive
- [ ] **AC9.2:** Same card styling as Login/Register (Elevation="8", 16px radius, generous padding)
- [ ] **AC9.3:** Email icon in hero section (Material.Filled.Email, 4rem size, circular gradient background)
- [ ] **AC9.4:** Page title "Recuperar Contraseña" (H4, primary color)
- [ ] **AC9.5:** Description text: "Ingrese su correo electrónico y le enviaremos instrucciones para restablecer su contraseña." (Body2, secondary color)
- [ ] **AC9.6:** Email field (MudTextField, Variant.Outlined, required, email validation, icon: Email)
- [ ] **AC9.7:** Submit button "Enviar Instrucciones" (Variant.Filled, Color.Primary, full-width, 56px height)
- [ ] **AC9.8:** Submit button shows loading state
- [ ] **AC9.9:** Back to login link: "Volver a Iniciar Sesión" (primary color, navigates to /auth/login)
- [ ] **AC9.10:** Success alert on email sent
- [ ] **AC9.11:** Smooth fade-in animation matches other auth pages

**ResetPassword Page:**
- [ ] **AC10.1:** ResetPassword.razor uses `@layout AuthLayout` directive
- [ ] **AC10.2:** Same card styling as other auth pages (Elevation="8", 16px radius, generous padding)
- [ ] **AC10.3:** Key icon in hero section (Material.Filled.Key, 4rem size, circular gradient background)
- [ ] **AC10.4:** Page title "Restablecer Contraseña" (H4, primary color)
- [ ] **AC10.5:** Description text: "Ingrese su nueva contraseña" (Body2, secondary color)
- [ ] **AC10.6:** New Password field (MudTextField, Variant.Outlined, required, visibility toggle, icon: Lock)
- [ ] **AC10.7:** Confirm Password field (MudTextField, Variant.Outlined, required, visibility toggle, icon: Lock)
- [ ] **AC10.8:** Password match validation
- [ ] **AC10.9:** Submit button "Restablecer Contraseña" (Variant.Filled, Color.Primary, full-width, 56px height)
- [ ] **AC10.10:** Submit button shows loading state
- [ ] **AC10.11:** Success alert on password reset complete
- [ ] **AC10.12:** Auto-redirect to /auth/login after 2 seconds on success
- [ ] **AC10.13:** Smooth fade-in animation matches other auth pages

**Spanish Localization:**
- [ ] **AC11.1:** All user-facing text uses IStringLocalizer<SharedResource>
- [ ] **AC11.2:** Resource keys created for all labels: Email, Password, Login, Register, etc.
- [ ] **AC11.3:** Resource keys created for all buttons: "Iniciar Sesión", "Registrar", "Cancelar", etc.
- [ ] **AC11.4:** Resource keys created for all messages: success, error, validation messages
- [ ] **AC11.5:** No hardcoded Spanish strings in Razor markup (all via @L["ResourceKey"])

**Mobile Optimization:**
- [ ] **AC12.1:** Auth card max-width: 480px on desktop, 100% on mobile (< 600px)
- [ ] **AC12.2:** Auth card padding reduces on mobile: 32px (< 960px), 24px (< 600px)
- [ ] **AC12.3:** Font size appropriate for mobile: 14px base at < 960px width
- [ ] **AC12.4:** Touch targets at least 44px height for all interactive elements
- [ ] **AC12.5:** Form fields stack vertically on mobile (no side-by-side)
- [ ] **AC12.6:** Logo icon reduces to 48px on mobile (from 64px desktop) via CSS media query
- [ ] **AC12.7:** Icon circle reduces to 80px on mobile (from 96px desktop)
- [ ] **AC12.8:** Button text remains readable at mobile sizes (14px minimum)
- [ ] **AC12.9:** Tested at 375px width (iPhone SE): All elements visible and functional

**Testing & Validation:**
- [ ] **AC13.1:** Login page loads without errors
- [ ] **AC13.2:** Login form submission works (both success and error cases)
- [ ] **AC13.3:** Password visibility toggle works (eye icon click toggles InputType)
- [ ] **AC13.4:** Session expired warning displays correctly when ?expired=true
- [ ] **AC13.5:** Forgot password link navigation works
- [ ] **AC13.6:** Register page renders correctly on desktop and mobile
- [ ] **AC13.7:** Register form validation works (required fields, email format, password match)
- [ ] **AC13.8:** Register form two-column grid works on desktop, stacks on mobile
- [ ] **AC13.9:** Register form submission shows loading state
- [ ] **AC13.10:** Success message displays and auto-redirects after 2 seconds
- [ ] **AC13.11:** Cancel button on Register page navigates back to Login
- [ ] **AC13.12:** ForgotPassword page styled consistently with other auth pages
- [ ] **AC13.13:** ForgotPassword email submission works
- [ ] **AC13.14:** ResetPassword page styled consistently with other auth pages
- [ ] **AC13.15:** ResetPassword submission works with password validation
- [ ] **AC13.16:** All pages tested at 375px (iPhone SE), 768px (iPad), 1920px (desktop)
- [ ] **AC13.17:** Fade-in animations smooth and not jarring (0.4s duration feels natural)
- [ ] **AC13.18:** No console errors or warnings on any auth page

---

#### Definition of Done

- [ ] All acceptance criteria met (100% completion)
- [ ] Code reviewed and approved by at least one team member
- [ ] Visual QA performed (compare to design specs and mockups in Visual Reference document)
- [ ] Spanish localization complete (all resource keys created, no hardcoded strings)
- [ ] Mobile-tested on real device or Chrome DevTools Device Mode at 375px, 768px, 1920px
- [ ] Accessibility verified (color contrast, keyboard navigation, tab order)
- [ ] No console errors or warnings in browser DevTools
- [ ] Form validation works correctly (required fields, email format, password match)
- [ ] Loading states work correctly (spinner + text during submission)
- [ ] Navigation between auth pages works correctly
- [ ] Auto-redirects work correctly (Register → Login, ResetPassword → Login)
- [ ] Animations are smooth and performant (no jank)
- [ ] Documentation updated if needed
- [ ] Committed to feature branch: `feature/US-DESIGN-1.2-authentication-redesign`
- [ ] Pull request created with screenshots of all auth pages (desktop + mobile)

---

#### Technical Notes

**Files Created:**
- `src/Presentation/Corelio.BlazorApp/Components/Layout/AuthLayout.razor` (NEW)
- `src/Presentation/Corelio.BlazorApp/Components/Layout/AuthLayout.razor.css` (NEW, optional for scoped styles)

**Files Modified:**
- `src/Presentation/Corelio.BlazorApp/Pages/Auth/Login.razor`
- `src/Presentation/Corelio.BlazorApp/Pages/Auth/Register.razor`
- `src/Presentation/Corelio.BlazorApp/Pages/Auth/ForgotPassword.razor`
- `src/Presentation/Corelio.BlazorApp/Pages/Auth/ResetPassword.razor`

**Resource Files:**
- `src/Presentation/Corelio.BlazorApp/Resources/SharedResource.es-MX.resx` (add new keys)

**Dependencies:**
- User Story 1.1 (Core Theme Infrastructure) must be completed first
- IStringLocalizer<SharedResource> already configured (assume existing)

**Breaking Changes:**
- None - Existing authentication logic unchanged, only visual redesign

**Performance Impact:**
- Minimal - Animation CSS adds < 1KB
- Page load time impact: < 50ms (negligible)

**Risks:**
- **Low Risk:** Animation performance on low-end devices - Mitigated by simple fade-in (0.4s, no complex transforms)
- **Low Risk:** Font size too small on mobile - Mitigated by testing at 375px width and 14px base font

**Testing Strategy:**
- Manual visual QA in browser (primary testing method)
- Manual form submission testing (success and error cases)
- Manual mobile responsive testing in Chrome DevTools Device Mode
- Manual keyboard navigation testing (tab order, enter key submit)
- No automated UI tests for this story (visual changes only)

**Follow-up Stories:**
- US-DESIGN-2.1: Core Reusable Components (independent, can start after 1.1)
- US-DESIGN-3.1: Apply Design System to Existing Pages (depends on 2.1)

---

#### Reference Materials

**Design System Specifications:**
- Login page mockup: `docs/recommendations/Design-System-Visual-Reference.md` (Section: Login Page Mockup Desktop & Mobile)
- Authentication styling: `docs/recommendations/UI-UX-Design-System-Recommendation.md` (Section: Phase 2)

**Implementation Guide:**
- Step-by-step instructions: `docs/recommendations/UI-UX-Implementation-Guide.md` (Section: Phase 2)
- AuthLayout code example: Implementation guide contains full code snippet

---

---

## Sprint 2: Component Library & Theming (16-18 hours)

### User Story 2.1: Core Reusable Components

**Story ID:** US-DESIGN-2.1
**Story Title:** Create Reusable UI Component Library

**Priority:** P1 (High - Enables Faster Development)
**Story Points:** 5
**Effort Estimate:** 6-8 hours
**Sprint:** Sprint 2

---

#### User Story

**As a** developer,
**I want** a library of polished, reusable UI components,
**So that** I can build new pages faster with consistent patterns and eliminate duplicate code.

---

#### Description

Create a library of core reusable components following the design system that address common UI patterns: page headers with breadcrumbs, loading states, empty states, and enhance existing TenantDisplay and UserDisplay components. These components eliminate inconsistency and accelerate development of new features.

**Context:**
- Current pages have duplicate code for common patterns (loading spinners, empty states, page headers)
- No standardized component library leads to inconsistency
- Developers spend time recreating common UI patterns for each new feature
- Component library will provide ~30% development velocity improvement

**Impact:**
- Faster feature development (reusable patterns)
- Consistent UX across all pages
- Reduced code duplication and maintenance burden
- Professional polish on all pages (loading, empty states)

**Related Documents:**
- `docs/recommendations/UI-UX-Design-System-Recommendation.md` (Section: Phase 3)
- `docs/recommendations/UI-UX-Implementation-Guide.md` (Section: Phase 3)
- `docs/recommendations/Design-System-Visual-Reference.md` (Component mockups)

---

#### Acceptance Criteria

(Due to length constraints, I'll provide a summary format - full AC would follow similar pattern to above)

**PageHeader Component (PageHeader.razor):**
- [ ] **AC1.1-1.20:** Component created with required Title parameter, optional Description/Breadcrumbs/Actions, responsive layout, proper spacing, border bottom, H4 typography, Body2 description, breadcrumb navigation, action button slot
- [ ] **AC1.21-1.25:** Mobile responsive (actions wrap, margins reduce), tested at 375px/768px/1920px, usage example documented

**LoadingState Component (LoadingState.razor):**
- [ ] **AC2.1-2.10:** Component created with optional Message parameter (default "Cargando..."), 64px primary spinner, Body1 message typography, centered flex layout, min-height 320px, padding 48px desktop/32px mobile, CSS variables used, responsive tested

**EmptyState Component (EmptyState.razor):**
- [ ] **AC3.1-3.15:** Component created with Icon/Title/Description (required), optional ActionText/ActionIcon/OnAction, 4rem icon in 96px circular gray background, H6 title, Body2 description (max-width 400px), optional action button (Filled/Primary), centered layout, min-height 400px, padding 48px desktop/32px mobile, responsive tested

**TenantDisplay Component Enhanced (TenantDisplay.razor):**
- [ ] **AC4.1-4.12:** Component updated with pill-shaped badge, 20px border-radius, gradient background (primary-400/10% to primary-400/5%), 2px primary-400 border, Store icon (Small, primary-700), tenant name (Body2, 600 weight, primary-700), 8px gap, responsive hide text on mobile (<600px, show icon only), smooth hover effect (0.2s ease)

**UserDisplay Component Enhanced (UserDisplay.razor):**
- [ ] **AC5.1-5.18:** Component updated with avatar (user initials, Primary background, Small size, 600 weight), user info (name Body2 600 weight, email Caption secondary), secondary-100 background, 8px vertical/16px horizontal padding, 24px border-radius (pill), dropdown icon (KeyboardArrowDown), hover effect (secondary-200 bg, shadow-md), dropdown menu ("Mi Perfil"/"Configuración"/"Cerrar Sesión" with icons), logout styled error color with divider, responsive hide user info on mobile (<600px, show avatar+dropdown only)

**CSS & Styling:**
- [ ] **AC6.1-6.8:** CSS classes added to app.css (`.page-header`, `.loading-state`, `.empty-state`, `.empty-state-icon`, `.user-display-activator:hover`), responsive media queries, CSS variables used throughout

**Testing & Validation:**
- [ ] **AC7.1-7.20:** All components render correctly with all parameter combinations, PageHeader breadcrumbs navigate, actions render custom buttons, LoadingState displays spinner, EmptyState triggers OnAction callback, TenantDisplay shows/hides text on mobile, UserDisplay shows correct initials, dropdown works, navigation works, hide info on mobile, all tested at 375px/768px/1024px/1920px, hover effects smooth (0.2s), CSS variables work

---

#### Definition of Done

- [ ] All 5 components created/updated (PageHeader NEW, LoadingState NEW, EmptyState NEW, TenantDisplay MODIFIED, UserDisplay MODIFIED)
- [ ] Code reviewed and approved
- [ ] Component documentation added to implementation guide with usage examples
- [ ] Visual QA performed (spacing, colors, typography, responsive behavior)
- [ ] Mobile-tested on multiple screen sizes (375px, 768px, 1024px, 1920px)
- [ ] No accessibility issues (color contrast, keyboard navigation)
- [ ] No console errors or warnings
- [ ] Components tested in isolation and in actual page context
- [ ] Committed to feature branch: `feature/US-DESIGN-2.1-reusable-components`
- [ ] Pull request created with screenshots of all components

---

#### Technical Notes

**Files Created:**
- `src/Presentation/Corelio.BlazorApp/Components/Shared/PageHeader.razor` (NEW)
- `src/Presentation/Corelio.BlazorApp/Components/Shared/LoadingState.razor` (NEW)
- `src/Presentation/Corelio.BlazorApp/Components/Shared/EmptyState.razor` (NEW)

**Files Modified:**
- `src/Presentation/Corelio.BlazorApp/Components/Shared/TenantDisplay.razor`
- `src/Presentation/Corelio.BlazorApp/Components/Shared/UserDisplay.razor`
- `src/Presentation/Corelio.BlazorApp/wwwroot/app.css` (add component CSS)

**Dependencies:**
- User Story 1.1 (Core Theme Infrastructure) must be completed first

**Breaking Changes:**
- TenantDisplay and UserDisplay maintain same interface (no breaking changes)
- New components are additive

---

---

### User Story 2.2: Multi-Tenant Theming Infrastructure

**Story ID:** US-DESIGN-2.2
**Story Title:** Implement Multi-Tenant Theme Customization

**Priority:** P1 (High - Competitive Differentiation)
**Story Points:** 8
**Effort Estimate:** 10-12 hours
**Sprint:** Sprint 2

---

#### User Story

**As a** tenant administrator,
**I want** to customize my company's primary color and logo,
**So that** the ERP system feels personalized to my business and reinforces my brand.

---

#### Description

Implement multi-tenant theming infrastructure that allows tenants to customize their primary color and upload a logo (logo upload out of scope for MVP, placeholder field only). This includes database schema changes, backend services with Redis caching, frontend dynamic theming, and API endpoints for theme management.

**Context:**
- All tenants currently see identical UI (no personalization)
- Competitive disadvantage - other ERPs allow tenant branding
- Tenant engagement and retention improved through personalization
- Simple branding (primary color only in MVP) keeps complexity manageable

**Impact:**
- Competitive differentiation (unique selling point)
- Tenant engagement and retention improvement
- Professional tenant-specific branding without full white-label complexity

**Related Documents:**
- `docs/recommendations/UI-UX-Design-System-Recommendation.md` (Section: Phase 4)
- `docs/recommendations/UI-UX-Implementation-Guide.md` (Section: Phase 4)

---

#### Acceptance Criteria

(Providing summary format due to length - full AC would include all technical details)

**Database Migration:**
- [ ] **AC1.1-1.6:** Migration "AddTenantBrandingFields" created, TenantConfiguration entity updated with PrimaryColor (string, nullable), LogoUrl (string, nullable), UseCustomTheme (bool, default false), migration applied successfully, columns created in database

**Application Layer Interfaces:**
- [ ] **AC2.1-2.5:** ITenantThemeService interface created in Application/Common/Interfaces, TenantThemeDto record created (TenantId, PrimaryColor, LogoUrl, UseCustomTheme), GetTenantThemeAsync method defined, InvalidateThemeCacheAsync method defined, IsValidHexColor method defined

**Infrastructure Layer Implementation:**
- [ ] **AC3.1-3.12:** TenantThemeService implemented in Infrastructure/Services, GetTenantThemeAsync retrieves from DB, caches in Redis with key "tenant-theme:{tenantId}", cache TTL 2 hours, returns cached value if available, returns null if UseCustomTheme false, InvalidateThemeCacheAsync removes cache, IsValidHexColor validates #RRGGBB regex, service registered in DI

**Blazor App Dynamic Theming:**
- [ ] **AC4.1-4.8:** DynamicThemeService created in BlazorApp/Services, GetThemeForTenantAsync builds MudTheme from TenantThemeDto, clones CorelioDefaultTheme and overrides primary color, DarkenColor/LightenColor methods (15% shade generation), HexToRgb/RgbToHex helpers, service registered in Program.cs

**MainLayout Integration:**
- [ ] **AC5.1-5.7:** MainLayout injects DynamicThemeService and AuthenticationStateProvider, LoadTenantTheme reads tenant_id claim, calls GetThemeForTenantAsync, applies theme via MudThemeProvider, StateHasChanged called, theme loads on OnInitializedAsync

**API Endpoints:**
- [ ] **AC6.1-6.15:** TenantThemeEndpoints.cs created, GET /api/v1/tenants/theme/current implemented, returns current tenant theme or default, requires authorization, PUT /api/v1/tenants/theme implemented, accepts UpdateTenantThemeRequest (PrimaryColor, UseCustomTheme), validates hex color (400 if invalid), updates DB, invalidates cache, requires "tenants.manage" permission, endpoints registered in MapAllEndpoints, tagged "Tenant Theme" in Swagger

**Error Handling:**
- [ ] **AC7.1-7.4:** Invalid hex returns 400 with message, config not found returns 404, DB errors logged and return 500, cache failures degrade gracefully

**Testing & Validation:**
- [ ] **AC8.1-8.20:** Migration applies successfully, table has new columns, TenantThemeService retrieves from DB, caches in Redis (verify with Redis CLI), cache TTL 2 hours, cache hit returns cached value, hex validation works, rejects invalid formats, DynamicThemeService generates correct shades, MainLayout loads theme after login, UI re-renders with new color, GET endpoint returns theme, PUT endpoint updates successfully, validates hex, invalidates cache, changes persist, visible throughout app

**Manual Testing Scenario:**
- [ ] **AC9.1-9.9:** Complete end-to-end scenario: Login as admin → GET current theme → PUT new color (#2E7D32 green) → Verify 200 response → Refresh page → Verify green UI → Check Redis cache → Wait/delete cache → Verify re-load

---

#### Definition of Done

- [ ] All acceptance criteria met
- [ ] Database migration applied successfully (no rollback)
- [ ] Code reviewed and approved
- [ ] Unit tests for TenantThemeService caching logic (cache hit, cache miss, TTL)
- [ ] Unit tests for DynamicThemeService color generation (darken, lighten, hex conversion)
- [ ] Unit tests for hex color validation (valid/invalid formats)
- [ ] Integration test for API endpoints (GET/PUT with valid/invalid data)
- [ ] Manual testing scenario completed (documented in test results)
- [ ] Redis caching verified (hit rate, TTL, invalidation)
- [ ] Performance tested (theme load time < 100ms)
- [ ] Security reviewed (authorization checks, no tenant isolation bypass)
- [ ] API documentation updated (Swagger/Scalar annotations)
- [ ] Implementation guide updated (how tenants use theming)
- [ ] Committed to feature branch: `feature/US-DESIGN-2.2-tenant-theming`
- [ ] Pull request created with screenshots of theme customization demo

---

#### Technical Notes

**Files Created:**
- `src/Core/Corelio.Application/Common/Interfaces/ITenantThemeService.cs` (NEW)
- `src/Core/Corelio.Application/DTOs/TenantThemeDto.cs` (NEW)
- `src/Infrastructure/Corelio.Infrastructure/Services/TenantThemeService.cs` (NEW)
- `src/Presentation/Corelio.BlazorApp/Services/DynamicThemeService.cs` (NEW)
- `src/Presentation/Corelio.WebAPI/Endpoints/TenantThemeEndpoints.cs` (NEW)
- Database migration file: `YYYYMMDDHHMMSS_AddTenantBrandingFields.cs`

**Files Modified:**
- `src/Core/Corelio.Domain/Entities/TenantConfiguration.cs` (add properties)
- `src/Infrastructure/Corelio.Infrastructure/DependencyInjection.cs` (register service)
- `src/Presentation/Corelio.BlazorApp/Program.cs` (register DynamicThemeService)
- `src/Presentation/Corelio.BlazorApp/Components/Layout/MainLayout.razor` (dynamic theme loading)
- `src/Presentation/Corelio.WebAPI/Endpoints/EndpointExtensions.cs` (register theme endpoints)

**Dependencies:**
- User Story 1.1 (Core Theme Infrastructure)
- Redis running in Aspire environment
- ITenantConfigurationRepository (assume exists)
- IDistributedCache (assume configured)

**Breaking Changes:**
- None - Fields are nullable, opt-in feature

**Performance Impact:**
- Theme cache hit: ~5ms (Redis lookup)
- Theme cache miss: ~50ms (DB query + Redis set)
- Cache hit rate expected: >90% after warmup
- No impact on non-themed tenants

**Risks:**
- **Medium Risk:** Database migration - Mitigated by testing on staging first, rollback plan documented
- **Medium Risk:** Redis dependency - Mitigated by graceful degradation (load from DB if cache fails)
- **Low Risk:** Color shade generation edge cases - Mitigated by unit tests with edge cases (#000000, #FFFFFF)

**Out of Scope:**
- Logo upload functionality (future enhancement, 8-12 hours)
- Secondary/accent color customization (future enhancement)
- Dark mode support (future enhancement)
- Theme templates/presets (future enhancement)

**Follow-up Stories:**
- US-DESIGN-3.1: Apply Design System to Existing Pages (can start after 2.1)
- Future: Logo upload functionality (Azure Blob Storage integration)

---

---

## Sprint 3: Consistency & Polish (4-6 hours)

### User Story 3.1: Apply Design System to Existing Pages

**Story ID:** US-DESIGN-3.1
**Story Title:** Apply Design System to ProductList and ProductForm

**Priority:** P2 (Medium - Consistency Across App)
**Story Points:** 3
**Effort Estimate:** 4-6 hours
**Sprint:** Sprint 3

---

#### User Story

**As a** user,
**I want** all pages to have consistent styling and visual quality,
**So that** the application feels polished and professional throughout.

---

#### Description

Apply the design system to existing pages (ProductList, ProductForm) using the new reusable components and styling patterns. Update table styling, form layouts, button consistency, and ensure proper localization and mobile responsiveness.

**Context:**
- ProductList and ProductForm currently exist with old styling
- Inconsistent with new authentication pages and design system
- Need to demonstrate design system benefits by refactoring existing pages
- Establishes pattern for future page updates

**Impact:**
- Consistent professional appearance across all pages
- Demonstrates design system value (before/after comparison)
- Establishes refactoring pattern for remaining pages

**Related Documents:**
- `docs/recommendations/UI-UX-Design-System-Recommendation.md` (Section: Phase 5)
- `docs/recommendations/UI-UX-Implementation-Guide.md` (Section: Phase 5)
- `docs/recommendations/Design-System-Visual-Reference.md` (ProductList mockup)

---

#### Acceptance Criteria

(Summary format due to length)

**ProductList Page:**
- [ ] **AC1.1-1.30:** Use PageHeader component (title "Gestión de Productos", description, breadcrumbs Inicio→Productos, action button "Nuevo Producto" with Add icon), use LoadingState during fetch ("Cargando productos..."), use EmptyState when no products (Inventory icon, "No hay productos" title, description, "Nuevo Producto" action), filters card (MudPaper Elevation="1", 12px radius, 24px padding), search/category/button (Outlined variant, Search icon), table card styling (12px radius, Elevation="1"), table headers (light gray #E9ECEF, 600 weight), SKU in monospace chip, price formatted es-MX ($1,234.56), Estado as MudChip (Success/Default), actions (Edit Primary, Delete Error), pagination centered (MudPagination Primary color)

**ProductForm Page:**
- [ ] **AC2.1-2.20:** Use PageHeader component, section headers ("Información General", "Precios e Inventario" in H6 primary-600 600 weight), two-column grid desktop (MudGrid Spacing="4" >= 960px), stacked mobile (<960px), horizontal divider between sections, all fields Variant.Outlined, numeric fields formatted es-MX, action buttons right-aligned (Cancel Outlined Secondary, Save Filled Primary with Save icon), loading state on save button

**Styling Consistency:**
- [ ] **AC3.1-3.5:** All buttons 8px radius, all cards 12px radius, all spacing uses 8px grid (CSS variables), all colors use design system palette, shadows subtle (elevation 1/2)

**Localization:**
- [ ] **AC4.1-4.4:** All Spanish text via IStringLocalizer, currency formatted es-MX culture, dates formatted dd/MM/yyyy es-MX, no hardcoded strings

**Mobile Responsiveness:**
- [ ] **AC5.1-5.9:** Tested at 375px (iPhone SE), table horizontal scroll, filters wrap vertically, action buttons stack vertically, form fields stack vertically <960px, padding reduces appropriately, touch targets ≥44px

**Testing:**
- [ ] **AC6.1-6.15:** ProductList displays correctly, shows LoadingState during fetch, shows EmptyState when empty, search/filter works, table styled correctly, SKU monospace chip, price formats correctly, Edit/Delete work, pagination works, ProductForm renders correctly, sections have primary headers, two-column grid desktop/stacked mobile, submission works, all tested at 375px/768px/1024px/1920px, no console errors

---

#### Definition of Done

- [ ] All acceptance criteria met
- [ ] Code reviewed and approved
- [ ] Visual QA performed (compare before/after screenshots)
- [ ] Spanish localization complete
- [ ] Mobile-tested at 375px/768px/1920px
- [ ] No accessibility issues
- [ ] Performance tested (page load <2 seconds)
- [ ] Before/after screenshots documented
- [ ] Existing functionality verified (no regressions)
- [ ] Committed to feature branch: `feature/US-DESIGN-3.1-existing-pages`
- [ ] Pull request created with before/after comparison

---

#### Technical Notes

**Files Modified:**
- `src/Presentation/Corelio.BlazorApp/Pages/Products/ProductList.razor`
- `src/Presentation/Corelio.BlazorApp/Pages/Products/ProductForm.razor`

**Dependencies:**
- User Story 1.1 (Core Theme Infrastructure)
- User Story 2.1 (Core Reusable Components - PageHeader, LoadingState, EmptyState)

**Breaking Changes:**
- None - Only visual updates, business logic unchanged

**Testing Strategy:**
- Manual visual QA (primary method)
- Manual functional testing (ensure no regressions)
- Before/after screenshot comparison

---

---

## Sprint Planning Recommendations

### Sprint Allocation

**Sprint 1 (Weeks 1-2): Foundation & First Impression**
- User Story 1.1: Core Theme Infrastructure (6-8 hours)
- User Story 1.2: Authentication Pages Redesign (8-10 hours)
- **Total:** 14-18 hours
- **Demo:** Stakeholders see professional login page and core theme

**Sprint 2 (Weeks 3-4): Component Library & Theming**
- User Story 2.1: Core Reusable Components (6-8 hours)
- User Story 2.2: Multi-Tenant Theming Infrastructure (10-12 hours)
- **Total:** 16-18 hours
- **Demo:** Component library showcase + tenant theme customization

**Sprint 3 (Weeks 5-6): Consistency & Polish**
- User Story 3.1: Apply Design System to Existing Pages (4-6 hours)
- **Total:** 4-6 hours (leaves capacity for bug fixes or other priorities)
- **Demo:** Consistent styling across all pages

### Total Project Estimate
- **Total Hours:** 34-44 hours
- **Total Story Points:** 29 points (5+8+5+8+3)
- **Total Sprints:** 3 sprints (assuming 2-week sprints, ~15-20 hours capacity per sprint)

---

## Success Metrics Tracking

### Qualitative Metrics (Post-Implementation)

**Metric 1: Visual Differentiation**
- **Target:** Stakeholder feedback rating "professional and distinctive" (≥8/10)
- **Measurement:** Survey after Sprint 1 demo (Login page)

**Metric 2: Consistency**
- **Target:** All pages use design system (colors, typography, spacing)
- **Measurement:** Visual QA checklist after Sprint 3

**Metric 3: Mobile-First**
- **Target:** UI works perfectly at 375px width (iPhone SE)
- **Measurement:** Manual testing at Sprint end

**Metric 4: Tenant Branding**
- **Target:** Admin can change primary color and see immediate effect
- **Measurement:** Live demo after Sprint 2

### Quantitative Metrics (Post-Launch)

**Metric 5: Performance**
- **Target:** Page load times remain <2 seconds
- **Measurement:** Lighthouse audits pre/post implementation

**Metric 6: Caching Efficiency**
- **Target:** Theme cache hit rate >90%
- **Measurement:** Redis metrics 1 week post-launch

**Metric 7: Accessibility**
- **Target:** Color contrast ratio >4.5:1 (WCAG AA)
- **Measurement:** Contrast checker tool (WebAIM)

**Metric 8: Development Velocity**
- **Target:** 30% reduction in time to build new pages
- **Measurement:** Track hours for next 3 pages built (compare to historical average)

### User Satisfaction (3 months post-launch)

**Metric 9: Tenant Theming Adoption**
- **Target:** 30% of tenants customize their theme
- **Measurement:** Database query (COUNT WHERE UseCustomTheme = true)

**Metric 10: User Satisfaction**
- **Target:** CSAT/NPS scores improve by 10+ points
- **Measurement:** User survey pre/post implementation

---

## Risk Assessment & Mitigation

### Technical Risks

| Risk | Likelihood | Impact | Mitigation Strategy |
|------|------------|--------|---------------------|
| **Font Loading Failure** (Google Fonts CDN down) | Low | Low | Use font-display: swap, fallback to system fonts (-apple-system, Segoe UI, etc.) |
| **Migration Fails in Production** | Low | Medium | Test migration on staging environment first, document rollback plan, schedule during low-traffic window |
| **Performance Degradation** | Low | Medium | Monitor page load times with Lighthouse, optimize if >2s, cache theme aggressively (2hr TTL) |
| **Tenant Chooses Low-Contrast Color** | Medium | Low | Future enhancement: validate contrast ratio in PUT endpoint, reject colors <4.5:1 ratio |
| **Redis Cache Unavailable** | Low | Low | Graceful degradation: load theme from database, log warning, continue operation |
| **CSS Variable Browser Support** | Very Low | Low | Target modern browsers only (Chrome 90+, Firefox 88+, Safari 14+, Edge 90+), document requirement |

### Business Risks

| Risk | Likelihood | Impact | Mitigation Strategy |
|------|------------|--------|---------------------|
| **Stakeholders Dislike Color Choice** | Low | Medium | Involve stakeholders early, show mockups in Sprint 0, iterate on feedback before implementation |
| **Scope Creep** (requests for additional features) | Medium | Medium | Document out-of-scope items clearly (logo upload, dark mode, theme templates), create future backlog items |
| **Timeline Slippage** | Low | Medium | Buffer included in estimates (6-8 hrs = expect 7), prioritize P0 stories first, P2 can slip to Sprint 4 if needed |

### Risk Heatmap

```
Impact
High    │
        │
Medium  │  ⚠️Perf  ⚠️Migration
        │
Low     │  ✅Font  ✅Redis  ✅Browser
        └─────────────────────────
         Low    Medium    High
              Likelihood
```

**Overall Risk Level:** 🟢 **LOW** - Well-defined scope, proven technologies, phased implementation with early validation

---

## Backlog Prioritization Guidance

### MoSCoW Prioritization

**Must Have (Sprint 1):**
- ✅ User Story 1.1: Core Theme Infrastructure
- ✅ User Story 1.2: Authentication Pages Redesign
- **Why:** Foundation for all other work + critical first impression

**Should Have (Sprint 2):**
- ✅ User Story 2.1: Core Reusable Components
- ✅ User Story 2.2: Multi-Tenant Theming Infrastructure
- **Why:** Enable faster development + competitive differentiation

**Could Have (Sprint 3):**
- ✅ User Story 3.1: Apply Design System to Existing Pages
- **Why:** Improves consistency but can be deferred if higher-priority features emerge

**Won't Have (Out of Scope):**
- ❌ Logo upload functionality
- ❌ Advanced theming (secondary/accent colors)
- ❌ Dark mode support
- ❌ Theme templates
- ❌ Dashboard page with charts

### WSJF (Weighted Shortest Job First) Scoring

| User Story | Business Value | Time Criticality | Risk Reduction | Job Size | WSJF Score |
|------------|----------------|------------------|----------------|----------|------------|
| US-DESIGN-1.1 | 10 | 10 | 10 | 5 | (10+10+10)/5 = **6.0** |
| US-DESIGN-1.2 | 10 | 10 | 8 | 8 | (10+10+8)/8 = **3.5** |
| US-DESIGN-2.1 | 8 | 6 | 6 | 5 | (8+6+6)/5 = **4.0** |
| US-DESIGN-2.2 | 9 | 6 | 5 | 8 | (9+6+5)/8 = **2.5** |
| US-DESIGN-3.1 | 5 | 3 | 2 | 3 | (5+3+2)/3 = **3.3** |

**Recommended Order (by WSJF):**
1. US-DESIGN-1.1 (Score 6.0)
2. US-DESIGN-2.1 (Score 4.0)
3. US-DESIGN-1.2 (Score 3.5)
4. US-DESIGN-3.1 (Score 3.3)
5. US-DESIGN-2.2 (Score 2.5)

**Note:** While WSJF suggests this order, dependencies force us to do 1.1 first, then 1.2 (or 2.1 in parallel), then 2.2 and 3.1.

---

## Dependencies & Sequencing

### Dependency Graph

```
US-DESIGN-1.1 (Core Theme)
    │
    ├──> US-DESIGN-1.2 (Auth Pages)
    │
    ├──> US-DESIGN-2.1 (Components)
    │        │
    │        └──> US-DESIGN-3.1 (Existing Pages)
    │
    └──> US-DESIGN-2.2 (Tenant Theming)
```

### Sequencing Rules

**Sequential (Must Complete in Order):**
- US-DESIGN-1.1 MUST be completed before US-DESIGN-1.2, US-DESIGN-2.1, US-DESIGN-2.2

**Parallel (Can Work Simultaneously After 1.1):**
- US-DESIGN-1.2 and US-DESIGN-2.1 can be done in parallel (no dependency)
- US-DESIGN-2.1 and US-DESIGN-2.2 can be done in parallel (no dependency)

**Sequential (Within Sprint 2):**
- US-DESIGN-2.1 MUST be completed before US-DESIGN-3.1 (depends on PageHeader, LoadingState, EmptyState)

---

## Change Log

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-01-27 | Development Team | Initial backlog creation - 5 user stories across 3 sprints |

---

## Appendix: Quick Reference

### Story Point Summary
- 5 points: 6-8 hours (US-DESIGN-1.1, US-DESIGN-2.1)
- 8 points: 8-12 hours (US-DESIGN-1.2, US-DESIGN-2.2)
- 3 points: 4-6 hours (US-DESIGN-3.1)

### Priority Levels
- **P0 (Critical):** Must have for MVP - US-DESIGN-1.1, US-DESIGN-1.2
- **P1 (High):** Should have for competitive advantage - US-DESIGN-2.1, US-DESIGN-2.2
- **P2 (Medium):** Nice to have for consistency - US-DESIGN-3.1

### Key Contacts
- **Epic Owner:** Product Owner
- **Technical Lead:** Development Team Lead
- **Design Reviewer:** UX/UI Stakeholder (if applicable)

### Related Documentation
1. **Business Case:** `docs/recommendations/UI-UX-Design-System-Recommendation.md`
2. **Implementation Guide:** `docs/recommendations/UI-UX-Implementation-Guide.md`
3. **Visual Reference:** `docs/recommendations/Design-System-Visual-Reference.md`
4. **User Story Templates:** `docs/recommendations/UI-UX-User-Stories-Template.md`
5. **Executive Summary:** `docs/recommendations/EXECUTIVE-SUMMARY.md`
6. **Main Index:** `docs/recommendations/README.md`

---

**Document Status:** ✅ Ready for Backlog
**Last Updated:** 2026-01-27
**Prepared By:** Development Team (Product Owner Mode)
**For:** Product Owner / Backlog Management
