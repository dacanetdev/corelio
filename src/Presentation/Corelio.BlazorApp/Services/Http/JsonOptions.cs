using System.Text.Json;
using System.Text.Json.Serialization;

namespace Corelio.BlazorApp.Services.Http;

/// <summary>
/// Shared JSON serializer options for HTTP client communication.
/// Ensures consistent serialization between API and Blazor app.
/// </summary>
public static class JsonOptions
{
    /// <summary>
    /// Default JSON serializer options matching the API configuration.
    /// </summary>
    public static JsonSerializerOptions Default { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };
}
