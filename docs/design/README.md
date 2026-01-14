# Corelio ERP - Design Documentation

## Overview

This folder contains all UI/UX design documentation for the Corelio ERP system - a professional, multi-tenant SaaS application for Mexican hardware stores (ferreterías).

**Design Philosophy:** "Confianza Industrial" (Industrial Trust)
- Professional & trustworthy
- Efficient & task-oriented
- Distinctive (NOT generic AI-generated UI)
- Modern industrial aesthetic with warm, approachable colors

---

## Document Index

### 🎨 Core Design System
**[UI-UX-DESIGN-SYSTEM.md](./UI-UX-DESIGN-SYSTEM.md)** - Main design system specification
- Complete color palette (Terracotta, Concrete Gray, Tool Steel)
- Typography system (Inter font family)
- Spacing & sizing scales
- Component patterns (buttons, forms, cards, tables)
- Layout patterns
- Authentication page designs
- Multi-tenant component designs
- MudBlazor theme configuration
- Responsive design guidelines
- Accessibility checklist

**Status:** ✅ Complete | **Required Reading:** Yes | **Audience:** All Frontend Developers

---

### 🏢 Tenant Customization
**[TENANT-THEME-CUSTOMIZATION.md](./TENANT-THEME-CUSTOMIZATION.md)** - Multi-tenant theming system
- Database schema for tenant themes
- `ITenantThemeService` interface and implementation
- Dynamic theme loading in Blazor
- Color palette generation algorithm
- Admin UI for theme customization
- Pre-defined theme templates
- API endpoints for theme management

**Status:** ✅ Complete | **Required Reading:** Before Sprint 4+ | **Audience:** Backend + Frontend Developers

---

### 🚀 Quick Start Guide
**[IMPLEMENTATION-QUICK-START.md](./IMPLEMENTATION-QUICK-START.md)** - Get started in 30 minutes
- Step-by-step setup instructions
- MudBlazor configuration
- Theme setup
- CSS variables
- Copy-paste component examples
- Login page template
- Product list page template
- Common patterns (loading, empty states, notifications)

**Status:** ✅ Complete | **Required Reading:** Yes (Start here!) | **Audience:** Frontend Developers implementing US-2.2.1

---

## Quick Navigation

### For Immediate Implementation (Sprint 2 Debt)
1. **Start here:** [IMPLEMENTATION-QUICK-START.md](./IMPLEMENTATION-QUICK-START.md)
2. **Reference:** [UI-UX-DESIGN-SYSTEM.md](./UI-UX-DESIGN-SYSTEM.md) (Component Patterns section)
3. **User Stories:**
   - US-2.2.1: Authentication Frontend (13 SP) - Use Login page template
   - US-2.1.1: Multi-Tenancy Frontend (5 SP) - Use TenantDisplay/UserDisplay components

### For Future Sprints (Sprint 4+)
1. **Tenant Theming:** [TENANT-THEME-CUSTOMIZATION.md](./TENANT-THEME-CUSTOMIZATION.md)
2. **Component Library:** Refer to Component Patterns in [UI-UX-DESIGN-SYSTEM.md](./UI-UX-DESIGN-SYSTEM.md)

---

## Color Palette (Quick Reference)

```css
/* Primary - Terracotta (Hardware Store) */
--color-primary-500: #e65946;

/* Secondary - Concrete Gray */
--color-secondary-500: #adb5bd;

/* Accent - Tool Steel */
--color-accent-500: #2a8a8f;

/* Semantic Colors */
--color-success: #2e7d32;  /* Wood Green */
--color-warning: #ed6c02;  /* Safety Orange */
--color-error: #d32f2f;    /* Alert Red */
--color-info: #0288d1;     /* Sky Blue */
```

**Why Terracotta?**
Terracotta (baked clay) connects to construction materials like bricks and tiles, common in Mexican hardware stores. It's warm, trustworthy, and professional without being cold or corporate.

---

## Typography (Quick Reference)

```css
/* Font Family */
font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', sans-serif;

/* Type Scale */
--text-h1: 2rem;      /* Page titles */
--text-h2: 1.5rem;    /* Section titles */
--text-body: 1rem;    /* Regular text (16px) */
--text-caption: 0.75rem; /* Small text */
```

