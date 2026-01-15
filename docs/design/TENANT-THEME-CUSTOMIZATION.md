# Tenant-Specific Theme Customization Guide
## Dynamic UI Branding per Hardware Store

**Version:** 1.0
**Last Updated:** 2026-01-13
**Feature:** Multi-Tenant Theme Customization

---

## Overview

Allow each tenant (hardware store) to customize their UI theme colors, creating a branded experience while maintaining consistent UX patterns. Each tenant can define their primary color, logo, and optional accent colors to match their store's branding.

### Business Value

- **Brand Consistency:** Hardware stores can match the ERP UI to their physical store branding
- **White-Label Capability:** Each tenant feels like the system is "theirs"
- **Professional Image:** Reinforces tenant brand identity
- **Competitive Advantage:** Premium feature for multi-tenant SaaS

---

## Architecture

### Database Schema

#### TenantConfiguration Entity (Existing)

Add theme-related fields to the existing `TenantConfiguration` entity:

```csharp
// src/Core/Corelio.Domain/Entities/TenantConfiguration.cs

namespace Corelio.Domain.Entities;

public class TenantConfiguration : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    // Existing fields...
    public SubscriptionPlan SubscriptionPlan { get; set; }
    public int MaxUsers { get; set; }

    // NEW: Theme Customization Fields
    public string? PrimaryColor { get; set; }       // Hex color: "#e65946"
    public string? SecondaryColor { get; set; }     // Optional
    public string? AccentColor { get; set; }        // Optional
    public string? LogoUrl { get; set; }            // Azure Blob Storage URL
    public string? FaviconUrl { get; set; }         // Optional
    public bool UseCustomTheme { get; set; }        // Enable/disable custom theme

    // Navigation property
    public virtual Tenant Tenant { get; set; } = null!;
}
```

#### Migration

```bash
# Add migration for theme fields
dotnet ef migrations add AddTenantThemeFields --project src/Infrastructure/Corelio.Infrastructure --startup-project src/Presentation/Corelio.WebAPI
```

### Theme Service

#### ITenantThemeService Interface

```csharp
// src/Core/Corelio.Application/Interfaces/ITenantThemeService.cs

namespace Corelio.Application.Interfaces;

public interface ITenantThemeService
{
    /// <summary>
    /// Gets the theme configuration for the current tenant
    /// </summary>
    Task<TenantThemeDto?> GetCurrentTenantThemeAsync();

    /// <summary>
    /// Gets the theme configuration for a specific tenant
    /// </summary>
    Task<TenantThemeDto?> GetTenantThemeAsync(Guid tenantId);

    /// <summary>
    /// Updates the theme configuration for the current tenant
    /// </summary>
    Task<Result<TenantThemeDto>> UpdateTenantThemeAsync(UpdateTenantThemeCommand command);

    /// <summary>
    /// Resets tenant theme to default system theme
    /// </summary>
    Task<Result> ResetToDefaultThemeAsync();

    /// <summary>
    /// Validates a color hex value
    /// </summary>
    bool IsValidHexColor(string hexColor);

    /// <summary>
    /// Generates color shades from primary color (50-900)
    /// </summary>
    TenantColorPalette GenerateColorPalette(string primaryHex);
}

public record TenantThemeDto
{
    public Guid TenantId { get; init; }
    public string? PrimaryColor { get; init; }
    public string? SecondaryColor { get; init; }
    public string? AccentColor { get; init; }
    public string? LogoUrl { get; init; }
    public string? FaviconUrl { get; init; }
    public bool UseCustomTheme { get; init; }
    public TenantColorPalette? ColorPalette { get; init; }
}

public record TenantColorPalette
{
    public string Primary50 { get; init; } = string.Empty;
    public string Primary100 { get; init; } = string.Empty;
    public string Primary200 { get; init; } = string.Empty;
    public string Primary300 { get; init; } = string.Empty;
    public string Primary400 { get; init; } = string.Empty;
    public string Primary500 { get; init; } = string.Empty;  // Base color
    public string Primary600 { get; init; } = string.Empty;
    public string Primary700 { get; init; } = string.Empty;
    public string Primary800 { get; init; } = string.Empty;
    public string Primary900 { get; init; } = string.Empty;
}
```

#### TenantThemeService Implementation

