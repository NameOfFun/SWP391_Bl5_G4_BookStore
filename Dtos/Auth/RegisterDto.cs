using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [MaxLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(256, ErrorMessage = "Email không được vượt quá 256 ký tự")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [MaxLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
    public string? PhoneNumber { get; set; }

    public string? ReturnUrl { get; set; }
}

public class RegisterResultDto
{
    public bool Succeeded { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public string? ReturnUrl { get; set; }
}
