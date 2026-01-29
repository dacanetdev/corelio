using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Corelio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CheckPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e1111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$12$9XY1aM3lapFOiGY/FhoPe.7yFNQEM5I7mV.0FvWHdiwcfbI/1/xPa");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e2222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$12$C0aR7VAyIrwnpVXnyJgghu1U7J/TSDPUBOToWBqTJd.KmNdktsGea");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e3333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$12$p0.21IB1wquXLAtxzYNNE.u8tr4lrGyNXU75ovrtUTLZsnrsJYLBe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e1111111-1111-1111-1111-111111111111"),
                column: "password_hash",
                value: "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e2222222-2222-2222-2222-222222222222"),
                column: "password_hash",
                value: "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e3333333-3333-3333-3333-333333333333"),
                column: "password_hash",
                value: "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIq7MRnH.m");
        }
    }
}
