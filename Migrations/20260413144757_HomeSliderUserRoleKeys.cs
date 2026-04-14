using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class HomeSliderUserRoleKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "HomeSlider",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "HomeSlider",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HomeSlider_CreatedByUserId",
                table: "HomeSlider",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeSlider_RoleId",
                table: "HomeSlider",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeSlider_AspNetRoles_RoleId",
                table: "HomeSlider",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_HomeSlider_AspNetUsers_CreatedByUserId",
                table: "HomeSlider",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeSlider_AspNetRoles_RoleId",
                table: "HomeSlider");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeSlider_AspNetUsers_CreatedByUserId",
                table: "HomeSlider");

            migrationBuilder.DropIndex(
                name: "IX_HomeSlider_CreatedByUserId",
                table: "HomeSlider");

            migrationBuilder.DropIndex(
                name: "IX_HomeSlider_RoleId",
                table: "HomeSlider");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "HomeSlider");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "HomeSlider");
        }
    }
}
