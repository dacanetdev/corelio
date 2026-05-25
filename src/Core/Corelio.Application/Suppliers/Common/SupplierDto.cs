namespace Corelio.Application.Suppliers.Common;

public record SupplierDto(
    Guid Id,
    string Name,
    string? Rfc,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Street,
    string? City,
    string? State,
    string? ZipCode,
    int PaymentTermsDays,
    string? TaxRegime,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
