using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class AboutAddInfoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberImageUrl",
                table: "About");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "About",
                newName: "WorkingHours");

            migrationBuilder.RenameColumn(
                name: "MemberName",
                table: "About",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "About",
                type: "nvarchar(300)",
                maxLength: 300,
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
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

            migrationBuilder.RenameColumn(
                name: "WorkingHours",
                table: "About",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "About",
                newName: "MemberName");

            migrationBuilder.AddColumn<string>(
                name: "MemberImageUrl",
                table: "About",
                type: "nvarchar(500)",
                maxLength: 500,
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
    }
}
