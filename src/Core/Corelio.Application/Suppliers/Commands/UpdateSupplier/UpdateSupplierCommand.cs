using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Suppliers.Commands.UpdateSupplier;

public record UpdateSupplierCommand(
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
    bool IsActive) : IRequest<Result<bool>>;
