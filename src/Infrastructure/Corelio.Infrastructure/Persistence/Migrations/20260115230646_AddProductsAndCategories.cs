using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddProductsAndCategories : Migration
{
    private static readonly string[] TenantNameColumns = ["tenant_id", "name"];
    private static readonly string[] TenantBarcodeColumns = ["tenant_id", "barcode"];
    private static readonly string[] TenantSkuColumns = ["tenant_id", "sku"];
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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
                columns: TenantNameColumns,
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
                columns: TenantBarcodeColumns,
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
                columns: TenantSkuColumns,
                unique: true,
                filter: "is_deleted = false");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "products");

        migrationBuilder.DropTable(
            name: "product_categories");
    }
}
