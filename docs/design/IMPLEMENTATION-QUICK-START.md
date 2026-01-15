# Corelio Design System - Quick Start Guide
## Get Started in 30 Minutes

**Last Updated:** 2026-01-13
**Target:** Frontend Developers implementing US-2.2.1 and US-2.1.1

---

## Prerequisites

```bash
# Ensure you're in the Blazor project directory
cd src/Presentation/Corelio.BlazorApp

# Verify MudBlazor is installed (should already be present)
dotnet list package | grep MudBlazor
```

---

## Step 1: Install Required Packages (5 min)

```bash
# Install additional required packages
dotnet add package Blazored.LocalStorage --version 4.5.0
dotnet add package Microsoft.Extensions.Localization --version 10.0.0

# MudBlazor should already be installed, but if not:
dotnet add package MudBlazor --version 8.0.0
```

---

## Step 2: Configure MudBlazor Theme (10 min)

### Create Custom Theme

Create `Services/ThemeConfiguration.cs`:

```csharp
using MudBlazor;

namespace Corelio.BlazorApp.Services;

public static class ThemeConfiguration
{
    public static MudTheme DefaultTheme => new()
    {
        Palette = new PaletteLight
        {
            // Terracotta Primary
            Primary = "#e65946",
            PrimaryDarken = "#d23b28",
            PrimaryLighten = "#f18375",

            // Concrete Gray Secondary
            Secondary = "#495057",
            SecondaryDarken = "#343a40",
            SecondaryLighten = "#868e96",

            // Tool Steel Accent
            Tertiary = "#2a8a8f",

            // Semantic Colors
            Success = "#2e7d32",
            Warning = "#ed6c02",
            Error = "#d32f2f",
            Info = "#0288d1",

            // Backgrounds
            AppbarBackground = "#ffffff",
            AppbarText = "#212529",
            DrawerBackground = "#f8f9fa",
            DrawerText = "#212529",
            Background = "#f8f9fa",
            Surface = "#ffffff",

            // Text
            TextPrimary = "#212529",
            TextSecondary = "#495057",
            TextDisabled = "#adb5bd",

            // Lines & Dividers
            DividerLight = "#dee2e6",
            Divider = "#ced4da",
            LinesDefault = "#e9ecef",
            LinesInputs = "#ced4da",
            TableLines = "#e9ecef",
            TableStriped = "#f8f9fa",

            HoverOpacity = 0.08
        },

        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = ["Inter", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "sans-serif"],
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.5
            },
            H1 = new H1 { FontSize = "2rem", FontWeight = 600, LineHeight = 1.25 },
            H2 = new H2 { FontSize = "1.75rem", FontWeight = 600, LineHeight = 1.3 },
            H3 = new H3 { FontSize = "1.5rem", FontWeight = 600, LineHeight = 1.3 },
            H4 = new H4 { FontSize = "1.25rem", FontWeight = 600, LineHeight = 1.4 },
            H5 = new H5 { FontSize = "1.125rem", FontWeight = 600, LineHeight = 1.4 },
            H6 = new H6 { FontSize = "1rem", FontWeight = 600, LineHeight = 1.5 },
            Button = new Button { FontSize = "0.875rem", FontWeight = 600, TextTransform = "none" },
            Body1 = new Body1 { FontSize = "1rem", LineHeight = 1.5 },
            Body2 = new Body2 { FontSize = "0.875rem", LineHeight = 1.5 },
            Caption = new Caption { FontSize = "0.75rem", LineHeight = 1.4 }
        },

        Shadows = new Shadow
        {
            Elevation =
            [
                "none",
                "0 1px 2px rgba(0, 0, 0, 0.05)",
                "0 2px 4px rgba(0, 0, 0, 0.08)",
                "0 4px 8px rgba(0, 0, 0, 0.1)",
                "0 8px 16px rgba(0, 0, 0, 0.12)",
                "0 12px 24px rgba(0, 0, 0, 0.14)"
            ]
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
            AppbarHeight = "64px",
            DrawerWidthLeft = "260px"
        }
    };
}
```

### Update Program.cs

