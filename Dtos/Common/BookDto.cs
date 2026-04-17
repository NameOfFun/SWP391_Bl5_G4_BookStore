using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Common
{
    public class BookDto
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [MaxLength(500, ErrorMessage = "Tiêu đề tối đa 500 ký tự")]
        public string Title { get; set; } = null!;

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

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Tags
        public List<int> TagIds { get; set; } = new();

        // Read-only display helpers
        public string? CategoryName { get; set; }
        public string? AuthorName { get; set; }
        public List<string> TagNames { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
    }
}
