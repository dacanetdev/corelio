using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the ProductMarginPrice entity.
/// </summary>
public class ProductMarginPriceConfiguration : IEntityTypeConfiguration<ProductMarginPrice>
{
    public void Configure(EntityTypeBuilder<ProductMarginPrice> builder)
    {
        builder.ToTable("product_margin_prices", t =>
        {
            t.HasCheckConstraint("ck_product_margin_prices_tier_number",
                "tier_number BETWEEN 1 AND 5");
            t.HasCheckConstraint("ck_product_margin_prices_margin_percentage",
                "margin_percentage IS NULL OR margin_percentage BETWEEN 0 AND 100");
        });

        // Primary key
        builder.HasKey(pm => pm.Id);
        builder.Property(pm => pm.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Tenant ID
        builder.Property(pm => pm.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // Properties
        builder.Property(pm => pm.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(pm => pm.TierNumber)
            .HasColumnName("tier_number")
            .IsRequired();

        builder.Property(pm => pm.MarginPercentage)
            .HasColumnName("margin_percentage")
            .HasColumnType("decimal(5,2)");

        builder.Property(pm => pm.SalePrice)
            .HasColumnName("sale_price")
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.PriceWithIva)
            .HasColumnName("price_with_iva")
            .HasColumnType("decimal(18,2)");

        // Audit properties from TenantAuditableEntity
        builder.Property(pm => pm.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(pm => pm.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(pm => pm.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(pm => pm.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(pm => new { pm.ProductId, pm.TierNumber })
            .IsUnique()
            .HasDatabaseName("ix_product_margin_prices_product_tier");

        // Relationships
        builder.HasOne(pm => pm.Product)
            .WithMany(p => p.MarginPrices)
            .HasForeignKey(pm => pm.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
