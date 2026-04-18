using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Dtos.Admin.User;

public class EditUserDto
{
    [Required]
    public string Id { get; set; } = null!;

    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [MaxLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(256, ErrorMessage = "Email tối đa 256 ký tự")]
    public string Email { get; set; } = null!;

    [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public List<string> SelectedRoles { get; set; } = new();

    [ValidateNever]
    public List<SelectListItem> AvailableRoles { get; set; } = new();
}
