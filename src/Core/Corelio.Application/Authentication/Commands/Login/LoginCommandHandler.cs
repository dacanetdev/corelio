using Corelio.Application.Authentication.Common;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Interfaces.Authentication;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Authentication.Commands.Login;

/// <summary>
/// Handler for the LoginCommand that authenticates a user and issues JWT tokens.
/// </summary>
public class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    ITenantService tenantService,
    TimeProvider timeProvider) : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
{
    public async Task<Result<AuthenticationResult>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Resolve tenant if subdomain provided
        Guid? tenantId = null;
        if (!string.IsNullOrEmpty(request.TenantSubdomain))
        {
            var tenant = await tenantService.GetTenantBySubdomainAsync(
                request.TenantSubdomain,
                cancellationToken);

            if (tenant is null)
            {
                return Result<AuthenticationResult>.Failure(
                    new Error("Tenant.NotFound", "Tenant not found.", ErrorType.NotFound));
            }

            tenantId = tenant.Id;
        }

        // Step 2: Find user by email with roles and permissions
        var user = await userRepository.GetByEmailAsync(request.Email, tenantId, cancellationToken);

        if (user is null)
        {
            return Result<AuthenticationResult>.Failure(
                new Error("Auth.InvalidCredentials", "Invalid email or password.", ErrorType.Unauthorized));
        }

        // Step 3: Verify password
        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;

            // Lock account if too many failed attempts (5 attempts = 15 minutes lockout)
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockedUntil = timeProvider.GetUtcNow().AddMinutes(15).UtcDateTime;
            }

            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthenticationResult>.Failure(
                new Error("Auth.InvalidCredentials", "Invalid email or password.", ErrorType.Unauthorized));
        }

        // Step 4: Check account status
        if (!user.IsActive)
        {
            return Result<AuthenticationResult>.Failure(
                new Error("User.Inactive", "User account is inactive.", ErrorType.Unauthorized));
        }

        if (!user.IsEmailConfirmed)
        {
            return Result<AuthenticationResult>.Failure(
                new Error("User.EmailNotConfirmed", "Email address has not been confirmed.", ErrorType.Unauthorized));
        }

        if (user.IsLocked)
        {
            var lockedUntil = user.LockedUntil!.Value;
            var remainingMinutes = Math.Ceiling((lockedUntil - timeProvider.GetUtcNow().UtcDateTime).TotalMinutes);

            return Result<AuthenticationResult>.Failure(
                new Error(
                    "User.Locked",
                    $"Account is locked due to too many failed login attempts. Try again in {remainingMinutes} minutes.",
                    ErrorType.Unauthorized));
        }

        // Step 5: Extract roles and permissions
        var roles = user.UserRoles
            .Where(ur => !ur.IsExpired)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToArray();

        var permissions = user.UserRoles
            .Where(ur => !ur.IsExpired)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToArray();

        // Step 6: Generate JWT access token
        var accessToken = jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Email,
            user.TenantId,
            roles,
            permissions);

        var expiresAt = timeProvider.GetUtcNow().AddMinutes(60).UtcDateTime; // 1 hour

        // Step 7: Generate refresh token and save to database
        var (refreshToken, refreshTokenHash) = refreshTokenGenerator.GenerateToken();

        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TenantId = user.TenantId,
            TokenHash = refreshTokenHash,
            Jti = Guid.NewGuid().ToString(),
            ExpiresAt = timeProvider.GetUtcNow().AddDays(7).UtcDateTime, // 7 days
            IsRevoked = false,
            IpAddress = null, // TODO: Get from HttpContext in future
            UserAgent = null, // TODO: Get from HttpContext in future
            DeviceId = null,
            UseCount = 0
        };

        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        // Step 8: Update user login tracking
        user.LastLoginAt = timeProvider.GetUtcNow().UtcDateTime;
        user.LastLoginIp = null; // TODO: Get from HttpContext in future
        user.FailedLoginAttempts = 0; // Reset on successful login
        user.LockedUntil = null; // Clear any lockout

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Step 9: Return authentication result
        var authResult = new AuthenticationResult(
            UserId: user.Id,
            Email: user.Email,
            TenantId: user.TenantId,
            Roles: roles,
            Permissions: permissions,
            Tokens: new TokenResponse(accessToken, refreshToken, expiresAt));

        return Result<AuthenticationResult>.Success(authResult);
    }
}
