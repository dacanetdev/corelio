# UI/UX Design System - Visual Reference Guide

## Document Purpose

This guide provides visual descriptions and ASCII mockups to help stakeholders visualize the proposed design system before implementation. Use this to understand what the final product will look like.

**Related Documents:**
- `UI-UX-Design-System-Recommendation.md` (full proposal)
- `README.md` (navigation guide)

**Target Audience:** Product Owner, Stakeholders, UX/UI Reviewers

**Last Updated:** 2026-01-27

---

## Design Philosophy: "Confianza Industrial" (Industrial Trust)

**Mood:** Professional, confident, warm, reliable
**Inspiration:** Mexican hardware stores - terracotta tools, concrete materials, steel equipment
**Target Users:** Mexican SME owners (hardware store managers/owners)

---

## Color Palette

### Primary: Terracotta Red
**Purpose:** Buttons, links, headers, highlights

```
┌─────────────────────────────────────────────────┐
│  Lightest                          Darkest      │
│  #FADBD8 → #F5B7B1 → #EC7063 → #E74C3C → #641E16 │
│  ░░░░░░░   ▒▒▒▒▒▒▒   ▓▓▓▓▓▓▓   ██████   ██████  │
│  50         100       300       400      900     │
│  Background Hover     Active    Base     Text    │
└─────────────────────────────────────────────────┘
```

**#E74C3C** - Base primary color (used for buttons, icons, borders)

### Secondary: Concrete Gray
**Purpose:** Text, borders, backgrounds, neutrals

```
┌─────────────────────────────────────────────────┐
│  Lightest                          Darkest      │
│  #F8F9FA → #E9ECEF → #6C757D → #495057 → #121416 │
│  ░░░░░░░   ▒▒▒▒▒▒▒   ▓▓▓▓▓▓▓   ██████   ██████  │
│  50         100       500       600      900     │
│  BG Light   Table     Secondary Primary  Text    │
│             Header    Text      Text              │
└─────────────────────────────────────────────────┘
```

**#6C757D** - Base secondary color (used for secondary text, borders)

### Semantic Colors

```
Success:  #28A745 ████ (Green - completed actions)
Warning:  #FFC107 ████ (Yellow - stock alerts)
Error:    #DC3545 ████ (Red - errors, destructive actions)
Info:     #17A2B8 ████ (Blue - informational messages)
```

---

## Typography

**Font Family:** Inter (Google Fonts)

### Type Scale Comparison

```
H1  ████████████████████ 40px / 2.5rem (700 weight)
    "Bienvenido a Corelio ERP"

H2  ██████████████████ 32px / 2rem (700 weight)
    "Panel de Control"

H3  ████████████████ 28px / 1.75rem (600 weight)
    "Productos Destacados"

H4  ██████████████ 24px / 1.5rem (600 weight)
    "Gestión de Productos"

H5  ████████████ 20px / 1.25rem (600 weight)
    "Información General"

H6  ██████████ 18px / 1.125rem (600 weight)
    "Precios e Inventario"

Body1  ████████ 16px / 1rem (400 weight)
       "Este es el texto principal del cuerpo. Legible y profesional."

Body2  ██████ 14px / 0.875rem (400 weight)
       "Texto secundario, descripciones, ayudas."

Button  ████ 14px / 0.875rem (600 weight, NO UPPERCASE)
        "Guardar Cambios"

Caption  ██ 12px / 0.75rem (400 weight)
         "Helper text, timestamps"
```

### Typography Examples

```
Before (Generic MudBlazor):
┌──────────────────────────┐
│ SAVE CHANGES             │ ← Uppercase (too aggressive)
│ (Roboto, 14px, 500)      │
└──────────────────────────┘

After (Corelio Design System):
┌──────────────────────────┐
│ Guardar Cambios          │ ← Sentence case (professional)
│ (Inter, 14px, 600)       │
└──────────────────────────┘
```

---

## Spacing System (8px Grid)

```
0px   ●
4px   ●─
8px   ●───
12px  ●─────
16px  ●───────
24px  ●───────────
32px  ●───────────────
48px  ●───────────────────────
64px  ●───────────────────────────────

Common Usage:
- 8px:  Component internal spacing
- 16px: Standard padding (buttons, inputs)
- 24px: Page padding, section spacing
- 32px: Card padding (mobile)
- 48px: Card padding (desktop), large section gaps
```

