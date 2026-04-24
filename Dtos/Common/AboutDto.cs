using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Common
{
    public class AboutDto
    {
        public int AboutId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [RegularExpression(@"^(?=.*[A-Za-z0-9\u00C0-\u024F\u1E00-\u1EFF])[A-Za-z0-9\u00C0-\u024F\u1E00-\u1EFF .,;:!?'""()\-–—…/]+$",
        ErrorMessage = "Tiêu đề chỉ được chứa chữ, số, khoảng trắng và dấu câu cơ bản; phải có ít nhất một chữ/số")]
        [MaxLength(500, ErrorMessage = "Tiêu đề tối đa 500 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [StringLength(200_000, MinimumLength = 1, ErrorMessage = "Nội dung tối đa 200.000 ký tự")]
        [Display(Name = "Nội dung")]
        public string ContentHtml { get; set; } = null!;

        public DateTime UpdatedAt { get; set; }
    }
}
