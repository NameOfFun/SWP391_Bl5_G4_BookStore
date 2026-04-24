using BookStore.Dtos;
using BookStore.Helpers;
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
            DeliveredAt = order.DeliveredAt,

            // Người nhận
            RecipientName    = order.ShippingName    ?? "—",
            RecipientPhone   = order.ShippingPhone   ?? "—",
            RecipientAddress = order.ShippingAddress ?? "—",
            DeliveryNote     = order.DeliveryNote,
            ProofOfDeliveryImage = order.ProofOfDeliveryImage,

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
        if (string.Equals((order.PaymentMethod ?? "").Trim(), "COD", StringComparison.OrdinalIgnoreCase))
            order.PaymentStatus = OrderPaymentStatuses.Paid;

        await _db.SaveChangesAsync();
        return (true, $"Đơn #{orderId} đã giao thành công! 🎉");
    }

    public async Task<(bool ok, string message)> UpdateDeliveryStatusAsync(DeliveryStatusUpdateDto dto, string shipperId)
    {
        var order = await _db.Orders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId && o.ShipperId == shipperId);

        if (order is null)
            return (false, "Không tìm thấy đơn hàng.");

        if (order.Status != OrderStatus.Delivering)
            return (false, $"Đơn #{dto.OrderId} chưa ở trạng thái Đang giao.");

        if (dto.IsSuccess)
        {
            order.Status = OrderStatus.Delivered;
            order.DeliveredAt = DateTime.Now;
            order.FailedNote = dto.Note; // Dùng chung trường Note cho cả 2
            order.ProofOfDeliveryImage = dto.ProofImagePath;
            if (string.Equals((order.PaymentMethod ?? "").Trim(), "COD", StringComparison.OrdinalIgnoreCase))
                order.PaymentStatus = OrderPaymentStatuses.Paid;
        }
        else
        {
            // Hàng không giao được — hoàn tồn kho (đơn vẫn tồn tại để tra cứu).
            foreach (var d in order.Details)
            {
                await _db.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE Book SET Stock = COALESCE(Stock, 0) + {d.Quantity} WHERE BookId = {d.BookId}");
            }

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
                Status        = o.Status,
                ProofOfDeliveryImage = o.ProofOfDeliveryImage
            }).ToList()
        };
    }

    // ─── Performance Analytics ────────────────────────────────
    public async Task<ShipperPerformanceViewModel> GetManagementPerformanceAsync()
    {
        return await BuildPerformanceViewModelAsync(null);
    }

    public async Task<ShipperPerformanceViewModel> GetShipperPerformanceAsync(string shipperId)
    {
        return await BuildPerformanceViewModelAsync(shipperId);
    }

    private async Task<ShipperPerformanceViewModel> BuildPerformanceViewModelAsync(string? shipperId)
    {
        var now   = DateTime.Now;
        var today = now.Date;
        var startOfWeek  = today.AddDays(-(int)today.DayOfWeek);
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        // Lấy tất cả đơn hàng đã assign (Shipped, Delivering, Delivered, DeliveryFailed)
        var query = _db.Orders.AsQueryable();
        if (!string.IsNullOrEmpty(shipperId))
        {
            query = query.Where(o => o.ShipperId == shipperId);
        }
        else
        {
            // Nếu là Management (null), chỉ lấy những đơn đã có Shipper
            query = query.Where(o => o.ShipperId != null);
        }

        // Chỉ lấy các trạng thái liên quan đến việc giao hàng
        var deliveryStatuses = new[] { OrderStatus.Delivered, OrderStatus.DeliveryFailed, OrderStatus.Delivering, OrderStatus.Shipped };
        var allOrders = await query
            .Where(o => deliveryStatuses.Contains(o.Status))
            .Include(o => o.Shipper)
            .ToListAsync();

        var deliveredOrders = allOrders.Where(o => o.Status == OrderStatus.Delivered).ToList();
        var failedOrders    = allOrders.Where(o => o.Status == OrderStatus.DeliveryFailed).ToList();

        // 1. Tổng quan
        var vm = new ShipperPerformanceViewModel
        {
            SelectedShipperId = shipperId,
            TotalOrders      = allOrders.Count,
            SuccessfulOrders = deliveredOrders.Count,
            FailedOrders     = failedOrders.Count,
            TotalCodRevenue  = deliveredOrders
                .Where(o => o.PaymentMethod != null && o.PaymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase))
                .Sum(o => o.GrandTotal),
            
            OrdersToday      = allOrders.Count(o => o.OrderDate.Date == today),
            OrdersThisWeek   = allOrders.Count(o => o.OrderDate >= startOfWeek),
            OrdersThisMonth  = allOrders.Count(o => o.OrderDate >= startOfMonth)
        };

        // 2. Daily Stats (7 ngày gần nhất cho biểu đồ)
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => today.AddDays(-i))
            .OrderBy(d => d)
            .ToList();

        vm.DailyStats = last7Days.Select(d => new TimeStatsDto
        {
            Date    = d,
            Count   = allOrders.Count(o => o.OrderDate.Date == d),
            Revenue = allOrders
                .Where(o => o.OrderDate.Date == d && o.Status == OrderStatus.Delivered)
                .Sum(o => o.GrandTotal)
        }).ToList();

        // 3. Chart Data (Thành công vs Thất bại theo ngày)
        vm.ChartData.Labels = last7Days.Select(d => d.ToString("dd/MM")).ToList();
        vm.ChartData.SuccessCounts = last7Days.Select(d => 
            allOrders.Count(o => o.OrderDate.Date == d && o.Status == OrderStatus.Delivered)).ToList();
        vm.ChartData.FailedCounts = last7Days.Select(d => 
            allOrders.Count(o => o.OrderDate.Date == d && o.Status == OrderStatus.DeliveryFailed)).ToList();

        // 4. Shipper List & Selected Name
        // Lấy danh sách TOÀN BỘ đơn hàng đã assign để có đủ list shipper ngay cả khi đang filter một người
        var allDeliveryOrders = await _db.Orders
            .Where(o => o.ShipperId != null && deliveryStatuses.Contains(o.Status))
            .Include(o => o.Shipper)
            .ToListAsync();

        vm.ShipperList = allDeliveryOrders
            .GroupBy(o => o.ShipperId)
            .Select(g => new ShipperAnalyticDto
            {
                ShipperId      = g.Key!,
                ShipperName    = g.First().Shipper?.Name ?? "N/A",
                TotalAssigned  = g.Count(),
                TotalDelivered = g.Count(o => o.Status == OrderStatus.Delivered),
                TotalFailed    = g.Count(o => o.Status == OrderStatus.DeliveryFailed),
                CodCollected   = g.Where(o => o.Status == OrderStatus.Delivered && 
                                             o.PaymentMethod != null && 
                                             o.PaymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase))
                                  .Sum(o => o.GrandTotal)
            })
            .OrderByDescending(s => s.TotalDelivered)
            .ToList();

        // 5. Recent Orders (Chi tiết các đơn hàng)
        vm.RecentOrders = allOrders
            .OrderByDescending(o => o.OrderDate)
            .Take(10)
            .Select(o => new DeliveryHistoryItemDto
            {
                OrderId       = o.OrderId,
                RecipientName = o.ShippingName ?? "—",
                DeliveryDate  = o.DeliveredAt,
                GrandTotal    = o.GrandTotal,
                Status        = o.Status,
                ProofOfDeliveryImage = o.ProofOfDeliveryImage
            }).ToList();

        if (!string.IsNullOrEmpty(shipperId))
        {
            vm.SelectedShipperName = vm.ShipperList.FirstOrDefault(s => s.ShipperId == shipperId)?.ShipperName;
        }

        return vm;
    }
}
