using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class AddShipperRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShipperId",
                table: "Order",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_ShipperId",
                table: "Order",
                column: "ShipperId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_ShipperId",
                table: "Order",
                column: "ShipperId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_ShipperId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ShipperId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShipperId",
                table: "Order");
        }
    }
}
