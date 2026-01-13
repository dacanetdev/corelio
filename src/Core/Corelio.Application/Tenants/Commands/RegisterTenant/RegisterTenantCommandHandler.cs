using Corelio.Application.Common.Interfaces.Authentication;
using Corelio.Application.Common.Interfaces.Email;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.Tenants.Commands.RegisterTenant;

/// <summary>
/// Handler for the RegisterTenantCommand that creates a new tenant with an owner user.
/// </summary>
public class RegisterTenantCommandHandler(
    ITenantRepository tenantRepository,
    IUserRepository userRepository,
    ITenantConfigurationRepository tenantConfigurationRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IEmailService emailService,
    TimeProvider timeProvider) : IRequestHandler<RegisterTenantCommand, Result<Guid>>
{
    // Owner role system GUID (from Role entity predefined GUIDs)
    private static readonly Guid OwnerRoleId = new("00000000-0000-0000-0000-000000000001");

    public async Task<Result<Guid>> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Validate subdomain uniqueness
        var subdomainExists = await tenantRepository.ExistsBySubdomainAsync(
            request.Subdomain.ToLowerInvariant(),
            cancellationToken);

        if (subdomainExists)
        {
            return Result<Guid>.Failure(
                new Error("Tenant.SubdomainExists", "A tenant with this subdomain already exists.", ErrorType.Conflict));
        }

        // Step 2: Begin database transaction (create tenant + owner user atomically)
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Step 3: Create tenant entity
            var tenant = new Tenant
            {
                Name = request.TenantName,
                LegalName = request.TenantName, // Can be updated later
                Rfc = request.RFC.ToUpperInvariant(),
                Subdomain = request.Subdomain.ToLowerInvariant(),
                SubscriptionPlan = SubscriptionPlan.Basic,
                SubscriptionStartsAt = timeProvider.GetUtcNow().UtcDateTime,
                SubscriptionEndsAt = timeProvider.GetUtcNow().AddDays(30).UtcDateTime, // 30-day trial
                IsTrial = true,
                TrialEndsAt = timeProvider.GetUtcNow().AddDays(30).UtcDateTime,
                IsActive = true,
                MaxUsers = 5, // Trial limit
                MaxProducts = 1000, // Trial limit
                MaxSalesPerMonth = 500 // Trial limit
            };

            await tenantRepository.AddAsync(tenant, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Step 4: Create owner user entity
            var ownerUser = new User
            {
                TenantId = tenant.Id,
                Email = request.OwnerEmail.ToLowerInvariant(),
                Username = request.OwnerEmail.ToLowerInvariant(),
                PasswordHash = passwordHasher.HashPassword(request.OwnerPassword),
                FirstName = request.OwnerFirstName,
                LastName = request.OwnerLastName,
                IsActive = true,
                IsEmailConfirmed = false, // Will be confirmed via email
                EmailConfirmationToken = Guid.NewGuid().ToString("N"), // Generate confirmation token
                TwoFactorEnabled = false,
                FailedLoginAttempts = 0
            };

            await userRepository.AddAsync(ownerUser, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Step 5: Assign Owner role to the user
            var ownerUserRole = new UserRole
            {
                UserId = ownerUser.Id,
                RoleId = OwnerRoleId,
                AssignedBy = ownerUser.Id // Owner assigns themselves
            };

            ownerUser.UserRoles.Add(ownerUserRole);
            userRepository.Update(ownerUser);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Step 6: Create default tenant configuration
            var tenantConfig = new TenantConfiguration
            {
                TenantId = tenant.Id,
                // CFDI Settings (to be configured later)
                CfdiPacTestMode = true,
                // POS Settings
                PosRequireCustomer = false,
                PosAutoPrintReceipt = false,
                // Feature Flags (Trial defaults)
                AllowNegativeInventory = false,
                FeatureMultiWarehouse = false,
                FeatureEcommerce = false
            };

            await tenantConfigurationRepository.AddAsync(tenantConfig, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Step 7: Commit transaction
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            // Step 8: Send welcome email with email confirmation link
            var confirmationLink = $"https://app.corelio.com.mx/confirm-email?token={ownerUser.EmailConfirmationToken}";
            await emailService.SendEmailConfirmationAsync(
                ownerUser.Email,
                confirmationLink,
                cancellationToken);

            return Result<Guid>.Success(tenant.Id);
        }
        catch (Exception ex)
        {
            // Rollback transaction on any error
            await unitOfWork.RollbackTransactionAsync(cancellationToken);

            return Result<Guid>.Failure(
                new Error("Tenant.CreationFailed", $"Failed to create tenant: {ex.Message}", ErrorType.Failure));
        }
    }
}