---

## Login Page Mockup (Desktop)

```
┌────────────────────────────────────────────────────────────┐
│                                                            │
│                   ┌─────────────────┐                     │
│                   │   ╭─────────╮   │ ← Circular gradient │
│                   │   │  🏪     │   │   background        │
│                   │   │ (Store) │   │   96px diameter     │
│                   │   ╰─────────╯   │                     │
│                   │                 │                     │
│                   │  Corelio ERP    │ ← H3, 28px, bold    │
│                   │                 │   Terracotta color  │
│                   │ Sistema de Gestión para Ferreterías   │
│                   │                 │ ← Body1, gray       │
│                   │                 │                     │
│                   │ ┌─────────────────────────────┐       │
│                   │ │ 📧 Email                   │       │
│                   │ │ ejemplo@ferreteria.mx      │       │
│                   │ └─────────────────────────────┘       │
│                   │                 │                     │
│                   │ ┌─────────────────────────────┐       │
│                   │ │ 🔒 Contraseña         👁️   │       │
│                   │ │ ••••••••                   │       │
│                   │ └─────────────────────────────┘       │
│                   │                 │                     │
│                   │ ┌─────────────────────────────┐       │
│                   │ │      Iniciar Sesión        │ ← 56px height
│                   │ │     (Terracotta color)      │   Full width
│                   │ └─────────────────────────────┘       │
│                   │                 │                     │
│                   │  ¿Olvidaste tu contraseña?  │ ← Link │
│                   │                 │                     │
│                   │ ────────────────────────────          │
│                   │                 │                     │
│                   │ ¿No tienes una cuenta?      │         │
│                   │ Contacta a tu administrador │         │
│                   └─────────────────┘                     │
│                                                            │
│               © 2026 Corelio ERP. Todos los derechos...   │
└────────────────────────────────────────────────────────────┘

Key Features:
- White card with 16px border-radius (softer corners)
- Elevation 8 (prominent shadow)
- 48px padding on desktop
- Generous whitespace (not cramped)
- Large logo icon (64px) in gradient circle
- Email/password fields with icons
- Large submit button (56px tall)
- Subtle background gradient
```

---

## Login Page Mockup (Mobile - 375px)

```
┌──────────────────────┐
│                      │
│   ┌────────────┐     │
│   │ ╭────────╮ │     │ ← Smaller logo
│   │ │  🏪    │ │     │   64px circle
│   │ ╰────────╯ │     │
│   │            │     │
│   │ Corelio    │     │
│   │ ERP        │     │ ← H4 (smaller)
│   │            │     │
│   │ Sistema de │     │
│   │ Gestión... │     │
│   │            │     │
│   │ ┌────────────┐   │
│   │ │ 📧 Email  │   │
│   │ └────────────┘   │
│   │            │     │
│   │ ┌────────────┐   │
│   │ │ 🔒 Pass 👁️│   │
│   │ └────────────┘   │
│   │            │     │
│   │ ┌────────────┐   │
│   │ │ Iniciar    │   │
│   │ │ Sesión     │   │ ← Still 56px
│   │ └────────────┘   │   tall
│   │            │     │
│   │ ¿Olvidaste?│     │
│   │            │     │
│   └────────────┘     │
│                      │
│ © 2026 Corelio ERP   │
└──────────────────────┘

Key Features:
- Full-width card (no side margins)
- 32px padding (reduced from 48px desktop)
- Logo reduces to 64px
- Font size base: 14px (from 16px)
- All elements stack vertically
- Touch targets: 44px minimum
```

---

## PageHeader Component Mockup

```
┌────────────────────────────────────────────────────────────┐
│ Inicio > Productos                                         │ ← Breadcrumbs
│                                                            │
│ ┌──────────────────────────────────┬─────────────────────┐│
│ │ Gestión de Productos             │ [+ Nuevo Producto] ││ ← H4 title
│ │                                  │                     ││   + action
│ │ Administre el catálogo de        │                     ││
│ │ productos de su ferretería       │                     ││ ← Body2 desc
│ └──────────────────────────────────┴─────────────────────┘│
│────────────────────────────────────────────────────────────│ ← Border
└────────────────────────────────────────────────────────────┘

Desktop Layout:
- Title and actions side-by-side
- Description max-width: 600px

Mobile Layout:
- Title on top
- Actions below (wrapped)
- Full-width description
```

