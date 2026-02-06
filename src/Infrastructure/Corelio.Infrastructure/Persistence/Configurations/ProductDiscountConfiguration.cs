using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the ProductDiscount entity.
/// </summary>
public class ProductDiscountConfiguration : IEntityTypeConfiguration<ProductDiscount>
{
    public void Configure(EntityTypeBuilder<ProductDiscount> builder)
    {
        builder.ToTable("product_discounts", t =>
        {
            t.HasCheckConstraint("ck_product_discounts_tier_number",
                "tier_number BETWEEN 1 AND 6");
            t.HasCheckConstraint("ck_product_discounts_discount_percentage",
                "discount_percentage BETWEEN 0 AND 100");
        });

        // Primary key
        builder.HasKey(pd => pd.Id);
        builder.Property(pd => pd.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Tenant ID
        builder.Property(pd => pd.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // Properties
        builder.Property(pd => pd.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(pd => pd.TierNumber)
            .HasColumnName("tier_number")
            .IsRequired();

        builder.Property(pd => pd.DiscountPercentage)
            .HasColumnName("discount_percentage")
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        // Audit properties from TenantAuditableEntity
        builder.Property(pd => pd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(pd => pd.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(pd => pd.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(pd => pd.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(pd => new { pd.ProductId, pd.TierNumber })
            .IsUnique()
            .HasDatabaseName("ix_product_discounts_product_tier");

        // Relationships
        builder.HasOne(pd => pd.Product)
            .WithMany(p => p.Discounts)
            .HasForeignKey(pd => pd.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
