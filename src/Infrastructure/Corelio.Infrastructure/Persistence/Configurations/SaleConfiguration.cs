using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Sale entity.
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(s => s.Folio)
            .HasColumnName("folio")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.CustomerId)
            .HasColumnName("customer_id");

        builder.Property(s => s.WarehouseId)
            .HasColumnName("warehouse_id")
            .IsRequired();

        builder.Property(s => s.SubTotal)
            .HasColumnName("sub_total")
            .HasColumnType("decimal(15,2)")
            .IsRequired();

        builder.Property(s => s.DiscountAmount)
            .HasColumnName("discount_amount")
            .HasColumnType("decimal(15,2)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(s => s.TaxAmount)
            .HasColumnName("tax_amount")
            .HasColumnType("decimal(15,2)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(s => s.Total)
            .HasColumnName("total")
            .HasColumnType("decimal(15,2)")
            .IsRequired();

        builder.Property(s => s.Notes)
            .HasColumnName("notes")
            .HasMaxLength(500);

        builder.Property(s => s.CompletedAt)
            .HasColumnName("completed_at");

        // Audit fields
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.CreatedBy).HasColumnName("created_by");
        builder.Property(s => s.UpdatedBy).HasColumnName("updated_by");

        // Unique folio per tenant
        builder.HasIndex(s => new { s.TenantId, s.Folio })
            .IsUnique()
            .HasDatabaseName("ix_sales_tenant_folio");

        builder.HasIndex(s => s.TenantId)
            .HasDatabaseName("ix_sales_tenant_id");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("ix_sales_status");

        builder.HasIndex(s => s.CreatedAt)
            .HasDatabaseName("ix_sales_created_at");

        // Relationships
        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Warehouse)
            .WithMany()
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Payments)
            .WithOne(p => p.Sale)
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
