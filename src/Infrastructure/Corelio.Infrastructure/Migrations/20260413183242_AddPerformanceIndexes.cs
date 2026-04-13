using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_inventory_items_warehouse_id",
                table: "inventory_items",
                newName: "ix_inventory_items_warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_tenant_created_at",
                table: "sales",
                columns: new[] { "tenant_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sales_tenant_created_at",
                table: "sales");

            migrationBuilder.RenameIndex(
                name: "ix_inventory_items_warehouse_id",
                table: "inventory_items",
                newName: "IX_inventory_items_warehouse_id");
        }
    }
}
