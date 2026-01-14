using System.Diagnostics.CodeAnalysis;

namespace Corelio.BlazorApp.Models.Common;

/// <summary>
/// Generic result wrapper for API responses.
/// </summary>
/// <typeparam name="T">Type of the result value.</typeparam>
[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory pattern for creating results")]
public record Result<T>
{
    /// <summary>
    /// Indicates if the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// The result value (only populated on success).
    /// </summary>
    public T? Value { get; init; }

    /// <summary>
    /// Error message (only populated on failure).
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Additional error details.
    /// </summary>
    public List<string>? Errors { get; init; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result<T> Success(T value) => new()
    {
        IsSuccess = true,
        Value = value
    };

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static Result<T> Failure(string error, List<string>? errors = null) => new()
    {
        IsSuccess = false,
        Error = error,
        Errors = errors
    };
}

/// <summary>
/// Result wrapper for operations without a return value.
/// </summary>
public record Result
{
    /// <summary>
    /// Indicates if the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Error message (only populated on failure).
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Additional error details.
    /// </summary>
    public List<string>? Errors { get; init; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new()
    {
        IsSuccess = true
    };

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static Result Failure(string error, List<string>? errors = null) => new()
    {
        IsSuccess = false,
        Error = error,
        Errors = errors
    };
}
