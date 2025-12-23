---
name: database-migration
description: Guides EF Core migration creation with multi-tenant and performance best practices
---

# Database Migration Best Practices

## Migration Workflow

### 1. Create Migration
```bash
# Standard migration
dotnet ef migrations add AddProductTable \
    --project src/Corelio.Infrastructure \
    --startup-project src/Corelio.WebAPI

# With output directory
dotnet ef migrations add AddProductTable \
    --project src/Corelio.Infrastructure \
    --startup-project src/Corelio.WebAPI \
    --output-dir Persistence/Migrations
```

### 2. Review Generated Migration
```bash
# View migration SQL without applying
dotnet ef migrations script \
    --project src/Corelio.Infrastructure \
    --startup-project src/Corelio.WebAPI
```

### 3. Apply Migration
```bash
# Update database
dotnet ef database update \
    --project src/Corelio.Infrastructure \
    --startup-project src/Corelio.WebAPI

# Update to specific migration
dotnet ef database update AddProductTable \
    --project src/Corelio.Infrastructure \
    --startup-project src/Corelio.WebAPI
```

### 4. Rollback Migration
```bash
# Remove last migration (before applying)
dotnet ef migrations remove \
    --project src/Corelio.Infrastructure \
    --startup-project src/Corelio.WebAPI

# Rollback to previous migration (after applying)
dotnet ef database update PreviousMigrationName \
    --project src/Corelio.Infrastructure \
    --startup-project src/Corelio.WebAPI
```

## Entity Configuration Patterns

### ✅ Fluent API Configuration (Preferred)
```csharp
// Corelio.Infrastructure/Persistence/Configurations/ProductConfiguration.cs
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table name
        builder.ToTable("products");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2); // For money

        // Multi-tenancy (CRITICAL)
        builder.Property(p => p.TenantId)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => new { p.TenantId, p.Sku })
            .IsUnique()
            .HasDatabaseName("ix_products_tenant_sku");

        builder.HasIndex(p => new { p.TenantId, p.CategoryId })
            .HasDatabaseName("ix_products_tenant_category");

        // Query filter (CRITICAL for multi-tenancy)
        builder.HasQueryFilter(p =>
            p.TenantId == EF.Property<Guid>(p, "TenantId"));

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
```

### ❌ Avoid Data Annotations
```csharp
// ❌ DON'T USE - Less flexible
public class Product
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
}

// ✅ USE FLUENT API instead
// See ProductConfiguration.cs above
```

## Multi-Tenancy Considerations

### Always Include TenantId in Indexes
```csharp
// ✅ CORRECT - TenantId first for query filter efficiency
builder.HasIndex(p => new { p.TenantId, p.Sku });
builder.HasIndex(s => new { s.TenantId, s.SaleDate });
builder.HasIndex(c => new { c.TenantId, c.Email });

// ❌ WRONG - Missing TenantId
builder.HasIndex(p => p.Sku); // Inefficient for multi-tenant queries
```

### Unique Constraints Per Tenant
```csharp
// ✅ CORRECT - SKU unique per tenant
builder.HasIndex(p => new { p.TenantId, p.Sku })
    .IsUnique();

// ❌ WRONG - SKU unique globally
builder.HasIndex(p => p.Sku)
    .IsUnique(); // Would prevent different tenants from using same SKU
```

### Foreign Keys Must Respect Tenant Boundaries
```csharp
// ✅ CORRECT - Composite foreign key includes TenantId
builder.HasOne(p => p.Category)
    .WithMany(c => c.Products)
    .HasForeignKey(p => new { p.TenantId, p.CategoryId })
    .HasPrincipalKey(c => new { c.TenantId, c.Id });

// ⚠️ ACCEPTABLE - Query filter prevents cross-tenant references
builder.HasOne(p => p.Category)
    .WithMany(c => c.Products)
    .HasForeignKey(p => p.CategoryId);
```

## Performance Optimization

### Covering Indexes
```csharp
// Include frequently selected columns
builder.HasIndex(s => new { s.TenantId, s.SaleDate })
    .IncludeProperties(s => new { s.TotalAmount, s.Status, s.CustomerId })
    .HasDatabaseName("ix_sales_tenant_date_covering");
```

### Filtered Indexes (PostgreSQL)
```csharp
// Index only active products
builder.HasIndex(p => new { p.TenantId, p.CategoryId })
    .HasFilter("is_active = true")
    .HasDatabaseName("ix_products_active");
```

