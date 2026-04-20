using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Common
{
    public class BookDto
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [MaxLength(100, ErrorMessage = "Tiêu đề tối đa 100 ký tự")]
        public string Title { get; set; } = null!;

        [MaxLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0, 999999999, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Range(0, 999999999, ErrorMessage = "Giá khuyến mãi phải lớn hơn hoặc bằng 0")]
        public decimal? PromotionalPrice { get; set; }

        public DateTime? PromotionalStartsAt { get; set; }

        public DateTime? PromotionalEndsAt { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho phải lớn hơn hoặc bằng 0")]
        public int Stock { get; set; }

        public int? CategoryId { get; set; }

        public int? AuthorId { get; set; }

        [MaxLength(255, ErrorMessage = "URL ảnh tối đa 255 ký tự")]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Tags
        public List<int> TagIds { get; set; } = new();

        // Read-only display helpers
        public string? CategoryName { get; set; }

        [MaxLength(100, ErrorMessage = "Tên tác giả tối đa 100 ký tự")]
        public string? AuthorName { get; set; }
        public List<string> TagNames { get; set; } = new();
        public DateTime? CreatedAt { get; set; }

        // Computed: true khi khuyến mãi có giá trị và nằm trong khoảng thời gian hiện tại
        public bool IsPromoActive =>
            PromotionalPrice.HasValue
            && (!PromotionalStartsAt.HasValue || PromotionalStartsAt.Value <= DateTime.Now)
            && (!PromotionalEndsAt.HasValue || PromotionalEndsAt.Value >= DateTime.Now);

        public decimal EffectivePrice => IsPromoActive ? PromotionalPrice!.Value : Price;
    }
}
