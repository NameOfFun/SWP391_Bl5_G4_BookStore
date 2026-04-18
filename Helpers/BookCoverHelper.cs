using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BookStore.Helpers;

public static class BookCoverHelper
{
    private static readonly HashSet<string> AllowedCoverExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private const long MaxCoverBytes = 5 * 1024 * 1024;

    /// <summary>Lưu file upload vào wwwroot/image/cover/{bookId}-{guid}.ext — trả về ~/image/cover/...</summary>
    public static async Task<string> SaveUploadedCoverAsync(IWebHostEnvironment env, int bookId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Không có file ảnh.");

        if (file.Length > MaxCoverBytes)
            throw new ArgumentException("Ảnh bìa tối đa 5 MB.");

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !AllowedCoverExtensions.Contains(ext))
            throw new ArgumentException("Chỉ chấp nhận ảnh: jpg, jpeg, png, webp, gif.");

        ext = ext.ToLowerInvariant();
        var name = $"{bookId}-{Guid.NewGuid():N}{ext}";
        var dir = Path.Combine(env.WebRootPath, "image", "cover");
        Directory.CreateDirectory(dir);
        var fullPath = Path.Combine(dir, name);
        await using (var fs = new FileStream(fullPath, FileMode.Create))
            await file.CopyToAsync(fs);

        return "~/image/cover/" + name;
    }

    /// <summary>
    /// Ưu tiên ImageUrl trong DB; nếu trống thì tìm file ~/image/cover/{bookId}-*.jpg
    /// </summary>
    public static string ResolveCoverPath(IWebHostEnvironment env, int bookId, string? imageUrl)
    {
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            var t = imageUrl.Trim();
            if (t.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                t.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return t;
            if (t.StartsWith("~/", StringComparison.Ordinal))
                return t[1..]; // "/..."
            if (t.StartsWith('/'))
                return t;
            return "/" + t.TrimStart('/');
        }

        var dir = Path.Combine(env.WebRootPath, "image", "cover");
        if (Directory.Exists(dir))
        {
            var files = Directory.GetFiles(dir, $"{bookId}-*.jpg");
            if (files.Length > 0)
                return "/image/cover/" + Path.GetFileName(files[0]);
        }

        return "/images/header-slider/home-v1/header-slide.jpg";
    }
}
