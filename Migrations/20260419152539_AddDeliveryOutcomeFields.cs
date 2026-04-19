using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryOutcomeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Order",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailedNote",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailedReason",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c781ad2b-1123-4f24-9b88-12c5b74681fb",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 19, 22, 25, 39, 232, DateTimeKind.Local).AddTicks(6589));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "s781ad2b-1123-4f24-9b88-12c5b74681fc",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 19, 22, 25, 39, 232, DateTimeKind.Local).AddTicks(6609));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3781ad2b-1123-4f24-9b88-12c5b74681f9",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 19, 22, 25, 39, 232, DateTimeKind.Local).AddTicks(6833));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a781ad2b-1123-4f24-9b88-12c5b74681fa",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 19, 22, 25, 39, 232, DateTimeKind.Local).AddTicks(6844));

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1,
                columns: new[] { "DeliveredAt", "FailedNote", "FailedReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 2,
                columns: new[] { "DeliveredAt", "FailedNote", "FailedReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 3,
                columns: new[] { "DeliveredAt", "FailedNote", "FailedReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 4,
                columns: new[] { "DeliveredAt", "FailedNote", "FailedReason" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "FailedNote",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "FailedReason",
                table: "Order");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c781ad2b-1123-4f24-9b88-12c5b74681fb",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 19, 22, 16, 5, 174, DateTimeKind.Local).AddTicks(5675));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "s781ad2b-1123-4f24-9b88-12c5b74681fc",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 19, 22, 16, 5, 174, DateTimeKind.Local).AddTicks(5694));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3781ad2b-1123-4f24-9b88-12c5b74681f9",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 19, 22, 16, 5, 174, DateTimeKind.Local).AddTicks(5890));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a781ad2b-1123-4f24-9b88-12c5b74681fa",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 19, 22, 16, 5, 174, DateTimeKind.Local).AddTicks(5993));
        }
    }
}