```csharp
// src/Infrastructure/Corelio.Infrastructure/Services/TenantThemeService.cs

using System.Drawing;
using Corelio.Application.Interfaces;
using Corelio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Corelio.Infrastructure.Services;

public class TenantThemeService(
    ApplicationDbContext dbContext,
    ITenantService tenantService,
    IDistributedCache cache) : ITenantThemeService
{
    private const string CacheKeyPrefix = "tenant-theme:";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(2);

    public async Task<TenantThemeDto?> GetCurrentTenantThemeAsync()
    {
        var tenantId = tenantService.GetCurrentTenantId();
        return await GetTenantThemeAsync(tenantId);
    }

    public async Task<TenantThemeDto?> GetTenantThemeAsync(Guid tenantId)
    {
        // Try cache first
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        var cachedTheme = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedTheme))
        {
            return JsonSerializer.Deserialize<TenantThemeDto>(cachedTheme);
        }

        // Load from database
        var config = await dbContext.TenantConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TenantId == tenantId);

        if (config == null || !config.UseCustomTheme)
        {
            return null; // Use default theme
        }

        var theme = new TenantThemeDto
        {
            TenantId = tenantId,
            PrimaryColor = config.PrimaryColor,
            SecondaryColor = config.SecondaryColor,
            AccentColor = config.AccentColor,
            LogoUrl = config.LogoUrl,
            FaviconUrl = config.FaviconUrl,
            UseCustomTheme = config.UseCustomTheme,
            ColorPalette = !string.IsNullOrEmpty(config.PrimaryColor)
                ? GenerateColorPalette(config.PrimaryColor)
                : null
        };

        // Cache for 2 hours
        var serialized = JsonSerializer.Serialize(theme);
        await cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheExpiration
        });

        return theme;
    }

    public async Task<Result<TenantThemeDto>> UpdateTenantThemeAsync(UpdateTenantThemeCommand command)
    {
        var tenantId = tenantService.GetCurrentTenantId();

        // Validate colors
        if (!string.IsNullOrEmpty(command.PrimaryColor) && !IsValidHexColor(command.PrimaryColor))
        {
            return Result<TenantThemeDto>.Failure("PrimaryColor must be a valid hex color (e.g., #e65946)");
        }

        var config = await dbContext.TenantConfigurations
            .FirstOrDefaultAsync(c => c.TenantId == tenantId);

        if (config == null)
        {
            return Result<TenantThemeDto>.Failure("Tenant configuration not found");
        }

        // Update theme fields
        config.PrimaryColor = command.PrimaryColor;
        config.SecondaryColor = command.SecondaryColor;
        config.AccentColor = command.AccentColor;
        config.LogoUrl = command.LogoUrl;
        config.FaviconUrl = command.FaviconUrl;
        config.UseCustomTheme = true;

        await dbContext.SaveChangesAsync();

        // Invalidate cache
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        await cache.RemoveAsync(cacheKey);

        var theme = await GetTenantThemeAsync(tenantId);
        return Result<TenantThemeDto>.Success(theme!);
    }

    public async Task<Result> ResetToDefaultThemeAsync()
    {
        var tenantId = tenantService.GetCurrentTenantId();

        var config = await dbContext.TenantConfigurations
            .FirstOrDefaultAsync(c => c.TenantId == tenantId);

        if (config == null)
        {
            return Result.Failure("Tenant configuration not found");
        }

        config.UseCustomTheme = false;
        config.PrimaryColor = null;
        config.SecondaryColor = null;
        config.AccentColor = null;

        await dbContext.SaveChangesAsync();

        // Invalidate cache
        var cacheKey = $"{CacheKeyPrefix}{tenantId}";
        await cache.RemoveAsync(cacheKey);

        return Result.Success();
    }

    public bool IsValidHexColor(string hexColor)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            return false;

        // Must start with #
        if (!hexColor.StartsWith("#"))
            return false;

        // Must be 7 characters (#RRGGBB)
        if (hexColor.Length != 7)
            return false;

        // Must be valid hex digits
        return hexColor[1..].All(c => Uri.IsHexDigit(c));
    }

    public TenantColorPalette GenerateColorPalette(string primaryHex)
    {
        // Convert hex to RGB
        var color = ColorTranslator.FromHtml(primaryHex);

        // Generate shades using HSL color space
        var hsl = RgbToHsl(color);

        return new TenantColorPalette
        {
            Primary50 = HslToHex(hsl.H, hsl.S, 0.95),
            Primary100 = HslToHex(hsl.H, hsl.S, 0.90),
            Primary200 = HslToHex(hsl.H, hsl.S, 0.80),
            Primary300 = HslToHex(hsl.H, hsl.S, 0.70),
            Primary400 = HslToHex(hsl.H, hsl.S, 0.60),
            Primary500 = primaryHex,  // Base color
            Primary600 = HslToHex(hsl.H, hsl.S, 0.45),
            Primary700 = HslToHex(hsl.H, hsl.S, 0.35),
            Primary800 = HslToHex(hsl.H, hsl.S, 0.25),
            Primary900 = HslToHex(hsl.H, hsl.S, 0.15)
        };
    }

    #region Color Conversion Helpers

    private record HslColor(double H, double S, double L);

    private HslColor RgbToHsl(Color color)
    {
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double delta = max - min;

        double h = 0;
        double s = 0;
        double l = (max + min) / 2.0;

        if (delta != 0)
        {
            s = l > 0.5 ? delta / (2.0 - max - min) : delta / (max + min);

            if (max == r)
                h = ((g - b) / delta) + (g < b ? 6 : 0);
            else if (max == g)
                h = ((b - r) / delta) + 2;
            else
                h = ((r - g) / delta) + 4;

            h /= 6.0;
        }

        return new HslColor(h * 360, s, l);
    }

    private string HslToHex(double h, double s, double l)
    {
        double r, g, b;

        if (s == 0)
        {
            r = g = b = l; // achromatic
        }
        else
        {
            double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            double p = 2 * l - q;

            r = HueToRgb(p, q, h / 360.0 + 1.0 / 3.0);
            g = HueToRgb(p, q, h / 360.0);
            b = HueToRgb(p, q, h / 360.0 - 1.0 / 3.0);
        }

        int ri = (int)(r * 255);
        int gi = (int)(g * 255);
        int bi = (int)(b * 255);

        return $"#{ri:X2}{gi:X2}{bi:X2}";
    }

    private double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
        return p;
    }

    #endregion
}
```

