using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Common
{
    public class BookTagDto
    {
        public int TagId { get; set; }

        [Required(ErrorMessage = "Tên tag không được để trống")]
        [MaxLength(99, ErrorMessage = "Tên tag không được vượt quá 99 ký tự")]
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
