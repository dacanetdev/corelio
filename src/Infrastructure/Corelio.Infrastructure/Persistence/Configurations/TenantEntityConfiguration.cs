using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Tenant entity.
/// </summary>
public class TenantEntityConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        // Primary key
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Properties
        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.LegalName)
            .HasColumnName("legal_name")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(t => t.Rfc)
            .HasColumnName("rfc")
            .HasMaxLength(13)
            .IsRequired();

        builder.Property(t => t.Subdomain)
            .HasColumnName("subdomain")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.CustomDomain)
            .HasColumnName("custom_domain")
            .HasMaxLength(255);

        builder.Property(t => t.SubscriptionPlan)
            .HasColumnName("subscription_plan")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.SubscriptionStartsAt)
            .HasColumnName("subscription_starts_at")
            .IsRequired();

        builder.Property(t => t.SubscriptionEndsAt)
            .HasColumnName("subscription_ends_at");

        builder.Property(t => t.MaxUsers)
            .HasColumnName("max_users")
            .HasDefaultValue(5);

        builder.Property(t => t.MaxProducts)
            .HasColumnName("max_products")
            .HasDefaultValue(1000);

        builder.Property(t => t.MaxSalesPerMonth)
            .HasColumnName("max_sales_per_month")
            .HasDefaultValue(5000);

        builder.Property(t => t.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(t => t.IsTrial)
            .HasColumnName("is_trial")
            .HasDefaultValue(false);

        builder.Property(t => t.TrialEndsAt)
            .HasColumnName("trial_ends_at");

        // Audit properties from AuditableEntity
        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(t => t.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(t => t.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(t => t.Subdomain)
            .IsUnique()
            .HasDatabaseName("ix_tenants_subdomain");

        builder.HasIndex(t => t.CustomDomain)
            .IsUnique()
            .HasDatabaseName("ix_tenants_custom_domain")
            .HasFilter("custom_domain IS NOT NULL");

        builder.HasIndex(t => t.Rfc)
            .IsUnique()
            .HasDatabaseName("ix_tenants_rfc");

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("ix_tenants_is_active");

        // Relationships
        builder.HasOne(t => t.Configuration)
            .WithOne(tc => tc.Tenant)
            .HasForeignKey<Domain.Entities.TenantConfiguration>(tc => tc.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Roles)
            .WithOne(r => r.Tenant)
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
