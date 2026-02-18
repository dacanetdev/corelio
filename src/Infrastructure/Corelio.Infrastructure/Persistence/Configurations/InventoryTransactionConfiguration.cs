using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the InventoryTransaction entity.
/// </summary>
public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("inventory_transactions");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(t => t.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(t => t.InventoryItemId)
            .HasColumnName("inventory_item_id")
            .IsRequired();

        builder.Property(t => t.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(t => t.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("decimal(15,4)")
            .IsRequired();

        builder.Property(t => t.PreviousQuantity)
            .HasColumnName("previous_quantity")
            .HasColumnType("decimal(15,4)")
            .IsRequired();

        builder.Property(t => t.NewQuantity)
            .HasColumnName("new_quantity")
            .HasColumnType("decimal(15,4)")
            .IsRequired();

        builder.Property(t => t.ReferenceId)
            .HasColumnName("reference_id");

        builder.Property(t => t.Notes)
            .HasColumnName("notes")
            .HasMaxLength(500);

        // Audit fields
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at");
        builder.Property(t => t.CreatedBy).HasColumnName("created_by");
        builder.Property(t => t.UpdatedBy).HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(t => t.InventoryItemId)
            .HasDatabaseName("ix_inventory_transactions_item_id");

        builder.HasIndex(t => t.ReferenceId)
            .HasDatabaseName("ix_inventory_transactions_reference_id")
            .HasFilter("reference_id IS NOT NULL");
    }
}
