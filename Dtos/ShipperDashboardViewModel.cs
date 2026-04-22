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

    /// <summary>Số đơn giao thất bại (DeliveryFailed).</summary>
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

// ─────────────────────────────────────────────
//  Delivery History ViewModel
// ─────────────────────────────────────────────
public class DeliveryHistoryViewModel
{
    public string ActiveFilter { get; set; } = "all";
    public List<DeliveryHistoryItemDto> Orders { get; set; } = [];

    public int TotalAll { get; set; }
    public int TotalSuccess { get; set; }
    public int TotalFailed { get; set; }
}

public class DeliveryHistoryItemDto
{
    public int OrderId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public DateTime? DeliveryDate { get; set; } // Using DeliveredAt
    public decimal GrandTotal { get; set; }
    public OrderStatus Status { get; set; }
}

// ─────────────────────────────────────────────
//  Shipper Performance & Analytics (Manager View)
// ─────────────────────────────────────────────
public class ShipperPerformanceViewModel
{
    // Tổng quan (Cards)
    public int TotalOrders { get; set; }
    public int SuccessfulOrders { get; set; }
    public int FailedOrders { get; set; }
    public decimal SuccessRate => TotalOrders > 0 ? (decimal)SuccessfulOrders * 100 / TotalOrders : 0;
    
    /// <summary>Tổng tiền COD shipper đang cầm (chưa nộp).</summary>
    public decimal TotalCodRevenue { get; set; }

    // Thống kê theo thời gian (Cho biểu đồ đường/cột)
    public List<TimeStatsDto> DailyStats { get; set; } = [];
    public int OrdersToday { get; set; }
    public int OrdersThisWeek { get; set; }
    public int OrdersThisMonth { get; set; }

    // Dữ liệu cho biểu đồ Chart.js
    public PerformanceChartDataDto ChartData { get; set; } = new();

    public string? SelectedShipperId { get; set; }
    public string? SelectedShipperName { get; set; }

    // Danh sách chi tiết các Shipper (Dành cho Manager)
    public List<ShipperAnalyticDto> ShipperList { get; set; } = [];

    // Danh sách đơn hàng gần đây của shipper được chọn (hoặc tất cả)
    public List<DeliveryHistoryItemDto> RecentOrders { get; set; } = [];
}

public class TimeStatsDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public decimal Revenue { get; set; }
}

public class ShipperAnalyticDto
{
    public string ShipperId { get; set; } = string.Empty;
    public string ShipperName { get; set; } = string.Empty;
    public int TotalAssigned { get; set; }
    public int TotalDelivered { get; set; }
    public int TotalFailed { get; set; }
    public decimal SuccessRate => TotalAssigned > 0 ? (decimal)TotalDelivered * 100 / TotalAssigned : 0;
    public decimal CodCollected { get; set; }
}

public class PerformanceChartDataDto
{
    public List<string> Labels { get; set; } = [];
    public List<int> SuccessCounts { get; set; } = [];
    public List<int> FailedCounts { get; set; } = [];
}
