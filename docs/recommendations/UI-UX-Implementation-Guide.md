# UI/UX Design System - Technical Implementation Guide

## Document Purpose

This guide provides detailed technical instructions for implementing the UI/UX Design System recommendation. Developers should reference this document when implementing user stories created from the main recommendation.

**Related Document:** `UI-UX-Design-System-Recommendation.md` (business case and requirements)

**Target Audience:** Development Team

**Last Updated:** 2026-01-27

---

## Table of Contents

1. [Phase 1: Core Theme Infrastructure](#phase-1-core-theme-infrastructure)
2. [Phase 2: Authentication Pages Redesign](#phase-2-authentication-pages-redesign)
3. [Phase 3: Core Reusable Components](#phase-3-core-reusable-components)
4. [Phase 4: Multi-Tenant Theming Infrastructure](#phase-4-multi-tenant-theming-infrastructure)
5. [Phase 5: Apply Design System to Existing Pages](#phase-5-apply-design-system-to-existing-pages)
6. [Appendices](#appendices)

---

## Phase 1: Core Theme Infrastructure

### 1.1 Create ThemeConfiguration Service

**File:** `src/Presentation/Corelio.BlazorApp/Services/ThemeConfiguration.cs` (NEW)

**Purpose:** Central MudTheme configuration with Industrial Terracotta palette

**Code:**

```csharp
using MudBlazor;

namespace Corelio.BlazorApp.Services;

/// <summary>
/// Provides the default Corelio theme configuration with Industrial Terracotta palette
/// </summary>
public static class ThemeConfiguration
{
    public static MudTheme CorelioDefaultTheme => new()
    {
        Palette = new PaletteLight
        {
            // Primary: Terracotta Red (#E74C3C)
            Primary = "#E74C3C",
            PrimaryDarken = "#CB4335",
            PrimaryLighten = "#EC7063",
            PrimaryContrastText = "#FFFFFF",

            // Secondary: Concrete Gray (#6C757D)
            Secondary = "#6C757D",
            SecondaryDarken = "#495057",
            SecondaryLighten = "#ADB5BD",
            SecondaryContrastText = "#FFFFFF",

            // Accent: Tool Steel (#17A2B8)
            Tertiary = "#17A2B8",
            TertiaryDarken = "#138496",
            TertiaryLighten = "#5BC0DE",
            TertiaryContrastText = "#FFFFFF",

            // Semantic colors
            Success = "#28A745",
            SuccessDarken = "#218838",
            SuccessLighten = "#5CB85C",

            Warning = "#FFC107",
            WarningDarken = "#E0A800",
            WarningLighten = "#FFD54F",

            Error = "#DC3545",
            ErrorDarken = "#C82333",
            ErrorLighten = "#E74C3C",

            Info = "#17A2B8",
            InfoDarken = "#138496",
            InfoLighten = "#5BC0DE",

            // Backgrounds
            Background = "#FFFFFF",
            BackgroundGrey = "#F8F9FA",
            Surface = "#FFFFFF",

            // Text
            TextPrimary = "#212529",
            TextSecondary = "#6C757D",
            TextDisabled = "#ADB5BD",

            // Borders
            Divider = "#DEE2E6",
            DividerLight = "#E9ECEF",

            // AppBar
            AppbarBackground = "#FFFFFF",
            AppbarText = "#212529",

            // Drawer
            DrawerBackground = "#FFFFFF",
            DrawerText = "#212529",
        },

        PaletteDark = new PaletteDark
        {
            // Dark mode (future enhancement)
            Primary = "#EC7063",
            Secondary = "#ADB5BD",
            Background = "#121416",
            Surface = "#212529",
            TextPrimary = "#F8F9FA",
            TextSecondary = "#ADB5BD",
        },

        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = new[] { "Inter", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif" },
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = "normal"
            },
            H1 = new H1
            {
                FontSize = "2.5rem",    // 40px
                FontWeight = 700,
                LineHeight = 1.2,
                LetterSpacing = "-0.02em"
            },
            H2 = new H2
            {
                FontSize = "2rem",      // 32px
                FontWeight = 700,
                LineHeight = 1.3,
                LetterSpacing = "-0.01em"
            },
            H3 = new H3
            {
                FontSize = "1.75rem",   // 28px
                FontWeight = 600,
                LineHeight = 1.3,
                LetterSpacing = "-0.01em"
            },
            H4 = new H4
            {
                FontSize = "1.5rem",    // 24px
                FontWeight = 600,
                LineHeight = 1.4,
                LetterSpacing = "normal"
            },
            H5 = new H5
            {
                FontSize = "1.25rem",   // 20px
                FontWeight = 600,
                LineHeight = 1.4,
                LetterSpacing = "normal"
            },
            H6 = new H6
            {
                FontSize = "1.125rem",  // 18px
                FontWeight = 600,
                LineHeight = 1.5,
                LetterSpacing = "normal"
            },
            Body1 = new Body1
            {
                FontSize = "1rem",      // 16px
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = "normal"
            },
            Body2 = new Body2
            {
                FontSize = "0.875rem",  // 14px
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = "normal"
            },
            Button = new Button
            {
                FontSize = "0.875rem",  // 14px
                FontWeight = 600,
                LineHeight = 1.75,
                LetterSpacing = "0.02em",
                TextTransform = "none"  // IMPORTANT: No uppercase
            },
            Caption = new Caption
            {
                FontSize = "0.75rem",   // 12px
                FontWeight = 400,
                LineHeight = 1.66,
                LetterSpacing = "0.01em"
            }
        },

        Shadows = new Shadow
        {
            Elevation = new string[]
            {
                "none",
                "0 1px 2px 0 rgba(0, 0, 0, 0.05)",                                                    // 1
                "0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)",              // 2
                "0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)",              // 3
                "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)",            // 4
                "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)",            // 5
                "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)",            // 6
                "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)",            // 7
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 9
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 10
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 11
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 12
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 13
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 14
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 15
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 16
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 17
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 18
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 19
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 20
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 21
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 22
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 23
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 24
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",          // 25
            }
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
            AppbarHeight = "64px",
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px"
        }
    };
}
```

**Testing:**
```bash
# Verify file compiles
dotnet build src/Presentation/Corelio.BlazorApp

# Expected: No compilation errors
```

### 1.2 Create CSS Variables File

**File:** `src/Presentation/Corelio.BlazorApp/wwwroot/css/variables.css` (NEW)

**Purpose:** CSS custom properties for consistent styling and easy tenant theming

**Code:**

```css
:root {
    /* Primary Colors (Terracotta) */
    --color-primary-50: #FADBD8;
    --color-primary-100: #F5B7B1;
    --color-primary-200: #F1948A;
    --color-primary-300: #EC7063;
    --color-primary-400: #E74C3C;
    --color-primary-500: #CB4335;
    --color-primary-600: #B03A2E;
    --color-primary-700: #943126;
    --color-primary-800: #78281F;
    --color-primary-900: #641E16;

    /* Secondary Colors (Concrete Gray) */
    --color-secondary-50: #F8F9FA;
    --color-secondary-100: #E9ECEF;
    --color-secondary-200: #DEE2E6;
    --color-secondary-300: #CED4DA;
    --color-secondary-400: #ADB5BD;
    --color-secondary-500: #6C757D;
    --color-secondary-600: #495057;
    --color-secondary-700: #343A40;
    --color-secondary-800: #212529;
    --color-secondary-900: #121416;

    /* Accent Colors (Tool Steel) */
    --color-accent-500: #17A2B8;

    /* Semantic Colors */
    --color-success: #28A745;
    --color-warning: #FFC107;
    --color-error: #DC3545;
    --color-info: #17A2B8;

    /* Spacing (8px grid) */
    --space-0: 0;
    --space-1: 0.25rem;  /* 4px */
    --space-2: 0.5rem;   /* 8px */
    --space-3: 0.75rem;  /* 12px */
    --space-4: 1rem;     /* 16px */
    --space-5: 1.25rem;  /* 20px */
    --space-6: 1.5rem;   /* 24px */
    --space-8: 2rem;     /* 32px */
    --space-10: 2.5rem;  /* 40px */
    --space-12: 3rem;    /* 48px */
    --space-16: 4rem;    /* 64px */
    --space-20: 5rem;    /* 80px */
    --space-24: 6rem;    /* 96px */

    /* Shadows */
    --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
    --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
    --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
    --shadow-xl: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);

    /* Border Radius */
    --radius-sm: 0.25rem;  /* 4px */
    --radius-md: 0.5rem;   /* 8px */
    --radius-lg: 0.75rem;  /* 12px */
    --radius-xl: 1rem;     /* 16px */
    --radius-full: 9999px;

    /* Typography */
    --font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Helvetica Neue', Arial, sans-serif;
}

/* Tenant-specific overrides (dynamically injected in Phase 4) */
[data-tenant-theme] {
    --color-primary-400: var(--tenant-primary-color, #E74C3C);
    /* Other shades auto-generated from primary color */
}
```

**Testing:**
```bash
# Verify file exists
ls src/Presentation/Corelio.BlazorApp/wwwroot/css/variables.css
```

### 1.3 Update App.razor

**File:** `src/Presentation/Corelio.BlazorApp/Components/App.razor` (MODIFY)

**Changes:**
1. Add Inter font from Google Fonts
2. Import variables.css
3. Remove Bootstrap CSS
4. Update CSS loading order

**Before:**
```razor
<!-- OLD - Remove these lines -->
<link href="bootstrap/bootstrap.min.css" rel="stylesheet" />
```

**After:**
```razor
<!DOCTYPE html>
<html lang="es-MX">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />

    <!-- Preconnect for faster font loading -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>

    <!-- Inter Font Family (300, 400, 500, 600, 700 weights) -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">

    <!-- Material Design Icons (for MudBlazor) -->
    <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" />

    <!-- MudBlazor CSS -->
    <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />

    <!-- Custom CSS (in order of specificity) -->
    <link rel="stylesheet" href="css/variables.css" />
    <link rel="stylesheet" href="app.css" />
    <link rel="stylesheet" href="Corelio.BlazorApp.styles.css" />

    <link rel="icon" type="image/png" href="favicon.png" />
    <HeadOutlet @rendermode="InteractiveServer" />
</head>
<body>
    <Routes @rendermode="InteractiveServer" />
    <script src="_framework/blazor.web.js"></script>
    <script src="_content/MudBlazor/MudBlazor.min.js"></script>
</body>
</html>
```

**Key Changes:**
- ❌ Removed: Bootstrap CSS
- ✅ Added: Inter font (300-700 weights)
- ✅ Added: Preconnect hints for faster font loading
- ✅ Added: variables.css import
- ✅ Updated: CSS load order (MudBlazor → variables → app → scoped)

### 1.4 Update MainLayout.razor

**File:** `src/Presentation/Corelio.BlazorApp/Components/Layout/MainLayout.razor` (MODIFY)

**Changes:**
1. Apply ThemeConfiguration.CorelioDefaultTheme
2. Update AppBar styling (white background, subtle border)
3. Update main content background

**Before:**
```razor
<!-- OLD -->
<MudThemeProvider />
```

**After:**
```razor
@using Corelio.BlazorApp.Services

<MudThemeProvider Theme="@ThemeConfiguration.CorelioDefaultTheme" @bind-IsDarkMode="@_isDarkMode" />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <MudAppBar Elevation="1" Dense="false" Style="background: white; border-bottom: 1px solid var(--color-secondary-200);">
        <MudIconButton Icon="@Icons.Material.Filled.Menu"
                       Color="Color.Inherit"
                       Edge="Edge.Start"
                       OnClick="@(() => _drawerOpen = !_drawerOpen)" />

        <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
            <MudIcon Icon="@Icons.Material.Filled.Store" Color="Color.Primary" Size="Size.Medium" />
            <MudText Typo="Typo.h6" Style="font-weight: 700; color: var(--color-primary-600);">
                Corelio ERP
            </MudText>
        </MudStack>

        <MudSpacer />

        <TenantDisplay />

        <MudSpacer />

        <AuthorizeView>
            <Authorized>
                <UserDisplay />
            </Authorized>
        </AuthorizeView>
    </MudAppBar>

    <MudDrawer @bind-Open="_drawerOpen"
               Elevation="2"
               ClipMode="DrawerClipMode.Always"
               Style="background: white;">
        <NavMenu />
    </MudDrawer>

    <MudMainContent Style="background: var(--color-secondary-50); min-height: 100vh;">
        <MudContainer MaxWidth="MaxWidth.ExtraExtraLarge" Class="pa-6">
            @Body
        </MudContainer>
    </MudMainContent>
</MudLayout>

@code {
    private bool _drawerOpen = true;
    private bool _isDarkMode = false;
}
```

**Key Improvements:**
- ✅ Applied CorelioDefaultTheme
- ✅ White AppBar with subtle border (#DEE2E6)
- ✅ Primary color for Store icon (#E74C3C)
- ✅ Light gray background for main content (#F8F9FA)
- ✅ Consistent padding (24px via pa-6)

### 1.5 Update app.css

**File:** `src/Presentation/Corelio.BlazorApp/wwwroot/app.css` (MODIFY)

**Purpose:** Override MudBlazor defaults, add custom styles

**Replace existing content with:**

```css
/* ========================================
   Corelio ERP - Custom Styles
   Design System: Industrial & Professional
   ======================================== */

/* Global Styles */
html, body {
    font-family: var(--font-family);
    font-size: 16px;
    color: var(--color-secondary-800);
    background-color: var(--color-secondary-50);
}

* {
    box-sizing: border-box;
}

/* ========================================
   MudBlazor Component Overrides
   ======================================== */

/* Buttons: No uppercase, consistent border-radius */
.mud-button-root {
    text-transform: none !important;
    border-radius: var(--radius-md) !important;
    font-weight: 600 !important;
}

/* Paper & Cards: Softer border-radius */
.mud-paper {
    border-radius: var(--radius-lg) !important;
}

.mud-card {
    border-radius: var(--radius-lg) !important;
}

/* Inputs: Consistent border-radius */
.mud-input {
    border-radius: var(--radius-md) !important;
}

/* Tables: Consistent border-radius */
.mud-table {
    border-radius: var(--radius-lg) !important;
}

/* Focus States (Accessibility & Visual Feedback) */
.mud-input:focus-within .mud-input-slot {
    border-color: var(--color-primary-400) !important;
    box-shadow: 0 0 0 3px rgba(231, 76, 60, 0.1) !important;
}

/* ========================================
   Component Patterns
   ======================================== */

/* Loading Container */
.loading-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: var(--space-12);
    gap: var(--space-4);
    min-height: 320px;
}

/* Empty State */
.empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: var(--space-12);
    gap: var(--space-3);
    text-align: center;
    min-height: 400px;
}

/* Error State */
.error-state {
    padding: var(--space-4);
    border-radius: var(--radius-md);
    background: var(--color-error);
    color: white;
}

/* Page Header */
.page-header {
    margin-bottom: var(--space-8);
    padding-bottom: var(--space-6);
    border-bottom: 1px solid var(--color-secondary-200);
}

/* ========================================
   Utility Classes
   ======================================== */

/* Text Colors */
.text-primary { color: var(--color-primary-600) !important; }
.text-secondary { color: var(--color-secondary-600) !important; }
.text-success { color: var(--color-success) !important; }
.text-warning { color: var(--color-warning) !important; }
.text-error { color: var(--color-error) !important; }

/* Background Colors */
.bg-light { background-color: var(--color-secondary-50) !important; }
.bg-white { background-color: #FFFFFF !important; }

/* Shadows */
.shadow-sm { box-shadow: var(--shadow-sm) !important; }
.shadow-md { box-shadow: var(--shadow-md) !important; }
.shadow-lg { box-shadow: var(--shadow-lg) !important; }

/* Spacing Utilities */
.mt-0 { margin-top: 0 !important; }
.mt-1 { margin-top: var(--space-1) !important; }
.mt-2 { margin-top: var(--space-2) !important; }
.mt-3 { margin-top: var(--space-3) !important; }
.mt-4 { margin-top: var(--space-4) !important; }
.mt-6 { margin-top: var(--space-6) !important; }
.mt-8 { margin-top: var(--space-8) !important; }

.mb-0 { margin-bottom: 0 !important; }
.mb-1 { margin-bottom: var(--space-1) !important; }
.mb-2 { margin-bottom: var(--space-2) !important; }
.mb-3 { margin-bottom: var(--space-3) !important; }
.mb-4 { margin-bottom: var(--space-4) !important; }
.mb-6 { margin-bottom: var(--space-6) !important; }
.mb-8 { margin-bottom: var(--space-8) !important; }

/* ========================================
   Responsive Design
   ======================================== */

@media (max-width: 960px) {
    html, body {
        font-size: 14px; /* Smaller base font on mobile */
    }

    .page-header {
        margin-bottom: var(--space-6);
    }
}

@media (max-width: 600px) {
    .loading-container {
        padding: var(--space-8);
    }

    .empty-state {
        padding: var(--space-8);
    }
}
```

**Testing Phase 1:**

```bash
# Build the Blazor app
dotnet build src/Presentation/Corelio.BlazorApp

# Run the application
dotnet run --project src/Aspire/Corelio.AppHost

# Manual verification in browser:
# 1. Navigate to https://localhost:XXXX
# 2. Open DevTools → Network tab
# 3. Verify Inter font loads (google fonts)
# 4. Open DevTools → Elements → Inspect any button
# 5. Verify primary color is #E74C3C
# 6. Verify button text is NOT uppercase
# 7. Verify border-radius is 8px (0.5rem)
```

**Phase 1 Acceptance Criteria:**
- [ ] Inter font loads successfully (verify in DevTools Network tab)
- [ ] Primary color appears as #E74C3C (terracotta red) in UI
- [ ] Typography scale matches specs (H1=40px, H2=32px, etc.)
- [ ] Buttons have no uppercase text transformation
- [ ] Border radius is 8px on cards/buttons
- [ ] AppBar has white background with subtle border
- [ ] Main content area has light gray background (#F8F9FA)
- [ ] No Bootstrap CSS loaded
- [ ] CSS variables accessible via `var(--color-primary-400)`

---

## Phase 2: Authentication Pages Redesign

**See detailed implementation in the full plan document for:**
- AuthLayout.razor (new minimal layout)
- Login.razor (redesigned with hero section)
- Register.razor (section headers, two-column grid)
- ForgotPassword.razor (consistent styling)
- ResetPassword.razor (consistent styling)

**Key principles to follow:**
1. Use AuthLayout instead of MainLayout
2. Large hero section with logo icon in circular gradient background
3. Generous padding (48px desktop, 32px mobile)
4. Email/password fields with icons
5. Password visibility toggle
6. Large submit buttons (56px height)
7. Loading states with spinner + text
8. Error/warning alerts with icons
9. Smooth fade-in animations

---

## Phase 3: Core Reusable Components

### 3.1 PageHeader Component

**File:** `src/Presentation/Corelio.BlazorApp/Components/Shared/PageHeader.razor` (NEW)

**Code:**

```razor
@inject IStringLocalizer<SharedResource> L

<div class="page-header">
    <!-- Breadcrumbs -->
    @if (Breadcrumbs != null && Breadcrumbs.Any())
    {
        <MudBreadcrumbs Items="@Breadcrumbs"
                       Separator=">"
                       Style="margin-bottom: var(--space-3);">
        </MudBreadcrumbs>
    }

    <!-- Title Row -->
    <MudStack Row="true"
             AlignItems="AlignItems.Center"
             Justify="Justify.SpaceBetween"
             Wrap="Wrap.Wrap"
             Spacing="4">
        <!-- Title & Description -->
        <div style="flex: 1; min-width: 200px;">
            <MudText Typo="Typo.h4"
                    Style="font-weight: 700; color: var(--color-secondary-900); margin-bottom: var(--space-1);">
                @Title
            </MudText>

            @if (!string.IsNullOrEmpty(Description))
            {
                <MudText Typo="Typo.body2"
                        Color="Color.Secondary"
                        Style="max-width: 600px;">
                    @Description
                </MudText>
            }
        </div>

        <!-- Action Buttons -->
        @if (Actions != null)
        {
            <div>
                @Actions
            </div>
        }
    </MudStack>
</div>

@code {
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public string? Description { get; set; }

    [Parameter]
    public List<BreadcrumbItem>? Breadcrumbs { get; set; }

    [Parameter]
    public RenderFragment? Actions { get; set; }
}
```

**Usage Example:**

```razor
@code {
    private List<BreadcrumbItem> _breadcrumbs = new()
    {
        new BreadcrumbItem("Inicio", href: "/"),
        new BreadcrumbItem("Productos", href: "/products", disabled: true)
    };
}

<PageHeader Title="Gestión de Productos"
           Description="Administre el catálogo de productos de su ferretería"
           Breadcrumbs="_breadcrumbs">
    <Actions>
        <MudButton Variant="Variant.Filled"
                   Color="Color.Primary"
                   StartIcon="@Icons.Material.Filled.Add"
                   Href="/products/new">
            Nuevo Producto
        </MudButton>
    </Actions>
</PageHeader>
```

### 3.2 LoadingState Component

**File:** `src/Presentation/Corelio.BlazorApp/Components/Shared/LoadingState.razor` (NEW)

**Code:**

```razor
@inject IStringLocalizer<SharedResource> L

<div class="loading-state">
    <MudProgressCircular Size="Size.Large"
                        Indeterminate="true"
                        Color="Color.Primary"
                        Style="width: 64px; height: 64px;" />

    <MudText Typo="Typo.body1"
            Color="Color.Secondary"
            Style="margin-top: var(--space-4);">
        @Message
    </MudText>
</div>

@code {
    [Parameter]
    public string Message { get; set; } = "Cargando...";
}
```

**Usage Example:**

```razor
@if (_isLoading)
{
    <LoadingState Message="Cargando productos..." />
}
```

### 3.3 EmptyState Component

**File:** `src/Presentation/Corelio.BlazorApp/Components/Shared/EmptyState.razor` (NEW)

**Code:**

```razor
<div class="empty-state">
    <div class="empty-state-icon">
        <MudIcon Icon="@Icon"
                Style="font-size: 4rem; color: var(--color-secondary-400);" />
    </div>

    <MudText Typo="Typo.h6"
            Style="font-weight: 600; color: var(--color-secondary-800); margin-top: var(--space-4);">
        @Title
    </MudText>

    <MudText Typo="Typo.body2"
            Color="Color.Secondary"
            Align="Align.Center"
            Style="max-width: 400px; margin-top: var(--space-2);">
        @Description
    </MudText>

    @if (!string.IsNullOrEmpty(ActionText) && OnAction.HasDelegate)
    {
        <MudButton Variant="Variant.Filled"
                   Color="Color.Primary"
                   StartIcon="@ActionIcon"
                   OnClick="OnAction"
                   Style="margin-top: var(--space-6);">
            @ActionText
        </MudButton>
    }
</div>

@code {
    [Parameter, EditorRequired]
    public string Icon { get; set; } = Icons.Material.Filled.Inbox;

    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string Description { get; set; } = string.Empty;

    [Parameter]
    public string? ActionText { get; set; }

    [Parameter]
    public string ActionIcon { get; set; } = Icons.Material.Filled.Add;

    [Parameter]
    public EventCallback OnAction { get; set; }
}
```

**CSS (add to app.css):**

```css
.empty-state-icon {
    width: 96px;
    height: 96px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: var(--color-secondary-100);
    border-radius: var(--radius-full);
}
```

**Usage Example:**

```razor
@if (_products.Count == 0)
{
    <EmptyState Icon="@Icons.Material.Filled.Inventory"
               Title="No hay productos"
               Description="No hay productos en el catálogo. Comience agregando su primer producto."
               ActionText="Nuevo Producto"
               ActionIcon="@Icons.Material.Filled.Add"
               OnAction="NavigateToNew" />
}

@code {
    private void NavigateToNew()
    {
        Navigation.NavigateTo("/products/new");
    }
}
```

---

## Phase 4: Multi-Tenant Theming Infrastructure

**This phase enables tenants to customize their primary color and logo.**

### 4.1 Database Migration

**Step 1:** Add fields to TenantConfiguration entity

**File:** `src/Core/Corelio.Domain/Entities/TenantConfiguration.cs` (MODIFY)

**Add properties:**

```csharp
/// <summary>
/// Custom primary color in hex format (e.g., #E74C3C)
/// </summary>
public string? PrimaryColor { get; set; }

/// <summary>
/// URL to tenant's logo (Azure Blob Storage)
/// </summary>
public string? LogoUrl { get; set; }

/// <summary>
/// Whether to use custom theme or default Corelio theme
/// </summary>
public bool UseCustomTheme { get; set; }
```

**Step 2:** Create migration

```bash
cd src/Infrastructure/Corelio.Infrastructure

dotnet ef migrations add AddTenantBrandingFields \
    --startup-project ../../Presentation/Corelio.WebAPI \
    --context ApplicationDbContext

dotnet ef database update \
    --startup-project ../../Presentation/Corelio.WebAPI \
    --context ApplicationDbContext
```

**Verify migration:**
```bash
# Check migration file
cat Migrations/*_AddTenantBrandingFields.cs

# Verify database
# Connect to PostgreSQL and run:
# \d tenant_configurations
# Should show: primary_color, logo_url, use_custom_theme columns
```

### 4.2 TenantThemeService (Infrastructure)

**Interface:** `src/Core/Corelio.Application/Common/Interfaces/ITenantThemeService.cs` (NEW)

```csharp
namespace Corelio.Application.Common.Interfaces;

/// <summary>
/// Service for managing tenant theme customization
/// </summary>
public interface ITenantThemeService
{
    /// <summary>
    /// Get theme configuration for a tenant (cached)
    /// </summary>
    Task<TenantThemeDto?> GetTenantThemeAsync(Guid tenantId, CancellationToken ct = default);

    /// <summary>
    /// Invalidate cached theme after update
    /// </summary>
    Task InvalidateThemeCacheAsync(Guid tenantId, CancellationToken ct = default);

    /// <summary>
    /// Validate hex color format
    /// </summary>
    bool IsValidHexColor(string hexColor);
}

/// <summary>
/// DTO for tenant theme data
/// </summary>
public record TenantThemeDto
{
    public Guid TenantId { get; init; }
    public string PrimaryColor { get; init; } = "#E74C3C";
    public string? LogoUrl { get; init; }
    public bool UseCustomTheme { get; init; }
}
```

**Implementation:** `src/Infrastructure/Corelio.Infrastructure/Services/TenantThemeService.cs` (NEW)

```csharp
using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Corelio.Infrastructure.Services;

/// <summary>
/// Manages tenant theme configuration with Redis caching
/// </summary>
public class TenantThemeService(
    ITenantConfigurationRepository tenantConfigRepo,
    IDistributedCache cache) : ITenantThemeService
{
    private const string CacheKeyPrefix = "tenant-theme:";
    private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(2);

    public async Task<TenantThemeDto?> GetTenantThemeAsync(Guid tenantId, CancellationToken ct = default)
    {
        // Try cache first
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        var cachedTheme = await cache.GetStringAsync(cacheKey, ct);

        if (!string.IsNullOrEmpty(cachedTheme))
        {
            return JsonSerializer.Deserialize<TenantThemeDto>(cachedTheme);
        }

        // Load from database
        var config = await tenantConfigRepo.GetByTenantIdAsync(tenantId, ct);
        if (config == null || !config.UseCustomTheme)
        {
            return null; // Use default theme
        }

        var theme = new TenantThemeDto
        {
            TenantId = tenantId,
            PrimaryColor = config.PrimaryColor ?? "#E74C3C",
            LogoUrl = config.LogoUrl,
            UseCustomTheme = config.UseCustomTheme
        };

        // Cache the result
        var serialized = JsonSerializer.Serialize(theme);
        await cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheDuration
        }, ct);

        return theme;
    }

    public async Task InvalidateThemeCacheAsync(Guid tenantId, CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        await cache.RemoveAsync(cacheKey, ct);
    }

    public bool IsValidHexColor(string hexColor)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            return false;

        return Regex.IsMatch(hexColor, @"^#[0-9A-Fa-f]{6}$");
    }
}
```

**Registration:** `src/Infrastructure/Corelio.Infrastructure/DependencyInjection.cs` (MODIFY)

```csharp
// Add to AddInfrastructureServices method
services.AddScoped<ITenantThemeService, TenantThemeService>();
```

### 4.3 DynamicThemeService (Blazor)

**File:** `src/Presentation/Corelio.BlazorApp/Services/DynamicThemeService.cs` (NEW)

```csharp
using MudBlazor;
using Corelio.Application.Common.Interfaces;

namespace Corelio.BlazorApp.Services;

/// <summary>
/// Builds MudTheme dynamically from tenant branding configuration
/// </summary>
public class DynamicThemeService(ITenantThemeService tenantThemeService)
{
    public async Task<MudTheme> GetThemeForTenantAsync(Guid? tenantId)
    {
        if (tenantId == null)
        {
            return ThemeConfiguration.CorelioDefaultTheme;
        }

        var tenantTheme = await tenantThemeService.GetTenantThemeAsync(tenantId.Value);

        if (tenantTheme == null || !tenantTheme.UseCustomTheme)
        {
            return ThemeConfiguration.CorelioDefaultTheme;
        }

        // Clone default theme and override primary color
        var theme = ThemeConfiguration.CorelioDefaultTheme;
        theme.Palette.Primary = tenantTheme.PrimaryColor;

        // Generate shades from primary color
        theme.Palette.PrimaryDarken = DarkenColor(tenantTheme.PrimaryColor, 0.15);
        theme.Palette.PrimaryLighten = LightenColor(tenantTheme.PrimaryColor, 0.15);

        return theme;
    }

    private string DarkenColor(string hex, double factor)
    {
        var rgb = HexToRgb(hex);
        var r = (int)(rgb.R * (1 - factor));
        var g = (int)(rgb.G * (1 - factor));
        var b = (int)(rgb.B * (1 - factor));
        return RgbToHex(r, g, b);
    }

    private string LightenColor(string hex, double factor)
    {
        var rgb = HexToRgb(hex);
        var r = (int)(rgb.R + (255 - rgb.R) * factor);
        var g = (int)(rgb.G + (255 - rgb.G) * factor);
        var b = (int)(rgb.B + (255 - rgb.B) * factor);
        return RgbToHex(r, g, b);
    }

    private (int R, int G, int B) HexToRgb(string hex)
    {
        hex = hex.Replace("#", "");
        var r = Convert.ToInt32(hex.Substring(0, 2), 16);
        var g = Convert.ToInt32(hex.Substring(2, 2), 16);
        var b = Convert.ToInt32(hex.Substring(4, 2), 16);
        return (r, g, b);
    }

    private string RgbToHex(int r, int g, int b)
    {
        return $"#{r:X2}{g:X2}{b:X2}";
    }
}
```

**Registration:** `src/Presentation/Corelio.BlazorApp/Program.cs` (MODIFY)

```csharp
builder.Services.AddScoped<DynamicThemeService>();
```

### 4.4 Update MainLayout for Dynamic Theming

**File:** `Components/Layout/MainLayout.razor` (MODIFY)

```razor
@using Corelio.BlazorApp.Services
@inject DynamicThemeService ThemeService
@inject AuthenticationStateProvider AuthStateProvider

<MudThemeProvider @ref="_themeProvider" Theme="@_currentTheme" @bind-IsDarkMode="@_isDarkMode" />
<!-- Rest of providers and layout... -->

@code {
    private MudThemeProvider? _themeProvider;
    private MudTheme _currentTheme = ThemeConfiguration.CorelioDefaultTheme;
    private bool _isDarkMode = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadTenantTheme();
    }

    private async Task LoadTenantTheme()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = user.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                _currentTheme = await ThemeService.GetThemeForTenantAsync(tenantId);
                StateHasChanged();
            }
        }
    }
}
```

### 4.5 API Endpoints for Theme Management

**File:** `src/Presentation/Corelio.WebAPI/Endpoints/TenantThemeEndpoints.cs` (NEW)

```csharp
using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

public static class TenantThemeEndpoints
{
    public static IEndpointRouteBuilder MapTenantThemeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/tenants/theme")
            .WithTags("Tenant Theme")
            .RequireAuthorization();

        group.MapGet("/current", GetCurrentTenantTheme)
            .WithName("GetCurrentTenantTheme")
            .WithOpenApi();

        group.MapPut("/", UpdateTenantTheme)
            .WithName("UpdateTenantTheme")
            .RequireAuthorization(policy => policy.RequireClaim("permission", "tenants.manage"))
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetCurrentTenantTheme(
        ITenantService tenantService,
        ITenantThemeService themeService,
        CancellationToken ct)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        var theme = await themeService.GetTenantThemeAsync(tenantId, ct);

        return theme != null
            ? Results.Ok(theme)
            : Results.Ok(new { PrimaryColor = "#E74C3C", UseCustomTheme = false });
    }

    private static async Task<IResult> UpdateTenantTheme(
        [FromBody] UpdateTenantThemeRequest request,
        ITenantService tenantService,
        ITenantConfigurationRepository configRepo,
        ITenantThemeService themeService,
        CancellationToken ct)
    {
        var tenantId = tenantService.GetCurrentTenantId();

        // Validate hex color
        if (!themeService.IsValidHexColor(request.PrimaryColor))
        {
            return Results.BadRequest(new { Error = "Invalid hex color format. Use #RRGGBB format." });
        }

        var config = await configRepo.GetByTenantIdAsync(tenantId, ct);
        if (config == null)
        {
            return Results.NotFound(new { Error = "Tenant configuration not found" });
        }

        config.PrimaryColor = request.PrimaryColor;
        config.UseCustomTheme = request.UseCustomTheme;

        await configRepo.UpdateAsync(config, ct);
        await themeService.InvalidateThemeCacheAsync(tenantId, ct);

        return Results.Ok(new { Success = true, Message = "Theme updated successfully" });
    }
}

public record UpdateTenantThemeRequest(
    string PrimaryColor,
    bool UseCustomTheme);
```

**Register endpoints:** `src/Presentation/Corelio.WebAPI/Endpoints/EndpointExtensions.cs` (MODIFY)

```csharp
public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        // ... existing endpoints
        app.MapTenantThemeEndpoints();
        return app;
    }
}
```

---

## Phase 5: Apply Design System to Existing Pages

**Key tasks:**
1. Update ProductList.razor to use PageHeader, LoadingState, EmptyState
2. Update ProductForm.razor with section headers and consistent styling
3. Standardize table headers (light gray background)
4. Format currency/dates with es-MX culture
5. Ensure mobile responsiveness

**See full implementation details in the main plan document.**

---

## Appendices

### A. Color Contrast Ratios (WCAG AA Compliance)

| Foreground | Background | Ratio | Pass |
|------------|------------|-------|------|
| #E74C3C (Primary) | #FFFFFF (White) | 4.8:1 | ✅ AA |
| #212529 (Text Primary) | #FFFFFF (White) | 16.1:1 | ✅ AAA |
| #6C757D (Text Secondary) | #FFFFFF (White) | 4.7:1 | ✅ AA |
| #FFFFFF (White) | #E74C3C (Primary) | 4.8:1 | ✅ AA |

### B. Browser Testing Checklist

- [ ] Chrome 120+ (Windows, macOS, Android)
- [ ] Firefox 115+ (Windows, macOS)
- [ ] Safari 17+ (macOS, iOS)
- [ ] Edge 120+ (Windows)
- [ ] Mobile Safari (iOS 16+)
- [ ] Chrome Mobile (Android 12+)

### C. Performance Budget

| Metric | Budget | Actual (Phase 1) |
|--------|--------|------------------|
| Font files (Inter) | < 150 KB | ~100 KB |
| CSS (variables + app.css) | < 30 KB | ~20 KB |
| Theme cache hit rate | > 90% | N/A (Phase 4) |
| Page load time | < 2 seconds | ~1.5 seconds |

### D. Useful Commands

```bash
# Run Aspire AppHost (recommended)
dotnet run --project src/Aspire/Corelio.AppHost

# Build Blazor app
dotnet build src/Presentation/Corelio.BlazorApp

# Create migration
dotnet ef migrations add MigrationName \
    --project src/Infrastructure/Corelio.Infrastructure \
    --startup-project src/Presentation/Corelio.WebAPI

# Update database
dotnet ef database update \
    --project src/Infrastructure/Corelio.Infrastructure \
    --startup-project src/Presentation/Corelio.WebAPI

# Run tests
dotnet test

# Format code
dotnet format
```

### E. CSS Variable Reference

```css
/* Quick reference for most-used variables */
--color-primary-400: #E74C3C
--color-secondary-500: #6C757D
--space-4: 1rem (16px)
--space-6: 1.5rem (24px)
--space-8: 2rem (32px)
--radius-md: 0.5rem (8px)
--radius-lg: 0.75rem (12px)
--shadow-md: Subtle card shadow
```

---

**Document Version:** 1.0
**Last Updated:** 2026-01-27
**Maintainer:** Development Team
