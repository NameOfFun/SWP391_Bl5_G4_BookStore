namespace BookStore.Models;

/// <summary>Trạng thái đơn hàng — phục vụ xử lý đơn (Staff) và lịch sử mua (Customer).</summary>
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}
