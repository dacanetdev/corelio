using Corelio.Application.Common.Models;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Users.Commands.AssignRoles;

public record AssignRolesCommand(
    Guid UserId,
    string[] RoleCodes) : IRequest<Result<bool>>;
