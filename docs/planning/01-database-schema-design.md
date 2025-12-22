# Corelio - Database Schema Design Specification

## Document Information
- **Project:** Corelio Multi-Tenant SaaS ERP
- **Database:** PostgreSQL 16
- **Version:** 1.0
- **Date:** 2025-12-20
- **Framework:** .NET 10 + EF Core 10

---

## Table of Contents
1. [Overview](#overview)
2. [Multi-Tenancy Strategy](#multi-tenancy-strategy)
3. [Core Tables](#core-tables)
4. [Business Tables](#business-tables)
5. [Indexes and Performance](#indexes-and-performance)
6. [Constraints and Validation](#constraints-and-validation)
7. [Data Types and Standards](#data-types-and-standards)

---

## Overview

### Design Principles
- **Row-Level Security:** Single database, shared schema with tenant_id on all business tables
- **ACID Compliance:** Full transactional integrity for financial data
- **Audit Trail:** Created/updated timestamps and user tracking on all entities
- **Soft Deletes:** Logical deletion with is_deleted flag where appropriate
- **CFDI Compliance:** SAT-specific fields for Mexican tax requirements

### Database Configuration
```sql
-- Enable UUID generation
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Enable full-text search for Spanish
CREATE EXTENSION IF NOT EXISTS "unaccent";

-- Set timezone
SET timezone = 'America/Mexico_City';
```

---

## Multi-Tenancy Strategy

### Tenant Isolation Pattern
All business tables include `tenant_id` column with automatic filtering via EF Core.

```sql
-- Example tenant filter (applied automatically by EF Core)
SELECT * FROM products WHERE tenant_id = '<current-tenant-id>';
```

### Tenant Resolution Flow
```
1. User authenticates → JWT issued with tenant_id claim
2. Middleware extracts tenant_id from JWT
3. EF Core query filter automatically applies WHERE tenant_id = ?
4. Save interceptor automatically sets tenant_id on INSERT
5. Update interceptor prevents cross-tenant modifications
```

---

## Core Tables

### 1. tenants
**Purpose:** Store tenant organization information

```sql
CREATE TABLE tenants (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    -- Business Information
    name VARCHAR(200) NOT NULL,
    legal_name VARCHAR(300) NOT NULL,
    rfc VARCHAR(13) NOT NULL UNIQUE, -- Mexican tax ID

    -- Domain Configuration
    subdomain VARCHAR(100) UNIQUE NOT NULL, -- e.g., 'ferreteria-lopez'
    custom_domain VARCHAR(200) UNIQUE, -- e.g., 'ferreteria-lopez.com'

    -- Subscription
    subscription_plan VARCHAR(50) NOT NULL DEFAULT 'basic', -- basic, premium, enterprise
    subscription_starts_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    subscription_ends_at TIMESTAMP,

    -- Limits
    max_users INT NOT NULL DEFAULT 5,
    max_products INT NOT NULL DEFAULT 1000,
    max_sales_per_month INT NOT NULL DEFAULT 5000,

    -- Status
    is_active BOOLEAN NOT NULL DEFAULT true,
    is_trial BOOLEAN NOT NULL DEFAULT false,
    trial_ends_at TIMESTAMP,

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT chk_rfc_format CHECK (rfc ~ '^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$'),
    CONSTRAINT chk_subdomain_format CHECK (subdomain ~ '^[a-z0-9-]+$'),
    CONSTRAINT chk_subscription_plan CHECK (subscription_plan IN ('basic', 'premium', 'enterprise'))
);

-- Indexes
CREATE INDEX idx_tenants_subdomain ON tenants(subdomain) WHERE is_active = true;
CREATE INDEX idx_tenants_custom_domain ON tenants(custom_domain) WHERE custom_domain IS NOT NULL;
CREATE INDEX idx_tenants_active ON tenants(is_active);

-- Comments
COMMENT ON TABLE tenants IS 'Tenant organizations using the Corelio system';
COMMENT ON COLUMN tenants.rfc IS 'RFC (Registro Federal de Contribuyentes) - Mexican tax ID';
COMMENT ON COLUMN tenants.subdomain IS 'Subdomain for tenant access: subdomain.corelio.com.mx';
```

### 2. tenant_configurations
**Purpose:** Store tenant-specific configuration settings

```sql
CREATE TABLE tenant_configurations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL UNIQUE REFERENCES tenants(id) ON DELETE CASCADE,

    -- CFDI Settings (Mexican Electronic Invoicing)
    cfdi_pac_provider VARCHAR(50), -- 'finkel', 'divertia', etc.
    cfdi_pac_api_url VARCHAR(500),
    cfdi_pac_api_key VARCHAR(500), -- Encrypted
    cfdi_pac_test_mode BOOLEAN NOT NULL DEFAULT true,
    cfdi_certificate_path VARCHAR(500), -- CSD certificate (.cer)
    cfdi_key_path VARCHAR(500), -- Private key (.key)
    cfdi_certificate_password VARCHAR(500), -- Encrypted
    cfdi_certificate_expires_at TIMESTAMP,
    cfdi_series VARCHAR(10) DEFAULT 'A', -- Invoice series (A, B, C, etc.)
    cfdi_next_folio INT NOT NULL DEFAULT 1, -- Auto-incrementing folio number

    -- Business Settings
    default_warehouse_id UUID, -- FK to warehouses (can't add constraint here due to circular dependency)
    default_tax_rate DECIMAL(5,4) NOT NULL DEFAULT 0.1600, -- IVA 16%
    currency VARCHAR(3) NOT NULL DEFAULT 'MXN',
    timezone VARCHAR(50) NOT NULL DEFAULT 'America/Mexico_City',
    business_hours_start TIME DEFAULT '09:00:00',
    business_hours_end TIME DEFAULT '18:00:00',

    -- POS Settings
    pos_auto_print_receipt BOOLEAN NOT NULL DEFAULT false,
    pos_require_customer BOOLEAN NOT NULL DEFAULT false,
    pos_default_payment_method VARCHAR(50) DEFAULT 'cash',
    pos_enable_barcode_scanner BOOLEAN NOT NULL DEFAULT true,
    pos_thermal_printer_name VARCHAR(200),
    pos_receipt_footer TEXT, -- Custom footer text

    -- Pricing Settings
    allow_negative_inventory BOOLEAN NOT NULL DEFAULT false,
    require_product_cost BOOLEAN NOT NULL DEFAULT true,
    auto_calculate_margin BOOLEAN NOT NULL DEFAULT true,

    -- Feature Flags
    feature_multi_warehouse BOOLEAN NOT NULL DEFAULT false,
    feature_ecommerce BOOLEAN NOT NULL DEFAULT false,
    feature_loyalty_program BOOLEAN NOT NULL DEFAULT false,
    feature_purchase_orders BOOLEAN NOT NULL DEFAULT false,

    -- Notification Settings
    email_notifications_enabled BOOLEAN NOT NULL DEFAULT true,
    sms_notifications_enabled BOOLEAN NOT NULL DEFAULT false,
    low_stock_notification_threshold DECIMAL(5,2) DEFAULT 20.00, -- Percentage

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT chk_tax_rate CHECK (default_tax_rate >= 0 AND default_tax_rate <= 1),
    CONSTRAINT chk_currency CHECK (currency IN ('MXN', 'USD')),
    CONSTRAINT chk_pac_provider CHECK (cfdi_pac_provider IN ('finkel', 'divertia', 'sw', 'edicom') OR cfdi_pac_provider IS NULL)
);

-- Indexes
CREATE INDEX idx_tenant_config_tenant ON tenant_configurations(tenant_id);

-- Comments
COMMENT ON TABLE tenant_configurations IS 'Per-tenant configuration and feature flags';
COMMENT ON COLUMN tenant_configurations.cfdi_pac_provider IS 'PAC (Proveedor Autorizado de Certificación) for CFDI stamping';
COMMENT ON COLUMN tenant_configurations.cfdi_series IS 'Invoice series identifier (A, B, C, etc.)';
```

### 3. users
**Purpose:** User accounts with multi-tenant support

```sql
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,

    -- Credentials
    email VARCHAR(320) NOT NULL,
    username VARCHAR(100) NOT NULL,
    password_hash VARCHAR(500) NOT NULL,

    -- Personal Information
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    mobile VARCHAR(20),

    -- Employment Info
    employee_code VARCHAR(50),
    position VARCHAR(100),
    hire_date DATE,

    -- Security
    is_active BOOLEAN NOT NULL DEFAULT true,
    is_email_confirmed BOOLEAN NOT NULL DEFAULT false,
    email_confirmation_token VARCHAR(500),
    password_reset_token VARCHAR(500),
    password_reset_expires_at TIMESTAMP,
    two_factor_enabled BOOLEAN NOT NULL DEFAULT false,
    two_factor_secret VARCHAR(200),

    -- Login Tracking
    last_login_at TIMESTAMP,
    last_login_ip VARCHAR(45), -- IPv6 compatible
    failed_login_attempts INT NOT NULL DEFAULT 0,
    locked_until TIMESTAMP,

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),
    updated_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT uk_users_tenant_email UNIQUE (tenant_id, email),
    CONSTRAINT uk_users_tenant_username UNIQUE (tenant_id, username),
    CONSTRAINT uk_users_tenant_employee_code UNIQUE (tenant_id, employee_code),
    CONSTRAINT chk_email_format CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'),
    CONSTRAINT chk_failed_attempts CHECK (failed_login_attempts >= 0 AND failed_login_attempts <= 10)
);

-- Indexes
CREATE INDEX idx_users_tenant ON users(tenant_id) WHERE is_active = true;
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_username ON users(tenant_id, username);
CREATE INDEX idx_users_active ON users(is_active);

-- Comments
COMMENT ON TABLE users IS 'User accounts scoped to tenants';
COMMENT ON COLUMN users.locked_until IS 'Account lock expiration after failed login attempts';
```

### 4. roles
**Purpose:** Permission-based roles

```sql
CREATE TABLE roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID REFERENCES tenants(id) ON DELETE CASCADE, -- NULL for system roles

    -- Role Information
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),

    -- Type
    is_system_role BOOLEAN NOT NULL DEFAULT false, -- Predefined roles (Owner, Cashier, etc.)
    is_default BOOLEAN NOT NULL DEFAULT false, -- Assigned to new users automatically

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT uk_roles_tenant_name UNIQUE (tenant_id, name),
    CONSTRAINT chk_system_role_no_tenant CHECK (
        (is_system_role = true AND tenant_id IS NULL) OR
        (is_system_role = false AND tenant_id IS NOT NULL)
    )
);

-- Indexes
CREATE INDEX idx_roles_tenant ON roles(tenant_id);
CREATE INDEX idx_roles_system ON roles(is_system_role) WHERE is_system_role = true;

-- System Roles (Seeded Data)
INSERT INTO roles (id, name, description, is_system_role, tenant_id) VALUES
    ('00000000-0000-0000-0000-000000000001', 'Owner', 'Full system access - all permissions', true, NULL),
    ('00000000-0000-0000-0000-000000000002', 'Administrator', 'Administrative access - user and configuration management', true, NULL),
    ('00000000-0000-0000-0000-000000000003', 'Cashier', 'Point of sale operations', true, NULL),
    ('00000000-0000-0000-0000-000000000004', 'InventoryManager', 'Inventory and product management', true, NULL),
    ('00000000-0000-0000-0000-000000000005', 'Accountant', 'Financial reports and CFDI management', true, NULL),
    ('00000000-0000-0000-0000-000000000006', 'Seller', 'Sales and customer management', true, NULL);
```

### 5. permissions
**Purpose:** Granular permission definitions

```sql
CREATE TABLE permissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    -- Permission Identifier
    code VARCHAR(100) NOT NULL UNIQUE, -- e.g., 'sales.create', 'products.delete'
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),

    -- Categorization
    module VARCHAR(50) NOT NULL, -- 'sales', 'products', 'inventory', 'cfdi', etc.
    category VARCHAR(50), -- 'create', 'read', 'update', 'delete', 'manage'

    -- Metadata
    is_dangerous BOOLEAN NOT NULL DEFAULT false, -- Requires extra confirmation
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT chk_permission_code_format CHECK (code ~ '^[a-z_]+\.[a-z_]+$'),
    CONSTRAINT chk_module CHECK (module IN ('sales', 'products', 'inventory', 'customers', 'cfdi', 'users', 'reports', 'settings'))
);

-- Indexes
CREATE INDEX idx_permissions_module ON permissions(module);
CREATE INDEX idx_permissions_code ON permissions(code);

-- Seed Permissions
INSERT INTO permissions (code, name, module, category, description, is_dangerous) VALUES
    -- Products
    ('products.view', 'View Products', 'products', 'read', 'View product catalog', false),
    ('products.create', 'Create Products', 'products', 'create', 'Create new products', false),
    ('products.edit', 'Edit Products', 'products', 'update', 'Modify existing products', false),
    ('products.delete', 'Delete Products', 'products', 'delete', 'Delete products', true),
    ('products.pricing', 'Manage Pricing', 'products', 'manage', 'Update product prices', false),

    -- Sales
    ('sales.create', 'Create Sales', 'sales', 'create', 'Process sales transactions', false),
    ('sales.view', 'View Sales', 'sales', 'read', 'View sales history', false),
    ('sales.cancel', 'Cancel Sales', 'sales', 'delete', 'Cancel completed sales', true),
    ('sales.discount', 'Apply Discounts', 'sales', 'manage', 'Apply discounts to sales', false),

    -- Inventory
    ('inventory.view', 'View Inventory', 'inventory', 'read', 'View inventory levels', false),
    ('inventory.adjust', 'Adjust Inventory', 'inventory', 'update', 'Perform inventory adjustments', false),
    ('inventory.transfer', 'Transfer Stock', 'inventory', 'manage', 'Transfer stock between warehouses', false),

    -- Customers
    ('customers.view', 'View Customers', 'customers', 'read', 'View customer information', false),
    ('customers.create', 'Create Customers', 'customers', 'create', 'Add new customers', false),
    ('customers.edit', 'Edit Customers', 'customers', 'update', 'Modify customer information', false),
    ('customers.delete', 'Delete Customers', 'customers', 'delete', 'Delete customers', true),

    -- CFDI
    ('cfdi.generate', 'Generate Invoices', 'cfdi', 'create', 'Generate CFDI invoices', false),
    ('cfdi.cancel', 'Cancel Invoices', 'cfdi', 'delete', 'Cancel CFDI invoices', true),
    ('cfdi.view', 'View Invoices', 'cfdi', 'read', 'View CFDI invoices', false),

    -- Users
    ('users.view', 'View Users', 'users', 'read', 'View user accounts', false),
    ('users.create', 'Create Users', 'users', 'create', 'Create new user accounts', false),
    ('users.edit', 'Edit Users', 'users', 'update', 'Modify user accounts', false),
    ('users.delete', 'Delete Users', 'users', 'delete', 'Deactivate user accounts', true),
    ('users.roles', 'Manage User Roles', 'users', 'manage', 'Assign and remove user roles', true),

    -- Reports
    ('reports.view', 'View Reports', 'reports', 'read', 'Access standard reports', false),
    ('reports.financial', 'Financial Reports', 'reports', 'read', 'Access financial reports', false),

    -- Settings
    ('settings.view', 'View Settings', 'settings', 'read', 'View system settings', false),
    ('settings.edit', 'Edit Settings', 'settings', 'update', 'Modify system settings', true);
```

### 6. role_permissions
**Purpose:** Many-to-many mapping between roles and permissions

```sql
CREATE TABLE role_permissions (
    role_id UUID NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    permission_id UUID NOT NULL REFERENCES permissions(id) ON DELETE CASCADE,

    -- Metadata
    assigned_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    assigned_by UUID REFERENCES users(id),

    PRIMARY KEY (role_id, permission_id)
);

-- Indexes
CREATE INDEX idx_role_permissions_role ON role_permissions(role_id);
CREATE INDEX idx_role_permissions_permission ON role_permissions(permission_id);

-- Seed Role Permissions
-- Owner: All permissions (use wildcard in code)
-- Cashier: Limited to POS operations
INSERT INTO role_permissions (role_id, permission_id)
SELECT '00000000-0000-0000-0000-000000000003', id FROM permissions
WHERE code IN ('sales.create', 'sales.view', 'products.view', 'customers.view', 'customers.create');

-- InventoryManager: Products and inventory
INSERT INTO role_permissions (role_id, permission_id)
SELECT '00000000-0000-0000-0000-000000000004', id FROM permissions
WHERE code IN ('products.view', 'products.create', 'products.edit', 'inventory.view', 'inventory.adjust', 'inventory.transfer');

-- Accountant: CFDI and reports
INSERT INTO role_permissions (role_id, permission_id)
SELECT '00000000-0000-0000-0000-000000000005', id FROM permissions
WHERE code IN ('cfdi.generate', 'cfdi.cancel', 'cfdi.view', 'sales.view', 'reports.view', 'reports.financial');
```

### 7. user_roles
**Purpose:** Many-to-many mapping between users and roles

```sql
CREATE TABLE user_roles (
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role_id UUID NOT NULL REFERENCES roles(id) ON DELETE CASCADE,

    -- Metadata
    assigned_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    assigned_by UUID REFERENCES users(id),
    expires_at TIMESTAMP, -- Optional: temporary role assignment

    PRIMARY KEY (user_id, role_id)
);

-- Indexes
CREATE INDEX idx_user_roles_user ON user_roles(user_id);
CREATE INDEX idx_user_roles_role ON user_roles(role_id);
CREATE INDEX idx_user_roles_expires ON user_roles(expires_at) WHERE expires_at IS NOT NULL;
```

---

## Business Tables

### 8. product_categories
**Purpose:** Hierarchical product categorization

```sql
CREATE TABLE product_categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,

    -- Category Information
    name VARCHAR(200) NOT NULL,
    description TEXT,
    image_url VARCHAR(500),

    -- Hierarchy
    parent_category_id UUID REFERENCES product_categories(id) ON DELETE SET NULL,
    level INT NOT NULL DEFAULT 0, -- Denormalized for performance
    path VARCHAR(500), -- Materialized path: /hardware/tools/power-tools/

    -- Display
    sort_order INT NOT NULL DEFAULT 0,
    color_hex VARCHAR(7), -- For UI display
    icon_name VARCHAR(50),

    -- Status
    is_active BOOLEAN NOT NULL DEFAULT true,

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),
    updated_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT uk_categories_tenant_name UNIQUE (tenant_id, name),
    CONSTRAINT chk_no_self_parent CHECK (id != parent_category_id),
    CONSTRAINT chk_level CHECK (level >= 0 AND level <= 5)
);

-- Indexes
CREATE INDEX idx_categories_tenant ON product_categories(tenant_id) WHERE is_active = true;
CREATE INDEX idx_categories_parent ON product_categories(parent_category_id);
CREATE INDEX idx_categories_path ON product_categories USING gist(path gist_trgm_ops);
```

### 9. products
**Purpose:** Product catalog with CFDI compliance fields

```sql
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,

    -- Product Identification
    sku VARCHAR(100) NOT NULL,
    barcode VARCHAR(100),
    barcode_type VARCHAR(20) DEFAULT 'EAN13', -- EAN13, UPC, CODE128, QR
    name VARCHAR(300) NOT NULL,
    description TEXT,
    short_description VARCHAR(500),

    -- Categorization
    category_id UUID REFERENCES product_categories(id) ON DELETE SET NULL,
    brand VARCHAR(100),
    manufacturer VARCHAR(200),
    model_number VARCHAR(100),

    -- Pricing (in tenant's currency)
    cost_price DECIMAL(15,2) NOT NULL DEFAULT 0.00,
    sale_price DECIMAL(15,2) NOT NULL,
    wholesale_price DECIMAL(15,2),
    msrp DECIMAL(15,2), -- Manufacturer's Suggested Retail Price

    -- Tax
    tax_rate DECIMAL(5,4) NOT NULL DEFAULT 0.1600, -- IVA 16% default
    is_tax_exempt BOOLEAN NOT NULL DEFAULT false,

    -- Inventory Management
    track_inventory BOOLEAN NOT NULL DEFAULT true,
    unit_of_measure VARCHAR(20) NOT NULL DEFAULT 'PCS', -- PCS, KG, M, L, etc.
    min_stock_level DECIMAL(10,2) DEFAULT 0,
    max_stock_level DECIMAL(10,2),
    reorder_point DECIMAL(10,2),
    reorder_quantity DECIMAL(10,2),

    -- Physical Properties
    weight_kg DECIMAL(10,3),
    length_cm DECIMAL(10,2),
    width_cm DECIMAL(10,2),
    height_cm DECIMAL(10,2),
    volume_cm3 DECIMAL(15,2),

    -- CFDI / SAT Compliance (Mexico)
    sat_product_code VARCHAR(8), -- Clave de producto/servicio SAT
    sat_unit_code VARCHAR(3), -- Clave de unidad SAT (e.g., E48, H87)
    sat_hazardous_material VARCHAR(4), -- Clave material peligroso (optional)

    -- Images and Media
    primary_image_url VARCHAR(500),
    images_json JSONB, -- Array of image URLs

    -- Product Type
    is_service BOOLEAN NOT NULL DEFAULT false,
    is_bundle BOOLEAN NOT NULL DEFAULT false,
    is_variant_parent BOOLEAN NOT NULL DEFAULT false,

    -- Status
    is_active BOOLEAN NOT NULL DEFAULT true,
    is_featured BOOLEAN NOT NULL DEFAULT false,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    deleted_at TIMESTAMP,
    deleted_by UUID REFERENCES users(id),

    -- SEO (for future e-commerce)
    slug VARCHAR(300),
    meta_title VARCHAR(200),
    meta_description VARCHAR(500),
    meta_keywords VARCHAR(500),

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),
    updated_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT uk_products_tenant_sku UNIQUE (tenant_id, sku),
    CONSTRAINT uk_products_tenant_barcode UNIQUE (tenant_id, barcode),
    CONSTRAINT chk_prices CHECK (cost_price >= 0 AND sale_price >= 0),
    CONSTRAINT chk_tax_rate CHECK (tax_rate >= 0 AND tax_rate <= 1),
    CONSTRAINT chk_sat_product_code CHECK (sat_product_code ~ '^[0-9]{8}$' OR sat_product_code IS NULL),
    CONSTRAINT chk_sat_unit_code CHECK (sat_unit_code ~ '^[A-Z0-9]{2,3}$' OR sat_unit_code IS NULL)
);

-- Indexes
CREATE INDEX idx_products_tenant ON products(tenant_id) WHERE is_deleted = false;
CREATE INDEX idx_products_sku ON products(tenant_id, sku);
CREATE INDEX idx_products_barcode ON products(tenant_id, barcode) WHERE barcode IS NOT NULL;
CREATE INDEX idx_products_category ON products(category_id) WHERE is_active = true;
CREATE INDEX idx_products_active ON products(is_active) WHERE is_deleted = false;
CREATE INDEX idx_products_featured ON products(is_featured) WHERE is_featured = true AND is_deleted = false;

-- Full-text search (Spanish)
CREATE INDEX idx_products_name_search ON products USING gin(to_tsvector('spanish', name));
CREATE INDEX idx_products_description_search ON products USING gin(to_tsvector('spanish', COALESCE(description, '')));

-- Trigram search for fuzzy matching
CREATE INDEX idx_products_name_trgm ON products USING gin(name gin_trgm_ops);

-- Comments
COMMENT ON COLUMN products.sat_product_code IS 'SAT product/service code for CFDI (8 digits)';
COMMENT ON COLUMN products.sat_unit_code IS 'SAT unit of measure code for CFDI';
```

### 10. product_variants
**Purpose:** Product variations (size, color, etc.)

```sql
CREATE TABLE product_variants (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES products(id) ON DELETE CASCADE,

    -- Variant Identification
    sku VARCHAR(100) NOT NULL,
    barcode VARCHAR(100),
    name VARCHAR(200) NOT NULL,

    -- Variant Attributes (JSON for flexibility)
    variant_attributes JSONB NOT NULL, -- {"color": "red", "size": "M", "material": "cotton"}

    -- Pricing Override (optional)
    cost_price DECIMAL(15,2),
    sale_price DECIMAL(15,2),

    -- Physical Properties Override
    weight_kg DECIMAL(10,3),

    -- Images
    image_url VARCHAR(500),

    -- Status
    is_active BOOLEAN NOT NULL DEFAULT true,

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT uk_variants_tenant_sku UNIQUE (tenant_id, sku),
    CONSTRAINT uk_variants_tenant_barcode UNIQUE (tenant_id, barcode)
);

-- Indexes
CREATE INDEX idx_variants_tenant ON product_variants(tenant_id);
CREATE INDEX idx_variants_product ON product_variants(product_id);
CREATE INDEX idx_variants_attributes ON product_variants USING gin(variant_attributes);
```

### 11. warehouses
**Purpose:** Physical inventory locations

```sql
CREATE TABLE warehouses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,

    -- Warehouse Information
    name VARCHAR(200) NOT NULL,
    code VARCHAR(50) NOT NULL,
    description TEXT,

    -- Location
    address TEXT,
    city VARCHAR(100),
    state VARCHAR(100),
    postal_code VARCHAR(10),
    country VARCHAR(3) DEFAULT 'MEX',
    latitude DECIMAL(10,8),
    longitude DECIMAL(11,8),

    -- Contact
    manager_name VARCHAR(200),
    phone VARCHAR(20),
    email VARCHAR(320),

    -- Type
    warehouse_type VARCHAR(50) DEFAULT 'main', -- main, secondary, retail, virtual

    -- Status
    is_primary BOOLEAN NOT NULL DEFAULT false,
    is_active BOOLEAN NOT NULL DEFAULT true,

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT uk_warehouses_tenant_code UNIQUE (tenant_id, code),
    CONSTRAINT uk_warehouses_tenant_name UNIQUE (tenant_id, name),
    CONSTRAINT chk_warehouse_type CHECK (warehouse_type IN ('main', 'secondary', 'retail', 'virtual', 'consignment'))
);

-- Indexes
CREATE INDEX idx_warehouses_tenant ON warehouses(tenant_id) WHERE is_active = true;
CREATE INDEX idx_warehouses_primary ON warehouses(tenant_id, is_primary) WHERE is_primary = true;

-- Ensure only one primary warehouse per tenant
CREATE UNIQUE INDEX uk_warehouses_one_primary_per_tenant
ON warehouses(tenant_id) WHERE is_primary = true;
```

### 12. inventory_items
**Purpose:** Current inventory levels per warehouse

```sql
CREATE TABLE inventory_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    warehouse_id UUID NOT NULL REFERENCES warehouses(id) ON DELETE CASCADE,

    -- Quantities
    quantity DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    reserved_quantity DECIMAL(10,2) NOT NULL DEFAULT 0.00, -- For pending orders
    available_quantity DECIMAL(10,2) GENERATED ALWAYS AS (quantity - reserved_quantity) STORED,

    -- Cost Tracking (weighted average)
    average_cost DECIMAL(15,2) NOT NULL DEFAULT 0.00,

    -- Inventory Audit
    last_restock_date TIMESTAMP,
    last_restock_quantity DECIMAL(10,2),
    last_counted_date TIMESTAMP,
    last_counted_by UUID REFERENCES users(id),

    -- Timestamps
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT uk_inventory_tenant_product_warehouse UNIQUE (tenant_id, product_id, warehouse_id),
    CONSTRAINT chk_quantities CHECK (
        quantity >= 0 AND
        reserved_quantity >= 0 AND
        reserved_quantity <= quantity
    )
);

-- Indexes
CREATE INDEX idx_inventory_tenant ON inventory_items(tenant_id);
CREATE INDEX idx_inventory_product ON inventory_items(product_id);
CREATE INDEX idx_inventory_warehouse ON inventory_items(warehouse_id);
CREATE INDEX idx_inventory_available ON inventory_items(available_quantity);

-- Low stock alert view
CREATE INDEX idx_inventory_low_stock ON inventory_items(tenant_id, product_id)
WHERE available_quantity <= (
    SELECT min_stock_level
    FROM products
    WHERE products.id = inventory_items.product_id
);
```

### 13. inventory_transactions
**Purpose:** Complete audit trail of inventory movements

```sql
CREATE TABLE inventory_transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES products(id),
    warehouse_id UUID NOT NULL REFERENCES warehouses(id),

    -- Transaction Details
    transaction_type VARCHAR(50) NOT NULL,
    quantity DECIMAL(10,2) NOT NULL, -- Positive for additions, negative for deductions
    unit_cost DECIMAL(15,2),
    total_cost DECIMAL(15,2),

    -- Balance After Transaction
    balance_after DECIMAL(10,2) NOT NULL,

    -- Reference
    reference_type VARCHAR(50), -- 'sale', 'purchase', 'adjustment', 'transfer', 'return'
    reference_id UUID, -- ID of the related document
    reference_number VARCHAR(50),

    -- Additional Info
    notes TEXT,
    reason_code VARCHAR(50),

    -- Timestamps
    transaction_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT chk_transaction_type CHECK (transaction_type IN (
        'purchase', 'sale', 'adjustment_positive', 'adjustment_negative',
        'transfer_in', 'transfer_out', 'return', 'damaged', 'lost', 'found'
    )),
    CONSTRAINT chk_reference_type CHECK (reference_type IN (
        'sale', 'purchase_order', 'adjustment', 'transfer', 'return', 'manual'
    ) OR reference_type IS NULL)
);

-- Indexes
CREATE INDEX idx_inv_trans_tenant ON inventory_transactions(tenant_id);
CREATE INDEX idx_inv_trans_product ON inventory_transactions(product_id);
CREATE INDEX idx_inv_trans_warehouse ON inventory_transactions(warehouse_id);
CREATE INDEX idx_inv_trans_date ON inventory_transactions(transaction_date DESC);
CREATE INDEX idx_inv_trans_reference ON inventory_transactions(reference_type, reference_id);
CREATE INDEX idx_inv_trans_type ON inventory_transactions(transaction_type);

-- Partition by month for performance (optional, for high-volume systems)
-- CREATE TABLE inventory_transactions_y2025m01 PARTITION OF inventory_transactions
-- FOR VALUES FROM ('2025-01-01') TO ('2025-02-01');
```

### 14. stock_movements
**Purpose:** Inter-warehouse transfer requests

```sql
CREATE TABLE stock_movements (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,

    -- Movement Number
    movement_number VARCHAR(50) NOT NULL,

    -- Product
    product_id UUID NOT NULL REFERENCES products(id),
    quantity DECIMAL(10,2) NOT NULL,

    -- Warehouses
    from_warehouse_id UUID REFERENCES warehouses(id),
    to_warehouse_id UUID NOT NULL REFERENCES warehouses(id),

    -- Status
    status VARCHAR(20) NOT NULL DEFAULT 'pending',

    -- Dates
    requested_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    approved_date TIMESTAMP,
    shipped_date TIMESTAMP,
    received_date TIMESTAMP,

    -- Notes
    notes TEXT,
    rejection_reason TEXT,

    -- Users
    requested_by UUID REFERENCES users(id),
    approved_by UUID REFERENCES users(id),
    received_by UUID REFERENCES users(id),

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT uk_stock_movements_number UNIQUE (tenant_id, movement_number),
    CONSTRAINT chk_different_warehouses CHECK (from_warehouse_id != to_warehouse_id),
    CONSTRAINT chk_quantity CHECK (quantity > 0),
    CONSTRAINT chk_status CHECK (status IN ('pending', 'approved', 'in_transit', 'received', 'cancelled', 'rejected'))
);

-- Indexes
CREATE INDEX idx_stock_movements_tenant ON stock_movements(tenant_id);
CREATE INDEX idx_stock_movements_status ON stock_movements(status);
CREATE INDEX idx_stock_movements_from_warehouse ON stock_movements(from_warehouse_id);
CREATE INDEX idx_stock_movements_to_warehouse ON stock_movements(to_warehouse_id);
CREATE INDEX idx_stock_movements_product ON stock_movements(product_id);
```

---

### 15. customers
**Purpose:** Customer master data with Mexican CFDI compliance

```sql
CREATE TABLE customers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,

    -- Customer Type
    customer_type VARCHAR(20) NOT NULL DEFAULT 'individual', -- individual, business

    -- Personal/Business Name
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    business_name VARCHAR(300),
    trade_name VARCHAR(300),

    -- Mexican Tax Identifiers (for CFDI)
    rfc VARCHAR(13), -- RFC for businesses or individuals with tax obligations
    curp VARCHAR(18), -- CURP for individuals
    tax_regime VARCHAR(3), -- SAT tax regime code (e.g., 601, 603, 612)

    -- Contact Information
    email VARCHAR(320),
    phone VARCHAR(20),
    mobile VARCHAR(20),
    website VARCHAR(200),

    -- CFDI Preferences
    cfdi_use VARCHAR(3) DEFAULT 'G03', -- SAT CFDI use code (G01, G02, G03, etc.)
    cfdi_email VARCHAR(320), -- Email for invoice delivery
    cfdi_payment_method VARCHAR(3) DEFAULT 'PUE', -- PUE (Pago en una sola exhibición), PPD (Pago en parcialidades o diferido)
    cfdi_payment_form VARCHAR(2) DEFAULT '01', -- 01=Cash, 02=Check, 03=Transfer, 04=Card, etc.

    -- Customer Category
    customer_category VARCHAR(50), -- retail, wholesale, vip, distributor

    -- Credit Settings
    credit_limit DECIMAL(15,2) DEFAULT 0.00,
    credit_days INT DEFAULT 0,
    current_balance DECIMAL(15,2) DEFAULT 0.00,

    -- Preferences
    preferred_payment_method VARCHAR(50),
    discount_percentage DECIMAL(5,2) DEFAULT 0.00,
    price_list VARCHAR(50) DEFAULT 'retail', -- retail, wholesale, distributor

    -- Notes
    notes TEXT,
    tags VARCHAR(500), -- Comma-separated tags

    -- Status
    is_active BOOLEAN NOT NULL DEFAULT true,
    is_vip BOOLEAN NOT NULL DEFAULT false,
    is_blocked BOOLEAN NOT NULL DEFAULT false,
    blocked_reason TEXT,

    -- Statistics (denormalized for performance)
    total_purchases DECIMAL(15,2) DEFAULT 0.00,
    total_purchases_count INT DEFAULT 0,
    last_purchase_date TIMESTAMP,
    first_purchase_date TIMESTAMP,

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),
    updated_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT chk_customer_type CHECK (customer_type IN ('individual', 'business')),
    CONSTRAINT chk_rfc_format CHECK (rfc ~ '^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$' OR rfc IS NULL),
    CONSTRAINT chk_curp_format CHECK (curp ~ '^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9]{2}$' OR curp IS NULL),
    CONSTRAINT chk_business_name_required CHECK (
        (customer_type = 'business' AND business_name IS NOT NULL) OR
        (customer_type = 'individual' AND first_name IS NOT NULL AND last_name IS NOT NULL)
    ),
    CONSTRAINT chk_credit_limit CHECK (credit_limit >= 0),
    CONSTRAINT chk_discount CHECK (discount_percentage >= 0 AND discount_percentage <= 100),
    CONSTRAINT uk_customers_tenant_rfc UNIQUE (tenant_id, rfc)
);

-- Indexes
CREATE INDEX idx_customers_tenant ON customers(tenant_id) WHERE is_active = true;
CREATE INDEX idx_customers_rfc ON customers(tenant_id, rfc) WHERE rfc IS NOT NULL;
CREATE INDEX idx_customers_email ON customers(email) WHERE email IS NOT NULL;
CREATE INDEX idx_customers_phone ON customers(phone) WHERE phone IS NOT NULL;
CREATE INDEX idx_customers_category ON customers(customer_category);
CREATE INDEX idx_customers_blocked ON customers(is_blocked) WHERE is_blocked = true;

-- Full-text search
CREATE INDEX idx_customers_name_search ON customers USING gin(
    to_tsvector('spanish', COALESCE(first_name || ' ' || last_name, business_name, ''))
);
CREATE INDEX idx_customers_name_trgm ON customers USING gin(
    COALESCE(first_name || ' ' || last_name, business_name) gin_trgm_ops
);

-- Comments
COMMENT ON TABLE customers IS 'Customer master data with Mexican CFDI compliance';
COMMENT ON COLUMN customers.rfc IS 'RFC (Registro Federal de Contribuyentes) - Mexican tax ID';
COMMENT ON COLUMN customers.curp IS 'CURP (Clave Única de Registro de Población) - Mexican citizen ID';
COMMENT ON COLUMN customers.cfdi_use IS 'SAT CFDI use code (UsoCFDI catalog)';
```

### 16. customer_addresses
**Purpose:** Customer shipping and billing addresses

```sql
CREATE TABLE customer_addresses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    customer_id UUID NOT NULL REFERENCES customers(id) ON DELETE CASCADE,

    -- Address Type
    address_type VARCHAR(20) NOT NULL DEFAULT 'billing', -- billing, shipping, both

    -- Address Details
    street VARCHAR(300) NOT NULL,
    exterior_number VARCHAR(20),
    interior_number VARCHAR(20),
    neighborhood VARCHAR(100), -- Colonia
    city VARCHAR(100) NOT NULL,
    municipality VARCHAR(100), -- Municipio/Delegación
    state VARCHAR(100) NOT NULL,
    postal_code VARCHAR(10) NOT NULL,
    country VARCHAR(3) NOT NULL DEFAULT 'MEX',

    -- Location
    latitude DECIMAL(10,8),
    longitude DECIMAL(11,8),

    -- Contact
    contact_name VARCHAR(200),
    contact_phone VARCHAR(20),
    delivery_notes TEXT,

    -- Status
    is_default BOOLEAN NOT NULL DEFAULT false,
    is_active BOOLEAN NOT NULL DEFAULT true,

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT chk_address_type CHECK (address_type IN ('billing', 'shipping', 'both')),
    CONSTRAINT chk_postal_code CHECK (postal_code ~ '^[0-9]{5}$'),
    CONSTRAINT chk_country CHECK (country IN ('MEX', 'USA', 'CAN'))
);

-- Indexes
CREATE INDEX idx_customer_addresses_tenant ON customer_addresses(tenant_id);
CREATE INDEX idx_customer_addresses_customer ON customer_addresses(customer_id);
CREATE INDEX idx_customer_addresses_default ON customer_addresses(customer_id, is_default) WHERE is_default = true;
CREATE INDEX idx_customer_addresses_postal_code ON customer_addresses(postal_code);

-- Ensure only one default address per type per customer
CREATE UNIQUE INDEX uk_customer_addresses_default_billing
ON customer_addresses(customer_id)
WHERE is_default = true AND address_type IN ('billing', 'both');

CREATE UNIQUE INDEX uk_customer_addresses_default_shipping
ON customer_addresses(customer_id)
WHERE is_default = true AND address_type IN ('shipping', 'both');
```

### 17. sales
**Purpose:** All sales transactions (POS, invoices, quotes, credit notes)

```sql
CREATE TABLE sales (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,

    -- Sale Number
    sale_number VARCHAR(50) NOT NULL,
    sale_type VARCHAR(20) NOT NULL DEFAULT 'pos', -- pos, invoice, quote, credit_note, debit_note

    -- Customer
    customer_id UUID REFERENCES customers(id) ON DELETE SET NULL,
    customer_name VARCHAR(300), -- Denormalized for performance and history

    -- Status
    status VARCHAR(20) NOT NULL DEFAULT 'draft', -- draft, completed, cancelled, refunded, partially_paid, paid

    -- Financial Summary
    subtotal DECIMAL(15,2) NOT NULL DEFAULT 0.00,
    discount_percentage DECIMAL(5,2) DEFAULT 0.00,
    discount_amount DECIMAL(15,2) DEFAULT 0.00,
    tax_amount DECIMAL(15,2) NOT NULL DEFAULT 0.00,
    total DECIMAL(15,2) NOT NULL,

    -- Payment
    paid_amount DECIMAL(15,2) DEFAULT 0.00,
    balance_due DECIMAL(15,2) GENERATED ALWAYS AS (total - paid_amount) STORED,
    payment_status VARCHAR(20) DEFAULT 'unpaid', -- unpaid, partial, paid, overpaid

    -- References
    original_sale_id UUID REFERENCES sales(id), -- For credit/debit notes
    quote_id UUID REFERENCES sales(id), -- If converted from quote

    -- Warehouse
    warehouse_id UUID REFERENCES warehouses(id),

    -- Dates
    sale_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    due_date TIMESTAMP,
    completed_at TIMESTAMP,
    cancelled_at TIMESTAMP,

    -- Cancellation
    cancellation_reason TEXT,
    cancelled_by UUID REFERENCES users(id),

    -- Notes
    notes TEXT,
    internal_notes TEXT,

    -- CFDI Reference (if invoiced)
    cfdi_invoice_id UUID, -- FK to cfdi_invoices (added later to avoid circular dependency)

    -- POS Specific
    pos_terminal VARCHAR(50),
    pos_cashier_id UUID REFERENCES users(id),

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),
    updated_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT uk_sales_tenant_number UNIQUE (tenant_id, sale_number),
    CONSTRAINT chk_sale_type CHECK (sale_type IN ('pos', 'invoice', 'quote', 'credit_note', 'debit_note')),
    CONSTRAINT chk_status CHECK (status IN ('draft', 'completed', 'cancelled', 'refunded', 'pending')),
    CONSTRAINT chk_payment_status CHECK (payment_status IN ('unpaid', 'partial', 'paid', 'overpaid')),
    CONSTRAINT chk_amounts CHECK (
        subtotal >= 0 AND
        discount_amount >= 0 AND
        tax_amount >= 0 AND
        total >= 0 AND
        paid_amount >= 0
    ),
    CONSTRAINT chk_discount_percentage CHECK (discount_percentage >= 0 AND discount_percentage <= 100)
);

-- Indexes
CREATE INDEX idx_sales_tenant ON sales(tenant_id);
CREATE INDEX idx_sales_number ON sales(tenant_id, sale_number);
CREATE INDEX idx_sales_customer ON sales(customer_id);
CREATE INDEX idx_sales_type ON sales(sale_type);
CREATE INDEX idx_sales_status ON sales(status);
CREATE INDEX idx_sales_payment_status ON sales(payment_status);
CREATE INDEX idx_sales_date ON sales(sale_date DESC);
CREATE INDEX idx_sales_warehouse ON sales(warehouse_id);
CREATE INDEX idx_sales_created_by ON sales(created_by);
CREATE INDEX idx_sales_cfdi ON sales(cfdi_invoice_id) WHERE cfdi_invoice_id IS NOT NULL;

-- Composite indexes for common queries
CREATE INDEX idx_sales_tenant_date ON sales(tenant_id, sale_date DESC);
CREATE INDEX idx_sales_tenant_status ON sales(tenant_id, status);
CREATE INDEX idx_sales_customer_date ON sales(customer_id, sale_date DESC);

-- Comments
COMMENT ON TABLE sales IS 'All sales transactions including POS, invoices, quotes, and credit notes';
COMMENT ON COLUMN sales.sale_type IS 'Transaction type: pos, invoice, quote, credit_note, debit_note';
```

### 18. sale_items
**Purpose:** Line items for sales transactions

```sql
CREATE TABLE sale_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    sale_id UUID NOT NULL REFERENCES sales(id) ON DELETE CASCADE,

    -- Product Reference
    product_id UUID REFERENCES products(id) ON DELETE SET NULL,
    product_variant_id UUID REFERENCES product_variants(id) ON DELETE SET NULL,

    -- Product Details (denormalized for history)
    product_sku VARCHAR(100),
    product_name VARCHAR(300) NOT NULL,
    product_description TEXT,

    -- Quantity and Units
    quantity DECIMAL(10,2) NOT NULL,
    unit_of_measure VARCHAR(20) NOT NULL DEFAULT 'PCS',

    -- Pricing
    unit_price DECIMAL(15,2) NOT NULL,
    unit_cost DECIMAL(15,2), -- For margin calculation
    discount_percentage DECIMAL(5,2) DEFAULT 0.00,
    discount_amount DECIMAL(15,2) DEFAULT 0.00,

    -- Calculated Amounts
    subtotal DECIMAL(15,2) NOT NULL, -- quantity * unit_price
    tax_rate DECIMAL(5,4) NOT NULL,
    tax_amount DECIMAL(15,2) NOT NULL,
    total DECIMAL(15,2) NOT NULL, -- subtotal - discount + tax

    -- CFDI / SAT Fields
    sat_product_code VARCHAR(8),
    sat_unit_code VARCHAR(3),

    -- Inventory
    inventory_deducted BOOLEAN NOT NULL DEFAULT false,
    inventory_transaction_id UUID, -- FK to inventory_transactions

    -- Line Item Order
    line_number INT NOT NULL DEFAULT 0,

    -- Notes
    notes TEXT,

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT chk_quantity CHECK (quantity > 0),
    CONSTRAINT chk_unit_price CHECK (unit_price >= 0),
    CONSTRAINT chk_amounts_sale_items CHECK (
        subtotal >= 0 AND
        discount_amount >= 0 AND
        tax_amount >= 0 AND
        total >= 0
    ),
    CONSTRAINT chk_discount_percentage_items CHECK (discount_percentage >= 0 AND discount_percentage <= 100)
);

-- Indexes
CREATE INDEX idx_sale_items_tenant ON sale_items(tenant_id);
CREATE INDEX idx_sale_items_sale ON sale_items(sale_id);
CREATE INDEX idx_sale_items_product ON sale_items(product_id);
CREATE INDEX idx_sale_items_line_number ON sale_items(sale_id, line_number);

-- Comments
COMMENT ON TABLE sale_items IS 'Line items for sales with denormalized product data for historical accuracy';
```

### 19. payments
**Purpose:** Payment records for sales

```sql
CREATE TABLE payments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    sale_id UUID NOT NULL REFERENCES sales(id) ON DELETE CASCADE,

    -- Payment Details
    payment_number VARCHAR(50) NOT NULL,
    payment_method VARCHAR(50) NOT NULL, -- cash, card, transfer, check, credit, mixed
    payment_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    amount DECIMAL(15,2) NOT NULL,

    -- Card/Transfer Details
    card_type VARCHAR(20), -- visa, mastercard, amex, etc.
    card_last_four VARCHAR(4),
    authorization_code VARCHAR(50),
    transaction_reference VARCHAR(100),

    -- Check Details
    check_number VARCHAR(50),
    check_bank VARCHAR(100),
    check_date DATE,

    -- Bank Transfer Details
    transfer_reference VARCHAR(100),
    transfer_bank VARCHAR(100),

    -- Change (for cash payments)
    amount_received DECIMAL(15,2),
    change_amount DECIMAL(15,2) DEFAULT 0.00,

    -- Status
    status VARCHAR(20) NOT NULL DEFAULT 'completed', -- pending, completed, failed, refunded

    -- Notes
    notes TEXT,

    -- Refund
    refund_date TIMESTAMP,
    refund_reason TEXT,
    refunded_by UUID REFERENCES users(id),

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT uk_payments_tenant_number UNIQUE (tenant_id, payment_number),
    CONSTRAINT chk_payment_method CHECK (payment_method IN (
        'cash', 'card', 'transfer', 'check', 'credit', 'mixed', 'paypal', 'stripe'
    )),
    CONSTRAINT chk_payment_status CHECK (status IN ('pending', 'completed', 'failed', 'refunded', 'cancelled')),
    CONSTRAINT chk_amount CHECK (amount > 0),
    CONSTRAINT chk_change CHECK (change_amount >= 0)
);

-- Indexes
CREATE INDEX idx_payments_tenant ON payments(tenant_id);
CREATE INDEX idx_payments_sale ON payments(sale_id);
CREATE INDEX idx_payments_date ON payments(payment_date DESC);
CREATE INDEX idx_payments_method ON payments(payment_method);
CREATE INDEX idx_payments_status ON payments(status);
CREATE INDEX idx_payments_created_by ON payments(created_by);

-- Comments
COMMENT ON TABLE payments IS 'Payment records for sales transactions';
COMMENT ON COLUMN payments.payment_method IS 'Payment method: cash, card, transfer, check, credit, mixed';
```

### 20. cfdi_invoices
**Purpose:** CFDI 4.0 electronic invoices (Mexican tax compliance)

```sql
CREATE TABLE cfdi_invoices (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    sale_id UUID NOT NULL REFERENCES sales(id) ON DELETE CASCADE,

    -- CFDI Identification
    cfdi_uuid UUID UNIQUE, -- Folio Fiscal from SAT
    series VARCHAR(10) NOT NULL,
    folio INT NOT NULL,
    cfdi_type VARCHAR(1) NOT NULL DEFAULT 'I', -- I=Ingreso, E=Egreso, T=Traslado, P=Pago

    -- CFDI Version
    cfdi_version VARCHAR(5) NOT NULL DEFAULT '4.0',

    -- Issue and Certification Dates
    issue_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    certification_date TIMESTAMP, -- When PAC stamped it

    -- Amounts
    subtotal DECIMAL(15,2) NOT NULL,
    discount DECIMAL(15,2) DEFAULT 0.00,
    taxes DECIMAL(15,2) NOT NULL,
    total DECIMAL(15,2) NOT NULL,

    -- Currency
    currency VARCHAR(3) NOT NULL DEFAULT 'MXN',
    exchange_rate DECIMAL(10,6) DEFAULT 1.000000,

    -- Payment Terms
    payment_method VARCHAR(3) NOT NULL, -- SAT payment method code (PUE, PPD)
    payment_form VARCHAR(2) NOT NULL, -- SAT payment form code (01-99)

    -- Customer CFDI Data (denormalized)
    customer_rfc VARCHAR(13) NOT NULL,
    customer_name VARCHAR(300) NOT NULL,
    customer_tax_regime VARCHAR(3) NOT NULL,
    customer_cfdi_use VARCHAR(3) NOT NULL,
    customer_postal_code VARCHAR(10),

    -- Issuer Data (from tenant)
    issuer_rfc VARCHAR(13) NOT NULL,
    issuer_name VARCHAR(300) NOT NULL,
    issuer_tax_regime VARCHAR(3) NOT NULL,

    -- PAC (Proveedor Autorizado de Certificación)
    pac_provider VARCHAR(50),
    pac_transaction_id VARCHAR(100),

    -- Digital Signatures
    original_string TEXT, -- Cadena original
    digital_seal TEXT, -- Sello digital
    sat_seal TEXT, -- Sello SAT
    sat_certificate_number VARCHAR(20),

    -- XML Storage
    xml_original TEXT, -- Original XML before stamping
    xml_stamped TEXT, -- Stamped XML with SAT seal
    xml_file_path VARCHAR(500), -- File storage path

    -- PDF
    pdf_file_path VARCHAR(500),
    qr_code_data TEXT,

    -- Status
    status VARCHAR(20) NOT NULL DEFAULT 'draft', -- draft, stamped, sent, cancelled

    -- Cancellation
    cancellation_date TIMESTAMP,
    cancellation_reason VARCHAR(500),
    cancellation_uuid UUID, -- UUID del comprobante de cancelación
    cancelled_by UUID REFERENCES users(id),

    -- Email Delivery
    email_sent BOOLEAN NOT NULL DEFAULT false,
    email_sent_at TIMESTAMP,
    email_sent_to VARCHAR(320),

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID REFERENCES users(id),

    -- Constraints
    CONSTRAINT uk_cfdi_tenant_series_folio UNIQUE (tenant_id, series, folio),
    CONSTRAINT chk_cfdi_type CHECK (cfdi_type IN ('I', 'E', 'T', 'P', 'N')),
    CONSTRAINT chk_cfdi_version CHECK (cfdi_version IN ('3.3', '4.0')),
    CONSTRAINT chk_cfdi_status CHECK (status IN ('draft', 'stamping', 'stamped', 'sent', 'cancelled', 'error')),
    CONSTRAINT chk_cfdi_amounts CHECK (
        subtotal >= 0 AND
        discount >= 0 AND
        taxes >= 0 AND
        total >= 0
    ),
    CONSTRAINT chk_rfc_issuer CHECK (issuer_rfc ~ '^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$'),
    CONSTRAINT chk_rfc_customer CHECK (customer_rfc ~ '^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$')
);

-- Indexes
CREATE INDEX idx_cfdi_tenant ON cfdi_invoices(tenant_id);
CREATE INDEX idx_cfdi_sale ON cfdi_invoices(sale_id);
CREATE INDEX idx_cfdi_uuid ON cfdi_invoices(cfdi_uuid) WHERE cfdi_uuid IS NOT NULL;
CREATE INDEX idx_cfdi_series_folio ON cfdi_invoices(tenant_id, series, folio);
CREATE INDEX idx_cfdi_status ON cfdi_invoices(status);
CREATE INDEX idx_cfdi_customer_rfc ON cfdi_invoices(customer_rfc);
CREATE INDEX idx_cfdi_issue_date ON cfdi_invoices(issue_date DESC);
CREATE INDEX idx_cfdi_created_by ON cfdi_invoices(created_by);

-- Add FK constraint to sales table
ALTER TABLE sales ADD CONSTRAINT fk_sales_cfdi
FOREIGN KEY (cfdi_invoice_id) REFERENCES cfdi_invoices(id);

-- Comments
COMMENT ON TABLE cfdi_invoices IS 'CFDI 4.0 electronic invoices for Mexican tax compliance';
COMMENT ON COLUMN cfdi_invoices.cfdi_uuid IS 'Folio Fiscal (UUID) assigned by SAT via PAC';
COMMENT ON COLUMN cfdi_invoices.pac_provider IS 'PAC provider used for stamping (Finkel, Divertia, etc.)';
```

### 21. cfdi_invoice_items
**Purpose:** Line items for CFDI invoices with SAT codes

```sql
CREATE TABLE cfdi_invoice_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    cfdi_invoice_id UUID NOT NULL REFERENCES cfdi_invoices(id) ON DELETE CASCADE,
    sale_item_id UUID REFERENCES sale_items(id) ON DELETE SET NULL,

    -- Line Item Order
    line_number INT NOT NULL DEFAULT 0,

    -- Product/Service Identification
    product_code VARCHAR(8) NOT NULL, -- SAT ClaveProdServ
    product_description VARCHAR(1000) NOT NULL,
    unit_code VARCHAR(3) NOT NULL, -- SAT ClaveUnidad

    -- Quantity and Pricing
    quantity DECIMAL(10,3) NOT NULL,
    unit_price DECIMAL(15,2) NOT NULL,
    discount DECIMAL(15,2) DEFAULT 0.00,
    subtotal DECIMAL(15,2) NOT NULL,

    -- Taxes
    tax_object VARCHAR(2) NOT NULL DEFAULT '02', -- 01=No objeto, 02=Sí objeto, 03=Sí objeto pero exento
    taxes_json JSONB, -- Array of tax details (IVA, IEPS, etc.)
    total_taxes DECIMAL(15,2) NOT NULL DEFAULT 0.00,

    -- Total
    total DECIMAL(15,2) NOT NULL,

    -- Additional Fields
    identification_number VARCHAR(100), -- Part number, SKU
    customs_info VARCHAR(500), -- Pedimento aduanal (if imported)

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT chk_quantity_cfdi CHECK (quantity > 0),
    CONSTRAINT chk_unit_price_cfdi CHECK (unit_price >= 0),
    CONSTRAINT chk_amounts_cfdi_items CHECK (
        subtotal >= 0 AND
        discount >= 0 AND
        total_taxes >= 0 AND
        total >= 0
    ),
    CONSTRAINT chk_tax_object CHECK (tax_object IN ('01', '02', '03'))
);

-- Indexes
CREATE INDEX idx_cfdi_items_tenant ON cfdi_invoice_items(tenant_id);
CREATE INDEX idx_cfdi_items_invoice ON cfdi_invoice_items(cfdi_invoice_id);
CREATE INDEX idx_cfdi_items_sale_item ON cfdi_invoice_items(sale_item_id);
CREATE INDEX idx_cfdi_items_product_code ON cfdi_invoice_items(product_code);

-- Comments
COMMENT ON TABLE cfdi_invoice_items IS 'Line items for CFDI invoices with SAT catalog codes';
COMMENT ON COLUMN cfdi_invoice_items.product_code IS 'SAT ClaveProdServ (8-digit product/service code)';
COMMENT ON COLUMN cfdi_invoice_items.unit_code IS 'SAT ClaveUnidad (unit of measure code)';
```

### 22. audit_logs
**Purpose:** Complete audit trail of all system changes

```sql
CREATE TABLE audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID REFERENCES tenants(id) ON DELETE SET NULL,

    -- Event Details
    event_type VARCHAR(50) NOT NULL, -- create, update, delete, login, logout, export, etc.
    entity_type VARCHAR(100) NOT NULL, -- products, sales, users, etc.
    entity_id UUID,
    entity_name VARCHAR(300),

    -- User Information
    user_id UUID REFERENCES users(id) ON DELETE SET NULL,
    user_email VARCHAR(320),
    user_ip VARCHAR(45),
    user_agent TEXT,

    -- Changes
    old_values JSONB,
    new_values JSONB,
    changed_fields TEXT[], -- Array of field names that changed

    -- Request Context
    request_method VARCHAR(10), -- GET, POST, PUT, DELETE
    request_path VARCHAR(500),
    request_id UUID,

    -- Result
    success BOOLEAN NOT NULL DEFAULT true,
    error_message TEXT,

    -- Timestamp
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT chk_event_type CHECK (event_type IN (
        'create', 'update', 'delete', 'login', 'logout', 'failed_login',
        'export', 'import', 'config_change', 'permission_change', 'cfdi_stamp', 'cfdi_cancel'
    ))
);

-- Indexes
CREATE INDEX idx_audit_tenant ON audit_logs(tenant_id);
CREATE INDEX idx_audit_user ON audit_logs(user_id);
CREATE INDEX idx_audit_entity ON audit_logs(entity_type, entity_id);
CREATE INDEX idx_audit_event_type ON audit_logs(event_type);
CREATE INDEX idx_audit_created_at ON audit_logs(created_at DESC);
CREATE INDEX idx_audit_success ON audit_logs(success) WHERE success = false;

-- Composite index for common queries
CREATE INDEX idx_audit_tenant_entity_date ON audit_logs(tenant_id, entity_type, created_at DESC);

-- Partition by month for performance (recommended for high-volume systems)
-- CREATE TABLE audit_logs_y2025m01 PARTITION OF audit_logs
-- FOR VALUES FROM ('2025-01-01') TO ('2025-02-01');

-- Comments
COMMENT ON TABLE audit_logs IS 'Complete audit trail of all system changes and events';
COMMENT ON COLUMN audit_logs.changed_fields IS 'Array of field names that were modified';
```

### 23. refresh_tokens
**Purpose:** JWT refresh token management with revocation support

```sql
CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL REFERENCES tenants(id) ON DELETE CASCADE,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,

    -- Token Details
    token_hash VARCHAR(500) NOT NULL UNIQUE, -- SHA256 hash of refresh token
    jti VARCHAR(100) NOT NULL UNIQUE, -- JWT ID for token identification

    -- Expiration
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Revocation
    is_revoked BOOLEAN NOT NULL DEFAULT false,
    revoked_at TIMESTAMP,
    revoked_by UUID REFERENCES users(id),
    revocation_reason VARCHAR(200),

    -- Device/Session Info
    ip_address VARCHAR(45),
    user_agent TEXT,
    device_id VARCHAR(100),

    -- Usage Tracking
    last_used_at TIMESTAMP,
    use_count INT NOT NULL DEFAULT 0,

    -- Constraints
    CONSTRAINT chk_expires_at CHECK (expires_at > created_at)
);

-- Indexes
CREATE INDEX idx_refresh_tokens_tenant ON refresh_tokens(tenant_id);
CREATE INDEX idx_refresh_tokens_user ON refresh_tokens(user_id);
CREATE INDEX idx_refresh_tokens_hash ON refresh_tokens(token_hash);
CREATE INDEX idx_refresh_tokens_jti ON refresh_tokens(jti);
CREATE INDEX idx_refresh_tokens_expires ON refresh_tokens(expires_at);
CREATE INDEX idx_refresh_tokens_active ON refresh_tokens(user_id, is_revoked, expires_at)
WHERE is_revoked = false AND expires_at > CURRENT_TIMESTAMP;

-- Cleanup expired tokens (run daily)
-- DELETE FROM refresh_tokens WHERE expires_at < CURRENT_TIMESTAMP - INTERVAL '7 days';

-- Comments
COMMENT ON TABLE refresh_tokens IS 'JWT refresh tokens with revocation support';
COMMENT ON COLUMN refresh_tokens.token_hash IS 'SHA256 hash of the refresh token for secure storage';
COMMENT ON COLUMN refresh_tokens.jti IS 'JWT ID claim for token identification and blacklisting';
```

### 24. sat_catalogs
**Purpose:** SAT catalog lookups (product codes, unit codes, tax regimes, etc.)

```sql
CREATE TABLE sat_catalogs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

    -- Catalog Information
    catalog_type VARCHAR(50) NOT NULL, -- product_service, unit, payment_form, payment_method, cfdi_use, tax_regime, etc.
    code VARCHAR(10) NOT NULL,
    description VARCHAR(500) NOT NULL,
    description_en VARCHAR(500),

    -- Hierarchy (for nested catalogs)
    parent_code VARCHAR(10),

    -- Metadata
    is_active BOOLEAN NOT NULL DEFAULT true,
    effective_date DATE,
    expiration_date DATE,

    -- Additional Data (flexible JSON for catalog-specific fields)
    metadata JSONB,

    -- Timestamps
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Constraints
    CONSTRAINT uk_sat_catalog_type_code UNIQUE (catalog_type, code),
    CONSTRAINT chk_catalog_type CHECK (catalog_type IN (
        'product_service', 'unit', 'payment_form', 'payment_method',
        'cfdi_use', 'tax_regime', 'postal_code', 'country', 'state',
        'hazardous_material', 'customs_regime', 'export_type'
    ))
);

-- Indexes
CREATE INDEX idx_sat_catalogs_type ON sat_catalogs(catalog_type);
CREATE INDEX idx_sat_catalogs_code ON sat_catalogs(catalog_type, code);
CREATE INDEX idx_sat_catalogs_active ON sat_catalogs(is_active) WHERE is_active = true;
CREATE INDEX idx_sat_catalogs_parent ON sat_catalogs(parent_code) WHERE parent_code IS NOT NULL;

-- Full-text search on descriptions
CREATE INDEX idx_sat_catalogs_description ON sat_catalogs USING gin(
    to_tsvector('spanish', description)
);

-- Common SAT catalog examples (seed data)
INSERT INTO sat_catalogs (catalog_type, code, description) VALUES
    -- Payment Methods (Forma de Pago)
    ('payment_form', '01', 'Efectivo'),
    ('payment_form', '02', 'Cheque nominativo'),
    ('payment_form', '03', 'Transferencia electrónica de fondos'),
    ('payment_form', '04', 'Tarjeta de crédito'),
    ('payment_form', '28', 'Tarjeta de débito'),
    ('payment_form', '99', 'Por definir'),

    -- Payment Methods (Método de Pago)
    ('payment_method', 'PUE', 'Pago en una sola exhibición'),
    ('payment_method', 'PPD', 'Pago en parcialidades o diferido'),

    -- CFDI Use (Uso de CFDI)
    ('cfdi_use', 'G01', 'Adquisición de mercancías'),
    ('cfdi_use', 'G02', 'Devoluciones, descuentos o bonificaciones'),
    ('cfdi_use', 'G03', 'Gastos en general'),
    ('cfdi_use', 'I01', 'Construcciones'),
    ('cfdi_use', 'I02', 'Mobiliario y equipo de oficina'),
    ('cfdi_use', 'P01', 'Por definir'),

    -- Tax Regimes (Régimen Fiscal)
    ('tax_regime', '601', 'General de Ley Personas Morales'),
    ('tax_regime', '603', 'Personas Morales con Fines no Lucrativos'),
    ('tax_regime', '605', 'Sueldos y Salarios e Ingresos Asimilados a Salarios'),
    ('tax_regime', '606', 'Arrendamiento'),
    ('tax_regime', '612', 'Personas Físicas con Actividades Empresariales y Profesionales'),
    ('tax_regime', '621', 'Régimen de Incorporación Fiscal'),

    -- Common Unit Codes (Clave Unidad)
    ('unit', 'E48', 'Unidad de servicio'),
    ('unit', 'H87', 'Pieza'),
    ('unit', 'KGM', 'Kilogramo'),
    ('unit', 'LTR', 'Litro'),
    ('unit', 'MTR', 'Metro'),
    ('unit', 'XBX', 'Caja');

-- Comments
COMMENT ON TABLE sat_catalogs IS 'SAT catalog data for CFDI compliance (product codes, units, payment methods, etc.)';
COMMENT ON COLUMN sat_catalogs.catalog_type IS 'Type of SAT catalog (product_service, unit, payment_form, etc.)';
```

---

## Concurrency Control and Row Versioning

### Strategy: Optimistic Concurrency with PostgreSQL xmin

Corelio uses **optimistic concurrency control** to prevent lost updates in multi-user scenarios. PostgreSQL's built-in `xmin` system column is used for automatic row versioning.

### How xmin Works

PostgreSQL automatically maintains an `xmin` column (transaction ID) for every row:
- Updated on every INSERT or UPDATE
- Can be used to detect concurrent modifications
- No manual column management required

### EF Core Implementation

**Entity Configuration (Fluent API):**

```csharp
// In ApplicationDbContext.OnModelCreating or entity configuration classes
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Use PostgreSQL's xmin for optimistic concurrency
        builder.Property<uint>("xmin")
               .HasColumnType("xid")
               .ValueGeneratedOnAddOrUpdate()
               .IsConcurrencyToken();

        // Alternative: Explicit row_version column (if xmin is not preferred)
        // builder.Property(p => p.RowVersion)
        //        .IsRowVersion();
    }
}
```

**Using Annotations (Alternative):**

```csharp
using Microsoft.EntityFrameworkCore;

public class Product : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Option 1: Use xmin (recommended for PostgreSQL)
    [Timestamp]
    public uint Xmin { get; set; }

    // Option 2: Explicit version column (alternative)
    // [Timestamp]
    // public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
```

### Handling Concurrency Conflicts

**Example: Update Product Price**

```csharp
public async Task<Result> UpdateProductPrice(Guid productId, decimal newPrice)
{
    var product = await _dbContext.Products.FindAsync(productId);
    if (product == null)
        return Result.NotFound();

    product.Price = newPrice;

    try
    {
        await _dbContext.SaveChangesAsync();
        return Result.Success();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        // Another user modified this product since we loaded it
        return Result.Conflict("Product was modified by another user. Please reload and try again.");
    }
}
```

### Critical Tables Requiring Concurrency Control

**High Priority (Financial/Inventory):**
1. **inventory_items** - Prevent overselling due to concurrent stock updates
2. **sales** - Prevent duplicate payment recording
3. **payments** - Critical for financial accuracy
4. **products** - Pricing changes
5. **cfdi_invoices** - Prevent duplicate stamping

**Medium Priority:**
6. **customers** - Balance updates
7. **users** - Password changes
8. **tenant_configurations** - Settings updates

### Inventory Concurrency Example

For inventory operations, combine optimistic concurrency with distributed locking:

```csharp
public class InventoryService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDistributedLockService _lockService; // Redis-based

    public async Task<Result> ReserveStock(Guid productId, decimal quantity)
    {
        // Acquire distributed lock to prevent race conditions across API instances
        using var lockHandle = await _lockService.AcquireLockAsync(
            $"inventory:product:{productId}",
            TimeSpan.FromSeconds(10));

        if (!lockHandle.IsAcquired)
            return Result.Conflict("Unable to acquire inventory lock");

        // Load inventory with concurrency token (xmin)
        var inventory = await _dbContext.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (inventory == null)
            return Result.NotFound();

        if (inventory.AvailableQuantity < quantity)
            return Result.Failure("Insufficient stock");

        // Update reserved quantity
        inventory.ReservedQuantity += quantity;

        try
        {
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (DbUpdateConcurrencyException)
        {
            // xmin mismatch - inventory was modified concurrently
            return Result.Conflict("Inventory was modified by another process. Retrying...");
        }
    }
}
```

### SQL Migration to Add xmin Support

If using explicit row version instead of xmin:

```sql
-- Add row_version to critical tables (alternative to xmin)
ALTER TABLE inventory_items ADD COLUMN row_version BIGSERIAL;
ALTER TABLE sales ADD COLUMN row_version BIGSERIAL;
ALTER TABLE payments ADD COLUMN row_version BIGSERIAL;
ALTER TABLE products ADD COLUMN row_version BIGSERIAL;
ALTER TABLE cfdi_invoices ADD COLUMN row_version BIGSERIAL;
ALTER TABLE customers ADD COLUMN row_version BIGSERIAL;
```

**Note:** When using PostgreSQL's `xmin`, no schema changes are required - xmin is a system column available on all tables.

### Best Practices

1. **Always use concurrency tokens** for financial and inventory operations
2. **Implement retry logic** with exponential backoff for conflict resolution
3. **Use distributed locks** (Redis) for critical sections across multiple API instances
4. **Log concurrency conflicts** to monitor contention hotspots
5. **Consider eventual consistency** for high-throughput scenarios (e.g., analytics)

### Retry Pattern Example (Using Polly)

```csharp
using Polly;

public class ResilientInventoryService
{
    private readonly IAsyncPolicy _retryPolicy;

    public ResilientInventoryService()
    {
        _retryPolicy = Policy
            .Handle<DbUpdateConcurrencyException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Concurrency conflict on attempt {RetryCount}. Retrying after {Delay}ms",
                        retryCount, timeSpan.TotalMilliseconds);
                });
    }

    public async Task<Result> UpdateInventory(Guid productId, decimal quantity)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            // Reload fresh data on each retry
            var inventory = await _dbContext.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            inventory.Quantity += quantity;
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        });
    }
}
```

### Testing Concurrency Control

**Unit Test Example:**

```csharp
[Fact]
public async Task UpdateProduct_ConcurrentModification_ThrowsDbUpdateConcurrencyException()
{
    // Arrange: Create product
    var product = new Product { Name = "Test Product", Price = 100.00m };
    _dbContext.Products.Add(product);
    await _dbContext.SaveChangesAsync();

    // Act: Load product in two separate contexts (simulating two users)
    using var context1 = CreateNewContext();
    using var context2 = CreateNewContext();

    var product1 = await context1.Products.FindAsync(product.Id);
    var product2 = await context2.Products.FindAsync(product.Id);

    // User 1 updates price
    product1.Price = 150.00m;
    await context1.SaveChangesAsync(); // Succeeds

    // User 2 tries to update price (with stale xmin)
    product2.Price = 200.00m;

    // Assert: User 2's save should fail with concurrency exception
    await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
    {
        await context2.SaveChangesAsync();
    });
}
```

---

## Schema Statistics (Estimated)

| Table Category | Number of Tables | Expected Growth |
|----------------|------------------|-----------------|
| Core (Multi-tenancy & Auth) | 7 | Low |
| Products & Inventory | 7 | High |
| Customers | 3 | Medium |
| Sales & Transactions | 4 | Very High |
| CFDI / Invoicing | 3 | High |
| **Total** | **24** | - |

## Database Size Estimates (per 1000 products, 5000 sales/month)

- Products & Inventory: ~50 MB
- Sales Transactions: ~100 MB/month
- CFDI Invoices: ~20 MB/month
- Total (first year): ~1.5 GB per tenant

## Backup and Maintenance Strategy

```sql
-- Daily backup
pg_dump -Fc corelio_production > backup_$(date +%Y%m%d).dump

-- Weekly vacuum
VACUUM ANALYZE;

-- Monthly reindex
REINDEX DATABASE corelio_production;

-- Archive old transactions (> 2 years)
-- Move to cold storage table or separate database
```