### Composite Indexes (Order Matters)
```csharp
// ✅ CORRECT - TenantId first, then most selective column
builder.HasIndex(s => new { s.TenantId, s.SaleDate, s.Status });

// ❌ LESS EFFICIENT - Status is least selective
builder.HasIndex(s => new { s.Status, s.TenantId, s.SaleDate });
```

### Table Partitioning (Large Tables)
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // For tables with millions of rows
    modelBuilder.Entity<Sale>()
        .ToTable("sales", t => t.HasComment("Partitioned by tenant_id"));

    // Create partitions in migration Up() method
}

// In migration file
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Existing table creation...

    // Add partitioning (PostgreSQL)
    migrationBuilder.Sql(@"
        CREATE TABLE sales_partitioned (LIKE sales INCLUDING ALL)
        PARTITION BY HASH (tenant_id);

        CREATE TABLE sales_p0 PARTITION OF sales_partitioned
        FOR VALUES WITH (MODULUS 4, REMAINDER 0);

        CREATE TABLE sales_p1 PARTITION OF sales_partitioned
        FOR VALUES WITH (MODULUS 4, REMAINDER 1);
        -- etc.
    ");
}
```

## Data Types

### Use Appropriate Types
```csharp
// Money/Currency
builder.Property(p => p.Price)
    .HasPrecision(18, 2) // Max 9,999,999,999,999,999.99
    .IsRequired();

// Percentages (store as decimal, not int)
builder.Property(t => t.TaxRate)
    .HasPrecision(5, 4); // 0.1600 for 16%

// Timestamps (UTC)
builder.Property(e => e.CreatedAt)
    .HasColumnType("timestamp with time zone");

// JSON columns (PostgreSQL)
builder.Property(p => p.Metadata)
    .HasColumnType("jsonb");

// Enums (store as string)
builder.Property(s => s.Status)
    .HasConversion<string>()
    .HasMaxLength(50);

// UUID/GUID
builder.Property(e => e.Id)
    .HasColumnType("uuid");

// RFC (Mexico tax ID) - uppercase, fixed length
builder.Property(c => c.Rfc)
    .HasMaxLength(13)
    .IsRequired()
    .HasColumnType("varchar(13)");
```

## Seed Data

### Global Reference Data (SAT Catalogs)
```csharp
public class SatProductCodeConfiguration : IEntityTypeConfiguration<SatProductCode>
{
    public void Configure(EntityTypeBuilder<SatProductCode> builder)
    {
        builder.ToTable("sat_product_codes");

        // No TenantId - global reference data
        builder.HasKey(s => s.Code);

        // Seed data
        builder.HasData(
            new SatProductCode { Code = "01010101", Description = "Animales vivos" },
            new SatProductCode { Code = "01010102", Description = "Carne y productos cárnicos" }
            // ... more codes
        );
    }
}
```

### Tenant-Specific Seed Data
```csharp
// DON'T seed tenant-specific data in migrations
// Use a seeder service instead
public class TenantDataSeeder
{
    public async Task SeedAsync(Guid tenantId)
    {
        // Create default categories for new tenant
        var categories = new[]
        {
            new ProductCategory { TenantId = tenantId, Name = "General" },
            new ProductCategory { TenantId = tenantId, Name = "Services" }
        };

        await _context.ProductCategories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }
}
```

## Migration Checklist

Before creating a migration:

- [ ] Entity implements `ITenantEntity` (if tenant-specific)
- [ ] Configuration class created in `Persistence/Configurations/`
- [ ] Configuration registered in `ApplicationDbContext.OnModelCreating()`
- [ ] All required properties marked with `IsRequired()`
- [ ] String properties have `HasMaxLength()` set
- [ ] Money/decimal properties have `HasPrecision()` set
- [ ] Indexes include `TenantId` as first column
- [ ] Unique constraints include `TenantId` (if tenant-specific)
- [ ] Foreign keys configured with proper delete behavior
- [ ] Query filter applied (for tenant entities)
- [ ] Audit fields (CreatedAt, UpdatedAt) configured
- [ ] Seed data added (if applicable)

After generating migration:

- [ ] Review generated SQL with `migrations script`
- [ ] Check for missing indexes
- [ ] Verify constraint names are clear
- [ ] Ensure no breaking changes without data migration
- [ ] Test migration on development database
- [ ] Test rollback (`database update PreviousMigration`)
- [ ] Document any manual steps required

## Common Patterns

### Audit Fields
```csharp
public abstract class BaseAuditableEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