```csharp
using Corelio.BlazorApp.Components;
using Corelio.BlazorApp.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor Services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.ShowTransitionDuration = 200;
    config.SnackbarConfiguration.HideTransitionDuration = 200;
});

// Add Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

// Add localization middleware
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("es-MX")
    .AddSupportedCultures(["es-MX"])
    .AddSupportedUICultures(["es-MX"]));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### Update App.razor

```razor
<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <ResourcePreloader />

    <!-- Google Fonts - Inter -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">

    <!-- MudBlazor CSS -->
    <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
    <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />

    <!-- Custom CSS -->
    <link rel="stylesheet" href="@Assets["app.css"]" />
    <link rel="stylesheet" href="@Assets["Corelio.BlazorApp.styles.css"]" />

    <ImportMap />
    <link rel="icon" type="image/png" href="favicon.png" />
    <HeadOutlet />
</head>

<body>
    <Routes />
    <ReconnectModal />

    <!-- MudBlazor JS -->
    <script src="_content/MudBlazor/MudBlazor.min.js"></script>
    <script src="@Assets["_framework/blazor.web.js"]"></script>
</body>

</html>
```

---

## Step 3: Create CSS Variables (5 min)

Create `wwwroot/css/custom.css`:

```css
:root {
    /* Primary Colors - Terracotta */
    --color-primary-50: #fef3f2;
    --color-primary-100: #fde6e4;
    --color-primary-200: #fbd2cd;
    --color-primary-300: #f7b1a9;
    --color-primary-400: #f18375;
    --color-primary-500: #e65946;
    --color-primary-600: #d23b28;
    --color-primary-700: #b1301e;
    --color-primary-800: #922b1c;
    --color-primary-900: #78291d;

    /* Secondary Colors - Concrete Gray */
    --color-secondary-50: #f8f9fa;
    --color-secondary-100: #f1f3f5;
    --color-secondary-200: #e9ecef;
    --color-secondary-300: #dee2e6;
    --color-secondary-400: #ced4da;
    --color-secondary-500: #adb5bd;
    --color-secondary-600: #868e96;
    --color-secondary-700: #495057;
    --color-secondary-800: #343a40;
    --color-secondary-900: #212529;

    /* Spacing Scale */
    --space-1: 0.25rem;
    --space-2: 0.5rem;
    --space-3: 0.75rem;
    --space-4: 1rem;
    --space-5: 1.25rem;
    --space-6: 1.5rem;
    --space-8: 2rem;
    --space-10: 2.5rem;
    --space-12: 3rem;

    /* Shadows */
    --shadow-sm: 0 1px 2px rgba(0, 0, 0, 0.05);
    --shadow-md: 0 2px 4px rgba(0, 0, 0, 0.08);
    --shadow-lg: 0 4px 8px rgba(0, 0, 0, 0.1);
    --shadow-xl: 0 8px 16px rgba(0, 0, 0, 0.12);

    /* Border Radius */
    --radius-sm: 0.25rem;
    --radius-md: 0.5rem;
    --radius-lg: 0.75rem;
    --radius-xl: 1rem;
}

/* Global Styles */
* {
    font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', sans-serif;
}

/* Custom Utility Classes */
.hover-elevation {
    transition: box-shadow 0.2s ease, transform 0.2s ease;
}

.hover-elevation:hover {
    box-shadow: var(--shadow-lg);
    transform: translateY(-2px);
}

