namespace Corelio.Application.Suppliers.Common;

public record SupplierListDto(
    Guid Id,
    string Name,
    string? Rfc,
    string? ContactName,
    string? Email,
    string? Phone,
    bool IsActive,
    DateTime CreatedAt);
