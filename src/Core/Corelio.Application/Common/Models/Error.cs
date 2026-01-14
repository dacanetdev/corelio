namespace Corelio.Application.Common.Models;

/// <summary>
/// Represents an error that occurred during operation execution.
/// </summary>
/// <param name="Code">A machine-readable error code (e.g., "User.NotFound", "Tenant.InvalidRFC").</param>
/// <param name="Message">A human-readable error message.</param>
/// <param name="Type">The category of the error.</param>
public record Error(string Code, string Message, ErrorType Type);