### Blazor Theme Provider

#### Dynamic Theme Service

```csharp
// src/Presentation/Corelio.BlazorApp/Services/DynamicThemeService.cs

using Corelio.Application.Interfaces;
using MudBlazor;

namespace Corelio.BlazorApp.Services;

public class DynamicThemeService(ITenantThemeService tenantThemeService)
{
    private MudTheme? _cachedTheme;
    private Guid? _cachedTenantId;

    public async Task<MudTheme> GetThemeAsync(Guid? tenantId = null)
    {
        // Return cached theme if tenant hasn't changed
        if (_cachedTheme != null && _cachedTenantId == tenantId)
        {
            return _cachedTheme;
        }

        var tenantTheme = tenantId.HasValue
            ? await tenantThemeService.GetTenantThemeAsync(tenantId.Value)
            : await tenantThemeService.GetCurrentTenantThemeAsync();

        _cachedTheme = BuildMudTheme(tenantTheme);
        _cachedTenantId = tenantId;

        return _cachedTheme;
    }

    public void ClearCache()
    {
        _cachedTheme = null;
        _cachedTenantId = null;
    }

    private MudTheme BuildMudTheme(TenantThemeDto? tenantTheme)
    {
        // Default theme (from UI-UX-DESIGN-SYSTEM.md)
        var defaultPrimary = "#e65946";    // Terracotta
        var defaultSecondary = "#495057";  // Concrete Gray
        var defaultTertiary = "#2a8a8f";   // Tool Steel

        // Use custom colors if available
        var primaryColor = tenantTheme?.PrimaryColor ?? defaultPrimary;
        var secondaryColor = tenantTheme?.SecondaryColor ?? defaultSecondary;
        var accentColor = tenantTheme?.AccentColor ?? defaultTertiary;

        return new MudTheme
        {
            Palette = new PaletteLight
            {
                Primary = primaryColor,
                Secondary = secondaryColor,
                Tertiary = accentColor,

                AppbarBackground = "#ffffff",
                AppbarText = "#212529",

                DrawerBackground = "#f8f9fa",
                DrawerText = "#212529",

                Success = "#2e7d32",
                Warning = "#ed6c02",
                Error = "#d32f2f",
                Info = "#0288d1",

                Background = "#f8f9fa",
                Surface = "#ffffff",

                TextPrimary = "#212529",
                TextSecondary = "#495057",
                TextDisabled = "#adb5bd",

                DividerLight = "#dee2e6",
                Divider = "#ced4da",

                LinesDefault = "#e9ecef",
                LinesInputs = "#ced4da",

                HoverOpacity = 0.08,

                TableLines = "#e9ecef",
                TableStriped = "#f8f9fa"
            },

            Typography = new Typography
            {
                Default = new Default
                {
                    FontFamily = ["Inter", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "sans-serif"],
                    FontSize = "1rem",
                    FontWeight = 400,
                    LineHeight = 1.5,
                    LetterSpacing = "normal"
                },

                H1 = new H1 { FontSize = "2rem", FontWeight = 600, LineHeight = 1.25, LetterSpacing = "-0.01em" },
                H2 = new H2 { FontSize = "1.75rem", FontWeight = 600, LineHeight = 1.3 },
                H3 = new H3 { FontSize = "1.5rem", FontWeight = 600, LineHeight = 1.3 },
                H4 = new H4 { FontSize = "1.25rem", FontWeight = 600, LineHeight = 1.4 },
                H5 = new H5 { FontSize = "1.125rem", FontWeight = 600, LineHeight = 1.4 },
                H6 = new H6 { FontSize = "1rem", FontWeight = 600, LineHeight = 1.5 },

                Button = new Button { FontSize = "0.875rem", FontWeight = 600, TextTransform = "none", LetterSpacing = "0.02em" },

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
                DrawerWidthLeft = "260px",
                DrawerWidthRight = "260px"
            }
        };
    }
}
```

