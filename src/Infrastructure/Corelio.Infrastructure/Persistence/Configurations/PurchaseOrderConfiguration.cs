using Corelio.Domain.Entities;
using Corelio.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the PurchaseOrder entity.
/// </summary>
public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");

        builder.HasKey(po => po.Id);
        builder.Property(po => po.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(po => po.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(po => po.OrderNumber)
            .HasColumnName("order_number")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(po => po.SupplierId)
            .HasColumnName("supplier_id")
            .IsRequired();

        builder.Property(po => po.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(PurchaseOrderStatus.Draft)
            .IsRequired();

        builder.Property(po => po.ExpectedDate)
            .HasColumnName("expected_date");

        builder.Property(po => po.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);

        builder.Property(po => po.Subtotal)
            .HasColumnName("subtotal")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(po => po.IvaAmount)
            .HasColumnName("iva_amount")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(po => po.Total)
            .HasColumnName("total")
            .HasPrecision(18, 2)
            .IsRequired();

        // Soft delete fields
        builder.Property(po => po.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(po => po.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(po => po.DeletedBy)
            .HasColumnName("deleted_by");

        // Audit fields
        builder.Property(po => po.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(po => po.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(po => po.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(po => po.UpdatedBy)
            .HasColumnName("updated_by");

        // Relationships
        builder.HasOne(po => po.Supplier)
            .WithMany()
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(po => po.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(po => new { po.TenantId, po.Status })
            .HasDatabaseName("ix_purchase_orders_tenant_status")
            .HasFilter("is_deleted = false");

        builder.HasIndex(po => new { po.TenantId, po.CreatedAt })
            .HasDatabaseName("ix_purchase_orders_tenant_created_at")
            .HasFilter("is_deleted = false");

        builder.HasIndex(po => new { po.TenantId, po.OrderNumber })
            .IsUnique()
            .HasDatabaseName("ix_purchase_orders_tenant_order_number");

        // Soft delete query filter
        builder.HasQueryFilter(po => !po.IsDeleted);
    }
}
