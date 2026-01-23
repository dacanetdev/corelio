using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchemaWithSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111112"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111113"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111114"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222221"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222222"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222223"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222224"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a3333333-3333-3333-3333-333333333331"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a3333333-3333-3333-3333-333333333332"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a3333333-3333-3333-3333-333333333333"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a4444444-4444-4444-4444-444444444441"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a4444444-4444-4444-4444-444444444442"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a5555555-5555-5555-5555-555555555551"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a5555555-5555-5555-5555-555555555552"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a6666666-6666-6666-6666-666666666661"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a6666666-6666-6666-6666-666666666662"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111113"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111114"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222221"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222222"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222223"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222224"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333331"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333332"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333333"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444441"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444442"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a5555555-5555-5555-5555-555555555551"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a5555555-5555-5555-5555-555555555552"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a6666666-6666-6666-6666-666666666661"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a6666666-6666-6666-6666-666666666662"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111113"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222221"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222222"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222223"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222224"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333331"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333332"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444441"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444442"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a5555555-5555-5555-5555-555555555551"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a5555555-5555-5555-5555-555555555552"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a6666666-6666-6666-6666-666666666661"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222221"), new Guid("d3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333331"), new Guid("d3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333332"), new Guid("d3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444441"), new Guid("d3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("d1111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("d2222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("d3333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "tenant_configurations",
                keyColumn: "id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "tenants",
                keyColumn: "id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000001"),
                columns: new[] { "created_at", "subscription_starts_at", "trial_ends_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { new Guid("d1111111-1111-1111-1111-111111111111"), new Guid("e1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { new Guid("d2222222-2222-2222-2222-222222222222"), new Guid("e2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { new Guid("d3333333-3333-3333-3333-333333333333"), new Guid("e3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e1111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e2222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e3333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(1861));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111112"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(1918));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111113"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(1923));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111114"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2391));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222221"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2404));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222222"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2436));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222223"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2441));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222224"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2446));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a3333333-3333-3333-3333-333333333331"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2450));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a3333333-3333-3333-3333-333333333332"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2456));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a3333333-3333-3333-3333-333333333333"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2460));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a4444444-4444-4444-4444-444444444441"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2465));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a4444444-4444-4444-4444-444444444442"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2469));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a5555555-5555-5555-5555-555555555551"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2477));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a5555555-5555-5555-5555-555555555552"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2482));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a6666666-6666-6666-6666-666666666661"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2486));

            migrationBuilder.UpdateData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a6666666-6666-6666-6666-666666666662"),
                column: "created_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 103, DateTimeKind.Utc).AddTicks(2491));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(8717));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9199));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111113"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9201));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111114"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9202));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222221"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9203));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222222"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9225));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222223"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9226));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222224"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9229));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333331"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9230));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333332"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9232));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333333"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9233));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444441"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9234));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444442"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9236));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a5555555-5555-5555-5555-555555555551"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9237));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a5555555-5555-5555-5555-555555555552"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9238));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a6666666-6666-6666-6666-666666666661"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9239));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a6666666-6666-6666-6666-666666666662"), new Guid("d1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9241));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9256));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9258));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1111111-1111-1111-1111-111111111113"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9259));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222221"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9260));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222222"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9261));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222223"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9262));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222224"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9263));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333331"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9264));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333332"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9266));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444441"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9267));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444442"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9268));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a5555555-5555-5555-5555-555555555551"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9269));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a5555555-5555-5555-5555-555555555552"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9270));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a6666666-6666-6666-6666-666666666661"), new Guid("d2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9271));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a2222222-2222-2222-2222-222222222221"), new Guid("d3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9276));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333331"), new Guid("d3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9277));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a3333333-3333-3333-3333-333333333332"), new Guid("d3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9279));

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4444444-4444-4444-4444-444444444441"), new Guid("d3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 111, DateTimeKind.Utc).AddTicks(9280));

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("d1111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 7, 56, 11, 110, DateTimeKind.Utc).AddTicks(162), new DateTime(2026, 1, 21, 7, 56, 11, 110, DateTimeKind.Utc).AddTicks(167) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("d2222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 7, 56, 11, 110, DateTimeKind.Utc).AddTicks(370), new DateTime(2026, 1, 21, 7, 56, 11, 110, DateTimeKind.Utc).AddTicks(371) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("d3333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 7, 56, 11, 110, DateTimeKind.Utc).AddTicks(428), new DateTime(2026, 1, 21, 7, 56, 11, 110, DateTimeKind.Utc).AddTicks(429) });

            migrationBuilder.UpdateData(
                table: "tenant_configurations",
                keyColumn: "id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 7, 56, 11, 108, DateTimeKind.Utc).AddTicks(7203), new DateTime(2026, 1, 21, 7, 56, 11, 108, DateTimeKind.Utc).AddTicks(7206) });

            migrationBuilder.UpdateData(
                table: "tenants",
                keyColumn: "id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000001"),
                columns: new[] { "created_at", "subscription_starts_at", "trial_ends_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 7, 56, 11, 107, DateTimeKind.Utc).AddTicks(1902), new DateTime(2026, 1, 21, 7, 56, 11, 106, DateTimeKind.Utc).AddTicks(6433), new DateTime(2026, 2, 20, 7, 56, 11, 107, DateTimeKind.Utc).AddTicks(672), new DateTime(2026, 1, 21, 7, 56, 11, 107, DateTimeKind.Utc).AddTicks(1906) });

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { new Guid("d1111111-1111-1111-1111-111111111111"), new Guid("e1111111-1111-1111-1111-111111111111") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 113, DateTimeKind.Utc).AddTicks(6237));

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { new Guid("d2222222-2222-2222-2222-222222222222"), new Guid("e2222222-2222-2222-2222-222222222222") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 113, DateTimeKind.Utc).AddTicks(6793));

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { new Guid("d3333333-3333-3333-3333-333333333333"), new Guid("e3333333-3333-3333-3333-333333333333") },
                column: "assigned_at",
                value: new DateTime(2026, 1, 21, 7, 56, 11, 113, DateTimeKind.Utc).AddTicks(6796));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e1111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 7, 56, 11, 113, DateTimeKind.Utc).AddTicks(667), new DateTime(2026, 1, 21, 7, 56, 11, 113, DateTimeKind.Utc).AddTicks(670) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e2222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 7, 56, 11, 113, DateTimeKind.Utc).AddTicks(768), new DateTime(2026, 1, 21, 7, 56, 11, 113, DateTimeKind.Utc).AddTicks(768) });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e3333333-3333-3333-3333-333333333333"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 1, 21, 7, 56, 11, 113, DateTimeKind.Utc).AddTicks(774), new DateTime(2026, 1, 21, 7, 56, 11, 113, DateTimeKind.Utc).AddTicks(775) });
        }
    }
}
