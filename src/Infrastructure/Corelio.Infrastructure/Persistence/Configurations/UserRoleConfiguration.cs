using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the UserRole join entity.
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        // Composite primary key
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Property(ur => ur.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(ur => ur.RoleId)
            .HasColumnName("role_id")
            .IsRequired();

        builder.Property(ur => ur.AssignedAt)
            .HasColumnName("assigned_at")
            .IsRequired();

        builder.Property(ur => ur.AssignedBy)
            .HasColumnName("assigned_by");

        builder.Property(ur => ur.ExpiresAt)
            .HasColumnName("expires_at");

        // Ignore computed property
        builder.Ignore(ur => ur.IsExpired);

        // Indexes
        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("ix_user_roles_user_id");

        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("ix_user_roles_role_id");

        builder.HasIndex(ur => ur.ExpiresAt)
            .HasDatabaseName("ix_user_roles_expires_at")
            .HasFilter("expires_at IS NOT NULL");
    }
}
