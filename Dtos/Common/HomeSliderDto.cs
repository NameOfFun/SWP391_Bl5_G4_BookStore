using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Common
{
    public class HomeSliderDto
    {
        public int HomeSliderId { get; set; }

        [Required(ErrorMessage = "URL hình ảnh không được để trống")]
        [MaxLength(255, ErrorMessage = "URL hình ảnh không được vượt quá 255 ký tự")]
        public string ImageUrl { get; set; } = null!;

        [MaxLength(255, ErrorMessage = "Nội dung chữ trên slider không được vượt quá 255 ký tự")]
        public string? Caption { get; set; }

        [MaxLength(255, ErrorMessage = "Đường dẫn không được vượt quá 255 ký tự")]
        public string? LinkUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
