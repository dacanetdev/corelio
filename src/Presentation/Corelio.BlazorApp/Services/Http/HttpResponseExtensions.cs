using System.Net.Http.Json;
using System.Text.Json;

namespace Corelio.BlazorApp.Services.Http;

/// <summary>
/// Extension methods for parsing HTTP error responses.
/// </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Parses an error response and returns a user-friendly error message.
    /// Handles both ProblemDetails (RFC 7807) and plain text responses.
    /// </summary>
    public static async Task<string> GetErrorMessageAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var contentType = response.Content.Headers.ContentType?.MediaType;

            // Try to parse as ProblemDetails (JSON)
            if (contentType?.Contains("json", StringComparison.OrdinalIgnoreCase) == true)
            {
                try
                {
                    var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(
                        cancellationToken: cancellationToken);

                    if (problemDetails?.Detail is not null)
                    {
                        return problemDetails.Detail;
                    }

                    if (problemDetails?.Title is not null)
                    {
                        return problemDetails.Title;
                    }
                }
                catch (JsonException)
                {
                    // Fall through to read as string
                }
            }

            // Fallback to plain text
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return !string.IsNullOrWhiteSpace(content) ? content : $"Error: {response.StatusCode}";
        }
        catch
        {
            return $"Error: {response.StatusCode}";
        }
    }

    /// <summary>
    /// Simplified ProblemDetails response model for parsing error responses.
    /// </summary>
    private record ProblemDetailsResponse(
        string? Title,
        string? Detail,
        int? Status,
        string? Instance);
}