**Why Inter?**
Inter is a highly legible system font optimized for UI. It's modern, professional, and performs well at small sizes (critical for data-dense tables).

---

## Spacing System (Quick Reference)

```css
/* 8px Base Unit */
--space-2: 0.5rem;   /* 8px  - Tight gaps */
--space-4: 1rem;     /* 16px - Default spacing */
--space-6: 1.5rem;   /* 24px - Large gaps */
--space-8: 2rem;     /* 32px - Section spacing */
```

---

## Component Patterns (Quick Reference)

### Primary Button
```razor
<MudButton Variant="Variant.Filled"
           Color="Color.Primary"
           Size="Size.Large"
           StartIcon="@Icons.Material.Filled.Save"
           Style="font-weight: 600; text-transform: none;">
    Guardar Cambios
</MudButton>
```

### Form Field
```razor
<MudTextField Label="Nombre del Producto"
              @bind-Value="model.Name"
              Variant="Variant.Outlined"
              Margin="Margin.Dense"
              Required="true"
              RequiredError="Este campo es requerido" />
```

### Card with Header
```razor
<MudCard Elevation="2" Style="border-radius: 12px;">
    <MudCardHeader Style="background: var(--color-primary-50);">
        <CardHeaderContent>
            <MudText Typo="Typo.h5" Style="font-weight: 600;">
                Título de la Tarjeta
            </MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent Style="padding: 24px;">
        <!-- Content -->
    </MudCardContent>
</MudCard>
```

---

## Implementation Checklist

### Sprint 2 Frontend Debt (US-2.2.1, US-2.1.1)

#### Setup (Day 1 - 2 hours)
- [ ] Install packages: MudBlazor, Blazored.LocalStorage, Localization
- [ ] Configure MudBlazor theme in `Program.cs`
- [ ] Create `Services/ThemeConfiguration.cs`
- [ ] Update `App.razor` with MudBlazor CSS/JS
- [ ] Create `wwwroot/css/custom.css` with CSS variables
- [ ] Load Inter font from Google Fonts
- [ ] Update `MainLayout.razor` with new header design

#### Authentication Pages (Day 1-2 - 8-10 hours)
- [ ] Create `AuthLayout.razor` (minimal layout)
- [ ] Implement `Login.razor` using design system
- [ ] Implement `Register.razor` using design system
- [ ] Implement `ForgotPassword.razor`
- [ ] Implement `ResetPassword.razor`
- [ ] Implement `Logout.razor`
- [ ] Create Spanish localization files (47 keys)

#### Multi-Tenant Components (Day 3 - 4-5 hours)
- [ ] Create `TenantDisplay.razor` component
- [ ] Create `UserDisplay.razor` component
- [ ] Integrate components into `MainLayout.razor`
- [ ] Create backend endpoint: `GET /api/v1/tenants/{id}/name`
- [ ] Test tenant switching (login as different tenants)

#### Testing & Verification (Day 3-4 - 3-4 hours)
- [ ] Test all 14 acceptance criteria for US-2.2.1
- [ ] Test all 10 acceptance criteria for US-2.1.1
- [ ] Verify Spanish localization (all UI text)
- [ ] Test responsive design (mobile, tablet, desktop)
- [ ] Test color contrast ratios (accessibility)
- [ ] Verify localStorage token storage
- [ ] Test token auto-refresh mechanism

**Total Effort:** 17-21 hours (3-4 days for 1 developer)

---

## Design Principles

### 1. Professional, Not Generic
❌ **Avoid:**
- Default MudBlazor blue theme
- Generic corporate aesthetics
- Excessive rounded corners
- Gradient backgrounds everywhere

✅ **Use:**
- Terracotta brand color
- Subtle shadows and elevation
- Purposeful whitespace
- Industrial-inspired design elements

### 2. Spanish-First, Not Translated
All UI text must be in Spanish (es-MX) using `IStringLocalizer`:
```razor
@inject IStringLocalizer<SharedResource> L

<MudTextField Label="@L["ProductName"]" />
```

Never hardcode Spanish strings:
```razor
<!-- ❌ BAD -->
<MudButton>Guardar</MudButton>

<!-- ✅ GOOD -->
<MudButton>@L["Save"]</MudButton>
```

