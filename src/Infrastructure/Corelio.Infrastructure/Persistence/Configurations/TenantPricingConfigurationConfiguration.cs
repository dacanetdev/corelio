using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the TenantPricingConfiguration entity.
/// </summary>
public class TenantPricingConfigurationConfiguration : IEntityTypeConfiguration<TenantPricingConfiguration>
{
    public void Configure(EntityTypeBuilder<TenantPricingConfiguration> builder)
    {
        builder.ToTable("tenant_pricing_configurations", t =>
        {
            t.HasCheckConstraint("ck_tenant_pricing_configurations_discount_tier_count",
                "discount_tier_count BETWEEN 1 AND 6");
            t.HasCheckConstraint("ck_tenant_pricing_configurations_margin_tier_count",
                "margin_tier_count BETWEEN 1 AND 5");
            t.HasCheckConstraint("ck_tenant_pricing_configurations_iva_percentage",
                "iva_percentage BETWEEN 0 AND 100");
        });

        // Primary key
        builder.HasKey(tpc => tpc.Id);
        builder.Property(tpc => tpc.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Tenant ID
        builder.Property(tpc => tpc.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // Properties
        builder.Property(tpc => tpc.DiscountTierCount)
            .HasColumnName("discount_tier_count")
            .HasDefaultValue(3)
            .IsRequired();

        builder.Property(tpc => tpc.MarginTierCount)
            .HasColumnName("margin_tier_count")
            .HasDefaultValue(3)
            .IsRequired();

        builder.Property(tpc => tpc.DefaultIvaEnabled)
            .HasColumnName("default_iva_enabled")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(tpc => tpc.IvaPercentage)
            .HasColumnName("iva_percentage")
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(16.00m)
            .IsRequired();

        // Audit properties from TenantAuditableEntity
        builder.Property(tpc => tpc.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(tpc => tpc.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(tpc => tpc.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(tpc => tpc.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(tpc => tpc.TenantId)
            .IsUnique()
            .HasDatabaseName("ix_tenant_pricing_configurations_tenant_id");

        // Relationships
        builder.HasMany(tpc => tpc.DiscountTierDefinitions)
            .WithOne(d => d.TenantPricingConfiguration)
            .HasForeignKey(d => d.TenantPricingConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(tpc => tpc.MarginTierDefinitions)
            .WithOne(m => m.TenantPricingConfiguration)
            .HasForeignKey(m => m.TenantPricingConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
