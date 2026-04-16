using BookStore.Dtos;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements;

public class ShipperService : IShipperService
{
    private readonly BookStoreDbContext _db;

    public ShipperService(BookStoreDbContext db) => _db = db;

    // ─── Dashboard ────────────────────────────────────────────
    public async Task<ShipperDashboardViewModel> GetDashboardAsync(string shipperId)
    {
        var allOrders = await _db.Orders
            .Where(o => o.ShipperId == shipperId)
            .ToListAsync();

        var assigned   = allOrders.Where(o => o.Status == OrderStatus.Shipped).ToList();
        var delivering = allOrders.Where(o => o.Status == OrderStatus.Delivering).ToList();
        var delivered  = allOrders.Where(o => o.Status == OrderStatus.Delivered).ToList();
        var failed     = allOrders.Where(o => o.Status == OrderStatus.Cancelled).ToList();

        // Urgent = Assigned + Delivering, sắp xếp theo ngày cũ nhất, lấy 5 đơn
        var urgent = allOrders
            .Where(o => o.Status == OrderStatus.Shipped || o.Status == OrderStatus.Delivering)
            .OrderBy(o => o.OrderDate)
            .Take(5)
            .Select(o => new UrgentOrderDto
            {
                OrderId         = o.OrderId,
                OrderDate       = o.OrderDate,
                ShippingName    = o.ShippingName    ?? "—",
                ShippingPhone   = o.ShippingPhone   ?? "—",
                ShippingAddress = o.ShippingAddress ?? "—",
                GrandTotal      = o.GrandTotal,
                Status          = o.Status
            }).ToList();

        return new ShipperDashboardViewModel
        {
            AssignedCount   = assigned.Count,
            DeliveringCount = delivering.Count,
            DeliveredCount  = delivered.Count,
            FailedCount     = failed.Count,
            UrgentOrders    = urgent
        };
    }

    // ─── Assigned Orders List ─────────────────────────────────
    public async Task<AssignedOrdersViewModel> GetAssignedOrdersAsync(string shipperId, string? filter)
    {
        var normalized = (filter ?? "all").ToLower().Trim();

        // Lấy tất cả đơn Shipped + Delivering
        var baseQuery = _db.Orders
            .Where(o => o.ShipperId == shipperId &&
                        (o.Status == OrderStatus.Shipped || o.Status == OrderStatus.Delivering))
            .OrderBy(o => o.OrderDate);   // cũ nhất lên trước (ưu tiên cao)

        var allOrders = await baseQuery.ToListAsync();

        var totalAssigned   = allOrders.Count(o => o.Status == OrderStatus.Shipped);
        var totalDelivering = allOrders.Count(o => o.Status == OrderStatus.Delivering);

        // Apply filter
        var filtered = normalized switch
        {
            "assigned"   => allOrders.Where(o => o.Status == OrderStatus.Shipped).ToList(),
            "delivering" => allOrders.Where(o => o.Status == OrderStatus.Delivering).ToList(),
            _            => allOrders
        };

        return new AssignedOrdersViewModel
        {
            ActiveFilter    = normalized,
            TotalAll        = allOrders.Count,
            TotalAssigned   = totalAssigned,
            TotalDelivering = totalDelivering,
            Orders = filtered.Select(o => new AssignedOrderItemDto
            {
                OrderId         = o.OrderId,
                OrderDate       = o.OrderDate,
                ShippingName    = o.ShippingName    ?? "—",
                ShippingPhone   = o.ShippingPhone   ?? "—",
                ShippingAddress = o.ShippingAddress ?? "—",
                GrandTotal      = o.GrandTotal,
                PaymentMethod   = o.PaymentMethod,
                PaymentStatus   = o.PaymentStatus,
                Status          = o.Status
            }).ToList()
        };
    }

    // ─── Accept: Shipped → Delivering ────────────────────────
    public async Task<(bool ok, string message)> AcceptOrderAsync(int orderId, string shipperId)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(
            o => o.OrderId == orderId && o.ShipperId == shipperId);

        if (order is null)
            return (false, "Không tìm thấy đơn hàng.");

        if (order.Status != OrderStatus.Shipped)
            return (false, $"Đơn #{orderId} không ở trạng thái Assigned (hiện: {order.Status}).");

        order.Status = OrderStatus.Delivering;
        await _db.SaveChangesAsync();
        return (true, $"Đã nhận đơn #{orderId} — bắt đầu giao!");
    }

    // ─── Reject: Shipped → Processing (trả lại để re-assign) ─
    public async Task<(bool ok, string message)> RejectOrderAsync(int orderId, string shipperId)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(
            o => o.OrderId == orderId && o.ShipperId == shipperId);

        if (order is null)
            return (false, "Không tìm thấy đơn hàng.");

        if (order.Status != OrderStatus.Shipped)
            return (false, $"Chỉ có thể từ chối đơn ở trạng thái Assigned.");

        // Trả về Processing để Staff/Admin có thể assign shipper khác
        order.Status   = OrderStatus.Processing;
        order.ShipperId = null;
        await _db.SaveChangesAsync();
        return (true, $"Đã từ chối đơn #{orderId} — đơn sẽ được phân bổ lại.");
    }
}