#### Update MainLayout.razor

```razor
@inherits LayoutComponentBase
@inject DynamicThemeService ThemeService
@inject CustomAuthenticationStateProvider AuthStateProvider

<MudThemeProvider @ref="_themeProvider" Theme="@_currentTheme" @bind-IsDarkMode="@_isDarkMode" />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <MudAppBar ...>
        <!-- Header content -->
    </MudAppBar>

    <MudDrawer ...>
        <!-- Navigation -->
    </MudDrawer>

    <MudMainContent>
        @Body
    </MudMainContent>
</MudLayout>

@code {
    private MudThemeProvider? _themeProvider;
    private MudTheme _currentTheme = new();
    private bool _isDarkMode = false;
    private Guid? _currentTenantId;

    protected override async Task OnInitializedAsync()
    {
        // Load theme based on authenticated tenant
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = authState.User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                _currentTenantId = tenantId;
                _currentTheme = await ThemeService.GetThemeAsync(tenantId);
                StateHasChanged();
            }
        }
    }

    public async Task RefreshTheme()
    {
        if (_currentTenantId.HasValue)
        {
            ThemeService.ClearCache();
            _currentTheme = await ThemeService.GetThemeAsync(_currentTenantId);
            StateHasChanged();
        }
    }
}
```

---

## API Endpoints

### Theme Management Endpoints

```csharp
// src/Presentation/Corelio.WebAPI/Endpoints/TenantThemeEndpoints.cs

using Corelio.Application.Interfaces;
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
            .WithSummary("Get theme for current tenant");

        group.MapPut("/", UpdateTenantTheme)
            .WithName("UpdateTenantTheme")
            .WithSummary("Update theme for current tenant")
            .RequireAuthorization("tenants.manage");

        group.MapPost("/reset", ResetTenantTheme)
            .WithName("ResetTenantTheme")
            .WithSummary("Reset to default theme")
            .RequireAuthorization("tenants.manage");

        group.MapPost("/preview", PreviewTheme)
            .WithName("PreviewTheme")
            .WithSummary("Preview theme without saving");

        return app;
    }

    private static async Task<IResult> GetCurrentTenantTheme(
        ITenantThemeService themeService)
    {
        var theme = await themeService.GetCurrentTenantThemeAsync();
        return theme != null
            ? Results.Ok(theme)
            : Results.Ok(new { message = "Using default theme" });
    }

    private static async Task<IResult> UpdateTenantTheme(
        [FromBody] UpdateTenantThemeCommand command,
        ITenantThemeService themeService)
    {
        var result = await themeService.UpdateTenantThemeAsync(command);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> ResetTenantTheme(
        ITenantThemeService themeService)
    {
        var result = await themeService.ResetToDefaultThemeAsync();
        return result.IsSuccess
            ? Results.Ok(new { message = "Theme reset to default" })
            : Results.BadRequest(result.Error);
    }

    private static IResult PreviewTheme(
        [FromBody] PreviewThemeRequest request,
        ITenantThemeService themeService)
    {
        if (!themeService.IsValidHexColor(request.PrimaryColor))
        {
            return Results.BadRequest("Invalid hex color");
        }

        var palette = themeService.GenerateColorPalette(request.PrimaryColor);
        return Results.Ok(new { primaryColor = request.PrimaryColor, palette });
    }
}

public record UpdateTenantThemeCommand
{
    public string? PrimaryColor { get; init; }
    public string? SecondaryColor { get; init; }
    public string? AccentColor { get; init; }
    public string? LogoUrl { get; init; }
    public string? FaviconUrl { get; init; }
}

public record PreviewThemeRequest
{
    public required string PrimaryColor { get; init; }
}
```

