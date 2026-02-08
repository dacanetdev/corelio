namespace Corelio.Application.Common.Models;

/// <summary>
/// Represents the type/category of an error.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Input validation failed.
    /// </summary>
    Validation,

    /// <summary>
    /// Requested resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// User is not authorized to perform the action.
    /// </summary>
    Unauthorized,

    /// <summary>
    /// Operation conflicts with existing state (e.g., duplicate key).
    /// </summary>
    Conflict,

    /// <summary>
    /// User is authenticated but not allowed to perform the action.
    /// </summary>
    Forbidden,

    /// <summary>
    /// General operation failure.
    /// </summary>
    Failure
}
