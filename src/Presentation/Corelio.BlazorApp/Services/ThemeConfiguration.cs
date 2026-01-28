using MudBlazor;

namespace Corelio.BlazorApp.Services;

/// <summary>
/// Provides the Corelio "Industrial Terracotta" design system theme configuration.
/// This theme establishes consistent colors, shadows, and spacing throughout the application.
/// Typography is handled via CSS (see wwwroot/app.css and variables.css).
/// </summary>
public static class ThemeConfiguration
{
    /// <summary>
    /// Gets the default Corelio theme with "Industrial Terracotta" color palette.
    /// Primary: #E74C3C (Terracotta Red), Secondary: #6C757D (Concrete Gray)
    /// </summary>
    public static MudTheme CorelioDefaultTheme => new()
    {
        PaletteLight = new PaletteLight
        {
            // Primary Color: Terracotta Red (#E74C3C)
            Primary = "#E74C3C",
            PrimaryDarken = "#CB4335",
            PrimaryLighten = "#EC7063",
            PrimaryContrastText = "#FFFFFF",

            // Secondary Color: Concrete Gray (#6C757D)
            Secondary = "#6C757D",
            SecondaryDarken = "#495057",
            SecondaryLighten = "#ADB5BD",
            SecondaryContrastText = "#FFFFFF",

            // Semantic Colors
            Success = "#28A745",
            Warning = "#FFC107",
            Error = "#DC3545",
            Info = "#17A2B8",

            // Background Colors
            Background = "#FFFFFF",
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            DrawerBackground = "#FFFFFF",

            // Text Colors
            TextPrimary = "#212529",
            TextSecondary = "#6C757D",
            TextDisabled = "#ADB5BD",

            // Border Colors
            Divider = "#DEE2E6",
            DividerLight = "#E9ECEF",
            LinesDefault = "#DEE2E6",
            LinesInputs = "#DEE2E6",

            // Table Colors
            TableLines = "#DEE2E6",
            TableStriped = "#F8F9FA",
            TableHover = "#E9ECEF"
        },

        // Note: Using default MudBlazor shadows instead of custom shadows
        // Custom shadows were causing IndexOutOfRangeException in MudThemeProvider
        // CSS shadows are defined in app.css variables instead

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
            AppbarHeight = "64px",
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px"
        }
    };
}