---

## Admin UI - Theme Customization Page

### Theme Settings Page

```razor
@page "/settings/theme"
@attribute [Authorize(Policy = "tenants.manage")]
@inject ITenantThemeService ThemeService
@inject ISnackbar Snackbar

<PageTitle>Personalización de Tema - Corelio ERP</PageTitle>

<MudContainer MaxWidth="MaxWidth.Large" Style="padding: 24px;">

    <!-- Page Header -->
    <MudStack Spacing="4" Style="margin-bottom: 32px;">
        <MudText Typo="Typo.h4" Style="font-weight: 600;">
            Personalización de Tema
        </MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">
            Personalice los colores de la interfaz para reflejar la identidad de su ferretería
        </MudText>
    </MudStack>

    <MudGrid Spacing="4">
        <!-- Left: Theme Editor -->
        <MudItem xs="12" md="6">
            <MudCard Elevation="2" Style="border-radius: 12px;">
                <MudCardHeader Style="background: var(--color-secondary-100);">
                    <CardHeaderContent>
                        <MudText Typo="Typo.h6" Style="font-weight: 600;">
                            Configuración de Colores
                        </MudText>
                    </CardHeaderContent>
                </MudCardHeader>
                <MudCardContent Style="padding: 24px;">
                    <MudStack Spacing="4">

                        <!-- Primary Color Picker -->
                        <div>
                            <MudText Typo="Typo.body2" Style="font-weight: 600; margin-bottom: 12px;">
                                Color Principal
                            </MudText>
                            <MudColorPicker @bind-Value="_primaryColor"
                                           Palette="_colorPalette"
                                           ColorPickerView="ColorPickerView.Spectrum"
                                           Style="width: 100%;" />
                            <MudText Typo="Typo.caption" Color="Color.Secondary" Style="margin-top: 8px;">
                                Hex: @_primaryColor.ToString()
                            </MudText>
                        </div>

                        <!-- Logo Upload -->
                        <div>
                            <MudText Typo="Typo.body2" Style="font-weight: 600; margin-bottom: 12px;">
                                Logo de la Empresa
                            </MudText>
                            <MudFileUpload T="IBrowserFile"
                                          Accept=".png,.jpg,.svg"
                                          FilesChanged="HandleLogoUpload">
                                <ButtonTemplate>
                                    <MudButton Variant="Variant.Outlined"
                                              Color="Color.Primary"
                                              StartIcon="@Icons.Material.Filled.CloudUpload"
                                              Style="text-transform: none;">
                                        Subir Logo
                                    </MudButton>
                                </ButtonTemplate>
                            </MudFileUpload>

                            @if (!string.IsNullOrEmpty(_logoUrl))
                            {
                                <MudImage Src="@_logoUrl"
                                         Height="80"
                                         Alt="Logo Preview"
                                         Style="margin-top: 12px; border: 1px solid #dee2e6; border-radius: 8px; padding: 8px;" />
                            }
                        </div>

                        <!-- Action Buttons -->
                        <MudStack Row="true" Spacing="3" Style="margin-top: 16px;">
                            <MudButton Variant="Variant.Filled"
                                      Color="Color.Primary"
                                      StartIcon="@Icons.Material.Filled.Save"
                                      OnClick="HandleSave"
                                      Disabled="@_isLoading"
                                      Style="font-weight: 600; text-transform: none;">
                                Guardar Cambios
                            </MudButton>

                            <MudButton Variant="Variant.Outlined"
                                      Color="Color.Secondary"
                                      StartIcon="@Icons.Material.Filled.Refresh"
                                      OnClick="HandleReset"
                                      Style="text-transform: none;">
                                Restaurar Predeterminado
                            </MudButton>
                        </MudStack>
                    </MudStack>
                </MudCardContent>
            </MudCard>
        </MudItem>

        <!-- Right: Live Preview -->
        <MudItem xs="12" md="6">
            <MudCard Elevation="2" Style="border-radius: 12px;">
                <MudCardHeader Style="background: var(--color-secondary-100);">
                    <CardHeaderContent>
                        <MudText Typo="Typo.h6" Style="font-weight: 600;">
                            Vista Previa
                        </MudText>
                    </CardHeaderContent>
                </MudCardHeader>
                <MudCardContent Style="padding: 24px;">
                    <!-- Preview UI Components -->
                    <MudStack Spacing="3">
                        <!-- Preview Header -->
                        <MudPaper Style="@GetPreviewHeaderStyle()" Elevation="0">
                            <MudText Typo="Typo.h6" Style="color: white; font-weight: 600;">
                                Corelio ERP
                            </MudText>
                        </MudPaper>

                        <!-- Preview Buttons -->
                        <MudStack Row="true" Spacing="2">
                            <MudButton Variant="Variant.Filled"
                                      Style="@GetPreviewButtonStyle()"
                                      Size="Size.Small">
                                Botón Principal
                            </MudButton>
                            <MudButton Variant="Variant.Outlined"
                                      Color="Color.Secondary"
                                      Size="Size.Small">
                                Botón Secundario
                            </MudButton>
                        </MudStack>

                        <!-- Preview Chip -->
                        <div>
                            <MudChip Style="@GetPreviewChipStyle()" Size="Size.Medium">
                                Inquilino: Mi Ferretería
                            </MudChip>
                        </div>

                        <!-- Preview Card -->
                        <MudPaper Elevation="1" Style="padding: 16px; border-radius: 8px;">
                            <MudText Typo="Typo.subtitle1" Style="font-weight: 600; margin-bottom: 8px;">
                                Ejemplo de Tarjeta
                            </MudText>
                            <MudText Typo="Typo.body2" Color="Color.Secondary">
                                Así se verán las tarjetas con el nuevo tema.
                            </MudText>
                        </MudPaper>

                        <!-- Color Palette Preview -->
                        <div>
                            <MudText Typo="Typo.body2" Style="font-weight: 600; margin-bottom: 12px;">
                                Paleta de Colores Generada
                            </MudText>
                            <MudStack Row="true" Spacing="1">
                                @foreach (var shade in GetColorShades())
                                {
                                    <MudPaper Style="@GetColorShadeStyle(shade)"
                                             Elevation="0"
                                             Width="40px"
                                             Height="40px">
                                    </MudPaper>
                                }
                            </MudStack>
                        </div>
                    </MudStack>
                </MudCardContent>
            </MudCard>
        </MudItem>
    </MudGrid>
</MudContainer>

@code {
    private MudColor _primaryColor = new("#e65946");  // Default terracotta
    private string? _logoUrl;
    private bool _isLoading = false;

    private MudColor[] _colorPalette = [
        new("#e65946"),  // Terracotta (default)
        new("#2a8a8f"),  // Tool Steel
        new("#2e7d32"),  // Forest Green
        new("#ed6c02"),  // Safety Orange
        new("#0288d1"),  // Sky Blue
        new("#7b1fa2"),  // Purple
        new("#c62828"),  // Red
        new("#455a64"),  // Blue Gray
    ];

    protected override async Task OnInitializedAsync()
    {
        var currentTheme = await ThemeService.GetCurrentTenantThemeAsync();
        if (currentTheme?.UseCustomTheme == true && !string.IsNullOrEmpty(currentTheme.PrimaryColor))
        {
            _primaryColor = new MudColor(currentTheme.PrimaryColor);
            _logoUrl = currentTheme.LogoUrl;
        }
    }

    private async Task HandleSave()
    {
        _isLoading = true;

        var command = new UpdateTenantThemeCommand
        {
            PrimaryColor = _primaryColor.ToString(),
            LogoUrl = _logoUrl
        };

        var result = await ThemeService.UpdateTenantThemeAsync(command);

        if (result.IsSuccess)
        {
            Snackbar.Add("Tema guardado correctamente", Severity.Success);
        }
        else
        {
            Snackbar.Add($"Error: {result.Error}", Severity.Error);
        }

        _isLoading = false;
    }

    private async Task HandleReset()
    {
        var result = await ThemeService.ResetToDefaultThemeAsync();
        if (result.IsSuccess)
        {
            _primaryColor = new("#e65946");  // Reset to default
            _logoUrl = null;
            Snackbar.Add("Tema restaurado a predeterminado", Severity.Success);
        }
    }

    private async Task HandleLogoUpload(IBrowserFile file)
    {
        // Upload to Azure Blob Storage (implementation not shown)
        // _logoUrl = await BlobStorageService.UploadLogoAsync(file);
        Snackbar.Add("Logo subido (implementar Azure Blob Storage)", Severity.Info);
    }

    private string GetPreviewHeaderStyle()
        => $"padding: 16px; background: {_primaryColor}; border-radius: 8px;";

    private string GetPreviewButtonStyle()
        => $"background: {_primaryColor}; color: white; font-weight: 600; text-transform: none;";

    private string GetPreviewChipStyle()
        => $"background: {_primaryColor}20; border: 2px solid {_primaryColor}; color: {_primaryColor}; font-weight: 600;";

    private List<string> GetColorShades()
    {
        var palette = ThemeService.GenerateColorPalette(_primaryColor.ToString());
        return [palette.Primary300, palette.Primary400, palette.Primary500, palette.Primary600, palette.Primary700];
    }

    private string GetColorShadeStyle(string color)
        => $"background: {color}; border-radius: 4px;";
}
```

