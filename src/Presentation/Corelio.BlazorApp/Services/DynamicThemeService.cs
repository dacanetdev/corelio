using MudBlazor;

namespace Corelio.BlazorApp.Services;

/// <summary>
/// Service for building dynamic MudTheme based on tenant configuration.
/// Provides color manipulation methods for generating theme color shades.
/// </summary>
public interface IDynamicThemeService
{
    /// <summary>
    /// Gets a MudTheme based on the tenant's custom settings.
    /// </summary>
    /// <param name="primaryColor">The tenant's custom primary color in hex format (#RRGGBB), or null for default.</param>
    /// <returns>A MudTheme configured with the specified primary color or the default theme.</returns>
    MudTheme GetThemeForTenant(string? primaryColor);
}

/// <summary>
/// Implementation of the dynamic theme service that generates MudTheme with tenant customizations.
/// </summary>
public class DynamicThemeService : IDynamicThemeService
{
    /// <inheritdoc />
    public MudTheme GetThemeForTenant(string? primaryColor)
    {
        // Use default theme if no custom color provided
        if (string.IsNullOrWhiteSpace(primaryColor))
        {
            return ThemeConfiguration.CorelioDefaultTheme;
        }

        // Validate hex color format
        if (!IsValidHexColor(primaryColor))
        {
            return ThemeConfiguration.CorelioDefaultTheme;
        }

        // Generate color shades
        var primaryDarken = DarkenColor(primaryColor, 0.15);
        var primaryLighten = LightenColor(primaryColor, 0.15);

        return new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                // Custom Primary Color with generated shades
                Primary = primaryColor,
                PrimaryDarken = primaryDarken,
                PrimaryLighten = primaryLighten,
                PrimaryContrastText = GetContrastColor(primaryColor),

                // Keep secondary and semantic colors from default theme
                Secondary = "#6C757D",
                SecondaryDarken = "#495057",
                SecondaryLighten = "#ADB5BD",
                SecondaryContrastText = "#FFFFFF",

                // Semantic Colors (unchanged)
                Success = "#28A745",
                Warning = "#FFC107",
                Error = "#DC3545",
                Info = "#17A2B8",

                // Background Colors (unchanged)
                Background = "#FFFFFF",
                Surface = "#FFFFFF",
                AppbarBackground = "#FFFFFF",
                DrawerBackground = "#FFFFFF",

                // Text Colors (unchanged)
                TextPrimary = "#212529",
                TextSecondary = "#6C757D",
                TextDisabled = "#ADB5BD",

                // Border Colors (unchanged)
                Divider = "#DEE2E6",
                DividerLight = "#E9ECEF",
                LinesDefault = "#DEE2E6",
                LinesInputs = "#DEE2E6",

                // Table Colors (unchanged)
                TableLines = "#DEE2E6",
                TableStriped = "#F8F9FA",
                TableHover = "#E9ECEF"
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

    /// <summary>
    /// Darkens a hex color by the specified percentage.
    /// </summary>
    /// <param name="hexColor">The hex color to darken (#RRGGBB format).</param>
    /// <param name="percentage">The percentage to darken (0.0 to 1.0).</param>
    /// <returns>The darkened hex color.</returns>
    public static string DarkenColor(string hexColor, double percentage)
    {
        var (r, g, b) = HexToRgb(hexColor);

        r = (int)(r * (1 - percentage));
        g = (int)(g * (1 - percentage));
        b = (int)(b * (1 - percentage));

        return RgbToHex(Math.Max(0, r), Math.Max(0, g), Math.Max(0, b));
    }

    /// <summary>
    /// Lightens a hex color by the specified percentage.
    /// </summary>
    /// <param name="hexColor">The hex color to lighten (#RRGGBB format).</param>
    /// <param name="percentage">The percentage to lighten (0.0 to 1.0).</param>
    /// <returns>The lightened hex color.</returns>
    public static string LightenColor(string hexColor, double percentage)
    {
        var (r, g, b) = HexToRgb(hexColor);

        r = (int)(r + (255 - r) * percentage);
        g = (int)(g + (255 - g) * percentage);
        b = (int)(b + (255 - b) * percentage);

        return RgbToHex(Math.Min(255, r), Math.Min(255, g), Math.Min(255, b));
    }

    /// <summary>
    /// Determines the best contrast color (black or white) for text on the given background.
    /// </summary>
    /// <param name="hexColor">The background color in hex format.</param>
    /// <returns>#FFFFFF for dark backgrounds, #000000 for light backgrounds.</returns>
    public static string GetContrastColor(string hexColor)
    {
        var (r, g, b) = HexToRgb(hexColor);

        // Calculate relative luminance using sRGB formula
        var luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

        // Use white text for dark backgrounds, black for light backgrounds
        return luminance > 0.5 ? "#000000" : "#FFFFFF";
    }

    /// <summary>
    /// Converts a hex color to RGB components.
    /// </summary>
    /// <param name="hexColor">The hex color (#RRGGBB format).</param>
    /// <returns>A tuple of (red, green, blue) values (0-255).</returns>
    public static (int r, int g, int b) HexToRgb(string hexColor)
    {
        // Remove # if present
        var hex = hexColor.TrimStart('#');

        var r = Convert.ToInt32(hex[..2], 16);
        var g = Convert.ToInt32(hex[2..4], 16);
        var b = Convert.ToInt32(hex[4..6], 16);

        return (r, g, b);
    }

    /// <summary>
    /// Converts RGB components to a hex color.
    /// </summary>
    /// <param name="r">Red component (0-255).</param>
    /// <param name="g">Green component (0-255).</param>
    /// <param name="b">Blue component (0-255).</param>
    /// <returns>The hex color in #RRGGBB format.</returns>
    public static string RgbToHex(int r, int g, int b)
    {
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Validates if a string is a valid hex color format.
    /// </summary>
    /// <param name="color">The color string to validate.</param>
    /// <returns>True if the color is a valid hex format (#RRGGBB), false otherwise.</returns>
    private static bool IsValidHexColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color) || color.Length != 7)
        {
            return false;
        }

        if (color[0] != '#')
        {
            return false;
        }

        for (var i = 1; i < 7; i++)
        {
            var c = color[i];
            if (!char.IsDigit(c) && (c < 'A' || c > 'F') && (c < 'a' || c > 'f'))
            {
                return false;
            }
        }

        return true;
    }
}