.text-truncate {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

/* Card Enhancements */
.mud-card {
    border-radius: var(--radius-xl) !important;
}

.mud-card-header {
    border-top-left-radius: var(--radius-xl) !important;
    border-top-right-radius: var(--radius-xl) !important;
}

/* Button Enhancements */
.mud-button-root {
    text-transform: none !important;
    font-weight: 600 !important;
}

/* Table Enhancements */
.mud-table-cell {
    padding: 12px 16px !important;
}

.mud-table-head .mud-table-cell {
    font-weight: 600 !important;
    background-color: var(--color-secondary-100) !important;
}
```

Reference in `App.razor`:
```html
<link rel="stylesheet" href="css/custom.css" />
```

---

## Step 4: Update MainLayout.razor (5 min)

Replace existing `MainLayout.razor`:

```razor
@using Corelio.BlazorApp.Services
@inherits LayoutComponentBase

<MudThemeProvider Theme="@ThemeConfiguration.DefaultTheme" @bind-IsDarkMode="@_isDarkMode" />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <!-- App Bar -->
    <MudAppBar Elevation="1" Dense="false">
        <MudIconButton Icon="@Icons.Material.Filled.Menu"
                       Color="Color.Inherit"
                       Edge="Edge.Start"
                       OnClick="@ToggleDrawer" />

        <MudImage Src="images/logo.svg" Height="32" Alt="Corelio ERP" Style="margin-right: 12px;" />
        <MudText Typo="Typo.h6" Style="font-weight: 600;">Corelio ERP</MudText>

        <MudSpacer />

        <!-- Tenant Display (placeholder - will be implemented in US-2.1.1) -->
        <AuthorizeView>
            <Authorized>
                <MudPaper Elevation="0"
                         Style="padding: 8px 16px; border-radius: 20px; background: var(--color-primary-100); border: 2px solid var(--color-primary-500); margin-right: 16px;">
                    <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
                        <MudIcon Icon="@Icons.Material.Filled.Store"
                                Size="Size.Small"
                                Style="color: var(--color-primary-700);" />
                        <MudText Typo="Typo.body2"
                                Style="font-weight: 600; color: var(--color-primary-700);">
                            Mi Ferretería
                        </MudText>
                    </MudStack>
                </MudPaper>

                <!-- User Display (placeholder - will be implemented in US-2.1.1) -->
                <MudPaper Elevation="0"
                         Style="padding: 8px 16px; border-radius: 24px; background: var(--color-secondary-100); cursor: pointer;">
                    <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
                        <MudAvatar Color="Color.Primary" Size="Size.Small" Style="font-weight: 600;">
                            JL
                        </MudAvatar>
                        <div>
                            <MudText Typo="Typo.body2" Style="font-weight: 600; line-height: 1.2;">
                                Juan López
                            </MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary" Style="line-height: 1;">
                                admin@ferreteria.mx
                            </MudText>
                        </div>
                        <MudIcon Icon="@Icons.Material.Filled.KeyboardArrowDown" Size="Size.Small" />
                    </MudStack>
                </MudPaper>
            </Authorized>
            <NotAuthorized>
                <MudButton Variant="Variant.Filled"
                          Color="Color.Primary"
                          Href="/auth/login"
                          Style="font-weight: 600; text-transform: none;">
                    Iniciar Sesión
                </MudButton>
            </NotAuthorized>
        </AuthorizeView>
    </MudAppBar>

    <!-- Drawer -->
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
    <MudMainContent Style="background: var(--bg-page, #f8f9fa); padding: 24px;">
        @Body
    </MudMainContent>
</MudLayout>

@code {
    private bool _drawerOpen = true;
    private bool _isDarkMode = false;

    private void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }
}
```

---

## Step 5: Update NavMenu.razor (5 min)

```razor
<MudNavMenu>
    <MudNavLink Href="/" Match="NavLinkMatch.All"
                Icon="@Icons.Material.Filled.Home"
                Style="font-weight: 500;">
        Inicio
    </MudNavLink>

    <MudNavGroup Title="Catálogos"
                 Icon="@Icons.Material.Filled.Category"
                 Expanded="true"
                 Style="font-weight: 500;">
        <MudNavLink Href="/products"
                    Icon="@Icons.Material.Filled.Inventory">
            Productos
        </MudNavLink>
        <MudNavLink Href="/categories"
                    Icon="@Icons.Material.Filled.Label">
            Categorías
        </MudNavLink>
        <MudNavLink Href="/customers"
                    Icon="@Icons.Material.Filled.People">
            Clientes
        </MudNavLink>
    </MudNavGroup>

    <MudNavGroup Title="Ventas"
                 Icon="@Icons.Material.Filled.PointOfSale"
                 Style="font-weight: 500;">
        <MudNavLink Href="/pos"
                    Icon="@Icons.Material.Filled.ShoppingCart">
            Punto de Venta
        </MudNavLink>
        <MudNavLink Href="/sales"
                    Icon="@Icons.Material.Filled.Receipt">
            Historial de Ventas
        </MudNavLink>
    </MudNavGroup>

    <MudNavGroup Title="Inventario"
                 Icon="@Icons.Material.Filled.Warehouse"
                 Style="font-weight: 500;">
        <MudNavLink Href="/stock"
                    Icon="@Icons.Material.Filled.Inventory2">
            Control de Stock
        </MudNavLink>
        <MudNavLink Href="/stock-movements"
                    Icon="@Icons.Material.Filled.SyncAlt">
            Movimientos
        </MudNavLink>
    </MudNavGroup>

    <MudDivider Style="margin: 12px 0;" />

    <MudNavLink Href="/settings"
                Icon="@Icons.Material.Filled.Settings"
                Style="font-weight: 500;">
        Configuración
    </MudNavLink>
</MudNavMenu>
```

---

## Component Examples (Copy & Paste)

### Example 1: Login Page

Create `Components/Pages/Auth/Login.razor`:

```razor
@page "/auth/login"
@layout MinimalLayout

