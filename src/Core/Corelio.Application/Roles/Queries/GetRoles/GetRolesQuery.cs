using Corelio.Application.Common.Models;
using Corelio.Application.Roles.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Roles.Queries.GetRoles;

public record GetRolesQuery : IRequest<Result<List<RoleDto>>>;
