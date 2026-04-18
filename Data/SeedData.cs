using BookStore.Models;

namespace BookStore.Data;

public static class SeedData
{
    public static void SeedCatalog(BookStoreDbContext db)
    {
        db.Database.EnsureCreated();

        if (!db.Categories.Any()) SeedCategories(db);
        if (!db.BookTags.Any())   SeedBookTags(db);
        if (!db.Authors.Any())    SeedAuthors(db);
        if (!db.Books.Any())      SeedBooks(db);
    }

    private static void SeedCategories(BookStoreDbContext db)
    {
        db.Categories.AddRange(
            new Category { Name = "Lịch sử", IsActive = true },
            new Category { Name = "Văn Học", IsActive = true },
            new Category { Name = "Kinh Dị", IsActive = true }
        );
        db.SaveChanges();
    }

    private static void SeedBookTags(BookStoreDbContext db)
    {
        db.BookTags.AddRange(
            new BookTag { Name = "#songcunglichsu", IsActive = true },
            new BookTag { Name = "#lichsuthegioi",  IsActive = true },
            new BookTag { Name = "#lichsuvietnam",  IsActive = true },
            new BookTag { Name = "#kinhdien",       IsActive = true },
            new BookTag { Name = "#hottrend",       IsActive = true },
            new BookTag { Name = "#bestseller",     IsActive = true },
            new BookTag { Name = "#kinhdi",         IsActive = true }
        );
        db.SaveChanges();
    }

    private static void SeedAuthors(BookStoreDbContext db)
    {
        db.Authors.AddRange(
            new Author { Name = "Thiên Giang Trần Kim",     IsActive = true },
            new Author { Name = "John Barrow",              IsActive = true },
            new Author { Name = "Ngô Sĩ Liên",             IsActive = true },
            new Author { Name = "Fredick Bacman",           IsActive = true },
            new Author { Name = "Paul Coelho",              IsActive = true },
            new Author { Name = "José Mauro de Vasconcelos",IsActive = true },
            new Author { Name = "Hector Malot",             IsActive = true },
            new Author { Name = "Emma Hạ My",              IsActive = true },
            new Author { Name = "Doo Vandenis",             IsActive = true },
            new Author { Name = "Shindo Gonai",             IsActive = true },
            new Author { Name = "Hồng Nương Tử",           IsActive = true }
        );
        db.SaveChanges();
    }

