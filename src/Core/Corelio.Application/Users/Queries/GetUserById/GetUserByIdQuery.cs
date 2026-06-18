using Corelio.Application.Common.Models;
using Corelio.Application.Users.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;
