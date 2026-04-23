using System.ComponentModel.DataAnnotations;
using BookStore.Models;

namespace BookStore.Dtos.Common;

/// <summary>Thông tin giao hàng + thanh toán khách hàng nhập khi checkout.</summary>
public class CheckoutDto
{
    [Required(ErrorMessage = "Họ tên người nhận không được để trống")]
    [MaxLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự")]
    public string ShippingName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [MaxLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
    // Chấp nhận các định dạng số Việt Nam:
    //  - Bắt đầu bằng 0 theo sau 9–10 chữ số (tổng 10–11 số)
    //  - Hoặc bắt đầu bằng +84 / 84 theo sau 9–10 chữ số
    [RegularExpression(@"^(0\d{9,10}|(\+?84)\d{9,10})$", ErrorMessage = "Số điện thoại không hợp lệ (ví dụ: 0987654321 hoặc +84987654321)")]
    public string ShippingPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
    [MaxLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    public string? DeliveryNote { get; set; }

    /// <summary>"COD" hoặc "BankTransfer" (mở rộng sau).</summary>
    [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
    [MaxLength(50)]
    [RegularExpression(@"^(COD|BankTransfer)$", ErrorMessage = "Phương thức thanh toán không hợp lệ")]
    public string PaymentMethod { get; set; } = "COD";
}

public class CheckoutLineVm
{
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public string CoverUrl { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class CheckoutViewModel
{
    public CheckoutDto Form { get; set; } = new();
    public List<CheckoutLineVm> Lines { get; set; } = new();
    public decimal Subtotal => Lines.Sum(l => l.LineTotal);
    public decimal DiscountAmount => 0m; // chưa hỗ trợ voucher
    public decimal GrandTotal => Subtotal - DiscountAmount;
    public int TotalQuantity => Lines.Sum(l => l.Quantity);
}

/// <summary>Mục hiển thị trong danh sách quản lý đơn hàng.</summary>
public class OrderListItemDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public int ItemCount { get; set; }
    public decimal GrandTotal { get; set; }
    public OrderStatus Status { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
}

public class OrderDetailLineDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public class OrderDetailDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }

    public string? CustomerUserId { get; set; }
    public string CustomerEmail { get; set; } = "";
    public string ShippingName { get; set; } = "";
    public string ShippingPhone { get; set; } = "";
    public string ShippingAddress { get; set; } = "";
    public string? DeliveryNote { get; set; }

    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }

    public string? ShipperId { get; set; }
    public string? ShipperName { get; set; }

    public DateTime? DeliveredAt { get; set; }
    public string? FailedReason { get; set; }
    public string? FailedNote { get; set; }

    public List<OrderDetailLineDto> Items { get; set; } = new();
}

public class ShipperOptionDto
{
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "";
}
