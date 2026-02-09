using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "module", "name" },
                values: new object[,]
                {
                    { new Guid("a7777777-7777-7777-7777-777777777771"), null, "pricing.view", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View pricing configuration and product prices", "Pricing", "View Pricing" },
                    { new Guid("a7777777-7777-7777-7777-777777777772"), null, "pricing.manage", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Configure pricing tiers and update product prices", "Pricing", "Manage Pricing" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id", "assigned_at", "assigned_by" },
                values: new object[,]
                {
                    { new Guid("a7777777-7777-7777-7777-777777777771"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a7777777-7777-7777-7777-777777777772"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a7777777-7777-7777-7777-777777777771"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("a7777777-7777-7777-7777-777777777772"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a7777777-7777-7777-7777-777777777771"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a7777777-7777-7777-7777-777777777772"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a7777777-7777-7777-7777-777777777771"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a7777777-7777-7777-7777-777777777772"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a7777777-7777-7777-7777-777777777771"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a7777777-7777-7777-7777-777777777772"));
        }
    }
}
