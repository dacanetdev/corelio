using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingModuleSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "iva_enabled",
                table: "products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "list_price",
                table: "products",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "net_cost",
                table: "products",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "product_discounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tier_number = table.Column<int>(type: "integer", nullable: false),
                    discount_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_discounts", x => x.id);
                    table.CheckConstraint("ck_product_discounts_discount_percentage", "discount_percentage BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_product_discounts_tier_number", "tier_number BETWEEN 1 AND 6");
                    table.ForeignKey(
                        name: "FK_product_discounts_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_margin_prices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tier_number = table.Column<int>(type: "integer", nullable: false),
                    margin_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    sale_price = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    price_with_iva = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_margin_prices", x => x.id);
                    table.CheckConstraint("ck_product_margin_prices_margin_percentage", "margin_percentage IS NULL OR margin_percentage BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_product_margin_prices_tier_number", "tier_number BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_product_margin_prices_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_pricing_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    discount_tier_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    margin_tier_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    default_iva_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    iva_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 16.00m),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_pricing_configurations", x => x.id);
                    table.CheckConstraint("ck_tenant_pricing_configurations_discount_tier_count", "discount_tier_count BETWEEN 1 AND 6");
                    table.CheckConstraint("ck_tenant_pricing_configurations_iva_percentage", "iva_percentage BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_tenant_pricing_configurations_margin_tier_count", "margin_tier_count BETWEEN 1 AND 5");
                });

            migrationBuilder.CreateTable(
                name: "discount_tier_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_pricing_configuration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tier_number = table.Column<int>(type: "integer", nullable: false),
                    tier_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discount_tier_definitions", x => x.id);
                    table.CheckConstraint("ck_discount_tier_definitions_tier_number", "tier_number BETWEEN 1 AND 6");
                    table.ForeignKey(
                        name: "FK_discount_tier_definitions_tenant_pricing_configurations_ten~",
                        column: x => x.tenant_pricing_configuration_id,
                        principalTable: "tenant_pricing_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "margin_tier_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_pricing_configuration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tier_number = table.Column<int>(type: "integer", nullable: false),
                    tier_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_margin_tier_definitions", x => x.id);
                    table.CheckConstraint("ck_margin_tier_definitions_tier_number", "tier_number BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_margin_tier_definitions_tenant_pricing_configurations_tenan~",
                        column: x => x.tenant_pricing_configuration_id,
                        principalTable: "tenant_pricing_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_discount_tier_definitions_config_id",
                table: "discount_tier_definitions",
                column: "tenant_pricing_configuration_id");

            migrationBuilder.CreateIndex(
                name: "ix_discount_tier_definitions_tenant_tier",
                table: "discount_tier_definitions",
                columns: new[] { "tenant_id", "tier_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_margin_tier_definitions_config_id",
                table: "margin_tier_definitions",
                column: "tenant_pricing_configuration_id");

            migrationBuilder.CreateIndex(
                name: "ix_margin_tier_definitions_tenant_tier",
                table: "margin_tier_definitions",
                columns: new[] { "tenant_id", "tier_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_discounts_product_tier",
                table: "product_discounts",
                columns: new[] { "product_id", "tier_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_margin_prices_product_tier",
                table: "product_margin_prices",
                columns: new[] { "product_id", "tier_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_pricing_configurations_tenant_id",
                table: "tenant_pricing_configurations",
                column: "tenant_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "discount_tier_definitions");

            migrationBuilder.DropTable(
                name: "margin_tier_definitions");

            migrationBuilder.DropTable(
                name: "product_discounts");

            migrationBuilder.DropTable(
                name: "product_margin_prices");

            migrationBuilder.DropTable(
                name: "tenant_pricing_configurations");

            migrationBuilder.DropColumn(
                name: "iva_enabled",
                table: "products");

            migrationBuilder.DropColumn(
                name: "list_price",
                table: "products");

            migrationBuilder.DropColumn(
                name: "net_cost",
                table: "products");
        }
    }
}