---

## Pre-defined Theme Templates

### Theme Template Library

```csharp
// src/Infrastructure/Corelio.Infrastructure/Services/TenantThemeTemplates.cs

namespace Corelio.Infrastructure.Services;

public static class TenantThemeTemplates
{
    public static Dictionary<string, ThemeTemplate> Templates = new()
    {
        ["terracotta"] = new()
        {
            Name = "Terracota Industrial (Predeterminado)",
            Description = "Cálido, confiable, inspirado en materiales de construcción",
            PrimaryColor = "#e65946",
            SecondaryColor = "#495057",
            AccentColor = "#2a8a8f"
        },

        ["forest-green"] = new()
        {
            Name = "Verde Ferretero",
            Description = "Ecológico, natural, herramientas de jardín",
            PrimaryColor = "#2e7d32",
            SecondaryColor = "#455a64",
            AccentColor = "#689f38"
        },

        ["steel-blue"] = new()
        {
            Name = "Azul Acero",
            Description = "Profesional, moderno, herramientas eléctricas",
            PrimaryColor = "#0288d1",
            SecondaryColor = "#455a64",
            AccentColor = "#00acc1"
        },

        ["safety-orange"] = new()
        {
            Name = "Naranja Seguridad",
            Description = "Visible, enérgico, equipos de seguridad",
            PrimaryColor = "#ed6c02",
            SecondaryColor = "#424242",
            AccentColor = "#f57c00"
        },

        ["industrial-gray"] = new()
        {
            Name = "Gris Industrial",
            Description = "Sobrio, profesional, suministros industriales",
            PrimaryColor = "#546e7a",
            SecondaryColor = "#37474f",
            AccentColor = "#78909c"
        }
    };
}

public record ThemeTemplate
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string PrimaryColor { get; init; }
    public required string SecondaryColor { get; init; }
    public required string AccentColor { get; init; }
}
```

