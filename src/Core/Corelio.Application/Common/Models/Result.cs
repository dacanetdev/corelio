namespace Corelio.Application.Common.Models;

/// <summary>
/// Represents the result of an operation that can either succeed with a value or fail with an error.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// The value returned on success (null if failed).
    /// </summary>
    public T? Value { get; init; }

    /// <summary>
    /// The error that occurred on failure (null if successful).
    /// </summary>
    public Error? Error { get; init; }

    /// <summary>
    /// Indicates whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    public static Result<T> Success(T value) => new()
    {
        IsSuccess = true,
        Value = value
    };

    /// <summary>
    /// Creates a failed result with the given error.
    /// </summary>
    public static Result<T> Failure(Error error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}
