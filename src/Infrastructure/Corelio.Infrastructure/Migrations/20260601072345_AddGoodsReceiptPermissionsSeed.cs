using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoodsReceiptPermissionsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "module", "name" },
                values: new object[,]
                {
                    { new Guid("e1111111-1111-1111-1111-111111111111"), null, "receipts.view",   new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View goods receipts",   "Purchases", "View Goods Receipts"   },
                    { new Guid("e1111111-1111-1111-1111-111111111112"), null, "receipts.create", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Create goods receipts", "Purchases", "Create Goods Receipts" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id", "assigned_at", "assigned_by" },
                values: new object[,]
                {
                    // Administrator gets both receipt permissions
                    { new Guid("e1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("e1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    // Manager gets both receipt permissions
                    { new Guid("e1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("e1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    // Cashier gets view-only
                    { new Guid("e1111111-1111-1111-1111-111111111111"), new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e1111111-1111-1111-1111-111111111111"), new Guid("d3333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("e1111111-1111-1111-1111-111111111111"));
            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("e1111111-1111-1111-1111-111111111112"));
        }
    }
}