---

## Product List Page Mockup

```
┌────────────────────────────────────────────────────────────┐
│ Inicio > Productos                                         │
│                                                            │
│ Gestión de Productos              [+ Nuevo Producto]      │
│ Administre el catálogo de productos...                    │
│────────────────────────────────────────────────────────────│
│                                                            │
│ ┌──────────────────────────────────────────────────────┐   │
│ │ 🔍 Buscar productos...  ▼ Categoría  [🔍 Buscar]   │   │ ← Filters
│ └──────────────────────────────────────────────────────┘   │
│                                                            │
│ ┌──────────────────────────────────────────────────────┐   │
│ │ Imagen SKU    Nombre      Categoría  Precio  Estado  │   │ ← Table
│ │────────────────────────────────────────────────────  │   │   header
│ │ [img]  ABC123 Martillo    Herram.   $250.00  ✓Activo│   │   (gray bg)
│ │ [img]  DEF456 Tornillos   Fijación  $45.50   ✓Activo│   │
│ │ [img]  GHI789 Pintura     Pinturas  $380.00  ✓Activo│   │
│ └──────────────────────────────────────────────────────┘   │
│                                                            │
│                   ◀ 1 2 3 4 5 ▶                            │ ← Pagination
└────────────────────────────────────────────────────────────┘

Key Features:
- Light gray table headers (#E9ECEF)
- SKU in monospace chip
- Price formatted: $250.00 (es-MX)
- Estado as colored chip (green for active)
- 12px border-radius on cards
- Subtle shadows (elevation 1)
```

---

## Empty State Mockup

```
┌────────────────────────────────────────────────────────────┐
│                                                            │
│                     ┌────────────┐                         │
│                     │  ╭──────╮  │ ← 96px circle          │
│                     │  │ 📦   │  │   gray background      │
│                     │  ╰──────╯  │                         │
│                     └────────────┘                         │
│                                                            │
│                   No hay productos                         │ ← H6, bold
│                                                            │
│         No hay productos en el catálogo.                   │
│       Comience agregando su primer producto.               │ ← Body2
│                                                            │
│                  ┌─────────────────┐                       │
│                  │ + Nuevo Producto│                       │ ← Button
│                  └─────────────────┘                       │
│                                                            │
└────────────────────────────────────────────────────────────┘

Key Features:
- Centered layout
- Large icon (4rem) in circular background
- Clear title and description
- Optional action button
- Generous spacing (48px padding)
- Min-height: 400px
```

---

## Loading State Mockup

```
┌────────────────────────────────────────────────────────────┐
│                                                            │
│                                                            │
│                        ⟳                                   │ ← 64px spinner
│                     ╱     ╲                                │   terracotta
│                    │   ⟳   │                               │   color
│                     ╲     ╱                                │
│                        ⟳                                   │
│                                                            │
│                 Cargando productos...                      │ ← Body1, gray
│                                                            │
│                                                            │
└────────────────────────────────────────────────────────────┘

Key Features:
- Centered layout
- 64px primary-colored spinner
- Message below spinner
- Min-height: 320px
- Simple, professional
```

---

## Multi-Tenant Theming Examples

### Default Corelio Theme (Terracotta Red)

```
┌────────────────────────────────────┐
│ 🏪 Corelio ERP          🏢 Ferr... │ ← AppBar (white bg)
│────────────────────────────────────│   Red accent
│ ┌────────────────────────────────┐ │
│ │ [Guardar Cambios]              │ │ ← Terracotta button
│ │  ████████ #E74C3C              │ │
│ └────────────────────────────────┘ │
│                                    │
│ 🏪 Ferretería Central              │ ← Tenant badge
│ ███ (Terracotta border)            │
└────────────────────────────────────┘
```

### Custom Tenant Theme (Green)

```
┌────────────────────────────────────┐
│ 🏪 Corelio ERP          🏢 Ferr... │ ← AppBar (white bg)
│────────────────────────────────────│   Green accent
│ ┌────────────────────────────────┐ │
│ │ [Guardar Cambios]              │ │ ← Green button
│ │  ████████ #2E7D32              │ │   (custom)
│ └────────────────────────────────┘ │
│                                    │
│ 🏪 Ferretería Central              │ ← Tenant badge
│ ███ (Green border)                 │   (custom)
└────────────────────────────────────┘
```

