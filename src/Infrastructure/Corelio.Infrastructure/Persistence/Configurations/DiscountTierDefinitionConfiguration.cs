using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the DiscountTierDefinition entity.
/// </summary>
public class DiscountTierDefinitionConfiguration : IEntityTypeConfiguration<DiscountTierDefinition>
{
    public void Configure(EntityTypeBuilder<DiscountTierDefinition> builder)
    {
        builder.ToTable("discount_tier_definitions", t =>
        {
            t.HasCheckConstraint("ck_discount_tier_definitions_tier_number",
                "tier_number BETWEEN 1 AND 6");
        });

        // Primary key
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Tenant ID
        builder.Property(d => d.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // Properties
        builder.Property(d => d.TenantPricingConfigurationId)
            .HasColumnName("tenant_pricing_configuration_id")
            .IsRequired();

        builder.Property(d => d.TierNumber)
            .HasColumnName("tier_number")
            .IsRequired();

        builder.Property(d => d.TierName)
            .HasColumnName("tier_name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        // Audit properties from TenantAuditableEntity
        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(d => d.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(d => d.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(d => new { d.TenantId, d.TierNumber })
            .IsUnique()
            .HasDatabaseName("ix_discount_tier_definitions_tenant_tier");

        builder.HasIndex(d => d.TenantPricingConfigurationId)
            .HasDatabaseName("ix_discount_tier_definitions_config_id");
    }
}
