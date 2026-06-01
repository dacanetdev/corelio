using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the GoodsReceiptItem entity.
/// </summary>
public class GoodsReceiptItemConfiguration : IEntityTypeConfiguration<GoodsReceiptItem>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptItem> builder)
    {
        builder.ToTable("goods_receipt_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(i => i.GoodsReceiptId)
            .HasColumnName("goods_receipt_id")
            .IsRequired();

        builder.Property(i => i.PurchaseOrderItemId)
            .HasColumnName("purchase_order_item_id")
            .IsRequired();

        builder.Property(i => i.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(i => i.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(i => i.QuantityReceived)
            .HasColumnName("quantity_received")
            .HasPrecision(18, 4)
            .IsRequired();

        // Relationships
        builder.HasOne(i => i.PurchaseOrderItem)
            .WithMany()
            .HasForeignKey(i => i.PurchaseOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for querying items by product
        builder.HasIndex(i => i.ProductId)
            .HasDatabaseName("ix_goods_receipt_items_product_id");
    }
}