**Customizable Elements:**
- Primary button color
- Link color
- Border accents
- Tenant badge color
- Icon highlights

**Fixed Elements (Not customizable):**
- Typography (Inter font)
- Spacing (8px grid)
- Secondary colors (grays)
- Layout structure

---

## Component Comparison: Before vs. After

### Buttons

```
BEFORE (Generic MudBlazor):
┌─────────────────┐
│  SAVE CHANGES   │ ← Uppercase (aggressive)
│  #1976D2 (Blue) │   Material Design blue
│  4px radius     │   Sharp corners
└─────────────────┘

AFTER (Corelio):
┌─────────────────┐
│ Guardar Cambios │ ← Sentence case (professional)
│ #E74C3C (Red)   │   Terracotta primary
│ 8px radius      │   Softer corners
└─────────────────┘
```

### Cards

```
BEFORE:
┌─────────────────┐
│ Content...      │ ← 4px radius, harsh shadow
│                 │
└─────────────────┘
  Shadow: 0 2px 4px rgba(0,0,0,0.2) (harsh)

AFTER:
╭─────────────────╮
│ Content...      │ ← 12px radius, subtle shadow
│                 │
╰─────────────────╯
  Shadow: 0 4px 6px rgba(0,0,0,0.1) (subtle)
```

### Table Headers

```
BEFORE:
┌────────────────────────────────┐
│ Name    | Price  | Status      │ ← White background
│─────────────────────────────────│   Same as rows
│ Product | $50.00 | Active      │
└────────────────────────────────┘

AFTER:
┌────────────────────────────────┐
│ Nombre  | Precio | Estado      │ ← Light gray (#E9ECEF)
│─────────────────────────────────│   Visual separation
│ Product | $50.00 | Activo      │
└────────────────────────────────┘
```

---

## Mobile Responsiveness

### Breakpoints

```
Mobile:    < 600px  (375px - 599px) iPhone SE, small phones
Tablet:    600-959px (600px - 959px) iPad, Android tablets
Desktop:   960-1279px                Laptop screens
Large:     1280-1919px               Desktop monitors
XLarge:    >= 1920px                 Large displays
```

### Adaptive Patterns

**TenantDisplay:**
```
Desktop (> 600px):          Mobile (< 600px):
┌────────────────┐          ┌─────┐
│ 🏪 Ferretería  │          │ 🏪  │ ← Icon only
└────────────────┘          └─────┘
```

**UserDisplay:**
```
Desktop (> 600px):          Mobile (< 600px):
┌──────────────────────┐    ┌────────┐
│ 👤 Juan Pérez        │    │ 👤  ▼  │ ← Avatar + dropdown
│    juan@ferr...  ▼   │    └────────┘
└──────────────────────┘
```

**ProductList Filters:**
```
Desktop:
┌──────────────────────────────────────────┐
│ 🔍 Search...  ▼ Category  [🔍 Buscar]   │ ← Horizontal
└──────────────────────────────────────────┘

Mobile:
┌──────────────┐
│ 🔍 Search... │
├──────────────┤ ← Stacked vertically
│ ▼ Category   │
├──────────────┤
│ [🔍 Buscar]  │
└──────────────┘
```

---

## Accessibility Features

### Color Contrast Ratios (WCAG AA)

```
✅ PASS: #E74C3C on #FFFFFF → 4.8:1 (AA)
✅ PASS: #212529 on #FFFFFF → 16.1:1 (AAA)
✅ PASS: #6C757D on #FFFFFF → 4.7:1 (AA)
✅ PASS: #FFFFFF on #E74C3C → 4.8:1 (AA)

❌ FAIL: #EC7063 on #FFFFFF → 3.2:1 (use for decorative only)
```

### Focus States

```
Input Focus:
┌──────────────────────────┐
│ Email: _________________ │
└──────────────────────────┘
      ↓ (user clicks)
┌──────────────────────────┐
│ Email: _________________ │ ← Terracotta border
└──────────────────────────┘   + subtle glow
  Box shadow: 0 0 0 3px rgba(231,76,60,0.1)
```

### Keyboard Navigation

```
Tab Order:
1. [Email field]
2. [Password field]
3. [Show password icon]
4. [Login button]
5. [Forgot password link]

Enter key submits form from any field
Escape key closes modals/dropdowns
```

---

## Design System Checklist

Use this checklist to verify design system compliance:

