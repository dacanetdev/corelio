using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Warehouse entity.
/// </summary>
public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("warehouses");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(w => w.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(w => w.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(w => w.IsDefault)
            .HasColumnName("is_default")
            .HasDefaultValue(false)
            .IsRequired();

        // Audit fields
        builder.Property(w => w.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(w => w.UpdatedAt).HasColumnName("updated_at");
        builder.Property(w => w.CreatedBy).HasColumnName("created_by");
        builder.Property(w => w.UpdatedBy).HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(w => w.TenantId)
            .HasDatabaseName("ix_warehouses_tenant_id");

        builder.HasIndex(w => new { w.TenantId, w.IsDefault })
            .HasDatabaseName("ix_warehouses_tenant_default")
            .HasFilter("is_default = true");
    }
}
