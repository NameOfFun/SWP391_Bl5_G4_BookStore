using BookStore.Models;

namespace BookStore.Dtos;

// ─────────────────────────────────────────────
//  Dashboard ViewModel (giữ nguyên)
// ─────────────────────────────────────────────
public class ShipperDashboardViewModel
{
    /// <summary>Số đơn đã được assign (Shipped) — chờ shipper nhận.</summary>
    public int AssignedCount { get; set; }

    /// <summary>Số đơn đang giao (Delivering — shipper đã nhận).</summary>
    public int DeliveringCount { get; set; }

    /// <summary>Số đơn đã giao thành công (Delivered).</summary>
    public int DeliveredCount { get; set; }

    /// <summary>Số đơn giao thất bại (Cancelled sau khi được assign).</summary>
    public int FailedCount { get; set; }

    /// <summary>3–5 đơn cần xử lý ngay, xếp theo ngày cũ nhất.</summary>
    public List<UrgentOrderDto> UrgentOrders { get; set; } = [];
}

public class UrgentOrderDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShippingName { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public OrderStatus Status { get; set; }

    public double HoursSinceOrder => (DateTime.Now - OrderDate).TotalHours;
}

// ─────────────────────────────────────────────
//  Assigned Orders ViewModel  (màn hình mới)
// ─────────────────────────────────────────────
public class AssignedOrdersViewModel
{
    public string ActiveFilter { get; set; } = "all";
    public List<AssignedOrderItemDto> Orders { get; set; } = [];

    // Totals cho filter badge
    public int TotalAll { get; set; }
    public int TotalAssigned { get; set; }
    public int TotalDelivering { get; set; }
}

public class AssignedOrderItemDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }

    // Thông tin khách
    public string ShippingName { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;

    // Tiền
    public decimal GrandTotal { get; set; }

    // Thanh toán
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }

    /// <summary>true = đã thanh toán qua online (VNPay…); false = COD chưa thu</summary>
    public bool IsPrepaid => PaymentStatus?.ToLower() == "paid";

    // Trạng thái
    public OrderStatus Status { get; set; }

    public double HoursSinceOrder => (DateTime.Now - OrderDate).TotalHours;
}

// ─────────────────────────────────────────────
//  Delivery Detail ViewModel
// ─────────────────────────────────────────────
public class DeliveryDetailViewModel
{
    // ── Thông tin đơn ──────────────────────────
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }

    // ── Người nhận ─────────────────────────────
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string RecipientAddress { get; set; } = string.Empty;
    public string? DeliveryNote { get; set; }

    // ── Sản phẩm ───────────────────────────────
    public List<DeliveryItemDto> Items { get; set; } = [];

    // ── Thanh toán ─────────────────────────────
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public int? VoucherId { get; set; }

    public bool IsPrepaid => PaymentStatus?.ToLower() == "paid";
    public bool IsCod => string.IsNullOrWhiteSpace(PaymentMethod) ||
                         PaymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase);

    // ── Navigation ─────────────────────────────
    /// <summary>Cho phép Accept/Reject nếu đang Shipped</summary>
    public bool CanAccept  => Status == OrderStatus.Shipped;
    /// <summary>Cho phép đánh dấu Delivered nếu đang Delivering</summary>
    public bool CanDeliver => Status == OrderStatus.Delivering;
}

public class DeliveryItemDto
{
    public string BookTitle { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? AuthorName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}
