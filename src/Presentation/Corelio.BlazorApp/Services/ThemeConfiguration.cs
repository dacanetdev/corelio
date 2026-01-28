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

        Shadows = new Shadow
        {
            // Subtle shadows following Tailwind CSS shadow system
            Elevation = new string[]
            {
                "none",                                                                                 // 0: No shadow
                "0 1px 2px 0 rgba(0, 0, 0, 0.05)",                                                    // 1: Subtle shadow
                "0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)",                   // 2: Light shadow
                "0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)",             // 3: Medium shadow
                "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)",           // 4: Standard cards
                "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)",           // 5: Same as 4
                "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)",           // 6: Same as 4
                "0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)",           // 7: Same as 4
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 8: Elevated modals
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 9: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 10: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 11: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 12: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 13: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 14: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 15: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 16: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 17: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 18: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 19: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 20: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 21: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 22: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)",         // 23: Same as 8
                "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)"          // 24: Same as 8
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
