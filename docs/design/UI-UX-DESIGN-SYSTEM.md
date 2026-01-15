# Corelio ERP - UI/UX Design System
## Professional Design System for Mexican Hardware Store Management

**Version:** 1.0
**Last Updated:** 2026-01-13
**Target Audience:** Hardware Store Owners & Staff in Mexico

---

## Design Philosophy

### Core Principles

**"Confianza Industrial"** (Industrial Trust)
- **Professional & Trustworthy:** Hardware stores deal with inventory, money, and customer relationships. The UI must inspire confidence.
- **Efficient & Task-Oriented:** Store owners need to complete tasks quickly. No unnecessary friction.
- **Accessible & Clear:** Staff of varying technical skill levels will use this daily.
- **Distinctive & Modern:** Avoid generic SaaS aesthetics. Embrace industrial design influences with modern polish.

### Visual Direction

**Industrial Modern:**
- Strong, geometric layouts with clear hierarchy
- Subtle industrial design influences (tools, construction materials)
- Warm, approachable color palette (not cold corporate blues)
- Generous whitespace with purposeful density where needed
- Professional without being sterile

**Mexican Context:**
- Colors inspired by Mexican hardware stores (terracotta, concrete, wood tones)
- Spanish language throughout (es-MX)
- Cultural considerations (family businesses, personal relationships)
- Date format: dd/MM/yyyy
- Currency: MXN ($1,234.56)

---

## Color System

### Primary Palette

```css
/* Industrial Terracotta - Primary Brand Color */
--color-primary-50:  #fef3f2;
--color-primary-100: #fde6e4;
--color-primary-200: #fbd2cd;
--color-primary-300: #f7b1a9;
--color-primary-400: #f18375;
--color-primary-500: #e65946;  /* Main primary */
--color-primary-600: #d23b28;
--color-primary-700: #b1301e;
--color-primary-800: #922b1c;
--color-primary-900: #78291d;

/* Concrete Gray - Secondary/Neutral */
--color-secondary-50:  #f8f9fa;
--color-secondary-100: #f1f3f5;
--color-secondary-200: #e9ecef;
--color-secondary-300: #dee2e6;
--color-secondary-400: #ced4da;
--color-secondary-500: #adb5bd;  /* Main secondary */
--color-secondary-600: #868e96;
--color-secondary-700: #495057;
--color-secondary-800: #343a40;
--color-secondary-900: #212529;

/* Tool Steel - Accent Color */
--color-accent-50:  #ecf8f8;
--color-accent-100: #d1eeee;
--color-accent-200: #a6ddde;
--color-accent-300: #6fc4c7;
--color-accent-400: #42a5a9;
--color-accent-500: #2a8a8f;  /* Main accent */
--color-accent-600: #226d77;
--color-accent-700: #215963;
--color-accent-800: #224952;
--color-accent-900: #203e46;
```

### Semantic Colors

```css
/* Success - Wood Green */
--color-success-light: #d4f4dd;
--color-success-main:  #2e7d32;  /* Forest green */
--color-success-dark:  #1b5e20;

/* Warning - Safety Orange */
--color-warning-light: #fff4e5;
--color-warning-main:  #ed6c02;  /* Construction orange */
--color-warning-dark:  #e65100;

/* Error - Alert Red */
--color-error-light: #ffebee;
--color-error-main:  #d32f2f;  /* Safety red */
--color-error-dark:  #c62828;

/* Info - Sky Blue */
--color-info-light: #e3f2fd;
--color-info-main:  #0288d1;  /* Clear sky */
--color-info-dark:  #01579b;
```

### Background System

```css
/* Application Backgrounds */
--bg-page:           #f8f9fa;  /* Light concrete */
--bg-surface:        #ffffff;  /* White */
--bg-surface-hover:  #f8f9fa;  /* Subtle hover */
--bg-surface-active: #e9ecef;  /* Active state */

/* Overlays */
--bg-overlay-light:  rgba(0, 0, 0, 0.04);
--bg-overlay-medium: rgba(0, 0, 0, 0.08);
--bg-overlay-dark:   rgba(0, 0, 0, 0.12);
```

### Text Colors

```css
/* Text Hierarchy */
--text-primary:   #212529;  /* Main text */
--text-secondary: #495057;  /* Secondary text */
--text-tertiary:  #868e96;  /* Subtle text */
--text-disabled:  #adb5bd;  /* Disabled state */
--text-on-primary: #ffffff; /* Text on primary color */
```

### Usage Guidelines

**Primary (Terracotta):**
- Primary actions (save, submit, confirm)
- Active states
- Key CTAs
- Tenant indicators

**Secondary (Concrete Gray):**
- Navigation elements
- Containers
- Secondary actions
- Borders and dividers

**Accent (Tool Steel):**
- Interactive elements (links, hover states)
- Info chips/badges
- Selected states
- Progress indicators

