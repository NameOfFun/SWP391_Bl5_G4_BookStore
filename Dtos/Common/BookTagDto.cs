using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Common
{
    public class BookTagDto
    {
        public int TagId { get; set; }

        [Required(ErrorMessage = "Tên tag không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên tag không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
