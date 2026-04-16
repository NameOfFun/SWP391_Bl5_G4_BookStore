using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Common
{
    public class HomeSliderDto
    {
        public int HomeSliderId { get; set; }

        [Required(ErrorMessage = "URL hình ảnh không được để trống")]
        [MaxLength(500, ErrorMessage = "URL hình ảnh không được vượt quá 500 ký tự")]
        public string ImageUrl { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string? Caption { get; set; }

        [MaxLength(500, ErrorMessage = "Đường dẫn không được vượt quá 500 ký tự")]
        public string? LinkUrl { get; set; }

        [Range(0, 9999, ErrorMessage = "Thứ tự hiển thị phải từ 0 đến 9999")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
