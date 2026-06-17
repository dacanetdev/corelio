using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryAverageCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "average_cost",
                table: "inventory_items",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_cost",
                table: "goods_receipt_items",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "average_cost",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "unit_cost",
                table: "goods_receipt_items");
        }
    }
}
