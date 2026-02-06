using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the MarginTierDefinition entity.
/// </summary>
public class MarginTierDefinitionConfiguration : IEntityTypeConfiguration<MarginTierDefinition>
{
    public void Configure(EntityTypeBuilder<MarginTierDefinition> builder)
    {
        builder.ToTable("margin_tier_definitions", t =>
        {
            t.HasCheckConstraint("ck_margin_tier_definitions_tier_number",
                "tier_number BETWEEN 1 AND 5");
        });

        // Primary key
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Tenant ID
        builder.Property(m => m.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // Properties
        builder.Property(m => m.TenantPricingConfigurationId)
            .HasColumnName("tenant_pricing_configuration_id")
            .IsRequired();

        builder.Property(m => m.TierNumber)
            .HasColumnName("tier_number")
            .IsRequired();

        builder.Property(m => m.TierName)
            .HasColumnName("tier_name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        // Audit properties from TenantAuditableEntity
        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(m => m.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(m => m.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(m => new { m.TenantId, m.TierNumber })
            .IsUnique()
            .HasDatabaseName("ix_margin_tier_definitions_tenant_tier");

        builder.HasIndex(m => m.TenantPricingConfigurationId)
            .HasDatabaseName("ix_margin_tier_definitions_config_id");
    }
}
