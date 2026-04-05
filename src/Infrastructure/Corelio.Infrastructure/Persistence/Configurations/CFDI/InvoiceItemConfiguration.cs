using Corelio.Domain.Entities.CFDI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations.CFDI;

/// <summary>
/// EF Core configuration for the InvoiceItem entity.
/// </summary>
public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("cfdi_invoice_items");

        builder.HasKey(ii => ii.Id);
        builder.Property(ii => ii.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(ii => ii.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(ii => ii.InvoiceId)
            .HasColumnName("invoice_id")
            .IsRequired();

        builder.Property(ii => ii.ItemNumber)
            .HasColumnName("item_number")
            .IsRequired();

        builder.Property(ii => ii.ProductId)
            .HasColumnName("product_id");

        builder.Property(ii => ii.ProductKey)
            .HasColumnName("product_key")
            .HasMaxLength(8)
            .HasDefaultValue("25171500");

        builder.Property(ii => ii.UnitKey)
            .HasColumnName("unit_key")
            .HasMaxLength(3)
            .HasDefaultValue("H87");

        builder.Property(ii => ii.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(ii => ii.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("decimal(10,3)");

        builder.Property(ii => ii.UnitValue)
            .HasColumnName("unit_value")
            .HasColumnType("decimal(15,2)");

        builder.Property(ii => ii.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(15,2)");

        builder.Property(ii => ii.Discount)
            .HasColumnName("discount")
            .HasColumnType("decimal(15,2)")
            .HasDefaultValue(0m);

        builder.Property(ii => ii.TaxObject)
            .HasColumnName("tax_object")
            .HasMaxLength(2)
            .HasDefaultValue("02");

        builder.Property(ii => ii.TaxRate)
            .HasColumnName("tax_rate")
            .HasColumnType("decimal(6,4)")
            .HasDefaultValue(0.16m);

        builder.Property(ii => ii.TaxAmount)
            .HasColumnName("tax_amount")
            .HasColumnType("decimal(15,2)");

        // Audit fields
        builder.Property(ii => ii.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ii => ii.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(ii => ii.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(ii => ii.UpdatedBy)
            .HasColumnName("updated_by");

        // Relationships
        builder.HasOne(ii => ii.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ii => ii.Product)
            .WithMany()
            .HasForeignKey(ii => ii.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(ii => ii.TenantId)
            .HasDatabaseName("ix_cfdi_invoice_items_tenant_id");

        builder.HasIndex(ii => ii.InvoiceId)
            .HasDatabaseName("ix_cfdi_invoice_items_invoice_id");
    }
}