### Colors ✅
- [ ] Primary color is #E74C3C (Terracotta Red)
- [ ] Secondary color is #6C757D (Concrete Gray)
- [ ] Success: #28A745, Warning: #FFC107, Error: #DC3545, Info: #17A2B8
- [ ] No Material Design blue (#1976D2) anywhere
- [ ] Color contrast ratios > 4.5:1 (WCAG AA)

### Typography ✅
- [ ] Inter font family loads from Google Fonts
- [ ] H1=40px, H2=32px, H3=28px, H4=24px, H5=20px, H6=18px
- [ ] Body1=16px, Body2=14px, Button=14px, Caption=12px
- [ ] No uppercase button text (text-transform: none)
- [ ] Font weights: 400 (regular), 600 (semi-bold), 700 (bold)

### Spacing ✅
- [ ] All spacing uses 8px grid (4px, 8px, 12px, 16px, 24px, 32px, 48px)
- [ ] Page padding: 24px (pa-6)
- [ ] Card padding: 48px desktop, 32px mobile
- [ ] Section gaps: 32px (var(--space-8))

### Components ✅
- [ ] Buttons: 8px border-radius, no uppercase
- [ ] Cards: 12px border-radius
- [ ] Auth cards: 16px border-radius (hero cards)
- [ ] Inputs: 8px border-radius
- [ ] Chips: 4px border-radius

### Shadows ✅
- [ ] Subtle shadows (no harsh Material Design)
- [ ] Elevation 1: 0 1px 2px rgba(0,0,0,0.05)
- [ ] Elevation 2-4: 0 4px 6px rgba(0,0,0,0.1)
- [ ] Elevation 8: 0 20px 25px rgba(0,0,0,0.1)

### Responsive ✅
- [ ] Mobile tested at 375px width (iPhone SE)
- [ ] Tablet tested at 768px width (iPad)
- [ ] Desktop tested at 1920px width
- [ ] Base font: 16px desktop, 14px mobile (< 960px)
- [ ] Touch targets: minimum 44px height

### Localization ✅
- [ ] All Spanish text uses IStringLocalizer
- [ ] No hardcoded strings in markup
- [ ] Currency: es-MX format ($1,234.56)
- [ ] Dates: dd/MM/yyyy format

---

## Visual Quality Checklist

Before marking a story "Done", verify:

### Desktop (1920px)
- [ ] Layout is not too wide (max-width constraints appropriate)
- [ ] Whitespace is generous (not cramped)
- [ ] Text is readable (line-height, max-width)
- [ ] Shadows are subtle
- [ ] Colors match design system

### Tablet (768px)
- [ ] Two-column grids stack appropriately
- [ ] Navigation is accessible
- [ ] Forms are usable
- [ ] Tables scroll horizontally if needed

### Mobile (375px)
- [ ] Login card is full-width
- [ ] Padding reduces appropriately
- [ ] Font size appropriate (14px base)
- [ ] Touch targets at least 44px
- [ ] No horizontal scroll (except tables)
- [ ] TenantDisplay shows icon only
- [ ] UserDisplay shows avatar only

---

## Inspiration & References

### Hardware Store Visual Language

**Colors:**
- Terracotta: Traditional brick, clay pots, terracotta tiles
- Concrete Gray: Industrial materials, cement, neutral reliability
- Tool Steel: Precision tools, craftsmanship, professionalism

**Typography:**
- Inter: Modern, clean, professional (vs. Roboto's tech feel)
- Generous spacing: Reflects the spaciousness of hardware stores
- Clear hierarchy: Easy to scan (like product aisles)

**Imagery:**
- Store icon: Represents the physical hardware store
- Material icons: Familiar, recognizable (inventory, sales, customers)

### Competitive Analysis

**What we avoid (Generic SaaS):**
- ❌ Blue Material Design (#1976D2)
- ❌ Uppercase button text
- ❌ Cramped padding
- ❌ Harsh shadows
- ❌ Tech-focused aesthetic

**What we embrace (Professional ERP):**
- ✅ Warm, welcoming primary color
- ✅ Sentence case text (professional, not aggressive)
- ✅ Generous whitespace (premium feel)
- ✅ Subtle shadows (elegant, refined)
- ✅ Business-focused aesthetic

---

**Document Version:** 1.0
**Last Updated:** 2026-01-27
**For:** Stakeholders & Product Owner
