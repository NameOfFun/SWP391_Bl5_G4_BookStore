using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberFieldsToAbout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MemberImageUrl",
                table: "About",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberName",
                table: "About",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "About",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c781ad2b-1123-4f24-9b88-12c5b74681fb",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 10, 31, 26, 605, DateTimeKind.Local).AddTicks(8498));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "s781ad2b-1123-4f24-9b88-12c5b74681fc",
                column: "CreatedDate",
                value: new DateTime(2026, 4, 22, 10, 31, 26, 605, DateTimeKind.Local).AddTicks(8517));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3781ad2b-1123-4f24-9b88-12c5b74681f9",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 22, 10, 31, 26, 605, DateTimeKind.Local).AddTicks(8642));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a781ad2b-1123-4f24-9b88-12c5b74681fa",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 22, 10, 31, 26, 605, DateTimeKind.Local).AddTicks(8661));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberImageUrl",
                table: "About");

            migrationBuilder.DropColumn(
                name: "MemberName",
                table: "About");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "About");

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
        }
    }
}
