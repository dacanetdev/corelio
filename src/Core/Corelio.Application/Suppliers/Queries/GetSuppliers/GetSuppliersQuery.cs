using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery(
    int Page,
    int Size,
    string? Search,
    bool? IsActive) : IRequest<Result<PagedResult<SupplierListDto>>>;
