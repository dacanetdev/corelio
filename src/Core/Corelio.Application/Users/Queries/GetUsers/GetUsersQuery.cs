using Corelio.Application.Common.Models;
using Corelio.Application.Users.Common;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    int Page,
    int Size,
    string? Search,
    bool? IsActive) : IRequest<Result<PagedResult<UserListDto>>>;
