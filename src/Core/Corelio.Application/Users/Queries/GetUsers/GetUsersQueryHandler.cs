using Corelio.Application.Common.Models;
using Corelio.Application.Users.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(
    IUserRepository userRepository) : IRequestHandler<GetUsersQuery, Result<PagedResult<UserListDto>>>
{
    public async Task<Result<PagedResult<UserListDto>>> Handle(
        GetUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await userRepository.GetPagedAsync(
            request.Page,
            request.Size,
            request.Search,
            request.IsActive,
            cancellationToken);

        var dtos = users.Select(u => new UserListDto(
            u.Id,
            u.FullName,
            u.Email,
            u.Position,
            u.IsActive,
            u.UserRoles.Select(ur => ur.Role.Name).ToArray(),
            u.LastLoginAt,
            u.CreatedAt)).ToList();

        return Result<PagedResult<UserListDto>>.Success(
            PagedResult<UserListDto>.Create(dtos, request.Page, request.Size, totalCount));
    }
}
