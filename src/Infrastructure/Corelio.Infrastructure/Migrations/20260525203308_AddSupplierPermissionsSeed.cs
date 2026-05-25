using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierPermissionsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "module", "name" },
                values: new object[,]
                {
                    { new Guid("b1111111-1111-1111-1111-111111111111"), null, "suppliers.view", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View supplier list and details", "Suppliers", "View Suppliers" },
                    { new Guid("b1111111-1111-1111-1111-111111111112"), null, "suppliers.create", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Create new suppliers", "Suppliers", "Create Suppliers" },
                    { new Guid("b1111111-1111-1111-1111-111111111113"), null, "suppliers.update", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Update existing suppliers", "Suppliers", "Update Suppliers" },
                    { new Guid("b1111111-1111-1111-1111-111111111114"), null, "suppliers.delete", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Delete suppliers", "Suppliers", "Delete Suppliers" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id", "assigned_at", "assigned_by" },
                values: new object[,]
                {
                    // Owner gets all supplier permissions
                    { new Guid("b1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1111111-1111-1111-1111-111111111113"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1111111-1111-1111-1111-111111111114"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    // Manager gets all supplier permissions
                    { new Guid("b1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1111111-1111-1111-1111-111111111113"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b1111111-1111-1111-1111-111111111114"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    // Viewer gets view-only
                    { new Guid("b1111111-1111-1111-1111-111111111111"), new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b1111111-1111-1111-1111-111111111113"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b1111111-1111-1111-1111-111111111114"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b1111111-1111-1111-1111-111111111113"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b1111111-1111-1111-1111-111111111114"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b1111111-1111-1111-1111-111111111111"), new Guid("d3333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111112"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111113"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111114"));
        }
    }
}

