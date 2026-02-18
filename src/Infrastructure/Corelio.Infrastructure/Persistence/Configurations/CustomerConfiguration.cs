using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Customer entity.
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        // Primary key
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Tenant ID
        builder.Property(c => c.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        // Customer type
        builder.Property(c => c.CustomerType)
            .HasColumnName("customer_type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Name fields
        builder.Property(c => c.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.BusinessName)
            .HasColumnName("business_name")
            .HasMaxLength(200);

        // Mexican tax fields
        builder.Property(c => c.Rfc)
            .HasColumnName("rfc")
            .HasMaxLength(13);

        builder.Property(c => c.Curp)
            .HasColumnName("curp")
            .HasMaxLength(18);

        builder.Property(c => c.TaxRegime)
            .HasColumnName("tax_regime")
            .HasMaxLength(10);

        builder.Property(c => c.CfdiUse)
            .HasColumnName("cfdi_use")
            .HasMaxLength(10);

        // Contact fields
        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        // Preferred payment method
        builder.Property(c => c.PreferredPaymentMethod)
            .HasColumnName("preferred_payment_method")
            .HasConversion<string>()
            .HasMaxLength(20);

        // Soft delete fields
        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(c => c.DeletedBy)
            .HasColumnName("deleted_by");

        // Audit fields
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(c => c.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(c => c.UpdatedBy)
            .HasColumnName("updated_by");

        // Computed property ignored
        builder.Ignore(c => c.FullName);

        // Indexes
        builder.HasIndex(c => c.TenantId)
            .HasDatabaseName("ix_customers_tenant_id")
            .HasFilter("is_deleted = false");

        builder.HasIndex(c => new { c.TenantId, c.Rfc })
            .IsUnique()
            .HasDatabaseName("ix_customers_tenant_rfc")
            .HasFilter("rfc IS NOT NULL AND is_deleted = false");

        builder.HasIndex(c => c.Email)
            .HasDatabaseName("ix_customers_email")
            .HasFilter("is_deleted = false");

        // Soft delete query filter
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
