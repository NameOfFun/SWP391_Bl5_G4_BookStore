using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Common
{
    public class BookDto
    {
       public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionalPrice { get; set; }
        public DateTime? PromotionalStartsAt { get; set; }
        public DateTime? PromotionalEndsAt { get; set; }
        public int Stock { get; set; }
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public List<int> TagIds { get; set; } = new();

        public string? CategoryName { get; set; }
        public string? AuthorName { get; set; }
        public List<string> TagNames { get; set; } = new();
        public DateTime? CreatedAt { get; set; }

        public bool IsPromoActive =>
            PromotionalPrice.HasValue
            && (!PromotionalStartsAt.HasValue || PromotionalStartsAt.Value <= DateTime.Now)
            && (!PromotionalEndsAt.HasValue || PromotionalEndsAt.Value >= DateTime.Now);

        public decimal EffectivePrice => IsPromoActive ? PromotionalPrice!.Value : Price;
    }
}
