using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the ProductCategory entity.
/// </summary>
public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("product_categories");

        // Primary key
        builder.HasKey(pc => pc.Id);
        builder.Property(pc => pc.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Tenant ID
        builder.Property(pc => pc.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // Properties
        builder.Property(pc => pc.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pc => pc.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.Property(pc => pc.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(500);

        builder.Property(pc => pc.ParentCategoryId)
            .HasColumnName("parent_category_id");

        builder.Property(pc => pc.Level)
            .HasColumnName("level")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(pc => pc.Path)
            .HasColumnName("path")
            .HasMaxLength(500);

        builder.Property(pc => pc.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(pc => pc.ColorHex)
            .HasColumnName("color_hex")
            .HasMaxLength(7);

        builder.Property(pc => pc.IconName)
            .HasColumnName("icon_name")
            .HasMaxLength(50);

        builder.Property(pc => pc.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        // Soft delete properties
        builder.Property(pc => pc.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(pc => pc.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(pc => pc.DeletedBy)
            .HasColumnName("deleted_by");

        // Audit properties from TenantAuditableEntity
        builder.Property(pc => pc.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(pc => pc.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(pc => pc.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(pc => pc.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(pc => pc.TenantId)
            .HasDatabaseName("ix_product_categories_tenant_id")
            .HasFilter("is_deleted = false");

        builder.HasIndex(pc => new { pc.TenantId, pc.Name })
            .IsUnique()
            .HasDatabaseName("ix_product_categories_tenant_name")
            .HasFilter("is_deleted = false");

        builder.HasIndex(pc => pc.ParentCategoryId)
            .HasDatabaseName("ix_product_categories_parent_id");

        builder.HasIndex(pc => pc.Path)
            .HasDatabaseName("ix_product_categories_path");

        builder.HasIndex(pc => pc.IsActive)
            .HasDatabaseName("ix_product_categories_is_active");

        // Relationships
        builder.HasOne(pc => pc.ParentCategory)
            .WithMany(pc => pc.ChildCategories)
            .HasForeignKey(pc => pc.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(pc => pc.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Query filter for soft delete
        builder.HasQueryFilter(pc => !pc.IsDeleted);
    }
}