---

## Testing

### Unit Tests

```csharp
// tests/Corelio.Infrastructure.Tests/Services/TenantThemeServiceTests.cs

using Corelio.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace Corelio.Infrastructure.Tests.Services;

public class TenantThemeServiceTests
{
    private readonly TenantThemeService _sut;

    public TenantThemeServiceTests()
    {
        // Setup with mocks...
        _sut = new TenantThemeService(dbContext, tenantService, cache);
    }

    [Theory]
    [InlineData("#e65946", true)]   // Valid
    [InlineData("#000000", true)]   // Black
    [InlineData("#FFFFFF", true)]   // White (uppercase)
    [InlineData("e65946", false)]   // Missing #
    [InlineData("#e6594", false)]   // Too short
    [InlineData("#e65946a", false)] // Too long
    [InlineData("#gggggg", false)]  // Invalid hex
    [InlineData("", false)]         // Empty
    [InlineData(null, false)]       // Null
    public void IsValidHexColor_ValidatesCorrectly(string? hexColor, bool expected)
    {
        // Act
        var result = _sut.IsValidHexColor(hexColor!);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GenerateColorPalette_CreatesValidShades()
    {
        // Arrange
        var primaryColor = "#e65946";

        // Act
        var palette = _sut.GenerateColorPalette(primaryColor);

        // Assert
        palette.Primary500.Should().Be(primaryColor);  // Base color unchanged
        palette.Primary50.Should().NotBeNullOrEmpty();
        palette.Primary900.Should().NotBeNullOrEmpty();

        // All shades should be valid hex colors
        _sut.IsValidHexColor(palette.Primary50).Should().BeTrue();
        _sut.IsValidHexColor(palette.Primary900).Should().BeTrue();
    }

    [Fact]
    public async Task GetTenantThemeAsync_ReturnsCachedTheme_WhenAvailable()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        // Setup cache mock...

        // Act
        var theme1 = await _sut.GetTenantThemeAsync(tenantId);
        var theme2 = await _sut.GetTenantThemeAsync(tenantId);

        // Assert
        theme1.Should().NotBeNull();
        theme2.Should().Be(theme1);  // Same instance from cache
        // Verify cache was hit (mock verification)
    }
}
```

