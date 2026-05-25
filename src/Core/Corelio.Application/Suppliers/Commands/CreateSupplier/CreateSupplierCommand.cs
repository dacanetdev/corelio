using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand(
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
    string? Notes) : IRequest<Result<Guid>>;
