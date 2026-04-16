using BookStore.Dtos;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements;

public class ShipperService : IShipperService
{
    private readonly BookStoreDbContext _db;

    public ShipperService(BookStoreDbContext db)
    {
        _db = db;
    }

    public async Task<ShipperDashboardViewModel> GetDashboardAsync(string shipperId)
    {
        // Lấy tất cả đơn hàng được gán cho shipper này
        var allOrders = await _db.Orders
            .Where(o => o.ShipperId == shipperId)
            .ToListAsync();

        // Đơn đã được assign (Shipped = đang trong tay shipper)
        var assigned = allOrders.Where(o => o.Status == OrderStatus.Shipped).ToList();

        // Đơn đang giao — cùng là Shipped (shipper nhận nhưng chưa hoàn thành)
        var delivering = assigned; // có thể mở rộng khi thêm trạng thái "Delivering" riêng

        // Đơn đã giao thành công
        var delivered = allOrders.Where(o => o.Status == OrderStatus.Delivered).ToList();

        // Đơn giao thất bại (Cancelled sau khi đã được assign shipper)
        var failed = allOrders.Where(o => o.Status == OrderStatus.Cancelled).ToList();

        // 3–5 đơn cần xử lý ngay: Shipped, sắp xếp theo ngày đặt hàng cũ nhất (ưu tiên cao nhất)
        var urgent = assigned
            .OrderBy(o => o.OrderDate)
            .Take(5)
            .Select(o => new UrgentOrderDto
            {
                OrderId    = o.OrderId,
                OrderDate  = o.OrderDate,
                ShippingName    = o.ShippingName    ?? "—",
                ShippingPhone   = o.ShippingPhone   ?? "—",
                ShippingAddress = o.ShippingAddress ?? "—",
                GrandTotal = o.GrandTotal,
                Status     = o.Status
            })
            .ToList();

        return new ShipperDashboardViewModel
        {
            AssignedCount  = assigned.Count,
            DeliveringCount = delivering.Count,
            DeliveredCount = delivered.Count,
            FailedCount    = failed.Count,
            UrgentOrders   = urgent
        };
    }
}
