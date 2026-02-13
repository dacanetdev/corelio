using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Filters;

/// <summary>
/// Endpoint filter that catches model binding errors and returns detailed, friendly error messages.
/// </summary>
public class ModelBindingErrorFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Check if there are model binding errors
        if (context.HttpContext.Items.TryGetValue("ModelBindingError", out var errorObj) && errorObj is string error)
        {
            var problemDetails = new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Invalid Request Data",
                Detail = error,
                Instance = context.HttpContext.Request.Path
            };

            return Results.BadRequest(problemDetails);
        }

        try
        {
            return await next(context);
        }
        catch (JsonException ex)
        {
            // Catch JSON deserialization errors and provide friendly messages
            var problemDetails = new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Invalid JSON Format",
                Detail = GetFriendlyJsonError(ex),
                Instance = context.HttpContext.Request.Path
            };

            problemDetails.Errors.Add("JSON", [ex.Message]);

            return Results.BadRequest(problemDetails);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("JSON"))
        {
            // Catch model binding errors
            var problemDetails = new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Model Binding Failed",
                Detail = GetFriendlyBindingError(ex),
                Instance = context.HttpContext.Request.Path
            };

            problemDetails.Errors.Add("Request", [ex.Message]);

            return Results.BadRequest(problemDetails);
        }
    }

    private static string GetFriendlyJsonError(JsonException ex)
    {
        var message = ex.Message;

        if (message.Contains("could not be converted"))
        {
            return "One or more fields have invalid data types. Please check that enums are sent as strings (e.g., 'EAN13') and numbers are not in quotes.";
        }

        if (message.Contains("required property"))
        {
            return "One or more required fields are missing from the request.";
        }

        return "The request body contains invalid JSON. Please check the format and data types.";
    }

    private static string GetFriendlyBindingError(InvalidOperationException ex)
    {
        var message = ex.Message;

        if (message.Contains("parameter") && message.Contains("from the request body"))
        {
            // Extract parameter name if possible
            var paramStart = message.IndexOf("parameter \"", StringComparison.Ordinal);
            if (paramStart >= 0)
            {
                var paramEnd = message.IndexOf('"', paramStart + 11);
                if (paramEnd >= 0)
                {
                    var paramName = message.Substring(paramStart + 11, paramEnd - paramStart - 11);
                    return $"Failed to deserialize the request body into '{paramName}'. Common issues:\n" +
                           "- Enum fields should be strings matching the enum name (e.g., \"BarcodeType\": \"EAN13\")\n" +
                           "- Required fields must be present in the JSON\n" +
                           "- Field names are case-sensitive\n" +
                           "- Check that data types match (strings in quotes, numbers without quotes)";
                }
            }
        }

        return "The request data could not be processed. Please verify the JSON format and field types match the API contract.";
    }
}
