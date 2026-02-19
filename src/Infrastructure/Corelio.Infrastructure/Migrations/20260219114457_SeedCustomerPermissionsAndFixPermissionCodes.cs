using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCustomerPermissionsAndFixPermissionCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111113"),
                columns: new[] { "code", "name" },
                values: new object[] { "users.update", "Update Users" });

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222223"),
                columns: new[] { "code", "name" },
                values: new object[] { "products.update", "Update Products" });

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a6666666-6666-6666-6666-666666666662"),
                columns: new[] { "code", "name" },
                values: new object[] { "settings.update", "Update Settings" });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "module", "name" },
                values: new object[,]
                {
                    { new Guid("a8888888-8888-8888-8888-888888888881"), null, "customers.view", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View customer list and details", "Customers", "View Customers" },
                    { new Guid("a8888888-8888-8888-8888-888888888882"), null, "customers.create", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Create new customers", "Customers", "Create Customers" },
                    { new Guid("a8888888-8888-8888-8888-888888888883"), null, "customers.update", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Edit customer details", "Customers", "Update Customers" }
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "is_dangerous", "module", "name" },
                values: new object[] { new Guid("a8888888-8888-8888-8888-888888888884"), null, "customers.delete", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Soft-delete customers", true, "Customers", "Delete Customers" });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id", "assigned_at", "assigned_by" },
                values: new object[,]
                {
                    { new Guid("a8888888-8888-8888-8888-888888888881"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a8888888-8888-8888-8888-888888888882"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a8888888-8888-8888-8888-888888888883"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a8888888-8888-8888-8888-888888888884"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a8888888-8888-8888-8888-888888888881"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a8888888-8888-8888-8888-888888888882"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a8888888-8888-8888-8888-888888888883"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a8888888-8888-8888-8888-888888888881"), new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a8888888-8888-8888-8888-888888888881"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a8888888-8888-8888-8888-888888888882"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a8888888-8888-8888-8888-888888888883"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a8888888-8888-8888-8888-888888888884"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a8888888-8888-8888-8888-888888888881"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a8888888-8888-8888-8888-888888888882"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a8888888-8888-8888-8888-888888888883"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a8888888-8888-8888-8888-888888888881"), new Guid("d3333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a8888888-8888-8888-8888-888888888881"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a8888888-8888-8888-8888-888888888882"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a8888888-8888-8888-8888-888888888883"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a8888888-8888-8888-8888-888888888884"));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111113"),
                columns: new[] { "code", "name" },
                values: new object[] { "users.edit", "Edit Users" });

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222223"),
                columns: new[] { "code", "name" },
                values: new object[] { "products.edit", "Edit Products" });

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a6666666-6666-6666-6666-666666666662"),
                columns: new[] { "code", "name" },
                values: new object[] { "settings.edit", "Edit Settings" });
        }
    }
}