---

## Implementation Checklist

### Phase 1: Backend (3-4 hours)
- [ ] Add theme fields to `TenantConfiguration` entity
- [ ] Create migration for theme fields
- [ ] Create `ITenantThemeService` interface
- [ ] Implement `TenantThemeService` with color palette generation
- [ ] Create API endpoints for theme management
- [ ] Write 15+ unit tests

### Phase 2: Blazor Theme Service (2-3 hours)
- [ ] Create `DynamicThemeService.cs`
- [ ] Update `MainLayout.razor` to load tenant theme
- [ ] Test theme switching on authentication
- [ ] Verify theme caching works

### Phase 3: Admin UI (4-5 hours)
- [ ] Create Theme Settings page (`/settings/theme`)
- [ ] Implement color picker with preview
- [ ] Implement logo upload (Azure Blob Storage integration)
- [ ] Add theme template selector
- [ ] Test theme customization end-to-end

### Phase 4: Documentation & Testing (2 hours)
- [ ] Document theme customization for users
- [ ] Create video tutorial (Spanish)
- [ ] Perform accessibility tests with custom colors
- [ ] Test with 5+ different color schemes

**Total Effort:** 11-14 hours (2 days)

---

## Security Considerations

### Validation
- **Color Format:** Must be valid hex color (#RRGGBB)
- **Logo Upload:** Validate file type (PNG, JPG, SVG only)
- **File Size:** Max 2MB for logos
- **Authorization:** Only `tenants.manage` permission can change theme

### Performance
- **Caching:** Theme cached for 2 hours (Redis)
- **Invalidation:** Cache cleared on theme update
- **Color Generation:** Palette generated once and cached

### Multi-Tenancy
- **Isolation:** Each tenant's theme is isolated
- **No Cross-Tenant Access:** Cannot view/modify other tenant themes
- **Query Filters:** Automatic tenant_id filtering on TenantConfiguration

---

## Future Enhancements

### V2 Features (Out of Scope for MVP)
- **Dark Mode Support:** Per-tenant dark theme
- **Font Customization:** Custom font family selection
- **Advanced Branding:** Custom header/footer HTML
- **Theme Marketplace:** Pre-built themes to purchase
- **White-Label Mode:** Hide "Powered by Corelio" footer

---

**Document Status:** ✅ Ready for Implementation
**Estimated Effort:** 11-14 hours (2 days)
**Dependencies:** US-2.2.1 (Authentication) must be complete
**Priority:** Medium (Nice-to-have for Sprint 4+)
