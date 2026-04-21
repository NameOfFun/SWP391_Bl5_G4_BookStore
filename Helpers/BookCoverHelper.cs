namespace BookStore.Helpers;

public static class BookCoverHelper
{
    private const string DefaultCover = "/images/header-slider/home-v1/header-slide.jpg";

    public static string ResolveCoverPath(string? imageUrl)
    {
        var t = imageUrl?.Trim();
        if (!string.IsNullOrEmpty(t) &&
            (t.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
             t.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            return t;
        return DefaultCover;
    }
}
