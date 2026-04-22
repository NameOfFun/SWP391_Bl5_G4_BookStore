using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Auth;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;
}

public class ForgotPasswordResultDto
{
    public bool Succeeded { get; set; }
    public string? ErrorMessage { get; set; }
}