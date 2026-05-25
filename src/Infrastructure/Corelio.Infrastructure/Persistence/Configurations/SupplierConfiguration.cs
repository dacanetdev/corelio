using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Supplier entity.
/// </summary>
public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Rfc)
            .HasColumnName("rfc")
            .HasMaxLength(13);

        builder.Property(s => s.ContactName)
            .HasColumnName("contact_name")
            .HasMaxLength(100);

        builder.Property(s => s.Email)
            .HasColumnName("email")
            .HasMaxLength(200);

        builder.Property(s => s.Phone)
            .HasColumnName("phone")
            .HasMaxLength(30);

        builder.Property(s => s.Street)
            .HasColumnName("street")
            .HasMaxLength(300);

        builder.Property(s => s.City)
            .HasColumnName("city")
            .HasMaxLength(100);

        builder.Property(s => s.State)
            .HasColumnName("state")
            .HasMaxLength(100);

        builder.Property(s => s.ZipCode)
            .HasColumnName("zip_code")
            .HasMaxLength(10);

        builder.Property(s => s.PaymentTermsDays)
            .HasColumnName("payment_terms_days")
            .HasDefaultValue(30);

        builder.Property(s => s.TaxRegime)
            .HasColumnName("tax_regime")
            .HasMaxLength(50);

        builder.Property(s => s.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);

        builder.Property(s => s.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        // Soft delete fields
        builder.Property(s => s.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(s => s.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(s => s.DeletedBy)
            .HasColumnName("deleted_by");

        // Audit fields
        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(s => s.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(s => s.UpdatedBy)
            .HasColumnName("updated_by");

        // Indexes
        builder.HasIndex(s => s.TenantId)
            .HasDatabaseName("ix_suppliers_tenant")
            .HasFilter("is_deleted = false");

        builder.HasIndex(s => new { s.TenantId, s.Rfc })
            .IsUnique()
            .HasDatabaseName("ix_suppliers_tenant_rfc")
            .HasFilter("rfc IS NOT NULL AND is_deleted = false");

        // Soft delete query filter
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