**AVOID:**
- ❌ Pure black (#000000) - Too harsh
- ❌ Pure white text on primary - Use --text-on-primary
- ❌ Multiple primary colors in same view - Confusing
- ❌ Red/green together - Color blind issues

---

## Typography System

### Font Families

```css
/* Primary Font - System Stack for Performance */
--font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI',
             'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', 'Fira Sans',
             'Droid Sans', 'Helvetica Neue', sans-serif;

/* Monospace - Numbers, Codes, SKUs */
--font-mono: 'JetBrains Mono', 'Fira Code', 'Consolas', 'Monaco',
             'Courier New', monospace;
```

### Type Scale

```css
/* Display - Large marketing/hero text */
--text-display-lg: 3.5rem;    /* 56px */
--text-display-md: 2.875rem;  /* 46px */
--text-display-sm: 2.25rem;   /* 36px */

/* Heading - Page titles, section headers */
--text-h1: 2rem;      /* 32px - Page title */
--text-h2: 1.5rem;    /* 24px - Section title */
--text-h3: 1.25rem;   /* 20px - Subsection */
--text-h4: 1.125rem;  /* 18px - Card title */
--text-h5: 1rem;      /* 16px - Small heading */
--text-h6: 0.875rem;  /* 14px - Tiny heading */

/* Body - Regular content */
--text-body-lg: 1.125rem;  /* 18px - Large body */
--text-body:    1rem;      /* 16px - Standard body */
--text-body-sm: 0.875rem;  /* 14px - Small body */

/* Caption - Labels, helper text */
--text-caption:    0.75rem;   /* 12px */
--text-overline:   0.625rem;  /* 10px */
```

### Font Weights

```css
--font-weight-light:   300;
--font-weight-regular: 400;
--font-weight-medium:  500;
--font-weight-semibold: 600;
--font-weight-bold:    700;
```

### Line Heights

```css
--line-height-tight:  1.25;  /* Headings */
--line-height-normal: 1.5;   /* Body text */
--line-height-relaxed: 1.75; /* Long-form content */
```

### Usage Guidelines

**Page Titles (H1):**
```razor
<MudText Typo="Typo.h2" Color="Color.Primary" Style="font-weight: 600;">
    Gestión de Productos
</MudText>
```

**Section Titles (H2):**
```razor
<MudText Typo="Typo.h4" Style="font-weight: 600; margin-bottom: 16px;">
    Información General
</MudText>
```

**Body Text:**
```razor
<MudText Typo="Typo.body1" Color="Color.Secondary">
    Complete los campos requeridos para continuar.
</MudText>
```

**Labels:**
```razor
<MudText Typo="Typo.body2" Style="font-weight: 500; margin-bottom: 8px;">
    Nombre del Producto
</MudText>
```

---

## Spacing System

### Scale (8px Base)

```css
/* Space Scale - Powers of 2 for consistency */
--space-0:  0;
--space-1:  0.25rem;  /* 4px  - Tight gaps */
--space-2:  0.5rem;   /* 8px  - Base unit */
--space-3:  0.75rem;  /* 12px - Small gaps */
--space-4:  1rem;     /* 16px - Default spacing */
--space-5:  1.25rem;  /* 20px - Medium gaps */
--space-6:  1.5rem;   /* 24px - Large gaps */
--space-8:  2rem;     /* 32px - Section spacing */
--space-10: 2.5rem;   /* 40px - Major sections */
--space-12: 3rem;     /* 48px - Page sections */
--space-16: 4rem;     /* 64px - Hero spacing */
--space-20: 5rem;     /* 80px - Large spacing */
```

### Component Spacing

```css
/* Form Elements */
--form-field-gap:     var(--space-4);  /* 16px between fields */
--form-section-gap:   var(--space-8);  /* 32px between sections */
--form-label-margin:  var(--space-2);  /* 8px below label */

/* Cards */
--card-padding:       var(--space-6);  /* 24px */
--card-gap:           var(--space-4);  /* 16px between cards */

/* Layout */
--page-padding:       var(--space-6);  /* 24px */
--section-gap:        var(--space-10); /* 40px between sections */
```

### Usage Guidelines

**Form Fields:**
```razor
<MudStack Spacing="4">  <!-- 16px gaps -->
    <MudTextField ... />
    <MudTextField ... />
    <MudTextField ... />
</MudStack>
```

**Card Content:**
```razor
<MudCard Style="padding: 24px;">
    <MudStack Spacing="3">  <!-- 12px gaps -->
        <MudText Typo="Typo.h5">Título</MudText>
        <MudText>Contenido</MudText>
    </MudStack>
</MudCard>
```

---

## Component Patterns

### Button Hierarchy

#### Primary Actions
```razor
<!-- Main CTA - Terracotta -->
<MudButton Variant="Variant.Filled"
           Color="Color.Primary"
           Size="Size.Large"
           StartIcon="@Icons.Material.Filled.Save"
           Style="font-weight: 600; text-transform: none;">
    Guardar Cambios
</MudButton>
```

#### Secondary Actions
```razor
<!-- Less important actions - Outlined -->
<MudButton Variant="Variant.Outlined"
           Color="Color.Secondary"
           Size="Size.Medium"
           Style="text-transform: none;">
    Cancelar
</MudButton>
```

#### Tertiary Actions
```razor
<!-- Subtle actions - Text only -->
<MudButton Variant="Variant.Text"
           Color="Color.Secondary"
           Size="Size.Small"
           Style="text-transform: none;">
    Ver Detalles
</MudButton>
```

#### Destructive Actions
```razor
<!-- Delete, Remove - Red -->
<MudButton Variant="Variant.Filled"
           Color="Color.Error"
           StartIcon="@Icons.Material.Filled.Delete"
           Style="font-weight: 600; text-transform: none;">
    Eliminar Producto
</MudButton>
```

### Form Patterns

#### Standard Form Field
```razor
<MudTextField Label="Nombre del Producto"
              @bind-Value="model.Name"
              Variant="Variant.Outlined"
              Margin="Margin.Dense"
              Required="true"
              RequiredError="Este campo es requerido"
              HelperText="Ingrese el nombre completo del producto"
              Style="margin-bottom: 16px;" />
```

#### Number Input (Currency)
```razor
<MudNumericField Label="Precio de Venta"
                 @bind-Value="model.SalePrice"
                 Variant="Variant.Outlined"
                 Margin="Margin.Dense"
                 Format="C2"
                 Culture="@(new CultureInfo("es-MX"))"
                 Adornment="Adornment.Start"
                 AdornmentIcon="@Icons.Material.Filled.AttachMoney"
                 Min="0"
                 Required="true"
                 Style="margin-bottom: 16px;" />
```

#### Date Picker (Mexican Format)
```razor
<MudDatePicker Label="Fecha de Venta"
               @bind-Date="model.SaleDate"
               Variant="Variant.Outlined"
               Margin="Margin.Dense"
               Culture="@(new CultureInfo("es-MX"))"
               DateFormat="dd/MM/yyyy"
               Required="true"
               Style="margin-bottom: 16px;" />
```

#### Select Dropdown
```razor
<MudSelect Label="Categoría"
           @bind-Value="model.CategoryId"
           Variant="Variant.Outlined"
           Margin="Margin.Dense"
           Required="true"
           AnchorOrigin="Origin.BottomCenter"
           Style="margin-bottom: 16px;">
    @foreach (var category in categories)
    {
        <MudSelectItem Value="@category.Id">@category.Name</MudSelectItem>
    }
</MudSelect>
```

### Card Patterns

#### Standard Content Card
```razor
<MudCard Elevation="2" Style="border-radius: 12px; overflow: hidden;">
    <MudCardHeader Style="background: var(--color-primary-50); border-bottom: 2px solid var(--color-primary-500);">
        <CardHeaderContent>
            <MudText Typo="Typo.h5" Style="font-weight: 600; color: var(--color-primary-700);">
                Información del Producto
            </MudText>
        </CardHeaderContent>
        <CardHeaderActions>
            <MudIconButton Icon="@Icons.Material.Filled.MoreVert" Color="Color.Default" />
        </CardHeaderActions>
    </MudCardHeader>
    <MudCardContent Style="padding: 24px;">
        <MudStack Spacing="3">
            <!-- Content here -->
        </MudStack>
    </MudCardContent>
</MudCard>
```

#### Stats Card (Dashboard)
```razor
<MudCard Elevation="1" Style="border-radius: 12px; border-left: 4px solid var(--color-success-main);">
    <MudCardContent Style="padding: 20px;">
        <MudStack Row="true" AlignItems="AlignItems.Center" Justify="Justify.SpaceBetween">
            <div>
                <MudText Typo="Typo.body2" Color="Color.Secondary" Style="margin-bottom: 4px;">
                    Ventas del Mes
                </MudText>
                <MudText Typo="Typo.h4" Style="font-weight: 700; color: var(--color-success-main);">
                    $45,230 MXN
                </MudText>
            </div>
            <MudIcon Icon="@Icons.Material.Filled.TrendingUp"
                     Size="Size.Large"
                     Color="Color.Success" />
        </MudStack>
    </MudCardContent>
</MudCard>
```

### Table Patterns

#### Data Table with Actions
```razor
<MudTable Items="@products"
          Hover="true"
          Breakpoint="Breakpoint.Sm"
          Elevation="0"
          Style="border-radius: 12px; overflow: hidden;">
    <HeaderContent>
        <MudTh Style="font-weight: 600; background: var(--color-secondary-100);">SKU</MudTh>
        <MudTh Style="font-weight: 600; background: var(--color-secondary-100);">Nombre</MudTh>
        <MudTh Style="font-weight: 600; background: var(--color-secondary-100);">Precio</MudTh>
        <MudTh Style="font-weight: 600; background: var(--color-secondary-100);">Stock</MudTh>
        <MudTh Style="font-weight: 600; background: var(--color-secondary-100);">Acciones</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd DataLabel="SKU">
            <MudChip Size="Size.Small" Color="Color.Default" Style="font-family: var(--font-mono);">
                @context.Sku
            </MudChip>
        </MudTd>
        <MudTd DataLabel="Nombre">@context.Name</MudTd>
        <MudTd DataLabel="Precio">
            <strong>@context.SalePrice.ToString("C2", new CultureInfo("es-MX"))</strong>
        </MudTd>
        <MudTd DataLabel="Stock">
            <MudChip Size="Size.Small"
                     Color="@(context.Stock > 10 ? Color.Success : Color.Warning)">
                @context.Stock
            </MudChip>
        </MudTd>
        <MudTd>
            <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" />
            <MudIconButton Icon="@Icons.Material.Filled.Delete" Size="Size.Small" Color="Color.Error" />
        </MudTd>
    </RowTemplate>
</MudTable>
```

### Alert & Notification Patterns

#### Success Notification
```razor
<MudAlert Severity="Severity.Success"
          Variant="Variant.Filled"
          Icon="@Icons.Material.Filled.CheckCircle"
          Style="border-radius: 8px; margin-bottom: 16px;">
    El producto se guardó correctamente.
</MudAlert>
```

#### Error Notification
```razor
<MudAlert Severity="Severity.Error"
          Variant="Variant.Filled"
          Icon="@Icons.Material.Filled.Error"
          Style="border-radius: 8px; margin-bottom: 16px;">
    Error al guardar. Verifique los campos e intente nuevamente.
</MudAlert>
```

#### Inline Validation Error
```razor
<MudTextField ...>
    <ValidationMessage For="() => model.Name"
                      Style="color: var(--color-error-main); font-size: 0.75rem; margin-top: 4px;" />
</MudTextField>
```

---

## Layout Patterns

### Main Application Layout

```razor
<!-- MainLayout.razor -->
<MudThemeProvider @bind-IsDarkMode="@_isDarkMode" Theme="_theme"/>
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <!-- App Bar -->
    <MudAppBar Elevation="1" Dense="false" Style="background: white; color: var(--text-primary); border-bottom: 1px solid var(--color-secondary-200);">
        <MudIconButton Icon="@Icons.Material.Filled.Menu"
                       Color="Color.Inherit"
                       Edge="Edge.Start"
                       OnClick="@ToggleDrawer" />

        <MudImage Src="images/corelio-logo.svg" Height="32" Alt="Corelio ERP" Style="margin-right: 12px;" />
        <MudText Typo="Typo.h6" Style="font-weight: 600;">Corelio ERP</MudText>

        <MudSpacer />

        <!-- Tenant Display -->
        <TenantDisplay />

        <MudSpacer />

        <!-- User Display -->
        <UserDisplay />
    </MudAppBar>

    <!-- Drawer (Sidebar Navigation) -->
    <MudDrawer @bind-Open="_drawerOpen"
               Elevation="2"
               Variant="@DrawerVariant.Responsive"
               Style="background: var(--color-secondary-50);">
        <MudDrawerHeader Style="background: var(--color-primary-500); color: white; padding: 24px;">
            <MudText Typo="Typo.h6" Style="font-weight: 600;">Menú Principal</MudText>
        </MudDrawerHeader>
        <NavMenu />
    </MudDrawer>

    <!-- Main Content -->
    <MudMainContent Style="background: var(--bg-page); padding: 24px;">
        @Body
    </MudMainContent>
</MudLayout>
```

### Page Container Pattern

```razor
@page "/products"

<PageTitle>Productos - Corelio ERP</PageTitle>

<!-- Page Header -->
<MudStack Spacing="4" Style="margin-bottom: 32px;">
    <MudBreadcrumbs Items="_breadcrumbs" Separator=">" Style="padding: 0;" />

    <MudStack Row="true" AlignItems="AlignItems.Center" Justify="Justify.SpaceBetween">
        <div>
            <MudText Typo="Typo.h4" Style="font-weight: 600; margin-bottom: 8px;">
                Gestión de Productos
            </MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary">
                Administre el catálogo de productos de su ferretería
            </MudText>
        </div>

        <MudButton Variant="Variant.Filled"
                   Color="Color.Primary"
                   StartIcon="@Icons.Material.Filled.Add"
                   Size="Size.Large"
                   Style="font-weight: 600; text-transform: none;">
            Nuevo Producto
        </MudButton>
    </MudStack>
</MudStack>

<!-- Filters/Search Bar -->
<MudCard Elevation="1" Style="margin-bottom: 24px; border-radius: 12px;">
    <MudCardContent Style="padding: 16px;">
        <MudStack Row="true" Spacing="3" AlignItems="AlignItems.End">
            <MudTextField Placeholder="Buscar productos..."
                         Variant="Variant.Outlined"
                         Margin="Margin.Dense"
                         Adornment="Adornment.Start"
                         AdornmentIcon="@Icons.Material.Filled.Search"
                         Style="flex: 1;" />

            <MudSelect Label="Categoría"
                      Variant="Variant.Outlined"
                      Margin="Margin.Dense"
                      Style="min-width: 200px;">
                <MudSelectItem Value="0">Todas</MudSelectItem>
            </MudSelect>

            <MudButton Variant="Variant.Outlined"
                      Color="Color.Secondary"
                      Style="text-transform: none;">
                Filtrar
            </MudButton>
        </MudStack>
    </MudCardContent>
</MudCard>

<!-- Content -->
<MudCard Elevation="2" Style="border-radius: 12px;">
    <MudCardContent Style="padding: 0;">
        <!-- Table or Grid here -->
    </MudCardContent>
</MudCard>
```

---

## Authentication Pages Design

### Login Page

```razor
@page "/auth/login"
@layout AuthLayout

<PageTitle>Iniciar Sesión - Corelio ERP</PageTitle>

<div style="min-height: 100vh; display: flex; align-items: center; justify-content: center; background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);">
    <MudContainer MaxWidth="MaxWidth.Small">
        <MudPaper Elevation="8" Style="padding: 48px; border-radius: 16px; background: white;">

            <!-- Logo & Title -->
            <MudStack Spacing="3" AlignItems="AlignItems.Center" Style="margin-bottom: 32px;">
                <MudImage Src="images/corelio-logo.svg" Height="64" Alt="Corelio ERP" />
                <MudText Typo="Typo.h4" Style="font-weight: 700; color: var(--color-primary-600);">
                    Corelio ERP
                </MudText>
                <MudText Typo="Typo.body1" Color="Color.Secondary" Align="Align.Center">
                    Sistema de Gestión para Ferreterías
                </MudText>
            </MudStack>

            <!-- Error Alert -->
            @if (!string.IsNullOrEmpty(_errorMessage))
            {
                <MudAlert Severity="Severity.Error"
                         Variant="Variant.Filled"
                         Style="border-radius: 8px; margin-bottom: 24px;">
                    @_errorMessage
                </MudAlert>
            }

            <!-- Login Form -->
            <EditForm Model="@_model" OnValidSubmit="HandleLogin">
                <DataAnnotationsValidator />

                <MudStack Spacing="4">
                    <MudTextField Label="Correo Electrónico"
                                 @bind-Value="_model.Email"
                                 Variant="Variant.Outlined"
                                 Margin="Margin.Dense"
                                 InputType="InputType.Email"
                                 Adornment="Adornment.Start"
                                 AdornmentIcon="@Icons.Material.Filled.Email"
                                 Required="true"
                                 RequiredError="El correo es requerido"
                                 HelperText="ejemplo@ferreteria.mx" />

                    <MudTextField Label="Contraseña"
                                 @bind-Value="_model.Password"
                                 Variant="Variant.Outlined"
                                 Margin="Margin.Dense"
                                 InputType="_passwordInput"
                                 Adornment="Adornment.End"
                                 AdornmentIcon="@(_showPassword ? Icons.Material.Filled.VisibilityOff : Icons.Material.Filled.Visibility)"
                                 OnAdornmentClick="TogglePasswordVisibility"
                                 Required="true"
                                 RequiredError="La contraseña es requerida" />

                    <MudButton Variant="Variant.Filled"
                              Color="Color.Primary"
                              Size="Size.Large"
                              FullWidth="true"
                              ButtonType="ButtonType.Submit"
                              Disabled="@_isLoading"
                              Style="font-weight: 600; text-transform: none; height: 48px;">
                        @if (_isLoading)
                        {
                            <MudProgressCircular Size="Size.Small" Indeterminate="true" />
                            <span style="margin-left: 12px;">Iniciando sesión...</span>
                        }
                        else
                        {
                            <span>Iniciar Sesión</span>
                        }
                    </MudButton>
                </MudStack>
            </EditForm>

            <!-- Footer Links -->
            <MudStack Spacing="2" AlignItems="AlignItems.Center" Style="margin-top: 24px;">
                <MudLink Href="/auth/forgot-password"
                        Color="Color.Primary"
                        Style="font-weight: 500;">
                    ¿Olvidaste tu contraseña?
                </MudLink>

                <MudDivider Style="width: 100%; margin: 16px 0;" />

                <MudText Typo="Typo.body2" Color="Color.Secondary" Align="Align.Center">
                    ¿Necesitas ayuda? Contacta a soporte
                </MudText>
            </MudStack>
        </MudPaper>

        <!-- Footer -->
        <MudText Typo="Typo.caption"
                Color="Color.Secondary"
                Align="Align.Center"
                Style="margin-top: 24px;">
            © 2026 Corelio ERP. Todos los derechos reservados.
        </MudText>
    </MudContainer>
</div>
```

### Register Page (Admin Only)

```razor
@page "/auth/register"
@attribute [Authorize(Policy = "users.create")]

<PageTitle>Registrar Usuario - Corelio ERP</PageTitle>

<MudContainer MaxWidth="MaxWidth.Medium" Style="padding: 24px;">

    <!-- Page Header -->
    <MudStack Spacing="4" Style="margin-bottom: 32px;">
        <MudBreadcrumbs Items="_breadcrumbs" Separator=">" />

        <div>
            <MudText Typo="Typo.h4" Style="font-weight: 600; margin-bottom: 8px;">
                Registrar Nuevo Usuario
            </MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary">
                Complete el formulario para crear una nueva cuenta de usuario
            </MudText>
        </div>
    </MudStack>

    <!-- Form Card -->
    <MudCard Elevation="2" Style="border-radius: 12px;">
        <MudCardContent Style="padding: 32px;">
            <EditForm Model="@_model" OnValidSubmit="HandleRegister">
                <DataAnnotationsValidator />

                <!-- Personal Information Section -->
                <MudText Typo="Typo.h6" Style="font-weight: 600; margin-bottom: 16px; color: var(--color-primary-600);">
                    Información Personal
                </MudText>

                <MudGrid Spacing="3" Style="margin-bottom: 32px;">
                    <MudItem xs="12" sm="6">
                        <MudTextField Label="Nombre"
                                     @bind-Value="_model.FirstName"
                                     Variant="Variant.Outlined"
                                     Required="true" />
                    </MudItem>
                    <MudItem xs="12" sm="6">
                        <MudTextField Label="Apellido"
                                     @bind-Value="_model.LastName"
                                     Variant="Variant.Outlined"
                                     Required="true" />
                    </MudItem>
                    <MudItem xs="12">
                        <MudTextField Label="Correo Electrónico"
                                     @bind-Value="_model.Email"
                                     Variant="Variant.Outlined"
                                     InputType="InputType.Email"
                                     Required="true" />
                    </MudItem>
                </MudGrid>

                <!-- Security Section -->
                <MudText Typo="Typo.h6" Style="font-weight: 600; margin-bottom: 16px; color: var(--color-primary-600);">
                    Seguridad
                </MudText>

                <MudGrid Spacing="3" Style="margin-bottom: 32px;">
                    <MudItem xs="12" sm="6">
                        <MudTextField Label="Contraseña"
                                     @bind-Value="_model.Password"
                                     Variant="Variant.Outlined"
                                     InputType="InputType.Password"
                                     Required="true"
                                     HelperText="Mínimo 8 caracteres" />
                    </MudItem>
                    <MudItem xs="12" sm="6">
                        <MudTextField Label="Confirmar Contraseña"
                                     @bind-Value="_model.ConfirmPassword"
                                     Variant="Variant.Outlined"
                                     InputType="InputType.Password"
                                     Required="true" />
                    </MudItem>
                </MudGrid>

                <!-- Roles Section -->
                <MudText Typo="Typo.h6" Style="font-weight: 600; margin-bottom: 16px; color: var(--color-primary-600);">
                    Roles y Permisos
                </MudText>

                <MudSelect Label="Rol del Usuario"
                          @bind-Value="_model.RoleId"
                          Variant="Variant.Outlined"
                          Required="true"
                          Style="margin-bottom: 32px;">
                    <MudSelectItem Value="@Guid.Empty" Disabled="true">Seleccione un rol</MudSelectItem>
                    @foreach (var role in _roles)
                    {
                        <MudSelectItem Value="@role.Id">@role.Name</MudSelectItem>
                    }
                </MudSelect>

                <!-- Actions -->
                <MudStack Row="true" Justify="Justify.FlexEnd" Spacing="3">
                    <MudButton Variant="Variant.Outlined"
                              Color="Color.Secondary"
                              Size="Size.Large"
                              OnClick="NavigateBack"
                              Style="text-transform: none;">
                        Cancelar
                    </MudButton>

                    <MudButton Variant="Variant.Filled"
                              Color="Color.Primary"
                              Size="Size.Large"
                              StartIcon="@Icons.Material.Filled.PersonAdd"
                              ButtonType="ButtonType.Submit"
                              Disabled="@_isLoading"
                              Style="font-weight: 600; text-transform: none;">
                        @if (_isLoading)
                        {
                            <MudProgressCircular Size="Size.Small" Indeterminate="true" />
                            <span style="margin-left: 8px;">Registrando...</span>
                        }
                        else
                        {
                            <span>Registrar Usuario</span>
                        }
                    </MudButton>
                </MudStack>
            </EditForm>
        </MudCardContent>
    </MudCard>
</MudContainer>
```

---

## Multi-Tenant Components

### TenantDisplay Component

```razor
<!-- Components/Shared/TenantDisplay.razor -->
@inject CustomAuthenticationStateProvider AuthStateProvider
@inject ITenantService TenantService
@inject IStringLocalizer<SharedResource> L

<AuthorizeView>
    <Authorized>
        @if (_isLoading)
        {
            <MudChip Size="Size.Medium"
                    Color="Color.Default"
                    Style="background: var(--bg-overlay-light);">
                <MudProgressCircular Size="Size.Small" Indeterminate="true" />
                <span style="margin-left: 8px;">@L["Loading"]</span>
            </MudChip>
        }
        else if (!string.IsNullOrEmpty(_tenantName))
        {
            <MudPaper Elevation="0"
                     Style="padding: 8px 16px; border-radius: 20px; background: var(--color-primary-100); border: 2px solid var(--color-primary-500);">
                <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
                    <MudIcon Icon="@Icons.Material.Filled.Store"
                            Size="Size.Small"
                            Style="color: var(--color-primary-700);" />
                    <MudText Typo="Typo.body2"
                            Style="font-weight: 600; color: var(--color-primary-700);">
                        @_tenantName
                    </MudText>
                </MudStack>
            </MudPaper>
        }
        else
        {
            <MudChip Size="Size.Medium"
                    Color="Color.Error"
                    Icon="@Icons.Material.Filled.ErrorOutline">
                @L["UnknownTenant"]
            </MudChip>
        }
    </Authorized>
</AuthorizeView>
```

### UserDisplay Component

```razor
<!-- Components/Shared/UserDisplay.razor -->
@inject CustomAuthenticationStateProvider AuthStateProvider
@inject NavigationManager Navigation
@inject IStringLocalizer<SharedResource> L

<AuthorizeView>
    <Authorized>
        <MudMenu AnchorOrigin="Origin.BottomRight"
                TransformOrigin="Origin.TopRight"
                Dense="false">
            <ActivatorContent>
                <MudPaper Elevation="0"
                         Style="padding: 8px 16px; border-radius: 24px; background: var(--color-secondary-100); cursor: pointer; transition: all 0.2s;"
                         @onmouseenter="@(() => _isHovering = true)"
                         @onmouseleave="@(() => _isHovering = false)"
                         Class="@(_isHovering ? "hover-elevation" : "")">
                    <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
                        <MudAvatar Color="Color.Primary"
                                  Size="Size.Small"
                                  Style="font-weight: 600;">
                            @GetInitials(context.User)
                        </MudAvatar>
                        <div>
                            <MudText Typo="Typo.body2" Style="font-weight: 600; line-height: 1.2;">
                                @GetDisplayName(context.User)
                            </MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary" Style="line-height: 1;">
                                @GetEmail(context.User)
                            </MudText>
                        </div>
                        <MudIcon Icon="@Icons.Material.Filled.KeyboardArrowDown" Size="Size.Small" />
                    </MudStack>
                </MudPaper>
            </ActivatorContent>
            <ChildContent>
                <MudMenuItem Icon="@Icons.Material.Filled.Person"
                            OnClick="NavigateToProfile">
                    Mi Perfil
                </MudMenuItem>
                <MudMenuItem Icon="@Icons.Material.Filled.Settings"
                            OnClick="NavigateToSettings">
                    Configuración
                </MudMenuItem>
                <MudDivider Style="margin: 4px 0;" />
                <MudMenuItem Icon="@Icons.Material.Filled.Logout"
                            OnClick="HandleLogout"
                            Style="color: var(--color-error-main);">
                    Cerrar Sesión
                </MudMenuItem>
            </ChildContent>
        </MudMenu>
    </Authorized>
</AuthorizeView>

<style>
    .hover-elevation {
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.12);
        background: var(--color-secondary-200) !important;
    }
</style>
```

---

## MudBlazor Theme Configuration

### Custom Theme Definition

```csharp
// Program.cs or ThemeService.cs

private MudTheme _theme = new()
{
    Palette = new PaletteLight
    {
        Primary = "#e65946",        // Terracotta
        Secondary = "#495057",      // Concrete Gray
        Tertiary = "#2a8a8f",       // Tool Steel

        AppbarBackground = "#ffffff",
        AppbarText = "#212529",

        DrawerBackground = "#f8f9fa",
        DrawerText = "#212529",

        Success = "#2e7d32",        // Wood Green
        Warning = "#ed6c02",        // Safety Orange
        Error = "#d32f2f",          // Alert Red
        Info = "#0288d1",           // Sky Blue

        Background = "#f8f9fa",     // Light concrete
        Surface = "#ffffff",

        TextPrimary = "#212529",
        TextSecondary = "#495057",
        TextDisabled = "#adb5bd",

        DividerLight = "#dee2e6",
        Divider = "#ced4da",

        LinesDefault = "#e9ecef",
        LinesInputs = "#ced4da",

        // Hover states
        HoverOpacity = 0.08,

        // Borders
        TableLines = "#e9ecef",
        TableStriped = "#f8f9fa"
    },

    Typography = new Typography
    {
        Default = new Default
        {
            FontFamily = new[] { "Inter", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "sans-serif" },
            FontSize = "1rem",
            FontWeight = 400,
            LineHeight = 1.5,
            LetterSpacing = "normal"
        },

        H1 = new H1
        {
            FontSize = "2rem",
            FontWeight = 600,
            LineHeight = 1.25,
            LetterSpacing = "-0.01em"
        },

        H2 = new H2
        {
            FontSize = "1.75rem",
            FontWeight = 600,
            LineHeight = 1.3
        },

        H3 = new H3
        {
            FontSize = "1.5rem",
            FontWeight = 600,
            LineHeight = 1.3
        },

        H4 = new H4
        {
            FontSize = "1.25rem",
            FontWeight = 600,
            LineHeight = 1.4
        },

        H5 = new H5
        {
            FontSize = "1.125rem",
            FontWeight = 600,
            LineHeight = 1.4
        },

        H6 = new H6
        {
            FontSize = "1rem",
            FontWeight = 600,
            LineHeight = 1.5
        },

        Button = new Button
        {
            FontSize = "0.875rem",
            FontWeight = 600,
            TextTransform = "none",  // No uppercase
            LetterSpacing = "0.02em"
        },

        Body1 = new Body1
        {
            FontSize = "1rem",
            LineHeight = 1.5
        },

        Body2 = new Body2
        {
            FontSize = "0.875rem",
            LineHeight = 1.5
        },

        Caption = new Caption
        {
            FontSize = "0.75rem",
            LineHeight = 1.4
        }
    },

    Shadows = new Shadow
    {
        // Subtle, modern shadows
        Elevation = new[]
        {
            "none",
            "0 1px 2px rgba(0, 0, 0, 0.05)",
            "0 2px 4px rgba(0, 0, 0, 0.08)",
            "0 4px 8px rgba(0, 0, 0, 0.1)",
            "0 8px 16px rgba(0, 0, 0, 0.12)",
            "0 12px 24px rgba(0, 0, 0, 0.14)"
        }
    },

    LayoutProperties = new LayoutProperties
    {
        DefaultBorderRadius = "8px",
        AppbarHeight = "64px",
        DrawerWidthLeft = "260px",
        DrawerWidthRight = "260px"
    }
};
```

---

## Responsive Design Guidelines

### Breakpoints

```css
/* Mobile First Approach */
--breakpoint-xs: 0;      /* < 600px  - Mobile portrait */
--breakpoint-sm: 600px;  /* 600-960px - Mobile landscape, small tablets */
--breakpoint-md: 960px;  /* 960-1280px - Tablets, small desktops */
--breakpoint-lg: 1280px; /* 1280-1920px - Desktops */
--breakpoint-xl: 1920px; /* > 1920px - Large desktops */
```

### Responsive Patterns

#### Mobile-First Grid
```razor
<MudGrid Spacing="3">
    <!-- Full width on mobile, half on tablet, quarter on desktop -->
    <MudItem xs="12" sm="6" md="4" lg="3">
        <MudCard><!-- Content --></MudCard>
    </MudItem>
</MudGrid>
```

#### Responsive Table
```razor
<MudTable Items="@items"
          Breakpoint="Breakpoint.Sm"
          Dense="@_isMobile"
          Hover="true">
    <!-- On mobile, table converts to cards -->
</MudTable>
```

#### Conditional Rendering
```razor
<!-- Desktop: Full user name -->
<MudHidden Breakpoint="Breakpoint.SmAndDown">
    <MudText>Juan López Rodríguez</MudText>
</MudHidden>

<!-- Mobile: Avatar only -->
<MudHidden Breakpoint="Breakpoint.MdAndUp">
    <MudAvatar Size="Size.Small">JL</MudAvatar>
</MudHidden>
```

---

## Accessibility Guidelines

### Keyboard Navigation
- All interactive elements must be keyboard accessible
- Use proper tab order with `tabindex`
- Visible focus indicators

### ARIA Labels
```razor
<MudIconButton Icon="@Icons.Material.Filled.Delete"
              Color="Color.Error"
              aria-label="Eliminar producto"
              OnClick="@HandleDelete" />
```

### Color Contrast
- Text: Minimum 4.5:1 contrast ratio (WCAG AA)
- UI components: Minimum 3:1 contrast ratio
- Test with Chrome DevTools Lighthouse

### Screen Reader Support
```razor
<MudAlert Severity="Severity.Success"
         role="alert"
         aria-live="polite">
    Producto guardado correctamente
</MudAlert>
```

---

## Implementation Checklist

### Phase 1: Foundation Setup
- [ ] Install MudBlazor package
- [ ] Configure custom theme in Program.cs
- [ ] Create CSS variables file (wwwroot/css/variables.css)
- [ ] Import Inter font from Google Fonts
- [ ] Set up Spanish localization (IStringLocalizer)

### Phase 2: Layout Components
- [ ] Update MainLayout.razor with new design
- [ ] Create TenantDisplay.razor
- [ ] Create UserDisplay.razor
- [ ] Update NavMenu.razor with new styling
- [ ] Test responsive behavior on mobile/tablet/desktop

### Phase 3: Authentication Pages
- [ ] Create AuthLayout.razor (minimal layout for auth pages)
- [ ] Implement Login.razor with new design
- [ ] Implement Register.razor with new design
- [ ] Implement ForgotPassword.razor
- [ ] Implement ResetPassword.razor
- [ ] Test all authentication flows

### Phase 4: Common Components
- [ ] Create reusable form field wrappers
- [ ] Create reusable card patterns
- [ ] Create reusable table patterns
- [ ] Create loading states components
- [ ] Create empty state components

### Phase 5: Testing & Refinement
- [ ] Test color contrast ratios
- [ ] Test keyboard navigation
- [ ] Test screen reader compatibility
- [ ] Test on real devices (mobile, tablet)
- [ ] Gather user feedback
- [ ] Refine based on feedback

---

## Resources

### Design Tools
- **Figma:** For mockups and prototypes
- **Coolors:** Color palette generator
- **Google Fonts:** Inter font family
- **Heroicons/Material Icons:** Icon library

### MudBlazor Documentation
- **Official Docs:** https://mudblazor.com/
- **Component Library:** https://mudblazor.com/components/
- **Theming Guide:** https://mudblazor.com/customization/theming

### Accessibility
- **WCAG 2.1 Guidelines:** https://www.w3.org/WAI/WCAG21/quickref/
- **ARIA Practices:** https://www.w3.org/WAI/ARIA/apg/
- **Contrast Checker:** https://webaim.org/resources/contrastchecker/

---

## FAQ

**Q: Why not use the default MudBlazor theme?**
A: Default themes look generic and "AI-generated". Custom theming creates a distinctive brand identity that resonates with hardware store owners.

**Q: Why terracotta as the primary color?**
A: Terracotta connects to construction materials (bricks, clay tiles) common in Mexican hardware stores. It's warm, trustworthy, and professional without being corporate.

**Q: Should we support dark mode?**
A: Not in MVP. Hardware stores typically operate in well-lit environments. Dark mode can be added later based on user feedback.

**Q: How do we handle very long tenant names?**
A: Use text truncation with ellipsis and a tooltip showing the full name:
```razor
<MudTooltip Text="@fullTenantName">
    <MudText Style="max-width: 200px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
        @tenantName
    </MudText>
</MudTooltip>
```

**Q: What about animations?**
A: Use subtle, purposeful animations:
- Button hover: 200ms ease
- Card elevation change: 200ms ease
- Page transitions: 300ms ease
- Loading spinners: Smooth, indeterminate
- AVOID: Flashy, distracting animations

---

**Document Status:** ✅ Ready for Implementation
**Next Steps:** Begin Phase 1 setup with US-2.2.1 implementation
**Owner:** Frontend Team
**Review Date:** 2026-01-20