// Configuration
builder.Property(e => e.CreatedAt)
    .IsRequired()
    .HasDefaultValueSql("CURRENT_TIMESTAMP");

builder.Property(e => e.UpdatedAt)
    .IsRequired()
    .HasDefaultValueSql("CURRENT_TIMESTAMP");
```

### Soft Delete
```csharp
public abstract class BaseEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

// Configuration
builder.HasQueryFilter(e => !e.IsDeleted);

builder.HasIndex(e => e.IsDeleted)
    .HasFilter("is_deleted = false");
```

### Optimistic Concurrency
```csharp
public class Product
{
    public byte[] RowVersion { get; set; }
}

// Configuration
builder.Property(p => p.RowVersion)
    .IsRowVersion(); // Uses xmin in PostgreSQL
```

## Breaking Changes

### Renaming Columns
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // ✅ CORRECT - Preserves data
    migrationBuilder.RenameColumn(
        name: "ProductName",
        table: "products",
        newName: "Name");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "Name",
        table: "products",
        newName: "ProductName");
}
```

### Changing Column Types
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // ⚠️ May lose data - test carefully!
    migrationBuilder.AlterColumn<string>(
        name: "Sku",
        table: "products",
        maxLength: 100, // Increased from 50
        nullable: false,
        oldMaxLength: 50);
}
```

### Adding Required Columns
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Step 1: Add as nullable
    migrationBuilder.AddColumn<string>(
        name: "Rfc",
        table: "customers",
        maxLength: 13,
        nullable: true);

    // Step 2: Populate existing rows
    migrationBuilder.Sql(@"
        UPDATE customers
        SET rfc = 'XAXX010101000'
        WHERE rfc IS NULL;
    ");

    // Step 3: Make required
    migrationBuilder.AlterColumn<string>(
        name: "Rfc",
        table: "customers",
        maxLength: 13,
        nullable: false);
}
```

## PostgreSQL-Specific Features

### JSONB Columns
```csharp
builder.Property(p => p.Metadata)
    .HasColumnType("jsonb");

// GIN index for JSON queries
migrationBuilder.Sql(@"
    CREATE INDEX ix_products_metadata_gin
    ON products USING gin (metadata);
");
```

### Full-Text Search
```csharp
migrationBuilder.Sql(@"
    ALTER TABLE products
    ADD COLUMN search_vector tsvector
    GENERATED ALWAYS AS (
        to_tsvector('spanish', coalesce(name, '') || ' ' || coalesce(description, ''))
    ) STORED;

    CREATE INDEX ix_products_search_vector
    ON products USING gin (search_vector);
");
```

### Generated Columns
```csharp
migrationBuilder.Sql(@"
    ALTER TABLE sales
    ADD COLUMN total_with_tax decimal(18,2)
    GENERATED ALWAYS AS (subtotal + tax_amount) STORED;
");
```

## Troubleshooting

### Migration Fails
```bash
# Check pending migrations
dotnet ef migrations list --project src/Corelio.Infrastructure

# View SQL that would be executed
dotnet ef migrations script --project src/Corelio.Infrastructure

# Drop database and recreate (DEVELOPMENT ONLY!)
dotnet ef database drop --project src/Corelio.Infrastructure --force
dotnet ef database update --project src/Corelio.Infrastructure
```

### Duplicate Migrations
```bash
# Remove last migration
dotnet ef migrations remove --project src/Corelio.Infrastructure

# Recreate with better name
dotnet ef migrations add DescriptiveName --project src/Corelio.Infrastructure
```

### Model Snapshot Out of Sync
```bash
# Delete all migrations and start fresh (DEVELOPMENT ONLY!)
rm -rf src/Corelio.Infrastructure/Persistence/Migrations/*
dotnet ef migrations add InitialCreate --project src/Corelio.Infrastructure
```

## Production Deployment

### Generate SQL Script
```bash
# Generate script for production deployment
dotnet ef migrations script \
    --project src/Corelio.Infrastructure \
    --startup-project src/Corelio.WebAPI \
    --idempotent \
    --output migration.sql
```

### Apply in CI/CD
```yaml
# GitHub Actions example
- name: Apply Database Migrations
  run: |
    dotnet ef database update \
      --project src/Corelio.Infrastructure \
      --startup-project src/Corelio.WebAPI \
      --connection "${{ secrets.CONNECTION_STRING }}"
```

---

**Remember**: Always test migrations on a copy of production data before deploying!
