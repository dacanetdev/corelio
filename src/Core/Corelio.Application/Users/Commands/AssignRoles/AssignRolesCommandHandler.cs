using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Users.Commands.AssignRoles;

public class AssignRolesCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<AssignRolesCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AssignRolesCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<bool>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        var user = await userRepository.GetByIdWithRolesAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure(
                new Error("User.NotFound", $"User with ID '{request.UserId}' not found.", ErrorType.NotFound));
        }

        var roleNames = request.RoleCodes.Select(r => r.Trim()).ToArray();
        var roles = await roleRepository.GetByCodesAsync(roleNames, tenantId.Value, cancellationToken);

        if (roles.Count != roleNames.Length)
        {
            var foundRoleNames = roles.Select(r => r.Name).ToArray();
            var invalidRoles = roleNames.Except(foundRoleNames).ToArray();

            return Result<bool>.Failure(
                new Error("User.InvalidRoles",
                    $"Invalid role codes: {string.Join(", ", invalidRoles)}",
                    ErrorType.Validation));
        }

        // Replace all existing roles
        user.UserRoles.Clear();

        foreach (var role in roles)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedBy = Guid.Empty
            });
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