    private static void SeedBooks(BookStoreDbContext db)
    {
        // Look up by name so IDs don't need to be hardcoded
        var catLichSu  = db.Categories.First(c => c.Name == "Lịch sử");
        var catVanHoc  = db.Categories.First(c => c.Name == "Văn Học");
        var catKinhDi  = db.Categories.First(c => c.Name == "Kinh Dị");

        var tagSong      = db.BookTags.First(t => t.Name == "#songcunglichsu");
        var tagLsTheGioi = db.BookTags.First(t => t.Name == "#lichsuthegioi");
        var tagLsVN      = db.BookTags.First(t => t.Name == "#lichsuvietnam");
        var tagKinhDien  = db.BookTags.First(t => t.Name == "#kinhdien");
        var tagHot       = db.BookTags.First(t => t.Name == "#hottrend");
        var tagBest      = db.BookTags.First(t => t.Name == "#bestseller");
        var tagKinhDi    = db.BookTags.First(t => t.Name == "#kinhdi");

        var aThienGiang = db.Authors.First(a => a.Name == "Thiên Giang Trần Kim");
        var aBarrow     = db.Authors.First(a => a.Name == "John Barrow");
        var aNgoSiLien  = db.Authors.First(a => a.Name == "Ngô Sĩ Liên");
        var aBackman    = db.Authors.First(a => a.Name == "Fredick Bacman");
        var aCoelho     = db.Authors.First(a => a.Name == "Paul Coelho");
        var aVasconcelos= db.Authors.First(a => a.Name == "José Mauro de Vasconcelos");
        var aMalot      = db.Authors.First(a => a.Name == "Hector Malot");
        var aEmma       = db.Authors.First(a => a.Name == "Emma Hạ My");
        var aVandenis   = db.Authors.First(a => a.Name == "Doo Vandenis");
        var aShindo     = db.Authors.First(a => a.Name == "Shindo Gonai");
        var aHong       = db.Authors.First(a => a.Name == "Hồng Nương Tử");

        db.Books.AddRange(
            new Book
            {
                Title = "Lịch Sử Thế Giới Tập 1",
                Description = "Được xuất bản từ những năm 50 của thế kỷ 19, \"Lịch sử thế giới\" là cuốn sách viết về bối cảnh lịch sử của hầu hết các nước trên thế giới. Tuy nhiên, tựa sách này có nhiều uẩn khúc chưa được giải đáp trọn vẹn. Vì thế đến nay, cuốn sách mới được xuất bản 1 lần duy nhất và cho đến hiện tại có rất ít người biết về cuốn sách lịch sử này.",
                Price = 500000, Stock = 0, Category = catLichSu, Author = aThienGiang,
                ImageUrl = "https://sachxua.vn/wp-content/uploads/2020/01/lich-su-the-gioi-sach-ls-600x901.jpg",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagSong],
            },
            new Book
            {
                Title = "Một chuyến du hành đến xứ Nam Hà (1972 – 1973)",
                Description = "\"Một chuyến du hành đến xứ Nam Hà\" ghi chép lại những sự vật, sự việc do chính tác giả nhìn thấy. Ngoài ra, tác giả còn phân tích thêm những vấn đề về lịch sử, kinh tế, xã hội, chính trị theo ý hiểu của riêng mình một cách tường tận và chi tiết.",
                Price = 200000, Stock = 20, Category = catLichSu, Author = aBarrow,
                ImageUrl = "https://sachxua.vn/wp-content/uploads/2020/01/mot-chuyen-du-hanh-xu-nam-ha-sach-ls-600x903.jpg",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagSong, tagLsTheGioi, tagLsVN],
            },
            new Book
            {
                Title = "Đại Việt sử ký toàn thư",
                Description = "Có thể coi \"Đại Việt sử ký toàn thư\" là một trong những cuốn quốc sử hào hùng của dân tộc ta và là một di sản không có tác phẩm nào có thể thay thế. Nội dung cuốn sách lịch sử này ghi chép lại hầu hết các diễn biến diễn ra trong lịch sử. Các nhân vật nổi tiếng về lịch sử được nhắc đến như: Phạm Công Trứ, Ngô Sỹ Liên, Lê Văn Hưu, Lê Hy…",
                Price = 1000000, Stock = 300, Category = catLichSu, Author = aNgoSiLien,
                ImageUrl = "https://sachxua.vn/wp-content/uploads/2020/01/dai-viet-su-ky-toan-thu-sach-ls.jpg",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagSong, tagLsVN],
            },
            new Book
            {
                Title = "Người Đàn Ông Mang Tên OVE",
                Description = "Người Đàn Ông Mang Tên Ove là tiểu thuyết đầu tay của nhà văn Thụy Điển Fredrik Backman, xuất bản năm 2012 và nhanh chóng trở thành một hiện tượng toàn cầu, được yêu mến bởi hàng triệu độc giả nhờ câu chuyện vừa hài hước, vừa cảm động về tình người.",
                Price = 300000, Stock = 20, Category = catVanHoc, Author = aBackman,
                ImageUrl = "https://cdn1.fahasa.com/media/catalog/product/8/9/8934974182375.jpg",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagKinhDien],
            },
            new Book
            {
                Title = "Nhà Giả Kim",
                Description = "Nhà Giả Kim là tiểu thuyết nổi tiếng nhất của nhà văn Brazil Paulo Coelho, xuất bản lần đầu tiên vào năm 1988 tại Brazil và nhanh chóng trở thành một hiện tượng toàn cầu, được dịch ra hơn 80 ngôn ngữ và bán hơn 65 triệu bản.",
                Price = 500000, Stock = 10, Category = catVanHoc, Author = aCoelho,
                ImageUrl = "https://cdn1.fahasa.com/media/catalog/product/i/m/image_195509_1_36793.jpg",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagKinhDien, tagHot, tagBest],
            },
            new Book
            {
                Title = "Cây Cam Ngọt Của Tôi",
                Description = "Cây Cam Ngọt Của Tôi là một tiểu thuyết bán tự truyện nổi tiếng của nhà văn Brazil José Mauro de Vasconcelos, xuất bản lần đầu tiên vào năm 1968 và nhanh chóng trở thành một tác phẩm kinh điển trong văn học thiếu nhi lẫn người lớn.",
                Price = 200000, Stock = 10, Category = catVanHoc, Author = aVasconcelos,
                ImageUrl = "https://cdn1.fahasa.com/media/catalog/product/i/m/image_217480.jpg",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagKinhDien, tagHot],
            },
            new Book
            {
                Title = "Không Gia Đình",
                Description = "Không Gia Đình là một tiểu thuyết kinh điển của nhà văn Pháp Hector Malot, xuất bản lần đầu tiên vào năm 1878, và từ đó trở thành một trong những tác phẩm văn học thiếu nhi nổi tiếng nhất thế giới.",
                Price = 100000, Stock = 30, Category = catVanHoc, Author = aMalot,
                ImageUrl = "https://cdn1.fahasa.com/media/catalog/product/z/5/z5767334826845_2e61b754a821ac7fd232d2553b051dd2_1.jpg",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagKinhDien, tagHot, tagBest],
            },
            new Book
            {
                Title = "Sĩ Số Lớp Vắng 0",
                Description = "\"Sĩ Số Lớp Vắng 0\" là cuốn sách kinh dị rùng rợn của tác giả Emma Hạ My – tác giả và họa sĩ hoạt họa đứng sau kênh Youtube \"Truyện của Emma\" nổi tiếng với những câu chuyện ma gây ám ảnh do chính cô tự sáng tác.",
                Price = 100000, Stock = 10, Category = catKinhDi, Author = aEmma,
                ImageUrl = "https://cdn1.fahasa.com/media/catalog/product/2/4/24d0a082-972d-44c5-bcc3-5b41f575cbd5.jpeg",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagKinhDien, tagKinhDi],
            },
            new Book
            {
                Title = "Lớp Có Tang Sự Không Cần Điểm Danh",
                Description = "\"Lớp Có Tang Sự Không Cần Điểm Danh\" mở ra một thế giới học đường bất thường, mỗi tháng lại có một lớp bị bao trùm bởi hiện tượng Tang Sự, một sự kiện kỳ bí khiến bảng tên lớp bị bôi đỏ và một chiếc quan tài lạnh lẽo xuất hiện ngay trong phòng học.",
                Price = 150000, Stock = 14, Category = catKinhDi, Author = aVandenis,
                ImageUrl = "https://cdn1.fahasa.com/media/flashmagazine/images/page_images/lop_co_tang_su_khong_can_diem_danh/2024_11_22_16_41_49_1-390x510.png",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagKinhDi],
            },
            new Book
            {
                Title = "Thầy Cúng Kể Chuyện Trừ Tà",
                Description = "\"Thầy Cúng Kể Chuyện Trừ Tà – Tổng Hợp 100 Câu Chuyện Ma Quái\" là một tuyển tập đặc sắc được nhà văn Shindo Gonai dày công sưu tầm từ nhiều nguồn khác nhau, kết hợp với những trải nghiệm thực tế của chính tác giả.",
                Price = 200000, Stock = 5, Category = catKinhDi, Author = aShindo,
                ImageUrl = "https://cdn1.fahasa.com/media/catalog/product/b/i/bia_thay_cung_ke_chuyen_tru_ta.png",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagKinhDien, tagBest, tagKinhDi],
            },
            new Book
            {
                Title = "Quỷ Tiết",
                Description = "\"Quỷ Tiết\" là nơi không gian bị xoắn vặn, thời gian đảo lộn và những linh hồn oan khuất không ngừng luân hồi.",
                Price = 200000, Stock = 15, Category = catKinhDi, Author = aHong,
                ImageUrl = "https://cdn1.fahasa.com/media/catalog/product/b/i/bia_quy_tiet.jpg",
                IsActive = true, CreatedAt = DateTime.Now,
                Tags = [tagKinhDien, tagKinhDi],
            }
        );

        db.SaveChanges();
    }
}
