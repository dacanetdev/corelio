using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the InventoryItem entity.
/// </summary>
public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(i => i.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(i => i.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(i => i.WarehouseId)
            .HasColumnName("warehouse_id")
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("decimal(15,4)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(i => i.ReservedQuantity)
            .HasColumnName("reserved_quantity")
            .HasColumnType("decimal(15,4)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(i => i.MinimumLevel)
            .HasColumnName("minimum_level")
            .HasColumnType("decimal(15,4)")
            .HasDefaultValue(0m)
            .IsRequired();

        // Audit fields
        builder.Property(i => i.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(i => i.UpdatedAt).HasColumnName("updated_at");
        builder.Property(i => i.CreatedBy).HasColumnName("created_by");
        builder.Property(i => i.UpdatedBy).HasColumnName("updated_by");

        // Computed property ignored
        builder.Ignore(i => i.AvailableQuantity);

        // Unique composite index: one stock record per product per warehouse per tenant
        builder.HasIndex(i => new { i.TenantId, i.ProductId, i.WarehouseId })
            .IsUnique()
            .HasDatabaseName("ix_inventory_items_tenant_product_warehouse");

        // Relationships
        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Warehouse)
            .WithMany(w => w.InventoryItems)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Transactions)
            .WithOne(t => t.InventoryItem)
            .HasForeignKey(t => t.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
