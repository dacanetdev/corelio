using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations.CFDI;

/// <summary>
/// EF Core configuration for the Invoice entity.
/// </summary>
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("cfdi_invoices");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(i => i.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(i => i.SaleId)
            .HasColumnName("sale_id");

        builder.Property(i => i.Folio)
            .HasColumnName("folio")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Serie)
            .HasColumnName("serie")
            .HasMaxLength(10)
            .HasDefaultValue("A");

        builder.Property(i => i.Uuid)
            .HasColumnName("uuid")
            .HasMaxLength(36);

        builder.Property(i => i.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(CfdiStatus.Draft);

        builder.Property(i => i.InvoiceType)
            .HasColumnName("invoice_type")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(CfdiType.Ingreso);

        // Issuer
        builder.Property(i => i.IssuerRfc)
            .HasColumnName("issuer_rfc")
            .HasMaxLength(13)
            .IsRequired();

        builder.Property(i => i.IssuerName)
            .HasColumnName("issuer_name")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(i => i.IssuerTaxRegime)
            .HasColumnName("issuer_tax_regime")
            .HasMaxLength(3)
            .IsRequired();

        // Receiver
        builder.Property(i => i.ReceiverRfc)
            .HasColumnName("receiver_rfc")
            .HasMaxLength(13)
            .IsRequired();

        builder.Property(i => i.ReceiverName)
            .HasColumnName("receiver_name")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(i => i.ReceiverTaxRegime)
            .HasColumnName("receiver_tax_regime")
            .HasMaxLength(3);

        builder.Property(i => i.ReceiverPostalCode)
            .HasColumnName("receiver_postal_code")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(i => i.ReceiverCfdiUse)
            .HasColumnName("receiver_cfdi_use")
            .HasMaxLength(3)
            .IsRequired();

        // Amounts
        builder.Property(i => i.Subtotal)
            .HasColumnName("subtotal")
            .HasColumnType("decimal(15,2)");

        builder.Property(i => i.Discount)
            .HasColumnName("discount")
            .HasColumnType("decimal(15,2)")
            .HasDefaultValue(0m);

        builder.Property(i => i.Total)
            .HasColumnName("total")
            .HasColumnType("decimal(15,2)");

        // Payment SAT codes
        builder.Property(i => i.PaymentForm)
            .HasColumnName("payment_form")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(i => i.PaymentMethod)
            .HasColumnName("payment_method")
            .HasMaxLength(3)
            .HasDefaultValue("PUE");

        // Stamp data
        builder.Property(i => i.StampDate)
            .HasColumnName("stamp_date");

        builder.Property(i => i.SatCertificateNumber)
            .HasColumnName("sat_certificate_number")
            .HasMaxLength(20);

        builder.Property(i => i.PacStampSignature)
            .HasColumnName("pac_stamp_signature")
            .HasColumnType("text");

        builder.Property(i => i.SatStampSignature)
            .HasColumnName("sat_stamp_signature")
            .HasColumnType("text");

        builder.Property(i => i.QrCodeData)
            .HasColumnName("qr_code_data")
            .HasColumnType("text");

        builder.Property(i => i.OriginalChain)
            .HasColumnName("original_chain")
            .HasColumnType("text");

        builder.Property(i => i.XmlContent)
            .HasColumnName("xml_content")
            .HasColumnType("text");

        // Cancellation
        builder.Property(i => i.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(2);

        builder.Property(i => i.CancellationDate)
            .HasColumnName("cancellation_date");

        // Audit fields
        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(i => i.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(i => i.UpdatedBy)
            .HasColumnName("updated_by");

        // Relationships
        builder.HasOne(i => i.Sale)
            .WithMany()
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(i => i.Items)
            .WithOne(ii => ii.Invoice)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(i => i.TenantId)
            .HasDatabaseName("ix_cfdi_invoices_tenant_id");

        builder.HasIndex(i => i.Uuid)
            .IsUnique()
            .HasFilter("uuid IS NOT NULL")
            .HasDatabaseName("ix_cfdi_invoices_uuid");

        builder.HasIndex(i => new { i.TenantId, i.Serie, i.Folio })
            .IsUnique()
            .HasDatabaseName("ix_cfdi_invoices_tenant_serie_folio");

        builder.HasIndex(i => new { i.TenantId, i.Status })
            .HasDatabaseName("ix_cfdi_invoices_tenant_status");
    }
}
