using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(Guid Id) : IRequest<Result<SupplierDto>>;