### 3. Task-Oriented, Not Feature-Rich
Hardware store staff need to complete tasks quickly. Avoid:
- Multi-step wizards when a single form suffices
- Excessive confirmation dialogs
- Hidden features in dropdown menus
- Complex navigation hierarchies

### 4. Accessible by Default
- 4.5:1 text contrast ratio (WCAG AA)
- 3:1 UI component contrast ratio
- Keyboard navigation support
- ARIA labels on interactive elements
- Focus indicators visible

---

## Tools & Resources

### Design Tools
- **Figma:** For mockups (future)
- **Coolors:** Color palette generator (https://coolors.co/)
- **Google Fonts:** Inter font family (https://fonts.google.com/specimen/Inter)
- **Material Icons:** Icon library (https://fonts.google.com/icons)

### Development Tools
- **MudBlazor Docs:** https://mudblazor.com/
- **Blazor Docs:** https://learn.microsoft.com/en-us/aspnet/core/blazor/
- **WebAIM Contrast Checker:** https://webaim.org/resources/contrastchecker/

### Testing Tools
- **Chrome DevTools:** Lighthouse accessibility audit
- **WAVE:** Web accessibility evaluation tool (https://wave.webaim.org/)
- **Responsive Design Mode:** Browser DevTools (F12)

---

## FAQ

**Q: Why not use the default MudBlazor theme?**
A: Default themes create generic, "AI-generated" UIs. Custom theming establishes a distinctive brand identity that resonates with Mexican hardware store owners.

**Q: Why terracotta as the primary color?**
A: Terracotta connects to construction materials (bricks, clay tiles) familiar to hardware stores. It's warm, trustworthy, and professional without being cold corporate blue.

**Q: Should we implement dark mode?**
A: Not in MVP. Hardware stores typically operate in well-lit environments. Add in Sprint 4+ based on user feedback.

**Q: Can tenants customize the theme colors?**
A: Yes! See [TENANT-THEME-CUSTOMIZATION.md](./TENANT-THEME-CUSTOMIZATION.md). This is a premium feature for Sprint 4+.

**Q: Do we need a design system library (Storybook)?**
A: Not yet. The documentation in this folder is sufficient for MVP. Consider Storybook in Sprint 6+ as the component library grows.

**Q: How do we handle very long tenant names?**
A: Use `text-overflow: ellipsis` with a `max-width`, and show full name in a tooltip:
```razor
<MudTooltip Text="@fullTenantName">
    <MudText Style="max-width: 200px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
        @tenantName
    </MudText>
</MudTooltip>
```

**Q: What about animations?**
A: Use subtle, purposeful animations (200-300ms ease). Avoid flashy, distracting effects:
- Button hover: 200ms ease
- Card elevation: 200ms ease
- Page transitions: 300ms ease
- Loading spinners: Smooth, indeterminate

---

## Feedback & Contributions

### Design Improvements
If you identify design inconsistencies or have suggestions for improvements:
1. Create an issue with the `design` label
2. Include screenshots/mockups if possible
3. Explain the problem and proposed solution
4. Tag the design review team

### Component Additions
When adding new reusable components:
1. Follow the design system patterns in [UI-UX-DESIGN-SYSTEM.md](./UI-UX-DESIGN-SYSTEM.md)
2. Document the component with XML comments
3. Add usage examples to `IMPLEMENTATION-QUICK-START.md`
4. Test on mobile, tablet, and desktop
5. Verify accessibility (ARIA labels, keyboard nav)

### Localization
When adding new UI strings:
1. Never hardcode Spanish text
2. Add key to `Resources/SharedResource.es-MX.resx`
3. Use descriptive keys (e.g., `ProductName`, not `Label1`)
4. Document the context in comments

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 2026-01-13 | Initial design system documentation | Design Team |
| - | - | Added tenant theme customization guide | Design Team |
| - | - | Created implementation quick-start guide | Design Team |

---

## Contact

**Design Team:** design@corelio.com
**Frontend Lead:** frontend@corelio.com
**Documentation Issues:** Create issue with `documentation` label

---

**Last Updated:** 2026-01-13
**Status:** ✅ Ready for Implementation
**Next Review:** 2026-01-20 (after US-2.2.1 and US-2.1.1 completion)
