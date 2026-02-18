using Corelio.Domain.Enums;

namespace Corelio.Application.Customers.Common;

/// <summary>
/// Summary customer DTO for list views.
/// </summary>
public record CustomerListDto(
    Guid Id,
    CustomerType CustomerType,
    string FullName,
    string? Rfc,
    string? Email,
    string? Phone,
    DateTime CreatedAt);
