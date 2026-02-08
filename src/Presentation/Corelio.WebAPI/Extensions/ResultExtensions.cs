using Corelio.Application.Common.Models;

namespace Corelio.WebAPI.Extensions;

/// <summary>
/// Extension methods for mapping Result&lt;T&gt; to HTTP results.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps an error to an appropriate HTTP result.
    /// </summary>
    /// <param name="error">The error to map.</param>
    /// <returns>An IResult representing the error.</returns>
    public static IResult ToHttpResult(this Error error)
    {
        return Results.Problem(
            statusCode: error.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            },
            title: error.Code,
            detail: error.Message);
    }

    /// <summary>
    /// Converts a successful result to an OK response or maps the error.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>An IResult representing the response.</returns>
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error!.ToHttpResult();
    }

    /// <summary>
    /// Converts a successful result to a Created response or maps the error.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="locationPath">The path for the Location header.</param>
    /// <param name="responseValue">The value to return in the response body.</param>
    /// <returns>An IResult representing the response.</returns>
    public static IResult ToCreatedResult<T>(this Result<T> result, string locationPath, object? responseValue = null)
    {
        return result.IsSuccess
            ? Results.Created(locationPath, responseValue ?? new { id = result.Value })
            : result.Error!.ToHttpResult();
    }

    /// <summary>
    /// Converts a successful result to a NoContent response or maps the error.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>An IResult representing the response.</returns>
    public static IResult ToNoContentResult<T>(this Result<T> result)
    {
        return result.IsSuccess
            ? Results.NoContent()
            : result.Error!.ToHttpResult();
    }
}
