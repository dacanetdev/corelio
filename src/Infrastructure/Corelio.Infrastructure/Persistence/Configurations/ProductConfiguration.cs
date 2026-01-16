using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Product entity.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        // Primary key
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Tenant ID
        builder.Property(p => p.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // Product Identification
        builder.Property(p => p.Sku)
            .HasColumnName("sku")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Barcode)
            .HasColumnName("barcode")
            .HasMaxLength(100);

        builder.Property(p => p.BarcodeType)
            .HasColumnName("barcode_type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.Property(p => p.ShortDescription)
            .HasColumnName("short_description")
            .HasMaxLength(500);

        // Categorization
        builder.Property(p => p.CategoryId)
            .HasColumnName("category_id");

        builder.Property(p => p.Brand)
            .HasColumnName("brand")
            .HasMaxLength(100);

        builder.Property(p => p.Manufacturer)
            .HasColumnName("manufacturer")
            .HasMaxLength(200);

        builder.Property(p => p.ModelNumber)
            .HasColumnName("model_number")
            .HasMaxLength(100);

        // Pricing
        builder.Property(p => p.CostPrice)
            .HasColumnName("cost_price")
            .HasColumnType("decimal(15,2)")
            .HasDefaultValue(0.00m)
            .IsRequired();

        builder.Property(p => p.SalePrice)
            .HasColumnName("sale_price")
            .HasColumnType("decimal(15,2)")
            .IsRequired();

        builder.Property(p => p.WholesalePrice)
            .HasColumnName("wholesale_price")
            .HasColumnType("decimal(15,2)");

        builder.Property(p => p.Msrp)
            .HasColumnName("msrp")
            .HasColumnType("decimal(15,2)");

        // Tax
        builder.Property(p => p.TaxRate)
            .HasColumnName("tax_rate")
            .HasColumnType("decimal(5,4)")
            .HasDefaultValue(0.1600m)
            .IsRequired();

        builder.Property(p => p.IsTaxExempt)
            .HasColumnName("is_tax_exempt")
            .HasDefaultValue(false)
            .IsRequired();

        // Inventory Management
        builder.Property(p => p.TrackInventory)
            .HasColumnName("track_inventory")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(p => p.UnitOfMeasure)
            .HasColumnName("unit_of_measure")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.MinStockLevel)
            .HasColumnName("min_stock_level")
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(p => p.MaxStockLevel)
            .HasColumnName("max_stock_level")
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.ReorderPoint)
            .HasColumnName("reorder_point")
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.ReorderQuantity)
            .HasColumnName("reorder_quantity")
            .HasColumnType("decimal(10,2)");

        // Physical Properties
        builder.Property(p => p.WeightKg)
            .HasColumnName("weight_kg")
            .HasColumnType("decimal(10,3)");

        builder.Property(p => p.LengthCm)
            .HasColumnName("length_cm")
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.WidthCm)
            .HasColumnName("width_cm")
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.HeightCm)
            .HasColumnName("height_cm")
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.VolumeCm3)
            .HasColumnName("volume_cm3")
            .HasColumnType("decimal(15,2)");

        // CFDI / SAT Compliance
        builder.Property(p => p.SatProductCode)
            .HasColumnName("sat_product_code")
            .HasMaxLength(8);

        builder.Property(p => p.SatUnitCode)
            .HasColumnName("sat_unit_code")
            .HasMaxLength(3);

        builder.Property(p => p.SatHazardousMaterial)
            .HasColumnName("sat_hazardous_material")
            .HasMaxLength(4);

        // Images and Media
        builder.Property(p => p.PrimaryImageUrl)
            .HasColumnName("primary_image_url")
            .HasMaxLength(500);

        builder.Property(p => p.ImagesJson)
            .HasColumnName("images_json")
            .HasColumnType("jsonb");

        // Product Type
        builder.Property(p => p.IsService)
            .HasColumnName("is_service")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(p => p.IsBundle)
            .HasColumnName("is_bundle")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(p => p.IsVariantParent)
            .HasColumnName("is_variant_parent")
            .HasDefaultValue(false)
            .IsRequired();

        // Status
        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(p => p.IsFeatured)
            .HasColumnName("is_featured")
            .HasDefaultValue(false)
            .IsRequired();

        // Soft delete properties
        builder.Property(p => p.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(p => p.DeletedBy)
            .HasColumnName("deleted_by");

        // SEO
        builder.Property(p => p.Slug)
            .HasColumnName("slug")
            .HasMaxLength(300);

        builder.Property(p => p.MetaTitle)
            .HasColumnName("meta_title")
            .HasMaxLength(200);

        builder.Property(p => p.MetaDescription)
            .HasColumnName("meta_description")
            .HasMaxLength(500);

        builder.Property(p => p.MetaKeywords)
            .HasColumnName("meta_keywords")
            .HasMaxLength(500);

        // Audit properties from TenantAuditableEntity
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(p => p.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(p => p.UpdatedBy)
            .HasColumnName("updated_by");

        // Computed properties (ignored in database)
        builder.Ignore(p => p.ProfitMargin);
        builder.Ignore(p => p.MarkupPercentage);

        // Indexes
        builder.HasIndex(p => p.TenantId)
            .HasDatabaseName("ix_products_tenant_id")
            .HasFilter("is_deleted = false");

        builder.HasIndex(p => new { p.TenantId, p.Sku })
            .IsUnique()
            .HasDatabaseName("ix_products_tenant_sku")
            .HasFilter("is_deleted = false");

        builder.HasIndex(p => new { p.TenantId, p.Barcode })
            .IsUnique()
            .HasDatabaseName("ix_products_tenant_barcode")
            .HasFilter("barcode IS NOT NULL AND is_deleted = false");

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("ix_products_category_id");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("ix_products_is_active");

        builder.HasIndex(p => p.IsFeatured)
            .HasDatabaseName("ix_products_is_featured")
            .HasFilter("is_featured = true AND is_deleted = false");

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("ix_products_name")
            .HasFilter("is_deleted = false");

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(pc => pc.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
