using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShipperModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_ShipperId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_ShipperId",
                table: "Order");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c781ad2b-1123-4f24-9b88-12c5b74681fb", "3781ad2b-1123-4f24-9b88-12c5b74681f9" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "s781ad2b-1123-4f24-9b88-12c5b74681fc", "a781ad2b-1123-4f24-9b88-12c5b74681fa" });

            migrationBuilder.DeleteData(
                table: "Author",
                keyColumn: "AuthorId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Author",
                keyColumn: "AuthorId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "BookTag",
                keyColumn: "TagId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "BookTag",
                keyColumn: "TagId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "BookTagMapping",
                keyColumns: new[] { "BookId", "TagId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "BookTagMapping",
                keyColumns: new[] { "BookId", "TagId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "BookTagMapping",
                keyColumns: new[] { "BookId", "TagId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "BookTagMapping",
                keyColumns: new[] { "BookId", "TagId" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "OrderDetailId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "OrderDetailId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "OrderDetailId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "OrderDetailId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "OrderDetailId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c781ad2b-1123-4f24-9b88-12c5b74681fb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "s781ad2b-1123-4f24-9b88-12c5b74681fc");

            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "BookId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "BookId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "BookId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "BookTag",
                keyColumn: "TagId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BookTag",
                keyColumn: "TagId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BookTag",
                keyColumn: "TagId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3781ad2b-1123-4f24-9b88-12c5b74681f9");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a781ad2b-1123-4f24-9b88-12c5b74681fa");

            migrationBuilder.DeleteData(
                table: "Author",
                keyColumn: "AuthorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Author",
                keyColumn: "AuthorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Author",
                keyColumn: "AuthorId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveryNote",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "FailedNote",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "FailedReason",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ProofOfDeliveryImage",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShipperId",
                table: "Order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Order",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryNote",
                table: "Order",
                type: "nvarchar(500)",
                maxLength: 500,
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

            migrationBuilder.AddColumn<string>(
                name: "ProofOfDeliveryImage",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipperId",
                table: "Order",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedDate", "Description", "IsSystemRole", "Name", "NormalizedName", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { "c781ad2b-1123-4f24-9b88-12c5b74681fb", null, new DateTime(2026, 4, 22, 23, 28, 54, 778, DateTimeKind.Local).AddTicks(9532), null, false, "Customer", "CUSTOMER", true, null },
                    { "s781ad2b-1123-4f24-9b88-12c5b74681fc", null, new DateTime(2026, 4, 22, 23, 28, 54, 778, DateTimeKind.Local).AddTicks(9559), null, false, "Shipper", "SHIPPER", true, null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "Avatar", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Status", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { "3781ad2b-1123-4f24-9b88-12c5b74681f9", 0, null, null, "C1", new DateTime(2026, 4, 22, 23, 28, 54, 778, DateTimeKind.Local).AddTicks(9828), "customer@test.com", true, false, null, "Customer", "CUSTOMER@TEST.COM", "CUSTOMER", "AQAAAAIAAYagAAAAEASa0nB4GfDRbKSXvjFCMnLP/wmAaAZyOsHP76uD+jjf2rNF8qFzdxW1oF7B9DUQtQ==", null, false, "S1", true, false, null, "customer" },
                    { "a781ad2b-1123-4f24-9b88-12c5b74681fa", 0, null, null, "C2", new DateTime(2026, 4, 22, 23, 28, 54, 778, DateTimeKind.Local).AddTicks(9839), "shipper@test.com", true, false, null, "Shipper", "SHIPPER@TEST.COM", "SHIPPER", "AQAAAAIAAYagAAAAEAkeVHFmSD0qERqwTs0YS/J412AX5FLmwWsk9tUwOoDcvMG8j2n7tYV9bqMcVW/hRw==", null, false, "S2", true, false, null, "shipper" }
                });

            migrationBuilder.InsertData(
                table: "Author",
                columns: new[] { "AuthorId", "Bio", "IsActive", "Name", "UpdatedByUserId" },
                values: new object[,]
                {
                    { 1, null, true, "Thiên Giang Trần Kim", null },
                    { 2, null, true, "John Barrow", null },
                    { 3, null, true, "Ngô Sĩ Liên", null },
                    { 4, null, true, "Fredick Bacman", null },
                    { 5, null, true, "Paulo Coelho", null }
                });

            migrationBuilder.InsertData(
                table: "BookTag",
                columns: new[] { "TagId", "IsActive", "Name", "UpdatedByUserId" },
                values: new object[,]
                {
                    { 1, true, "#songcunglichsu", null },
                    { 2, true, "#lichsuthegioi", null },
                    { 3, true, "#lichsuvietnam", null },
                    { 4, true, "#kinhdien", null },
                    { 5, true, "#hottrend", null }
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "CategoryId", "Description", "IsActive", "Name", "UpdatedByUserId" },
                values: new object[,]
                {
                    { 1, null, true, "Lịch sử", null },
                    { 2, null, true, "Văn Học", null },
                    { 3, null, true, "Kinh Dị", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "c781ad2b-1123-4f24-9b88-12c5b74681fb", "3781ad2b-1123-4f24-9b88-12c5b74681f9" },
                    { "s781ad2b-1123-4f24-9b88-12c5b74681fc", "a781ad2b-1123-4f24-9b88-12c5b74681fa" }
                });

            migrationBuilder.InsertData(
                table: "Book",
                columns: new[] { "BookId", "AuthorId", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsActive", "LastPricingChangeAt", "LastPricingChangeByUserId", "Price", "PricingNote", "PromotionalEndsAt", "PromotionalPrice", "PromotionalStartsAt", "Stock", "Title", "UpdatedByUserId" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cuốn sách viết về bối cảnh lịch sử của hầu hết các nước trên thế giới.", "https://sachxua.vn/wp-content/uploads/2020/01/lich-su-the-gioi-sach-ls-600x901.jpg", true, null, null, 500000m, null, null, null, null, 50, "Lịch Sử Thế Giới Tập 1", null },
                    { 2, 2, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ghi chép lại những sự vật, sự việc do chính tác giả nhìn thấy.", "https://sachxua.vn/wp-content/uploads/2020/01/mot-chuyen-du-hanh-xu-nam-ha-sach-ls-600x903.jpg", true, null, null, 200000m, null, null, null, null, 20, "Một chuyến du hành đến xứ Nam Hà", null },
                    { 3, 5, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tiểu thuyết nổi tiếng nhất của nhà văn Brazil Paulo Coelho.", "https://cdn1.fahasa.com/media/catalog/product/i/m/image_195509_1_36793.jpg", true, null, null, 500000m, null, null, null, null, 100, "Nhà Giả Kim", null }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "OrderId", "DeliveredAt", "DeliveryNote", "DiscountAmount", "FailedNote", "FailedReason", "GrandTotal", "OrderDate", "PaymentMethod", "PaymentStatus", "ProofOfDeliveryImage", "ShipperId", "ShippingAddress", "ShippingName", "ShippingPhone", "Status", "SubTotal", "UserId", "VoucherId" },
                values: new object[,]
                {
                    { 1, null, "Giao giờ hành chính", 0m, null, null, 700000m, new DateTime(2024, 4, 19, 5, 0, 0, 0, DateTimeKind.Unspecified), "COD", "Pending", null, "a781ad2b-1123-4f24-9b88-12c5b74681fa", "123 Đường Láng, Hà Nội", "Nguyễn Văn Khách", "0987654321", 3, 700000m, "3781ad2b-1123-4f24-9b88-12c5b74681f9", null },
                    { 2, null, "Để ở bảo vệ", 50000m, null, null, 450000m, new DateTime(2024, 4, 19, 6, 0, 0, 0, DateTimeKind.Unspecified), "Banking", "Paid", null, "a781ad2b-1123-4f24-9b88-12c5b74681fa", "Trường Đại học FPT, Hòa Lạc", "Trần Thị Khách", "0123456789", 3, 500000m, "3781ad2b-1123-4f24-9b88-12c5b74681f9", null },
                    { 3, null, null, 0m, null, null, 200000m, new DateTime(2024, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "COD", "Pending", null, "a781ad2b-1123-4f24-9b88-12c5b74681fa", "Khu đô thị Vincity, Gia Lâm", "Lê Văn Shipper Test", "0999888777", 6, 200000m, "3781ad2b-1123-4f24-9b88-12c5b74681f9", null },
                    { 4, null, null, 0m, null, null, 500000m, new DateTime(2024, 4, 19, 9, 0, 0, 0, DateTimeKind.Unspecified), "COD", "Pending", null, null, "99 Cầu Giấy, Hà Nội", "Hoàng Văn Pool", "0888777666", 1, 500000m, "3781ad2b-1123-4f24-9b88-12c5b74681f9", null }
                });

            migrationBuilder.InsertData(
                table: "BookTagMapping",
                columns: new[] { "BookId", "TagId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 2, 2 },
                    { 3, 4 }
                });

            migrationBuilder.InsertData(
                table: "OrderDetail",
                columns: new[] { "OrderDetailId", "BookId", "OrderId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 500000m },
                    { 2, 2, 1, 1, 200000m },
                    { 3, 1, 2, 1, 500000m },
                    { 4, 2, 3, 1, 200000m },
                    { 5, 3, 4, 1, 500000m }
                });

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
    }
}
