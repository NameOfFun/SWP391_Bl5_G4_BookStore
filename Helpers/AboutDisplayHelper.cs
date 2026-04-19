using System.Net;
using System.Text.RegularExpressions;

namespace BookStore.Helpers;

public static class AboutDisplayHelper
{
    /// <summary>Rút gọn nội dung HTML thành đoạn thuần chữ cho danh sách tin.</summary>
    public static string PlainTextSummary(string? html, int maxLen = 280)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var text = Regex.Replace(html, "<.*?>", " ", RegexOptions.Singleline);
        text = WebUtility.HtmlDecode(text);
        text = Regex.Replace(text, @"\s+", " ").Trim();
        if (text.Length <= maxLen)
            return text;
        return text[..maxLen].TrimEnd() + "…";
    }

    public static string ListingImage(int aboutId)
    {
        var n = (Math.Abs(aboutId) % 4) + 1;
        return $"/images/news-event/news-listing-{n:D2}.jpg";
    }

    public static string DetailHeroImage(int aboutId) => "/images/news-event/new-detail-img.jpg";
}
