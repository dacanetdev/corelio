using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchemaWithSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    event_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entity_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    user_ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    old_values = table.Column<string>(type: "jsonb", nullable: true),
                    new_values = table.Column<string>(type: "jsonb", nullable: true),
                    changed_fields = table.Column<string[]>(type: "text[]", nullable: true),
                    request_method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    request_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    success = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_dangerous = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    parent_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    level = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    color_hex = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    icon_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_categories_product_categories_parent_category_id",
                        column: x => x.parent_category_id,
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    legal_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    rfc = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    subdomain = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    custom_domain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    subscription_plan = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    subscription_starts_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subscription_ends_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    max_users = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    max_products = table.Column<int>(type: "integer", nullable: false, defaultValue: 1000),
                    max_sales_per_month = table.Column<int>(type: "integer", nullable: false, defaultValue: 5000),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_trial = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    trial_ends_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    barcode_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    short_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    manufacturer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    model_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cost_price = table.Column<decimal>(type: "numeric(15,2)", nullable: false, defaultValue: 0.00m),
                    sale_price = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    wholesale_price = table.Column<decimal>(type: "numeric(15,2)", nullable: true),
                    msrp = table.Column<decimal>(type: "numeric(15,2)", nullable: true),
                    tax_rate = table.Column<decimal>(type: "numeric(5,4)", nullable: false, defaultValue: 0.1600m),
                    is_tax_exempt = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    track_inventory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    unit_of_measure = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    min_stock_level = table.Column<decimal>(type: "numeric(10,2)", nullable: false, defaultValue: 0m),
                    max_stock_level = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    reorder_point = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    reorder_quantity = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    weight_kg = table.Column<decimal>(type: "numeric(10,3)", nullable: true),
                    length_cm = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    width_cm = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    height_cm = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    volume_cm3 = table.Column<decimal>(type: "numeric(15,2)", nullable: true),
                    sat_product_code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    sat_unit_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    sat_hazardous_material = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    primary_image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    images_json = table.Column<string>(type: "jsonb", nullable: true),
                    is_service = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_bundle = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_variant_parent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    slug = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    meta_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    meta_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    meta_keywords = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_products_product_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_system_role = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_roles_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cfdi_pac_provider = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cfdi_pac_api_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cfdi_pac_api_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cfdi_pac_test_mode = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    cfdi_certificate_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cfdi_key_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cfdi_certificate_password = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cfdi_certificate_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cfdi_series = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "A"),
                    cfdi_next_folio = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    default_warehouse_id = table.Column<Guid>(type: "uuid", nullable: true),
                    default_tax_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false, defaultValue: 0.1600m),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "MXN"),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "America/Mexico_City"),
                    business_hours_start = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    business_hours_end = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    pos_auto_print_receipt = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    pos_require_customer = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    pos_default_payment_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Cash"),
                    pos_enable_barcode_scanner = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    pos_thermal_printer_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pos_receipt_footer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    allow_negative_inventory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    require_product_cost = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    auto_calculate_margin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    feature_multi_warehouse = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    feature_ecommerce = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    feature_loyalty_program = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    feature_purchase_orders = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    email_notifications_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    sms_notifications_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    low_stock_notification_threshold = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 20.00m),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_configurations", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenant_configurations_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    mobile = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    employee_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    hire_date = table.Column<DateOnly>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_email_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    email_confirmation_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    password_reset_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    password_reset_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    two_factor_secret = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    failed_login_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    locked_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    assigned_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    jti = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_by = table.Column<Guid>(type: "uuid", nullable: true),
                    revocation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    device_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    last_used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    use_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    assigned_by = table.Column<Guid>(type: "uuid", nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "module", "name" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), null, "users.view", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View user list", "Users", "View Users" },
                    { new Guid("a1111111-1111-1111-1111-111111111112"), null, "users.create", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Create new users", "Users", "Create Users" },
                    { new Guid("a1111111-1111-1111-1111-111111111113"), null, "users.edit", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Edit user details", "Users", "Edit Users" }
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "is_dangerous", "module", "name" },
                values: new object[] { new Guid("a1111111-1111-1111-1111-111111111114"), null, "users.delete", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Delete users", true, "Users", "Delete Users" });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "module", "name" },
                values: new object[,]
                {
                    { new Guid("a2222222-2222-2222-2222-222222222221"), null, "products.view", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View product list", "Products", "View Products" },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), null, "products.create", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Create new products", "Products", "Create Products" },
                    { new Guid("a2222222-2222-2222-2222-222222222223"), null, "products.edit", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Edit product details", "Products", "Edit Products" },
                    { new Guid("a2222222-2222-2222-2222-222222222224"), null, "products.delete", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Delete products", "Products", "Delete Products" },
                    { new Guid("a3333333-3333-3333-3333-333333333331"), null, "sales.view", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View sales transactions", "Sales", "View Sales" },
                    { new Guid("a3333333-3333-3333-3333-333333333332"), null, "sales.create", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Create new sales", "Sales", "Create Sales" }
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "is_dangerous", "module", "name" },
                values: new object[] { new Guid("a3333333-3333-3333-3333-333333333333"), null, "sales.cancel", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Cancel sales transactions", true, "Sales", "Cancel Sales" });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "module", "name" },
                values: new object[,]
                {
                    { new Guid("a4444444-4444-4444-4444-444444444441"), null, "inventory.view", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View inventory levels", "Inventory", "View Inventory" },
                    { new Guid("a4444444-4444-4444-4444-444444444442"), null, "inventory.adjust", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Adjust inventory levels", "Inventory", "Adjust Inventory" },
                    { new Guid("a5555555-5555-5555-5555-555555555551"), null, "reports.view", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View business reports", "Reports", "View Reports" },
                    { new Guid("a5555555-5555-5555-5555-555555555552"), null, "reports.export", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Export reports to file", "Reports", "Export Reports" },
                    { new Guid("a6666666-6666-6666-6666-666666666661"), null, "settings.view", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View system settings", "Settings", "View Settings" }
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "is_dangerous", "module", "name" },
                values: new object[] { new Guid("a6666666-6666-6666-6666-666666666662"), null, "settings.edit", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Modify system settings", true, "Settings", "Edit Settings" });

            migrationBuilder.InsertData(
                table: "tenants",
                columns: new[] { "id", "created_at", "created_by", "custom_domain", "is_active", "is_trial", "legal_name", "max_products", "max_sales_per_month", "max_users", "name", "rfc", "subdomain", "subscription_ends_at", "subscription_plan", "subscription_starts_at", "trial_ends_at", "updated_at", "updated_by" },
                values: new object[] { new Guid("b0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, true, true, "Ferretería Demo S.A. de C.V.", 5000, 10000, 10, "Demo Hardware Store", "FDE010101ABC", "demo", null, "Premium", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "created_by", "description", "is_system_role", "name", "tenant_id", "updated_at", "updated_by" },
                values: new object[] { new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Full system access", true, "Administrator", new Guid("b0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "created_by", "description", "name", "tenant_id", "updated_at", "updated_by" },
                values: new object[] { new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Store manager with most permissions", "Manager", new Guid("b0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "created_by", "description", "is_default", "name", "tenant_id", "updated_at", "updated_by" },
                values: new object[] { new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Point of sale operator", true, "Cashier", new Guid("b0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "tenant_configurations",
                columns: new[] { "id", "auto_calculate_margin", "business_hours_end", "business_hours_start", "cfdi_certificate_expires_at", "cfdi_certificate_password", "cfdi_certificate_path", "cfdi_key_path", "cfdi_next_folio", "cfdi_pac_api_key", "cfdi_pac_api_url", "cfdi_pac_provider", "cfdi_pac_test_mode", "cfdi_series", "created_at", "created_by", "currency", "default_tax_rate", "default_warehouse_id", "email_notifications_enabled", "low_stock_notification_threshold", "pos_enable_barcode_scanner", "pos_receipt_footer", "pos_thermal_printer_name", "require_product_cost", "tenant_id", "timezone", "updated_at", "updated_by" },
                values: new object[] { new Guid("c0000000-0000-0000-0000-000000000001"), true, new TimeOnly(18, 0, 0), new TimeOnly(9, 0, 0), null, null, null, null, 1, null, null, null, true, "A", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "MXN", 0.1600m, null, true, 20.00m, true, null, null, true, new Guid("b0000000-0000-0000-0000-000000000001"), "America/Mexico_City", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "created_at", "created_by", "email", "email_confirmation_token", "employee_code", "first_name", "hire_date", "is_active", "is_email_confirmed", "last_login_at", "last_login_ip", "last_name", "locked_until", "mobile", "password_hash", "password_reset_expires_at", "password_reset_token", "phone", "position", "tenant_id", "two_factor_secret", "updated_at", "updated_by", "username" },
                values: new object[,]
                {
                    { new Guid("e1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@demo.corelio.app", null, null, "Admin", null, true, true, null, null, "User", null, null, "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m", null, null, null, null, new Guid("b0000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "" },
                    { new Guid("e2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "manager@demo.corelio.app", null, null, "Manager", null, true, true, null, null, "User", null, null, "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m", null, null, null, null, new Guid("b0000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "" },
                    { new Guid("e3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "cashier@demo.corelio.app", null, null, "Cashier", null, true, true, null, null, "User", null, null, "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m", null, null, null, null, new Guid("b0000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id", "assigned_at", "assigned_by" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a1111111-1111-1111-1111-111111111113"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a1111111-1111-1111-1111-111111111114"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a2222222-2222-2222-2222-222222222221"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a2222222-2222-2222-2222-222222222223"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a2222222-2222-2222-2222-222222222224"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a3333333-3333-3333-3333-333333333331"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a3333333-3333-3333-3333-333333333332"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a3333333-3333-3333-3333-333333333333"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a4444444-4444-4444-4444-444444444441"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a4444444-4444-4444-4444-444444444442"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a5555555-5555-5555-5555-555555555551"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a5555555-5555-5555-5555-555555555552"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a6666666-6666-6666-6666-666666666661"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a6666666-6666-6666-6666-666666666662"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a1111111-1111-1111-1111-111111111113"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a2222222-2222-2222-2222-222222222221"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a2222222-2222-2222-2222-222222222223"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a2222222-2222-2222-2222-222222222224"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a3333333-3333-3333-3333-333333333331"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a3333333-3333-3333-3333-333333333332"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a4444444-4444-4444-4444-444444444441"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a4444444-4444-4444-4444-444444444442"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a5555555-5555-5555-5555-555555555551"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a5555555-5555-5555-5555-555555555552"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a6666666-6666-6666-6666-666666666661"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a2222222-2222-2222-2222-222222222221"), new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a3333333-3333-3333-3333-333333333331"), new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a3333333-3333-3333-3333-333333333332"), new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a4444444-4444-4444-4444-444444444441"), new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "role_id", "user_id", "assigned_at", "assigned_by", "expires_at" },
                values: new object[,]
                {
                    { new Guid("d1111111-1111-1111-1111-111111111111"), new Guid("e1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { new Guid("d2222222-2222-2222-2222-222222222222"), new Guid("e2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { new Guid("d3333333-3333-3333-3333-333333333333"), new Guid("e3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_created_at",
                table: "audit_logs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_entity_id",
                table: "audit_logs",
                column: "entity_id",
                filter: "entity_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_entity_type",
                table: "audit_logs",
                column: "entity_type");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_event_type",
                table: "audit_logs",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_tenant_entity_created",
                table: "audit_logs",
                columns: new[] { "tenant_id", "entity_type", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_tenant_id",
                table: "audit_logs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id",
                filter: "user_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_code",
                table: "permissions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_permissions_module",
                table: "permissions",
                column: "module");

            migrationBuilder.CreateIndex(
                name: "ix_product_categories_is_active",
                table: "product_categories",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_product_categories_parent_id",
                table: "product_categories",
                column: "parent_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_categories_path",
                table: "product_categories",
                column: "path");

            migrationBuilder.CreateIndex(
                name: "ix_product_categories_tenant_id",
                table: "product_categories",
                column: "tenant_id",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_product_categories_tenant_name",
                table: "product_categories",
                columns: new[] { "tenant_id", "name" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_is_active",
                table: "products",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_products_is_featured",
                table: "products",
                column: "is_featured",
                filter: "is_featured = true AND is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_products_name",
                table: "products",
                column: "name",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_products_tenant_barcode",
                table: "products",
                columns: new[] { "tenant_id", "barcode" },
                unique: true,
                filter: "barcode IS NOT NULL AND is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_products_tenant_id",
                table: "products",
                column: "tenant_id",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_products_tenant_sku",
                table: "products",
                columns: new[] { "tenant_id", "sku" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_expires_at",
                table: "refresh_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_is_revoked",
                table: "refresh_tokens",
                column: "is_revoked");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_jti",
                table: "refresh_tokens",
                column: "jti",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_tenant_id",
                table: "refresh_tokens",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_id",
                table: "role_permissions",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_is_system_role",
                table: "roles",
                column: "is_system_role");

            migrationBuilder.CreateIndex(
                name: "ix_roles_tenant_id",
                table: "roles",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_tenant_name",
                table: "roles",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_configurations_tenant_id",
                table: "tenant_configurations",
                column: "tenant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenants_custom_domain",
                table: "tenants",
                column: "custom_domain",
                unique: true,
                filter: "custom_domain IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_is_active",
                table: "tenants",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_rfc",
                table: "tenants",
                column: "rfc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenants_subdomain",
                table: "tenants",
                column: "subdomain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_expires_at",
                table: "user_roles",
                column: "expires_at",
                filter: "expires_at IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_is_active",
                table: "users",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_email",
                table: "users",
                columns: new[] { "tenant_id", "email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_id",
                table: "users",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_username",
                table: "users",
                columns: new[] { "tenant_id", "username" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "tenant_configurations");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "product_categories");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "tenants");
        }
    }
}
