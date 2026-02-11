using MudBlazor;

namespace Corelio.BlazorApp.Services;

/// <summary>
/// Provides the Corelio "Warm Precision" design system theme configuration.
/// Primary: Refined Terracotta (#C0392B), Neutrals: Warm scale, Sidebar: Dark (#1A1D23).
/// Typography is handled via CSS (see wwwroot/app.css and variables.css) using Plus Jakarta Sans.
/// </summary>
public static class ThemeConfiguration
{
    /// <summary>
    /// Gets the default Corelio theme with "Warm Precision" color palette.
    /// </summary>
    public static MudTheme CorelioDefaultTheme => new()
    {
        PaletteLight = new PaletteLight
        {
            // Primary: Refined Terracotta
            Primary = "#C0392B",
            PrimaryDarken = "#A33025",
            PrimaryLighten = "#E8655A",
            PrimaryContrastText = "#FFFFFF",

            // Secondary: Warm Neutral
            Secondary = "#6B655C",
            SecondaryDarken = "#524D46",
            SecondaryLighten = "#B0A99E",
            SecondaryContrastText = "#FFFFFF",

            // Semantic Colors
            Success = "#059669",
            Warning = "#D97706",
            Error = "#DC2626",
            Info = "#2563EB",

            // Background & Surface
            Background = "#F8F7F4",
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            DrawerBackground = "#2A2420",

            // Text Colors
            TextPrimary = "#1A1D23",
            TextSecondary = "#6B655C",
            TextDisabled = "#B0A99E",

            // Border & Divider Colors
            Divider = "#E4E0D9",
            DividerLight = "#EFECEA",
            LinesDefault = "#E4E0D9",
            LinesInputs = "#D1CCC3",

            // Table Colors
            TableLines = "#E4E0D9",
            TableStriped = "#F8F7F4",
            TableHover = "#F3F1ED"
        },

        // Note: Using default MudBlazor shadows. Custom shadows in CSS variables.

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            AppbarHeight = "56px",
            DrawerWidthLeft = "272px",
            DrawerWidthRight = "300px"
        }
    };
}
