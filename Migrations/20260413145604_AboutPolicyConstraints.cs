using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class AboutPolicyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "Policy",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Policy",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "About",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "About",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policy_RoleId",
                table: "Policy",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Policy_UpdatedByUserId",
                table: "Policy",
                column: "UpdatedByUserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Policy_Title_NonEmpty",
                table: "Policy",
                sql: "LEN(LTRIM(RTRIM([Title]))) > 0");

            migrationBuilder.CreateIndex(
                name: "IX_About_RoleId",
                table: "About",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_About_UpdatedByUserId",
                table: "About",
                column: "UpdatedByUserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_About_Title_NonEmpty",
                table: "About",
                sql: "LEN(LTRIM(RTRIM([Title]))) > 0");

            migrationBuilder.AddForeignKey(
                name: "FK_About_AspNetRoles_RoleId",
                table: "About",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_About_AspNetUsers_UpdatedByUserId",
                table: "About",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Policy_AspNetRoles_RoleId",
                table: "Policy",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Policy_AspNetUsers_UpdatedByUserId",
                table: "Policy",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_About_AspNetRoles_RoleId",
                table: "About");

            migrationBuilder.DropForeignKey(
                name: "FK_About_AspNetUsers_UpdatedByUserId",
                table: "About");

            migrationBuilder.DropForeignKey(
                name: "FK_Policy_AspNetRoles_RoleId",
                table: "Policy");

            migrationBuilder.DropForeignKey(
                name: "FK_Policy_AspNetUsers_UpdatedByUserId",
                table: "Policy");

            migrationBuilder.DropIndex(
                name: "IX_Policy_RoleId",
                table: "Policy");

            migrationBuilder.DropIndex(
                name: "IX_Policy_UpdatedByUserId",
                table: "Policy");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Policy_Title_NonEmpty",
                table: "Policy");

            migrationBuilder.DropIndex(
                name: "IX_About_RoleId",
                table: "About");

            migrationBuilder.DropIndex(
                name: "IX_About_UpdatedByUserId",
                table: "About");

            migrationBuilder.DropCheckConstraint(
                name: "CK_About_Title_NonEmpty",
                table: "About");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Policy");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Policy");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "About");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "About");
        }
    }
}
