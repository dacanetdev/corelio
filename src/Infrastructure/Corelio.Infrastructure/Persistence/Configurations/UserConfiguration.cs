using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        // Primary key
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(u => u.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // Credentials
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.Username)
            .HasColumnName("username")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(500)
            .IsRequired();

        // Personal Information
        builder.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        // Ignore computed property
        builder.Ignore(u => u.FullName);

        builder.Property(u => u.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(u => u.Mobile)
            .HasColumnName("mobile")
            .HasMaxLength(20);

        // Employment Info
        builder.Property(u => u.EmployeeCode)
            .HasColumnName("employee_code")
            .HasMaxLength(50);

        builder.Property(u => u.Position)
            .HasColumnName("position")
            .HasMaxLength(100);

        builder.Property(u => u.HireDate)
            .HasColumnName("hire_date");

        // Security
        builder.Property(u => u.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(u => u.IsEmailConfirmed)
            .HasColumnName("is_email_confirmed")
            .HasDefaultValue(false);

        builder.Property(u => u.EmailConfirmationToken)
            .HasColumnName("email_confirmation_token")
            .HasMaxLength(500);

        builder.Property(u => u.PasswordResetToken)
            .HasColumnName("password_reset_token")
            .HasMaxLength(500);

        builder.Property(u => u.PasswordResetExpiresAt)
            .HasColumnName("password_reset_expires_at");

        builder.Property(u => u.TwoFactorEnabled)
            .HasColumnName("two_factor_enabled")
            .HasDefaultValue(false);

        builder.Property(u => u.TwoFactorSecret)
            .HasColumnName("two_factor_secret")
            .HasMaxLength(500);

        // Login Tracking
        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at");

        builder.Property(u => u.LastLoginIp)
            .HasColumnName("last_login_ip")
            .HasMaxLength(45);

        builder.Property(u => u.FailedLoginAttempts)
            .HasColumnName("failed_login_attempts")
            .HasDefaultValue(0);

        builder.Property(u => u.LockedUntil)
            .HasColumnName("locked_until");

        // Ignore computed property
        builder.Ignore(u => u.IsLocked);

        // Audit properties from AuditableEntity
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(u => u.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(u => u.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique()
            .HasDatabaseName("ix_users_tenant_email");

        builder.HasIndex(u => new { u.TenantId, u.Username })
            .IsUnique()
            .HasDatabaseName("ix_users_tenant_username");

        builder.HasIndex(u => u.TenantId)
            .HasDatabaseName("ix_users_tenant_id");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("ix_users_is_active");

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
