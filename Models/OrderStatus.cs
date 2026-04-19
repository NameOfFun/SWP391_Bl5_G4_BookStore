namespace BookStore.Models;

/// <summary>Trạng thái đơn hàng — phục vụ xử lý đơn (Staff) và lịch sử mua (Customer).</summary>
public enum OrderStatus
{
    Pending    = 0,
    Confirmed  = 1,
    Processing = 2,
    /// <summary>Đơn đã assign cho Shipper, chờ shipper nhận.</summary>
    Shipped    = 3,
    Delivered  = 4,
    Cancelled  = 5,
    /// <summary>Shipper đã nhận đơn, đang trên đường giao.</summary>
    Delivering = 6,
    /// <summary>Giao hàng thất bại.</summary>
    DeliveryFailed = 7
}
