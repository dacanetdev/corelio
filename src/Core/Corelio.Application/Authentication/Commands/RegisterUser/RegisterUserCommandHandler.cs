using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Interfaces.Authentication;
using Corelio.Application.Common.Interfaces.Email;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.RegisterUser;

/// <summary>
/// Handler for the RegisterUserCommand that creates a new user within the current tenant.
/// </summary>
public class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService,
    IPasswordHasher passwordHasher,
    IEmailService emailService) : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Get current tenant ID (NEVER trust client input for tenant!)
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        // Step 2: Check if email already exists within the tenant
        var emailExists = await userRepository.ExistsByEmailAsync(
            request.Email.ToLowerInvariant(),
            tenantId.Value,
            cancellationToken);

        if (emailExists)
        {
            return Result<Guid>.Failure(
                new Error("User.EmailExists", "A user with this email already exists.", ErrorType.Conflict));
        }

        // Step 3: Validate role codes exist and belong to tenant or are system roles
        var roleNames = request.RoleCodes.Select(r => r.Trim()).ToArray();

        var roles = await roleRepository.GetByCodesAsync(roleNames, tenantId.Value, cancellationToken);

        if (roles.Count != roleNames.Length)
        {
            var foundRoleNames = roles.Select(r => r.Name).ToArray();
            var invalidRoles = roleNames.Except(foundRoleNames).ToArray();

            return Result<Guid>.Failure(
                new Error(
                    "User.InvalidRoles",
                    $"Invalid role codes: {string.Join(", ", invalidRoles)}",
                    ErrorType.Validation));
        }

        // Step 4: Create user entity
        var user = new User
        {
            TenantId = tenantId.Value,
            Email = request.Email.ToLowerInvariant(),
            Username = request.Email.ToLowerInvariant(),
            PasswordHash = passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            IsEmailConfirmed = false,
            EmailConfirmationToken = Guid.NewGuid().ToString("N"),
            TwoFactorEnabled = false,
            FailedLoginAttempts = 0
        };

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 5: Assign roles to the user
        foreach (var role in roles)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedBy = Guid.Empty // TODO: Get current user ID from ICurrentUserProvider
            };

            user.UserRoles.Add(userRole);
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 6: Send email confirmation
        var confirmationLink = $"https://app.corelio.com.mx/confirm-email?token={user.EmailConfirmationToken}";
        await emailService.SendEmailConfirmationAsync(
            user.Email,
            confirmationLink,
            cancellationToken);

        return Result<Guid>.Success(user.Id);
    }
}
