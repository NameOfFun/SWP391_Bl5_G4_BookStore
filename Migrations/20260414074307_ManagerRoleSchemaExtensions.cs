using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class ManagerRoleSchemaExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Voucher",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<string>(
                name: "LastManagedByUserId",
                table: "Voucher",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Voucher",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "CustomerRequest",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "CustomerRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestType",
                table: "CustomerRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNotes",
                table: "CustomerRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "CustomerRequest",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolvedByUserId",
                table: "CustomerRequest",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HiddenAt",
                table: "BookRating",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HiddenByUserId",
                table: "BookRating",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "BookRating",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BookRating",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPricingChangeAt",
                table: "Book",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastPricingChangeByUserId",
                table: "Book",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PricingNote",
                table: "Book",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PromotionalEndsAt",
                table: "Book",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PromotionalPrice",
                table: "Book",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PromotionalStartsAt",
                table: "Book",
                type: "datetime",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_LastManagedByUserId",
                table: "Voucher",
                column: "LastManagedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRequest_OrderId",
                table: "CustomerRequest",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRequest_ResolvedByUserId",
                table: "CustomerRequest",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookRating_HiddenByUserId",
                table: "BookRating",
                column: "HiddenByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Book_LastPricingChangeByUserId",
                table: "Book",
                column: "LastPricingChangeByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_AspNetUsers_LastPricingChangeByUserId",
                table: "Book",
                column: "LastPricingChangeByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BookRating_AspNetUsers_HiddenByUserId",
                table: "BookRating",
                column: "HiddenByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRequest_AspNetUsers_ResolvedByUserId",
                table: "CustomerRequest",
                column: "ResolvedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRequest_Order_OrderId",
                table: "CustomerRequest",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_AspNetUsers_LastManagedByUserId",
                table: "Voucher",
                column: "LastManagedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_AspNetUsers_LastPricingChangeByUserId",
                table: "Book");

            migrationBuilder.DropForeignKey(
                name: "FK_BookRating_AspNetUsers_HiddenByUserId",
                table: "BookRating");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRequest_AspNetUsers_ResolvedByUserId",
                table: "CustomerRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRequest_Order_OrderId",
                table: "CustomerRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_AspNetUsers_LastManagedByUserId",
                table: "Voucher");

            migrationBuilder.DropIndex(
                name: "IX_Voucher_LastManagedByUserId",
                table: "Voucher");

            migrationBuilder.DropIndex(
                name: "IX_CustomerRequest_OrderId",
                table: "CustomerRequest");

            migrationBuilder.DropIndex(
                name: "IX_CustomerRequest_ResolvedByUserId",
                table: "CustomerRequest");

            migrationBuilder.DropIndex(
                name: "IX_BookRating_HiddenByUserId",
                table: "BookRating");

            migrationBuilder.DropIndex(
                name: "IX_Book_LastPricingChangeByUserId",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "LastManagedByUserId",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "CustomerRequest");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "CustomerRequest");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "CustomerRequest");

            migrationBuilder.DropColumn(
                name: "ResolutionNotes",
                table: "CustomerRequest");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "CustomerRequest");

            migrationBuilder.DropColumn(
                name: "ResolvedByUserId",
                table: "CustomerRequest");

            migrationBuilder.DropColumn(
                name: "HiddenAt",
                table: "BookRating");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "BookRating");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "BookRating");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BookRating");

            migrationBuilder.DropColumn(
                name: "LastPricingChangeAt",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "LastPricingChangeByUserId",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "PricingNote",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "PromotionalEndsAt",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "PromotionalPrice",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "PromotionalStartsAt",
                table: "Book");
        }
    }
}