<PageTitle>Iniciar Sesión - Corelio ERP</PageTitle>

<div style="min-height: 100vh; display: flex; align-items: center; justify-content: center; background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);">
    <MudContainer MaxWidth="MaxWidth.Small">
        <MudPaper Elevation="8" Style="padding: 48px; border-radius: 16px; background: white;">

            <!-- Logo & Title -->
            <MudStack Spacing="3" AlignItems="AlignItems.Center" Style="margin-bottom: 32px;">
                <MudIcon Icon="@Icons.Material.Filled.Store" Size="Size.Large" Color="Color.Primary" />
                <MudText Typo="Typo.h4" Style="font-weight: 700; color: var(--color-primary-600);">
                    Corelio ERP
                </MudText>
                <MudText Typo="Typo.body1" Color="Color.Secondary" Align="Align.Center">
                    Sistema de Gestión para Ferreterías
                </MudText>
            </MudStack>

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
                                 HelperText="ejemplo@ferreteria.mx" />

                    <MudTextField Label="Contraseña"
                                 @bind-Value="_model.Password"
                                 Variant="Variant.Outlined"
                                 Margin="Margin.Dense"
                                 InputType="@(_showPassword ? InputType.Text : InputType.Password)"
                                 Adornment="Adornment.End"
                                 AdornmentIcon="@(_showPassword ? Icons.Material.Filled.VisibilityOff : Icons.Material.Filled.Visibility)"
                                 OnAdornmentClick="TogglePasswordVisibility"
                                 Required="true" />

                    <MudButton Variant="Variant.Filled"
                              Color="Color.Primary"
                              Size="Size.Large"
                              FullWidth="true"
                              ButtonType="ButtonType.Submit"
                              Style="font-weight: 600; text-transform: none; height: 48px;">
                        Iniciar Sesión
                    </MudButton>
                </MudStack>
            </EditForm>

            <!-- Footer Links -->
            <MudStack Spacing="2" AlignItems="AlignItems.Center" Style="margin-top: 24px;">
                <MudLink Href="/auth/forgot-password" Color="Color.Primary" Style="font-weight: 500;">
                    ¿Olvidaste tu contraseña?
                </MudLink>
            </MudStack>
        </MudPaper>

        <MudText Typo="Typo.caption" Color="Color.Secondary" Align="Align.Center" Style="margin-top: 24px;">
            © 2026 Corelio ERP. Todos los derechos reservados.
        </MudText>
    </MudContainer>
</div>

