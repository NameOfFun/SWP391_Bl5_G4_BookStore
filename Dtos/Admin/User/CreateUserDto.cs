using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Dtos.Admin.User;

public class CreateUserDto
{
    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [MaxLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(256, ErrorMessage = "Email tối đa 256 ký tự")]
    public string Email { get; set; } = null!;

    [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ (10 chữ số, bắt đầu bằng 0)")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    [MaxLength(100, ErrorMessage = "Mật khẩu tối đa 100 ký tự")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn vai trò")]
    public string SelectedRole { get; set; } = null!;

    [ValidateNever]
    public List<SelectListItem> AvailableRoles { get; set; } = new();
}
