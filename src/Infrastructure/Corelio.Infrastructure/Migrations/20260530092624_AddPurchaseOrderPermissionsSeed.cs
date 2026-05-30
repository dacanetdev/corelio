using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseOrderPermissionsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "category", "code", "created_at", "description", "module", "name" },
                values: new object[,]
                {
                    { new Guid("c1111111-1111-1111-1111-111111111111"), null, "purchases.view",    new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "View purchase orders",          "Purchases", "View Purchase Orders"    },
                    { new Guid("c1111111-1111-1111-1111-111111111112"), null, "purchases.create",  new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Create purchase orders",        "Purchases", "Create Purchase Orders"  },
                    { new Guid("c1111111-1111-1111-1111-111111111113"), null, "purchases.submit",  new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Submit purchase orders",        "Purchases", "Submit Purchase Orders"  },
                    { new Guid("c1111111-1111-1111-1111-111111111114"), null, "purchases.approve", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Approve purchase orders",       "Purchases", "Approve Purchase Orders" },
                    { new Guid("c1111111-1111-1111-1111-111111111115"), null, "purchases.cancel",  new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Cancel purchase orders",        "Purchases", "Cancel Purchase Orders"  }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id", "assigned_at", "assigned_by" },
                values: new object[,]
                {
                    // Administrator gets all purchase permissions
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c1111111-1111-1111-1111-111111111113"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c1111111-1111-1111-1111-111111111114"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c1111111-1111-1111-1111-111111111115"), new Guid("d1111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    // Manager gets all purchase permissions
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c1111111-1111-1111-1111-111111111113"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c1111111-1111-1111-1111-111111111114"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c1111111-1111-1111-1111-111111111115"), new Guid("d2222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    // Viewer/Cashier gets view-only
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("d3333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("d1111111-1111-1111-1111-111111111111") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111112"), new Guid("d1111111-1111-1111-1111-111111111111") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111113"), new Guid("d1111111-1111-1111-1111-111111111111") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111114"), new Guid("d1111111-1111-1111-1111-111111111111") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111115"), new Guid("d1111111-1111-1111-1111-111111111111") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("d2222222-2222-2222-2222-222222222222") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111112"), new Guid("d2222222-2222-2222-2222-222222222222") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111113"), new Guid("d2222222-2222-2222-2222-222222222222") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111114"), new Guid("d2222222-2222-2222-2222-222222222222") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111115"), new Guid("d2222222-2222-2222-2222-222222222222") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("d3333333-3333-3333-3333-333333333333") });

            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("c1111111-1111-1111-1111-111111111111"));
            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("c1111111-1111-1111-1111-111111111112"));
            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("c1111111-1111-1111-1111-111111111113"));
            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("c1111111-1111-1111-1111-111111111114"));
            migrationBuilder.DeleteData(table: "permissions", keyColumn: "id", keyValue: new Guid("c1111111-1111-1111-1111-111111111115"));
        }
    }
}