@code {
    private LoginModel _model = new();
    private bool _showPassword = false;

    private void TogglePasswordVisibility()
    {
        _showPassword = !_showPassword;
    }

    private async Task HandleLogin()
    {
        // Implementation in US-2.2.1
        Console.WriteLine($"Login attempt: {_model.Email}");
    }

    private class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
```

### Example 2: Product List Page

Create `Components/Pages/Products/ProductList.razor`:

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

<!-- Search & Filters -->
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

<!-- Products Table -->
<MudCard Elevation="2" Style="border-radius: 12px;">
    <MudCardContent Style="padding: 0;">
        <MudTable Items="@_products" Hover="true" Breakpoint="Breakpoint.Sm" Elevation="0">
            <HeaderContent>
                <MudTh Style="font-weight: 600;">SKU</MudTh>
                <MudTh Style="font-weight: 600;">Nombre</MudTh>
                <MudTh Style="font-weight: 600;">Precio</MudTh>
                <MudTh Style="font-weight: 600;">Stock</MudTh>
                <MudTh Style="font-weight: 600;">Acciones</MudTh>
            </HeaderContent>
            <RowTemplate>
                <MudTd DataLabel="SKU">
                    <MudChip Size="Size.Small" Color="Color.Default" Style="font-family: monospace;">
                        @context.Sku
                    </MudChip>
                </MudTd>
                <MudTd DataLabel="Nombre">@context.Name</MudTd>
                <MudTd DataLabel="Precio">
                    <strong>@context.Price.ToString("C2")</strong>
                </MudTd>
                <MudTd DataLabel="Stock">
                    <MudChip Size="Size.Small" Color="@(context.Stock > 10 ? Color.Success : Color.Warning)">
                        @context.Stock
                    </MudChip>
                </MudTd>
                <MudTd>
                    <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" />
                    <MudIconButton Icon="@Icons.Material.Filled.Delete" Size="Size.Small" Color="Color.Error" />
                </MudTd>
            </RowTemplate>
        </MudTable>
    </MudCardContent>
</MudCard>

@code {
    private List<BreadcrumbItem> _breadcrumbs = [
        new BreadcrumbItem("Inicio", href: "/"),
        new BreadcrumbItem("Productos", href: "/products", disabled: true)
    ];

    private List<Product> _products =
    [
        new() { Sku = "CLAV001", Name = "Clavos 2.5\"", Price = 45.50m, Stock = 150 },
        new() { Sku = "TORN001", Name = "Tornillos 1/4\"", Price = 32.00m, Stock = 8 },
        new() { Sku = "PINT001", Name = "Pintura Blanca 4L", Price = 285.00m, Stock = 22 }
    ];

    private class Product
    {
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
```

---

## Color Quick Reference

```html
<!-- Primary (Terracotta) -->
<MudButton Color="Color.Primary">Primary Button</MudButton>

<!-- Secondary (Concrete Gray) -->
<MudButton Color="Color.Secondary">Secondary Button</MudButton>

<!-- Success (Wood Green) -->
<MudAlert Severity="Severity.Success">Success Alert</MudAlert>

<!-- Warning (Safety Orange) -->
<MudAlert Severity="Severity.Warning">Warning Alert</MudAlert>

<!-- Error (Alert Red) -->
<MudAlert Severity="Severity.Error">Error Alert</MudAlert>

<!-- Info (Sky Blue) -->
<MudAlert Severity="Severity.Info">Info Alert</MudAlert>
```

---

## Typography Quick Reference

```html
<!-- Page Title -->
<MudText Typo="Typo.h4" Style="font-weight: 600;">Page Title</MudText>

<!-- Section Title -->
<MudText Typo="Typo.h5" Style="font-weight: 600;">Section Title</MudText>

<!-- Body Text -->
<MudText Typo="Typo.body1">Regular body text</MudText>

<!-- Small Text -->
<MudText Typo="Typo.body2" Color="Color.Secondary">Small secondary text</MudText>

<!-- Caption -->
<MudText Typo="Typo.caption" Color="Color.Secondary">Caption text</MudText>
```

---

## Common Patterns

### Loading State
```razor
@if (_isLoading)
{
    <MudStack AlignItems="AlignItems.Center" Spacing="3" Style="padding: 48px;">
        <MudProgressCircular Size="Size.Large" Indeterminate="true" Color="Color.Primary" />
        <MudText Typo="Typo.body1" Color="Color.Secondary">Cargando...</MudText>
    </MudStack>
}
```

### Empty State
```razor
@if (!_products.Any())
{
    <MudStack AlignItems="AlignItems.Center" Spacing="3" Style="padding: 48px;">
        <MudIcon Icon="@Icons.Material.Filled.Inbox" Size="Size.Large" Color="Color.Secondary" />
        <MudText Typo="Typo.h6">No hay productos</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">
            Comience agregando su primer producto
        </MudText>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add">
            Agregar Producto
        </MudButton>
    </MudStack>
}
```

### Success Notification
```razor
@inject ISnackbar Snackbar

@code {
    private void ShowSuccess()
    {
        Snackbar.Add("Producto guardado correctamente", Severity.Success);
    }
}
```

---

## Next Steps

1. **Implement US-2.2.1 (Authentication Frontend)** using the design system
2. **Implement US-2.1.1 (Multi-Tenant UI)** with TenantDisplay and UserDisplay components
3. **Create reusable components** as needed (LoadingState, EmptyState, etc.)
4. **Test responsive design** on mobile, tablet, desktop
5. **Add tenant theme customization** (optional, see TENANT-THEME-CUSTOMIZATION.md)

---

## Troubleshooting

### MudBlazor not rendering?
- Ensure `MudBlazor.min.css` and `MudBlazor.min.js` are loaded in `App.razor`
- Check browser console for errors
- Verify `AddMudServices()` is in `Program.cs`

### Fonts not loading?
- Check network tab for Google Fonts request
- Verify Inter font is loaded in `App.razor` head
- Clear browser cache

### Colors not matching design?
- Verify `custom.css` is loaded after MudBlazor CSS
- Use browser DevTools to inspect CSS variables
- Ensure `ThemeConfiguration.DefaultTheme` is applied in `MainLayout.razor`

---

**Ready to implement!** Follow this guide for US-2.2.1 and US-2.1.1. Reference `UI-UX-DESIGN-SYSTEM.md` for detailed specifications.
