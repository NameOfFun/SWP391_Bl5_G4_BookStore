namespace BookStore.Dtos.Admin.Voucher;

public class VoucherDetailsDto
{
    public int VoucherId { get; set; }
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = "P";
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int TimesUsed { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? LastManagedByName { get; set; }

    public string DisplayDiscount => DiscountType == "P"
        ? $"{DiscountValue:0.##}%"
        : $"{DiscountValue:N0}đ";

    public string StatusBadge
    {
        get
        {
            if (!IsActive) return "inactive";
            if (ValidTo.HasValue && ValidTo.Value.Date < DateTime.Today) return "expired";
            return "active";
        }
    }
}
