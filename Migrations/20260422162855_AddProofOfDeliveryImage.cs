using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class AddProofOfDeliveryImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "About");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "About");

            migrationBuilder.DropColumn(
                name: "EstablishedYear",
                table: "About");

            migrationBuilder.DropColumn(
                name: "Intro",
                table: "About");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "About");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "About");

            migrationBuilder.AddColumn<string>(
                name: "ProofOfDeliveryImage",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c781ad2b-1123-4f24-9b88-12c5b74681fb",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 23, 28, 54, 778, DateTimeKind.Local).AddTicks(9532));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "s781ad2b-1123-4f24-9b88-12c5b74681fc",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 23, 28, 54, 778, DateTimeKind.Local).AddTicks(9559));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3781ad2b-1123-4f24-9b88-12c5b74681f9",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 22, 23, 28, 54, 778, DateTimeKind.Local).AddTicks(9828));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a781ad2b-1123-4f24-9b88-12c5b74681fa",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 22, 23, 28, 54, 778, DateTimeKind.Local).AddTicks(9839));

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "ProofOfDeliveryImage",
                value: null);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 2,
                column: "ProofOfDeliveryImage",
                value: null);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 3,
                column: "ProofOfDeliveryImage",
                value: null);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 4,
                column: "ProofOfDeliveryImage",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProofOfDeliveryImage",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "About",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "About",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstablishedYear",
                table: "About",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Intro",
                table: "About",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "About",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingHours",
                table: "About",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c781ad2b-1123-4f24-9b88-12c5b74681fb",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 11, 34, 54, 251, DateTimeKind.Local).AddTicks(236));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "s781ad2b-1123-4f24-9b88-12c5b74681fc",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 11, 34, 54, 251, DateTimeKind.Local).AddTicks(251));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3781ad2b-1123-4f24-9b88-12c5b74681f9",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 22, 11, 34, 54, 251, DateTimeKind.Local).AddTicks(406));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a781ad2b-1123-4f24-9b88-12c5b74681fa",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 22, 11, 34, 54, 251, DateTimeKind.Local).AddTicks(415));
        }
    }
}
