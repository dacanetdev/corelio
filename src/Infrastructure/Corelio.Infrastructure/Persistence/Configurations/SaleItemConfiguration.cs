using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the SaleItem entity.
/// </summary>
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(i => i.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(i => i.SaleId)
            .HasColumnName("sale_id")
            .IsRequired();

        builder.Property(i => i.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(i => i.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(i => i.ProductSku)
            .HasColumnName("product_sku")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("decimal(15,2)")
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("decimal(15,4)")
            .IsRequired();

        builder.Property(i => i.DiscountPercentage)
            .HasColumnName("discount_percentage")
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(i => i.TaxPercentage)
            .HasColumnName("tax_percentage")
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(i => i.LineTotal)
            .HasColumnName("line_total")
            .HasColumnType("decimal(15,2)")
            .IsRequired();

        // Audit fields
        builder.Property(i => i.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(i => i.UpdatedAt).HasColumnName("updated_at");
        builder.Property(i => i.CreatedBy).HasColumnName("created_by");
        builder.Property(i => i.UpdatedBy).HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(i => i.SaleId)
            .HasDatabaseName("ix_sale_items_sale_id");

        builder.HasIndex(i => i.ProductId)
            .HasDatabaseName("ix_sale_items_product_id");

        // Relationships
        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
