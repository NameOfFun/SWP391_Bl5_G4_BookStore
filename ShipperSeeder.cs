// ============================================================
//  SHIPPER SEED SCRIPT  — chạy bằng:
//  Thêm vào Program.cs (tạm thời) hoặc chạy riêng
//  Seed tạo user + đơn hàng test cho Shipper Dashboard
// ============================================================
//  CÁCH SỬ DỤNG:
//  1. Mở Program.cs
//  2. Dán đoạn code ShipperSeeder.SeedAsync(...) vào sau block role seeding
//  3. Chạy app 1 lần → xóa đi
// ============================================================

using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore;

public static class ShipperSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var db          = services.GetRequiredService<BookStoreDbContext>();

        // ── 1. Tạo User ──────────────────────────────────────────
        var shipper   = await GetOrCreateUser(userManager, "shipper01",  "shipper01@bookstore.vn",  "Test@123", "Shipper");
        var customer1 = await GetOrCreateUser(userManager, "customer01", "customer01@bookstore.vn", "Test@123", "Customer");
        var customer2 = await GetOrCreateUser(userManager, "customer02", "customer02@bookstore.vn", "Test@123", "Customer");

        // ── 2. Author + Category + Book ──────────────────────────
        var author1 = await EnsureAuthor(db, "Nguyễn Nhật Ánh", "Nhà văn nổi tiếng với nhiều tác phẩm thiếu nhi.");
        var author2 = await EnsureAuthor(db, "Tô Hoài",         "Tác giả của Dế Mèn Phiêu Lưu Ký.");
        var author3 = await EnsureAuthor(db, "Nam Quốc Chánh",  "Tác giả trẻ viết về kinh doanh và khởi nghiệp.");

        var catLit = await EnsureCategory(db, "Văn học",    "Tiểu thuyết, truyện ngắn, thơ ca.");
        var catKid = await EnsureCategory(db, "Thiếu nhi",  "Sách dành cho trẻ em và thanh thiếu niên.");
        var catBiz = await EnsureCategory(db, "Kinh doanh", "Sách về kinh tế, quản trị, khởi nghiệp.");

        var book1 = await EnsureBook(db, "Tôi thấy hoa vàng trên cỏ xanh", 85000m,  catLit.CategoryId, author1.AuthorId);
        var book2 = await EnsureBook(db, "Mắt biếc",                        79000m,  catLit.CategoryId, author1.AuthorId);
        var book3 = await EnsureBook(db, "Dế Mèn Phiêu Lưu Ký",            55000m,  catKid.CategoryId, author2.AuthorId);
        var book4 = await EnsureBook(db, "Khởi nghiệp tinh gọn",           120000m, catBiz.CategoryId, author3.AuthorId);

        // ── 3. Orders ─────────────────────────────────────────────
        // Chỉ tạo nếu shipper01 chưa có đơn nào
        bool hasOrders = await db.Orders.AnyAsync(o => o.ShipperId == shipper.Id);
        if (hasOrders)
        {
            Console.WriteLine("[Seeder] Shipper đã có đơn hàng — bỏ qua.");
            return;
        }

        var now = DateTime.Now;
        var orders = new List<(Order order, List<(Book book, int qty)> items)>
        {
            // ── SHIPPED (đang giao) ──────────────────────────────
            (new Order
            {
                UserId = customer1.Id, ShipperId = shipper.Id,
                OrderDate = now.AddHours(-26), Status = OrderStatus.Shipped,
                SubTotal = 85000, DiscountAmount = 0, GrandTotal = 85000,
                ShippingName = "Nguyễn Văn An", ShippingPhone = "0901234567",
                ShippingAddress = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                PaymentMethod = "COD", PaymentStatus = "Pending"
            }, [(book1, 1)]),

            (new Order
            {
                UserId = customer2.Id, ShipperId = shipper.Id,
                OrderDate = now.AddHours(-14), Status = OrderStatus.Shipped,
                SubTotal = 164000, DiscountAmount = 6000, GrandTotal = 158000,
                ShippingName = "Trần Thị Bình", ShippingPhone = "0912345678",
                ShippingAddress = "456 Lê Lợi, Quận 3, TP.HCM",
                PaymentMethod = "COD", PaymentStatus = "Pending"
            }, [(book1, 1), (book2, 1)]),

            (new Order
            {
                UserId = customer1.Id, ShipperId = shipper.Id,
                OrderDate = now.AddHours(-6), Status = OrderStatus.Shipped,
                SubTotal = 120000, DiscountAmount = 10000, GrandTotal = 110000,
                ShippingName = "Lê Minh Cường", ShippingPhone = "0923456789",
                ShippingAddress = "789 Trần Hưng Đạo, Quận 5, TP.HCM",
                PaymentMethod = "VNPay", PaymentStatus = "Paid"
            }, [(book4, 1)]),

            (new Order
            {
                UserId = customer2.Id, ShipperId = shipper.Id,
                OrderDate = now.AddHours(-2), Status = OrderStatus.Shipped,
                SubTotal = 55000, DiscountAmount = 0, GrandTotal = 55000,
                ShippingName = "Phạm Thu Hà", ShippingPhone = "0934567890",
                ShippingAddress = "321 Bùi Viện, Quận 1, TP.HCM",
                PaymentMethod = "COD", PaymentStatus = "Pending"
            }, [(book3, 1)]),

            // ── DELIVERED (đã giao thành công) ──────────────────
            (new Order
            {
                UserId = customer1.Id, ShipperId = shipper.Id,
                OrderDate = now.AddDays(-3), Status = OrderStatus.Delivered,
                SubTotal = 205000, DiscountAmount = 21000, GrandTotal = 184000,
                ShippingName = "Hoàng Văn Đức", ShippingPhone = "0945678901",
                ShippingAddress = "654 Phan Xích Long, Phú Nhuận, TP.HCM",
                PaymentMethod = "COD", PaymentStatus = "Paid"
            }, [(book1, 1), (book4, 1)]),

            (new Order
            {
                UserId = customer2.Id, ShipperId = shipper.Id,
                OrderDate = now.AddDays(-2), Status = OrderStatus.Delivered,
                SubTotal = 79000, DiscountAmount = 0, GrandTotal = 79000,
                ShippingName = "Vũ Ngọc Lan", ShippingPhone = "0956789012",
                ShippingAddress = "987 Nguyễn Đình Chiểu, Quận 3, TP.HCM",
                PaymentMethod = "VNPay", PaymentStatus = "Paid"
            }, [(book2, 1)]),

            (new Order
            {
                UserId = customer1.Id, ShipperId = shipper.Id,
                OrderDate = now.AddDays(-1), Status = OrderStatus.Delivered,
                SubTotal = 175000, DiscountAmount = 15000, GrandTotal = 160000,
                ShippingName = "Đặng Bảo Ngọc", ShippingPhone = "0967890123",
                ShippingAddress = "147 Đinh Tiên Hoàng, Bình Thạnh, TP.HCM",
                PaymentMethod = "COD", PaymentStatus = "Paid"
            }, [(book1, 1), (book3, 2)]),

            // ── CANCELLED (giao thất bại) ────────────────────────
            (new Order
            {
                UserId = customer2.Id, ShipperId = shipper.Id,
                OrderDate = now.AddDays(-5), Status = OrderStatus.Cancelled,
                SubTotal = 85000, DiscountAmount = 0, GrandTotal = 85000,
                ShippingName = "Ngô Thị Mai", ShippingPhone = "0978901234",
                ShippingAddress = "258 Cách Mạng Tháng 8, Quận 10, TP.HCM",
                PaymentMethod = "COD", PaymentStatus = "Pending"
            }, [(book1, 1)]),

            (new Order
            {
                UserId = customer1.Id, ShipperId = shipper.Id,
                OrderDate = now.AddDays(-4), Status = OrderStatus.Cancelled,
                SubTotal = 120000, DiscountAmount = 0, GrandTotal = 120000,
                ShippingName = "Lý Thành Nam", ShippingPhone = "0989012345",
                ShippingAddress = "369 Tô Hiến Thành, Quận 10, TP.HCM",
                PaymentMethod = "COD", PaymentStatus = "Pending"
            }, [(book4, 1)]),
        };

        foreach (var (order, items) in orders)
        {
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            foreach (var (book, qty) in items)
            {
                db.OrderDetails.Add(new OrderDetail
                {
                    OrderId   = order.OrderId,
                    BookId    = book.BookId,
                    Quantity  = qty,
                    UnitPrice = book.Price ?? 0
                });
            }
        }

        await db.SaveChangesAsync();
        Console.WriteLine("[Seeder] ✅ Seed hoàn tất!");
        Console.WriteLine("[Seeder] Đăng nhập: shipper01 / Test@123");
        Console.WriteLine("[Seeder] Dashboard:  /Shipper/Dashboard");
    }

    // ── Helpers ───────────────────────────────────────────────
    private static async Task<IdentityUser> GetOrCreateUser(
        UserManager<IdentityUser> um, string username, string email, string password, string role)
    {
        var user = await um.FindByNameAsync(username);
        if (user is not null) return user;

        user = new IdentityUser { UserName = username, Email = email, EmailConfirmed = true };
        var result = await um.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception($"Tạo user '{username}' thất bại: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        await um.AddToRoleAsync(user, role);
        Console.WriteLine($"[Seeder] Created user: {username} ({role})");
        return user;
    }

    private static async Task<Author> EnsureAuthor(BookStoreDbContext db, string name, string bio)
    {
        var existing = await db.Authors.FirstOrDefaultAsync(a => a.Name == name);
        if (existing is not null) return existing;
        var author = new Author { Name = name, Bio = bio, IsActive = true };
        db.Authors.Add(author);
        await db.SaveChangesAsync();
        return author;
    }

    private static async Task<Category> EnsureCategory(BookStoreDbContext db, string name, string desc)
    {
        var existing = await db.Categories.FirstOrDefaultAsync(c => c.Name == name);
        if (existing is not null) return existing;
        var cat = new Category { Name = name, Description = desc, IsActive = true };
        db.Categories.Add(cat);
        await db.SaveChangesAsync();
        return cat;
    }

    private static async Task<Book> EnsureBook(BookStoreDbContext db, string title, decimal price, int catId, int authorId)
    {
        var existing = await db.Books.FirstOrDefaultAsync(b => b.Title == title);
        if (existing is not null) return existing;
        var book = new Book
        {
            Title = title, Price = price, Stock = 50,
            CategoryId = catId, AuthorId = authorId,
            IsActive = true, CreatedAt = DateTime.Now
        };
        db.Books.Add(book);
        await db.SaveChangesAsync();
        return book;
    }
}
