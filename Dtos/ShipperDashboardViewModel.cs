using BookStore.Models;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Dtos;

public class ShipperDashboardViewModel
{
    /// <summary>Số đơn đã được assign cho shipper này (Shipped).</summary>
    public int AssignedCount { get; set; }

    /// <summary>Số đơn đang giao (Shipped — shipper đã nhận, chưa xong).</summary>
    public int DeliveringCount { get; set; }

    /// <summary>Số đơn đã giao thành công (Delivered).</summary>
    public int DeliveredCount { get; set; }

    /// <summary>Số đơn giao thất bại (Cancelled sau khi Shipped).</summary>
    public int FailedCount { get; set; }

    /// <summary>3–5 đơn cần xử lý ngay (Shipped, xếp theo ngày đặt cũ nhất).</summary>
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

    /// <summary>Số giờ kể từ ngày đặt hàng (để tính mức độ khẩn cấp).</summary>
    public double HoursSinceOrder => (DateTime.Now - OrderDate).TotalHours;
}
