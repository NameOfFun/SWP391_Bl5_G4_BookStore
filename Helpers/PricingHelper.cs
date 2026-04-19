using BookStore.Models;

namespace BookStore.Helpers;

public static class PricingHelper
{
    public static decimal GetEffectiveUnitPrice(Book book)
    {
        var now = DateTime.UtcNow;
        if (book.PromotionalPrice.HasValue
            && book.PromotionalStartsAt <= now
            && (!book.PromotionalEndsAt.HasValue || book.PromotionalEndsAt >= now))
            return book.PromotionalPrice.Value;
        return book.Price ?? 0;
    }

    /// <summary>Giá niêm yết gạch ngang khi có trường khuyến mãi (cùng logic trang chi tiết).</summary>
    public static decimal? GetDisplayListPrice(Book book) =>
        book.PromotionalPrice.HasValue ? book.Price : null;
}
