using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Admin.Role;

public class CreateRoleDto
{
    [Required(ErrorMessage = "Tên vai trò không được để trống")]
    [MaxLength(100, ErrorMessage = "Tên vai trò tối đa 100 ký tự")]
    public string Name { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
    public string? Description { get; set; }
}
