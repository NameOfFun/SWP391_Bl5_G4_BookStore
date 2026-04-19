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
        var failed     = allOrders.Where(o => o.Status == OrderStatus.DeliveryFailed).ToList();

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

    // ─── Delivery Detail ──────────────────────────────────────
    public async Task<DeliveryDetailViewModel?> GetDeliveryDetailAsync(int orderId, string shipperId)
    {
        var order = await _db.Orders
            .Include(o => o.Details)
                .ThenInclude(d => d.Book)
                    .ThenInclude(b => b.Author)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.ShipperId == shipperId);

        if (order is null) return null;

        return new DeliveryDetailViewModel
        {
            // Thông tin đơn
            OrderId   = order.OrderId,
            OrderDate = order.OrderDate,
            Status    = order.Status,

            // Người nhận
            RecipientName    = order.ShippingName    ?? "—",
            RecipientPhone   = order.ShippingPhone   ?? "—",
            RecipientAddress = order.ShippingAddress ?? "—",
            DeliveryNote     = order.DeliveryNote,

            // Sản phẩm
            Items = order.Details.Select(d => new DeliveryItemDto
            {
                BookTitle  = d.Book?.Title      ?? "—",
                ImageUrl   = d.Book?.ImageUrl,
                AuthorName = d.Book?.Author?.Name,
                Quantity   = d.Quantity,
                UnitPrice  = d.UnitPrice
            }).ToList(),

            // Thanh toán
            PaymentMethod  = order.PaymentMethod,
            PaymentStatus  = order.PaymentStatus,
            SubTotal       = order.SubTotal,
            DiscountAmount = order.DiscountAmount,
            GrandTotal     = order.GrandTotal,
            VoucherId      = order.VoucherId
        };
    }

    // ─── Mark Delivered: Delivering → Delivered ───────────────
    public async Task<(bool ok, string message)> MarkDeliveredAsync(int orderId, string shipperId)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(
            o => o.OrderId == orderId && o.ShipperId == shipperId);

        if (order is null)
            return (false, "Không tìm thấy đơn hàng.");

        if (order.Status != OrderStatus.Delivering)
            return (false, $"Đơn #{orderId} chưa ở trạng thái Đang giao.");

        order.Status = OrderStatus.Delivered;
        order.DeliveredAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return (true, $"Đơn #{orderId} đã giao thành công! 🎉");
    }

    public async Task<(bool ok, string message)> UpdateDeliveryStatusAsync(DeliveryStatusUpdateDto dto, string shipperId)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(
            o => o.OrderId == dto.OrderId && o.ShipperId == shipperId);

        if (order is null)
            return (false, "Không tìm thấy đơn hàng.");

        if (order.Status != OrderStatus.Delivering)
            return (false, $"Đơn #{dto.OrderId} chưa ở trạng thái Đang giao.");

        if (dto.IsSuccess)
        {
            order.Status = OrderStatus.Delivered;
            order.DeliveredAt = DateTime.Now;
            order.FailedNote = dto.Note; // Dùng chung trường Note cho cả 2
        }
        else
        {
            order.Status = OrderStatus.DeliveryFailed;
            order.FailedReason = dto.FailedReason;
            order.FailedNote = dto.Note;
        }

        await _db.SaveChangesAsync();

        var statusMsg = dto.IsSuccess ? "giao thành công" : "giao thất bại";
        return (true, $"Đã cập nhật đơn #{dto.OrderId} là {statusMsg}.");
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

    public async Task<DeliveryHistoryViewModel> GetDeliveryHistoryAsync(string shipperId, string? filter)
    {
        var normalized = (filter ?? "all").ToLower().Trim();

        // Lấy tất cả đơn Delivered + DeliveryFailed của shipper này
        var baseQuery = _db.Orders
            .Where(o => o.ShipperId == shipperId &&
                        (o.Status == OrderStatus.Delivered || o.Status == OrderStatus.DeliveryFailed));

        var allOrders = await baseQuery
            .OrderByDescending(o => o.DeliveredAt ?? o.OrderDate)
            .ToListAsync();

        var totalSuccess = allOrders.Count(o => o.Status == OrderStatus.Delivered);
        var totalFailed  = allOrders.Count(o => o.Status == OrderStatus.DeliveryFailed);

        // Apply filter
        var filtered = normalized switch
        {
            "success" => allOrders.Where(o => o.Status == OrderStatus.Delivered).ToList(),
            "failed"  => allOrders.Where(o => o.Status == OrderStatus.DeliveryFailed).ToList(),
            _         => allOrders
        };

        return new DeliveryHistoryViewModel
        {
            ActiveFilter = normalized,
            TotalAll     = allOrders.Count,
            TotalSuccess = totalSuccess,
            TotalFailed  = totalFailed,
            Orders = filtered.Select(o => new DeliveryHistoryItemDto
            {
                OrderId       = o.OrderId,
                RecipientName = o.ShippingName ?? "—",
                DeliveryDate  = o.DeliveredAt,
                GrandTotal    = o.GrandTotal,
                Status        = o.Status
            }).ToList()
        };
    }
}
