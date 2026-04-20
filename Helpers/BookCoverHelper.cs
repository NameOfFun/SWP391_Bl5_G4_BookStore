namespace BookStore.Helpers;

public static class BookCoverHelper
{
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
