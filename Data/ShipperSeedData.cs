using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BookStore.Data
{
    public static class ShipperSeedData
    {
        // Fixed IDs for Identity Users to link with Orders
        public const string CUSTOMER_ID = "3781ad2b-1123-4f24-9b88-12c5b74681f9";
        public const string SHIPPER_ID = "a781ad2b-1123-4f24-9b88-12c5b74681fa";

        private const string ROLE_CUSTOMER_ID = "c781ad2b-1123-4f24-9b88-12c5b74681fb";
        private const string ROLE_SHIPPER_ID  = "s781ad2b-1123-4f24-9b88-12c5b74681fc";

        public static void Seed(ModelBuilder modelBuilder)
        {
            // 0. Seed Roles and Users (Required for Order Foreign Keys)
            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = ROLE_CUSTOMER_ID, Name = "Customer", NormalizedName = "CUSTOMER", Status = true, CreatedDate = DateTime.Now },
                new ApplicationRole { Id = ROLE_SHIPPER_ID,  Name = "Shipper",  NormalizedName = "SHIPPER",  Status = true, CreatedDate = DateTime.Now }
            );

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = CUSTOMER_ID,
                    UserName = "customer",
                    NormalizedUserName = "CUSTOMER",
                    Email = "customer@test.com",
                    NormalizedEmail = "CUSTOMER@TEST.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEASa0nB4GfDRbKSXvjFCMnLP/wmAaAZyOsHP76uD+jjf2rNF8qFzdxW1oF7B9DUQtQ==", // Customer@123
                    SecurityStamp = "S1",
                    ConcurrencyStamp = "C1",
                    Name = "Customer",
                    Status = true,
                    CreatedAt = DateTime.Now
                },
                new ApplicationUser
                {
                    Id = SHIPPER_ID,
                    UserName = "shipper",
                    NormalizedUserName = "SHIPPER",
                    Email = "shipper@test.com",
                    NormalizedEmail = "SHIPPER@TEST.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEAkeVHFmSD0qERqwTs0YS/J412AX5FLmwWsk9tUwOoDcvMG8j2n7tYV9bqMcVW/hRw==", // Shipper@123
                    SecurityStamp = "S2",
                    ConcurrencyStamp = "C2",
                    Name = "Shipper",
                    Status = true,
                    CreatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().HasData(
                new Microsoft.AspNetCore.Identity.IdentityUserRole<string> { UserId = CUSTOMER_ID, RoleId = ROLE_CUSTOMER_ID },
                new Microsoft.AspNetCore.Identity.IdentityUserRole<string> { UserId = SHIPPER_ID,  RoleId = ROLE_SHIPPER_ID }
            );

            // 1. Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Lịch sử", IsActive = true },
                new Category { CategoryId = 2, Name = "Văn Học", IsActive = true },
                new Category { CategoryId = 3, Name = "Kinh Dị", IsActive = true }
            );

            // 2. Seed Authors
            modelBuilder.Entity<Author>().HasData(
                new Author { AuthorId = 1, Name = "Thiên Giang Trần Kim", IsActive = true },
                new Author { AuthorId = 2, Name = "John Barrow", IsActive = true },
                new Author { AuthorId = 3, Name = "Ngô Sĩ Liên", IsActive = true },
                new Author { AuthorId = 4, Name = "Fredick Bacman", IsActive = true },
                new Author { AuthorId = 5, Name = "Paulo Coelho", IsActive = true }
            );

            // 3. Seed BookTags
            modelBuilder.Entity<BookTag>().HasData(
                new BookTag { TagId = 1, Name = "#songcunglichsu", IsActive = true },
                new BookTag { TagId = 2, Name = "#lichsuthegioi", IsActive = true },
                new BookTag { TagId = 3, Name = "#lichsuvietnam", IsActive = true },
                new BookTag { TagId = 4, Name = "#kinhdien", IsActive = true },
                new BookTag { TagId = 5, Name = "#hottrend", IsActive = true }
            );

            // 4. Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    Title = "Lịch Sử Thế Giới Tập 1",
                    Description = "Cuốn sách viết về bối cảnh lịch sử của hầu hết các nước trên thế giới.",
                    Price = 500000,
                    Stock = 50,
                    CategoryId = 1,
                    AuthorId = 1,
                    ImageUrl = "https://sachxua.vn/wp-content/uploads/2020/01/lich-su-the-gioi-sach-ls-600x901.jpg",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)
                },
                new Book
                {
                    BookId = 2,
                    Title = "Một chuyến du hành đến xứ Nam Hà",
                    Description = "Ghi chép lại những sự vật, sự việc do chính tác giả nhìn thấy.",
                    Price = 200000,
                    Stock = 20,
                    CategoryId = 1,
                    AuthorId = 2,
                    ImageUrl = "https://sachxua.vn/wp-content/uploads/2020/01/mot-chuyen-du-hanh-xu-nam-ha-sach-ls-600x903.jpg",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)
                },
                new Book
                {
                    BookId = 3,
                    Title = "Nhà Giả Kim",
                    Description = "Tiểu thuyết nổi tiếng nhất của nhà văn Brazil Paulo Coelho.",
                    Price = 500000,
                    Stock = 100,
                    CategoryId = 2,
                    AuthorId = 5,
                    ImageUrl = "https://cdn1.fahasa.com/media/catalog/product/i/m/image_195509_1_36793.jpg",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            );

            // 5. Seed BookTag Mapping (Shadow Entity)
            modelBuilder.Entity("BookTagMapping").HasData(
                new { BookId = 1, TagId = 1 },
                new { BookId = 2, TagId = 1 },
                new { BookId = 2, TagId = 2 },
                new { BookId = 3, TagId = 4 }
            );

            // 6. Seed Orders for Shipper Testing
            var orderDate = new DateTime(2024, 4, 19, 10, 0, 0);

            modelBuilder.Entity<Order>().HasData(
                // Case 1: Assigned to Shipper (Status Shipped - Waiting for Acceptance)
                new Order
                {
                    OrderId = 1,
                    UserId = CUSTOMER_ID,
                    ShipperId = SHIPPER_ID,
                    OrderDate = orderDate.AddHours(-5),
                    Status = OrderStatus.Shipped,
                    SubTotal = 700000,
                    DiscountAmount = 0,
                    GrandTotal = 700000,
                    ShippingName = "Nguyễn Văn Khách",
                    ShippingPhone = "0987654321",
                    ShippingAddress = "123 Đường Láng, Hà Nội",
                    PaymentMethod = "COD",
                    PaymentStatus = "Pending",
                    DeliveryNote = "Giao giờ hành chính"
                },
                new Order
                {
                    OrderId = 2,
                    UserId = CUSTOMER_ID,
                    ShipperId = SHIPPER_ID,
                    OrderDate = orderDate.AddHours(-4),
                    Status = OrderStatus.Shipped,
                    SubTotal = 500000,
                    DiscountAmount = 50000,
                    GrandTotal = 450000,
                    ShippingName = "Trần Thị Khách",
                    ShippingPhone = "0123456789",
                    ShippingAddress = "Trường Đại học FPT, Hòa Lạc",
                    PaymentMethod = "Banking",
                    PaymentStatus = "Paid",
                    DeliveryNote = "Để ở bảo vệ"
                },
                // Case 2: Currently Delivering (Accepted by Shipper)
                new Order
                {
                    OrderId = 3,
                    UserId = CUSTOMER_ID,
                    ShipperId = SHIPPER_ID,
                    OrderDate = orderDate.AddHours(-10),
                    Status = OrderStatus.Delivering,
                    SubTotal = 200000,
                    DiscountAmount = 0,
                    GrandTotal = 200000,
                    ShippingName = "Lê Văn Shipper Test",
                    ShippingPhone = "0999888777",
                    ShippingAddress = "Khu đô thị Vincity, Gia Lâm",
                    PaymentMethod = "COD",
                    PaymentStatus = "Pending"
                },
                // Case 3: Confirmed but Not Assigned (Pool for testing assignment)
                new Order
                {
                    OrderId = 4,
                    UserId = CUSTOMER_ID,
                    ShipperId = null,
                    OrderDate = orderDate.AddHours(-1),
                    Status = OrderStatus.Confirmed,
                    SubTotal = 500000,
                    DiscountAmount = 0,
                    GrandTotal = 500000,
                    ShippingName = "Hoàng Văn Pool",
                    ShippingPhone = "0888777666",
                    ShippingAddress = "99 Cầu Giấy, Hà Nội",
                    PaymentMethod = "COD",
                    PaymentStatus = "Pending"
                }
            );

            // 7. Seed OrderDetails
            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail { OrderDetailId = 1, OrderId = 1, BookId = 1, Quantity = 1, UnitPrice = 500000 },
                new OrderDetail { OrderDetailId = 2, OrderId = 1, BookId = 2, Quantity = 1, UnitPrice = 200000 },
                new OrderDetail { OrderDetailId = 3, OrderId = 2, BookId = 1, Quantity = 1, UnitPrice = 500000 },
                new OrderDetail { OrderDetailId = 4, OrderId = 3, BookId = 2, Quantity = 1, UnitPrice = 200000 },
                new OrderDetail { OrderDetailId = 5, OrderId = 4, BookId = 3, Quantity = 1, UnitPrice = 500000 }
            );
        }
    }
}
