using Corelio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Corelio.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the AuditLog entity.
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        // Primary key
        builder.HasKey(al => al.Id);
        builder.Property(al => al.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(al => al.TenantId)
            .HasColumnName("tenant_id");

        builder.Property(al => al.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(al => al.EntityType)
            .HasColumnName("entity_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(al => al.EntityId)
            .HasColumnName("entity_id");

        builder.Property(al => al.EntityName)
            .HasColumnName("entity_name")
            .HasMaxLength(200);

        builder.Property(al => al.UserId)
            .HasColumnName("user_id");

        builder.Property(al => al.UserEmail)
            .HasColumnName("user_email")
            .HasMaxLength(256);

        builder.Property(al => al.UserIp)
            .HasColumnName("user_ip")
            .HasMaxLength(45);

        builder.Property(al => al.UserAgent)
            .HasColumnName("user_agent")
            .HasMaxLength(500);

        builder.Property(al => al.OldValues)
            .HasColumnName("old_values")
            .HasColumnType("jsonb");

        builder.Property(al => al.NewValues)
            .HasColumnName("new_values")
            .HasColumnType("jsonb");

        builder.Property(al => al.ChangedFields)
            .HasColumnName("changed_fields")
            .HasColumnType("text[]");

        builder.Property(al => al.RequestMethod)
            .HasColumnName("request_method")
            .HasMaxLength(10);

        builder.Property(al => al.RequestPath)
            .HasColumnName("request_path")
            .HasMaxLength(500);

        builder.Property(al => al.RequestId)
            .HasColumnName("request_id");

        builder.Property(al => al.Success)
            .HasColumnName("success")
            .HasDefaultValue(true);

        builder.Property(al => al.ErrorMessage)
            .HasColumnName("error_message")
            .HasMaxLength(2000);

        // Properties from BaseEntity
        builder.Property(al => al.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(al => al.TenantId)
            .HasDatabaseName("ix_audit_logs_tenant_id");

        builder.HasIndex(al => al.EventType)
            .HasDatabaseName("ix_audit_logs_event_type");

        builder.HasIndex(al => al.EntityType)
            .HasDatabaseName("ix_audit_logs_entity_type");

        builder.HasIndex(al => al.EntityId)
            .HasDatabaseName("ix_audit_logs_entity_id")
            .HasFilter("entity_id IS NOT NULL");

        builder.HasIndex(al => al.UserId)
            .HasDatabaseName("ix_audit_logs_user_id")
            .HasFilter("user_id IS NOT NULL");

        builder.HasIndex(al => al.CreatedAt)
            .HasDatabaseName("ix_audit_logs_created_at");

        // Composite index for common query patterns
        builder.HasIndex(al => new { al.TenantId, al.EntityType, al.CreatedAt })
            .HasDatabaseName("ix_audit_logs_tenant_entity_created");
    }
}
