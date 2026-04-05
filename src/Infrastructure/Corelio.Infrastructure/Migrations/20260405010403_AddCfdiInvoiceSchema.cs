using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCfdiInvoiceSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "cfdi_certificate_data",
                table: "tenant_configurations",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issuer_name",
                table: "tenant_configurations",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issuer_postal_code",
                table: "tenant_configurations",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issuer_rfc",
                table: "tenant_configurations",
                type: "character varying(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issuer_tax_regime",
                table: "tenant_configurations",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cfdi_invoices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sale_id = table.Column<Guid>(type: "uuid", nullable: true),
                    folio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    serie = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "A"),
                    uuid = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    invoice_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Ingreso"),
                    issuer_rfc = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    issuer_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    issuer_tax_regime = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    receiver_rfc = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    receiver_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    receiver_tax_regime = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    receiver_postal_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    receiver_cfdi_use = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    discount = table.Column<decimal>(type: "numeric(15,2)", nullable: false, defaultValue: 0m),
                    total = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    payment_form = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    payment_method = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "PUE"),
                    stamp_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sat_certificate_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    pac_stamp_signature = table.Column<string>(type: "text", nullable: true),
                    sat_stamp_signature = table.Column<string>(type: "text", nullable: true),
                    qr_code_data = table.Column<string>(type: "text", nullable: true),
                    original_chain = table.Column<string>(type: "text", nullable: true),
                    xml_content = table.Column<string>(type: "text", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    cancellation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfdi_invoices", x => x.id);
                    table.ForeignKey(
                        name: "FK_cfdi_invoices_sales_sale_id",
                        column: x => x.sale_id,
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "cfdi_invoice_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_number = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    product_key = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false, defaultValue: "25171500"),
                    unit_key = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "H87"),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                    unit_value = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    discount = table.Column<decimal>(type: "numeric(15,2)", nullable: false, defaultValue: 0m),
                    tax_object = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, defaultValue: "02"),
                    tax_rate = table.Column<decimal>(type: "numeric(6,4)", nullable: false, defaultValue: 0.16m),
                    tax_amount = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfdi_invoice_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_cfdi_invoice_items_cfdi_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "cfdi_invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cfdi_invoice_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "tenant_configurations",
                keyColumn: "id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000001"),
                columns: new[] { "cfdi_certificate_data", "issuer_name", "issuer_postal_code", "issuer_rfc", "issuer_tax_regime" },
                values: new object[] { null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "ix_cfdi_invoice_items_invoice_id",
                table: "cfdi_invoice_items",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_cfdi_invoice_items_product_id",
                table: "cfdi_invoice_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_cfdi_invoice_items_tenant_id",
                table: "cfdi_invoice_items",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_cfdi_invoices_sale_id",
                table: "cfdi_invoices",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "ix_cfdi_invoices_tenant_id",
                table: "cfdi_invoices",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_cfdi_invoices_tenant_serie_folio",
                table: "cfdi_invoices",
                columns: new[] { "tenant_id", "serie", "folio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cfdi_invoices_tenant_status",
                table: "cfdi_invoices",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_cfdi_invoices_uuid",
                table: "cfdi_invoices",
                column: "uuid",
                unique: true,
                filter: "uuid IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cfdi_invoice_items");

            migrationBuilder.DropTable(
                name: "cfdi_invoices");

            migrationBuilder.DropColumn(
                name: "cfdi_certificate_data",
                table: "tenant_configurations");

            migrationBuilder.DropColumn(
                name: "issuer_name",
                table: "tenant_configurations");

            migrationBuilder.DropColumn(
                name: "issuer_postal_code",
                table: "tenant_configurations");

            migrationBuilder.DropColumn(
                name: "issuer_rfc",
                table: "tenant_configurations");

            migrationBuilder.DropColumn(
                name: "issuer_tax_regime",
                table: "tenant_configurations");
        }
    }
}
