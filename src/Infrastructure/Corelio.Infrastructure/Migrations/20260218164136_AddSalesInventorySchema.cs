using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesInventorySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    business_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    rfc = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    curp = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    tax_regime = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    cfdi_use = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    preferred_payment_method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
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
                    table.PrimaryKey("PK_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "warehouses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inventory_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(15,4)", nullable: false, defaultValue: 0m),
                    reserved_quantity = table.Column<decimal>(type: "numeric(15,4)", nullable: false, defaultValue: 0m),
                    minimum_level = table.Column<decimal>(type: "numeric(15,4)", nullable: false, defaultValue: 0m),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventory_items_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    folio = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(15,2)", nullable: false, defaultValue: 0m),
                    tax_amount = table.Column<decimal>(type: "numeric(15,2)", nullable: false, defaultValue: 0m),
                    total = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.id);
                    table.ForeignKey(
                        name: "FK_sales_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_sales_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inventory_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inventory_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(15,4)", nullable: false),
                    previous_quantity = table.Column<decimal>(type: "numeric(15,4)", nullable: false),
                    new_quantity = table.Column<decimal>(type: "numeric(15,4)", nullable: false),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_transactions_inventory_items_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalTable: "inventory_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sale_id = table.Column<Guid>(type: "uuid", nullable: false),
                    method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_payments_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sale_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sale_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    product_sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(15,4)", nullable: false),
                    discount_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    tax_percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    line_total = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_sale_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sale_items_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customers_email",
                table: "customers",
                column: "email",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_customers_tenant_id",
                table: "customers",
                column: "tenant_id",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_customers_tenant_rfc",
                table: "customers",
                columns: new[] { "tenant_id", "rfc" },
                unique: true,
                filter: "rfc IS NOT NULL AND is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_items_product_id",
                table: "inventory_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_tenant_product_warehouse",
                table: "inventory_items",
                columns: new[] { "tenant_id", "product_id", "warehouse_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inventory_items_warehouse_id",
                table: "inventory_items",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transactions_item_id",
                table: "inventory_transactions",
                column: "inventory_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transactions_reference_id",
                table: "inventory_transactions",
                column: "reference_id",
                filter: "reference_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_payments_sale_id",
                table: "payments",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_tenant_id",
                table: "payments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_sale_items_product_id",
                table: "sale_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_sale_items_sale_id",
                table: "sale_items",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_created_at",
                table: "sales",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_sales_customer_id",
                table: "sales",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_status",
                table: "sales",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_sales_tenant_folio",
                table: "sales",
                columns: new[] { "tenant_id", "folio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_tenant_id",
                table: "sales",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_sales_warehouse_id",
                table: "sales",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_warehouses_tenant_default",
                table: "warehouses",
                columns: new[] { "tenant_id", "is_default" },
                filter: "is_default = true");

            migrationBuilder.CreateIndex(
                name: "ix_warehouses_tenant_id",
                table: "warehouses",
                column: "tenant_id");

            // Seed default warehouse for the demo tenant
            migrationBuilder.InsertData(
                table: "warehouses",
                columns: ["id", "tenant_id", "name", "type", "is_default", "created_at", "updated_at", "created_by", "updated_by"],
                values: [
                    new Guid("e0000000-0000-0000-0000-000000000001"),
                    new Guid("b0000000-0000-0000-0000-000000000001"),
                    "Almacén Principal",
                    "Main",
                    true,
                    new DateTime(2026, 1, 21, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 1, 21, 0, 0, 0, DateTimeKind.Utc),
                    null,
                    null
                ]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "warehouses",
                keyColumn: "id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DropTable(
                name: "inventory_transactions");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "sale_items");

            migrationBuilder.DropTable(
                name: "inventory_items");

            migrationBuilder.DropTable(
                name: "sales");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "warehouses");
        }
    }
}
