using Corelio.Application.Common.Models;
using Corelio.Application.Sales.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Sales.Queries.GetSaleById;

/// <summary>
/// Query to get a full sale by ID.
/// </summary>
public record GetSaleByIdQuery(Guid Id) : IRequest<Result<SaleDto>>;
