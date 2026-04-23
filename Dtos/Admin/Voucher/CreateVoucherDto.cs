using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos.Admin.Voucher;

public class CreateVoucherDto
{
    [Required(ErrorMessage = "Mã voucher không được để trống")]
    [RegularExpression(@"^[A-Z0-9]{4,50}$", ErrorMessage = "Mã voucher chỉ gồm chữ hoa và số, dài 4–50 ký tự")]
    public string Code { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Loại giảm giá không được để trống")]
    public string DiscountType { get; set; } = "P";

    [Required(ErrorMessage = "Giá trị giảm không được để trống")]
    [Range(0.01, 1_000_000_000, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
    public decimal DiscountValue { get; set; }

    [Range(0, 1_000_000_000, ErrorMessage = "Đơn tối thiểu không hợp lệ")]
    public decimal? MinOrderAmount { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    [Range(1, 1_000_000, ErrorMessage = "Giới hạn lượt dùng phải ít nhất là 1")]
    public int? UsageLimit { get; set; }
}
