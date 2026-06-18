using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehousePermissionsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "module", "name" },
                values: new object[,]
                {
                    { new Guid("f1111111-1111-1111-1111-111111111111"), null, "warehouses.view",   new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View warehouse list",   "Inventory", "View Warehouses"   },
                    { new Guid("f1111111-1111-1111-1111-111111111112"), null, "warehouses.create", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Create new warehouses", "Inventory", "Create Warehouses" },
                    { new Guid("f1111111-1111-1111-1111-111111111113"), null, "warehouses.update", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Edit warehouse details", "Inventory", "Update Warehouses" },
                    { new Guid("f1111111-1111-1111-1111-111111111114"), null, "warehouses.delete", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Delete warehouses",     "Inventory", "Delete Warehouses" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id", "assigned_at", "assigned_by" },
                values: new object[,]
                {
                    // Administrator gets all warehouse permissions
                    { new Guid("f1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("f1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("f1111111-1111-1111-1111-111111111113"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("f1111111-1111-1111-1111-111111111114"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    // Manager gets view + create + update (no delete)
                    { new Guid("f1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("f1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("f1111111-1111-1111-1111-111111111113"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f1111111-1111-1111-1111-111111111113"), new Guid("d1111111-1111-1111-1111-111111111111") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f1111111-1111-1111-1111-111111111114"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f1111111-1111-1111-1111-111111111113"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("f1111111-1111-1111-1111-111111111111"));
            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("f1111111-1111-1111-1111-111111111112"));
            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("f1111111-1111-1111-1111-111111111113"));
            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("f1111111-1111-1111-1111-111111111114"));
        }
    }
}
