using Corelio.Application.Common.Models;
using Corelio.Application.Users.Common;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(
    IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithRolesAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserDto>.Failure(
                new Error("User.NotFound", $"User with ID '{request.UserId}' not found.", ErrorType.NotFound));
        }

        var dto = new UserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.Email,
            user.Phone,
            user.Mobile,
            user.Position,
            user.EmployeeCode,
            user.HireDate,
            user.IsActive,
            user.UserRoles.Select(ur => ur.Role.Name).ToArray(),
            user.LastLoginAt,
            user.CreatedAt,
            user.UpdatedAt);

        return Result<UserDto>.Success(dto);
    }
}
