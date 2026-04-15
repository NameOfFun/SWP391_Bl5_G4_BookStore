using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class CatalogUpdatedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Category",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "BookTag",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Book",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Author",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_UpdatedByUserId",
                table: "Category",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTag_UpdatedByUserId",
                table: "BookTag",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Book_UpdatedByUserId",
                table: "Book",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Author_UpdatedByUserId",
                table: "Author",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Author_AspNetUsers_UpdatedByUserId",
                table: "Author",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Book_AspNetUsers_UpdatedByUserId",
                table: "Book",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTag_AspNetUsers_UpdatedByUserId",
                table: "BookTag",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Category_AspNetUsers_UpdatedByUserId",
                table: "Category",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Author_AspNetUsers_UpdatedByUserId",
                table: "Author");

            migrationBuilder.DropForeignKey(
                name: "FK_Book_AspNetUsers_UpdatedByUserId",
                table: "Book");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTag_AspNetUsers_UpdatedByUserId",
                table: "BookTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_UpdatedByUserId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_UpdatedByUserId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_BookTag_UpdatedByUserId",
                table: "BookTag");

            migrationBuilder.DropIndex(
                name: "IX_Book_UpdatedByUserId",
                table: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Author_UpdatedByUserId",
                table: "Author");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BookTag");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Author");
        }
    }
}
