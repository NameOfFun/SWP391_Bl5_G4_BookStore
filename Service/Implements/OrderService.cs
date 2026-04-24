using BookStore.Dtos.Common;
using BookStore.Helpers;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookStore.Service.Implements;

public class OrderService : IOrderService
{
    private readonly BookStoreDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVoucherService _voucherService;
    private readonly ICartService _cartService;

    // Giới hạn độ dài lý do hủy đơn — khớp kích thước cột FailedReason hợp lý
    // và tránh việc payload quá lớn gây lỗi DbUpdateException.
    private const int MaxCancelReasonLength = 500;

    public OrderService(BookStoreDbContext db, UserManager<ApplicationUser> userManager,
        IVoucherService voucherService, ICartService cartService)
    {
        _db = db;
        _userManager = userManager;
        _voucherService = voucherService;
        _cartService = cartService;
    }

    // ─── Checkout view ────────────────────────────────────────
    public async Task<CheckoutViewModel> GetCheckoutAsync(string userId)
    {
        // Chuẩn hóa giỏ (cắt SL theo tồn, xóa dòng hết hàng) giống trang Giỏ hàng — tránh lệch checkout vs DB.
        await _cartService.GetCartAsync(userId);

        var vm = new CheckoutViewModel();

        var cart = await _db.Carts.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return vm;

        var items = await _db.CartItems
            .AsNoTracking()
            .Include(i => i.Book)
            .Where(i => i.CartId == cart.CartId)
            .ToListAsync();

        foreach (var item in items)
        {
            var book = item.Book;
            if (book == null || !book.IsActive) continue;
            var stock = book.Stock ?? 0;
            if (stock < 1) continue;

            var qty = Math.Min(item.Quantity, stock);
            vm.Lines.Add(new CheckoutLineVm
            {
                BookId = book.BookId,
                Title = book.Title ?? "Sách",
                CoverUrl = BookCoverHelper.ResolveCoverPath(book.ImageUrl),
                UnitPrice = PricingHelper.GetEffectiveUnitPrice(book),
                Quantity = qty
            });
        }

        // Đổ thông tin mặc định từ profile người dùng.
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            vm.Form.ShippingName = user.Name ?? user.UserName ?? "";
            vm.Form.ShippingPhone = user.PhoneNumber ?? "";
            vm.Form.ShippingAddress = user.Address ?? "";
        }

        return vm;
    }

    // ─── Place order ──────────────────────────────────────────
    public async Task<(bool Ok, string? Error, int OrderId)> PlaceOrderAsync(string userId, CheckoutDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        // Kiểm tra thêm ở tầng service — controller đã validate ModelState,
        // nhưng vẫn kiểm tra lại để phòng trường hợp service bị gọi ở chỗ khác.
        if (string.IsNullOrWhiteSpace(dto.ShippingName))
            return (false, "Họ tên người nhận không được để trống.", 0);
        if (string.IsNullOrWhiteSpace(dto.ShippingPhone))
            return (false, "Số điện thoại không được để trống.", 0);
        if (string.IsNullOrWhiteSpace(dto.ShippingAddress))
            return (false, "Địa chỉ giao hàng không được để trống.", 0);

        var method = (dto.PaymentMethod ?? "").Trim();
        if (method != "COD" && method != "BankTransfer")
            return (false, "Phương thức thanh toán không hợp lệ.", 0);

        await _cartService.GetCartAsync(userId);
        // Tránh entity Book/CartItem còn track từ bước trên gây SaveChanges lệch với DB (ví dụ sau khi trừ kho bằng SQL).
        _db.ChangeTracker.Clear();

        var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
            return (false, "Giỏ hàng trống — không thể đặt hàng.", 0);

        var items = await _db.CartItems
            .Include(i => i.Book)
            .Where(i => i.CartId == cart.CartId)
            .ToListAsync();

        if (items.Count == 0)
            return (false, "Giỏ hàng trống — không thể đặt hàng.", 0);

        // Lọc các dòng hợp lệ và kiểm tra tồn kho
        var validLines = new List<(CartItem Item, Book Book, decimal UnitPrice)>();
        foreach (var item in items)
        {
            if (item.Book == null || !item.Book.IsActive)
                return (false, $"Một số sách đã ngừng bán — vui lòng quay lại giỏ hàng và kiểm tra.", 0);

            var stock = item.Book.Stock ?? 0;
            if (stock < item.Quantity)
                return (false, $"Sách \"{item.Book.Title}\" chỉ còn {stock} cuốn, không đủ cho đơn hàng.", 0);

            validLines.Add((item, item.Book, PricingHelper.GetEffectiveUnitPrice(item.Book)));
        }

        if (validLines.Count == 0)
            return (false, "Không có sản phẩm hợp lệ để đặt hàng.", 0);

        var subTotal = validLines.Sum(v => v.UnitPrice * v.Item.Quantity);
        var discount = 0m;
        int? voucherId = null;

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            if (!string.IsNullOrWhiteSpace(dto.VoucherCode))
            {
                var normalizedCode = dto.VoucherCode.Trim().ToUpperInvariant();
                try
                {
                    var (vid, disc) = await _voucherService.ValidateForCheckoutAsync(normalizedCode, subTotal);
                    int rows = await _db.Database.ExecuteSqlRawAsync(
                        "UPDATE Voucher SET TimesUsed = TimesUsed + 1 " +
                        "WHERE VoucherId = {0} " +
                        "AND (UsageLimit IS NULL OR TimesUsed < UsageLimit)",
                        vid);

                    if (rows == 0)
                        return (false, "Mã giảm giá đã hết lượt sử dụng.", 0);

                    discount = disc;
                    voucherId = vid;
                }
                catch (ArgumentException ex)
                {
                    return (false, ex.Message, 0);
                }
            }

            var grandTotal = Math.Max(0m, subTotal - discount);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                VoucherId = voucherId,
                SubTotal = subTotal,
                DiscountAmount = discount,
                GrandTotal = grandTotal,
                ShippingName = dto.ShippingName.Trim(),
                ShippingPhone = dto.ShippingPhone.Trim(),
                ShippingAddress = dto.ShippingAddress.Trim(),
                DeliveryNote = string.IsNullOrWhiteSpace(dto.DeliveryNote) ? null : dto.DeliveryNote.Trim(),
                PaymentMethod = method,
                // COD: thu tiền khi giao; BankTransfer: chờ nhân viên xác nhận (MarkBankPaymentReceived).
                PaymentStatus = OrderPaymentStatuses.Pending
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var (cartItem, book, unitPrice) in validLines)
            {
                _db.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    BookId = book.BookId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = unitPrice
                });
            }

            await _db.SaveChangesAsync();

            // Trừ kho nguyên tử (tránh race khi hai đơn cùng mua — chỉ trừ nếu đủ tồn tại thời điểm UPDATE).
            foreach (var (cartItem, book, _) in validLines)
            {
                var n = await _db.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE Book SET Stock = COALESCE(Stock, 0) - {cartItem.Quantity}
                    WHERE BookId = {book.BookId} AND COALESCE(Stock, 0) >= {cartItem.Quantity}");
                if (n != 1)
                    return (false,
                        $"Sách \"{book.Title}\" không đủ tồn kho tại thời điểm đặt hàng (có thể vừa bán hết). Vui lòng làm mới giỏ và thử lại.",
                        0);
            }

            // Dọn giỏ hàng — gỡ Book khỏi tracker để EF không ghi đè Stock đã cập nhật bằng SQL phía trên.
            foreach (var line in items)
            {
                if (line.Book != null)
                    _db.Entry(line.Book).State = EntityState.Detached;
            }

            _db.CartItems.RemoveRange(items);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
            return (true, null, order.OrderId);
        }
        catch (DbUpdateException ex)
        {
            await TryRollbackAsync(tx);
            var detail = ex.GetBaseException().Message;
            return (false, $"Không thể tạo đơn hàng: {detail}", 0);
        }
        catch (Exception ex)
        {
            await TryRollbackAsync(tx);
            return (false, $"Không thể tạo đơn hàng: {ex.Message}", 0);
        }
    }

    private static async Task TryRollbackAsync(IDbContextTransaction? tx)
    {
        if (tx == null) return;
        try
        {
            await tx.RollbackAsync();
        }
        catch
        {
            // Giao dịch có thể đã hủy khi lỗi xảy ra giữa chừng.
        }
    }

    // ─── My orders ────────────────────────────────────────────
    public async Task<IReadOnlyList<OrderListItemDto>> GetMyOrdersAsync(string userId)
    {
        var orders = await _db.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderListItemDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CustomerName = o.ShippingName ?? "",
                CustomerPhone = o.ShippingPhone ?? "",
                ItemCount = o.Details.Sum(d => d.Quantity),
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.PaymentStatus
            })
            .ToListAsync();

        return orders;
    }

    // ─── Detail ───────────────────────────────────────────────
    public async Task<OrderDetailDto?> GetDetailAsync(int orderId, string? userIdForOwnership = null)
    {
        if (orderId <= 0) return null;

        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Details)
                .ThenInclude(d => d.Book)
            .Include(o => o.Shipper)
            .Include(o => o.Voucher)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null) return null;
        if (userIdForOwnership != null && order.UserId != userIdForOwnership) return null;

        var customerEmail = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == order.UserId)
            .Select(u => u.Email ?? u.UserName ?? "")
            .FirstOrDefaultAsync() ?? "";

        return new OrderDetailDto
        {
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            Status = order.Status,
            CustomerUserId = order.UserId,
            CustomerEmail = customerEmail,
            ShippingName = order.ShippingName ?? "",
            ShippingPhone = order.ShippingPhone ?? "",
            ShippingAddress = order.ShippingAddress ?? "",
            DeliveryNote = order.DeliveryNote,
            PaymentMethod = order.PaymentMethod,
            PaymentStatus = order.PaymentStatus,
            VoucherCode = order.Voucher?.Code,
            SubTotal = order.SubTotal,
            DiscountAmount = order.DiscountAmount,
            GrandTotal = order.GrandTotal,
            ShipperId = order.ShipperId,
            ShipperName = order.Shipper?.Name ?? order.Shipper?.UserName,
            DeliveredAt = order.DeliveredAt,
            FailedReason = order.FailedReason,
            FailedNote = order.FailedNote,
            Items = order.Details.Select(d => new OrderDetailLineDto
            {
                BookId = d.BookId,
                Title = d.Book?.Title ?? "—",
                ImageUrl = d.Book?.ImageUrl,
                UnitPrice = d.UnitPrice,
                Quantity = d.Quantity
            }).ToList()
        };
    }

    // ─── Management list ──────────────────────────────────────
    public async Task<(IReadOnlyList<OrderListItemDto> Items, int TotalCount)> GetManagementListAsync(
        string? search, OrderStatus? status, int page, int pageSize)
    {
        var q = _db.Orders.AsNoTracking().AsQueryable();

        if (status.HasValue)
            q = q.Where(o => o.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            if (int.TryParse(s, out var id))
                q = q.Where(o => o.OrderId == id
                              || (o.ShippingName != null && o.ShippingName.Contains(s))
                              || (o.ShippingPhone != null && o.ShippingPhone.Contains(s)));
            else
                q = q.Where(o => (o.ShippingName != null && o.ShippingName.Contains(s))
                              || (o.ShippingPhone != null && o.ShippingPhone.Contains(s)));
        }

        var totalCount = await q.CountAsync();

        var items = await q
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderListItemDto
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CustomerName = o.ShippingName ?? "",
                CustomerPhone = o.ShippingPhone ?? "",
                ItemCount = o.Details.Sum(d => d.Quantity),
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.PaymentStatus
            })
            .ToListAsync();

        return (items, totalCount);
    }

    // ─── State transitions ────────────────────────────────────
    public async Task<(bool Ok, string Message)> ConfirmAsync(int orderId)
    {
        if (orderId <= 0) return (false, "Mã đơn hàng không hợp lệ.");

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null) return (false, "Không tìm thấy đơn hàng.");
        if (order.Status != OrderStatus.Pending)
            return (false, $"Chỉ xác nhận được đơn đang chờ (hiện: {Translate(order.Status)}).");

        order.Status = OrderStatus.Confirmed;
        await _db.SaveChangesAsync();
        return (true, $"Đã xác nhận đơn #{orderId}.");
    }

    public async Task<(bool Ok, string Message)> MoveToProcessingAsync(int orderId)
    {
        if (orderId <= 0) return (false, "Mã đơn hàng không hợp lệ.");

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null) return (false, "Không tìm thấy đơn hàng.");
        if (order.Status != OrderStatus.Confirmed)
            return (false, $"Chỉ chuyển sang xử lý khi đơn đã xác nhận (hiện: {Translate(order.Status)}).");

        order.Status = OrderStatus.Processing;
        await _db.SaveChangesAsync();
        return (true, $"Đơn #{orderId} đã chuyển sang Đang xử lý.");
    }

    public async Task<(bool Ok, string Message)> AssignShipperAsync(int orderId, string shipperUserId)
    {
        if (orderId <= 0) return (false, "Mã đơn hàng không hợp lệ.");
        if (string.IsNullOrWhiteSpace(shipperUserId))
            return (false, "Vui lòng chọn shipper.");

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null) return (false, "Không tìm thấy đơn hàng.");
        if (order.Status != OrderStatus.Processing)
            return (false, $"Chỉ gán shipper khi đơn ở trạng thái Đang xử lý (hiện: {Translate(order.Status)}).");

        var shipper = await _userManager.FindByIdAsync(shipperUserId);
        if (shipper == null)
            return (false, "Không tìm thấy shipper được chọn.");
        if (!await _userManager.IsInRoleAsync(shipper, "Shipper"))
            return (false, "Người dùng được chọn không phải là shipper.");
        // Kiểm tra lại ở thời điểm gán để tránh dùng shipper đã bị vô hiệu hóa
        // sau khi dropdown đã render.
        if (!shipper.Status)
            return (false, "Shipper này đã bị vô hiệu hóa — vui lòng chọn shipper khác.");

        order.ShipperId = shipperUserId;
        order.Status = OrderStatus.Shipped;
        await _db.SaveChangesAsync();
        return (true, $"Đã giao đơn #{orderId} cho shipper {shipper.Name ?? shipper.UserName}.");
    }

    public async Task<(bool Ok, string Message)> CancelAsync(int orderId, string? reason)
    {
        if (orderId <= 0) return (false, "Mã đơn hàng không hợp lệ.");

        var order = await _db.Orders
            .Include(o => o.Details)
                .ThenInclude(d => d.Book)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null) return (false, "Không tìm thấy đơn hàng.");

        if (order.Status != OrderStatus.Pending
            && order.Status != OrderStatus.Confirmed
            && order.Status != OrderStatus.Processing)
        {
            return (false, $"Không thể hủy đơn ở trạng thái {Translate(order.Status)}.");
        }

        // Hoàn kho
        foreach (var detail in order.Details)
        {
            if (detail.Book != null)
                detail.Book.Stock = (detail.Book.Stock ?? 0) + detail.Quantity;
        }

        var cleanReason = (reason ?? string.Empty).Trim();
        if (cleanReason.Length > MaxCancelReasonLength)
            cleanReason = cleanReason[..MaxCancelReasonLength];

        // Hoàn lượt dùng voucher
        if (order.VoucherId.HasValue)
        {
            var voucher = await _db.Vouchers
                .FirstOrDefaultAsync(v => v.VoucherId == order.VoucherId.Value);
            if (voucher != null && voucher.TimesUsed > 0)
                voucher.TimesUsed--;
        }

        order.Status = OrderStatus.Cancelled;
        order.FailedReason = string.IsNullOrWhiteSpace(cleanReason) ? "Đơn bị hủy bởi quản trị." : cleanReason;
        await _db.SaveChangesAsync();
        return (true, $"Đã hủy đơn #{orderId} và hoàn kho.");
    }

    public async Task<(bool Ok, string Message)> MarkBankPaymentReceivedAsync(int orderId)
    {
        if (orderId <= 0) return (false, "Mã đơn hàng không hợp lệ.");

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null) return (false, "Không tìm thấy đơn hàng.");

        if (!string.Equals((order.PaymentMethod ?? "").Trim(), "BankTransfer", StringComparison.OrdinalIgnoreCase))
            return (false, "Chỉ có thể xác nhận thanh toán cho đơn chuyển khoản.");

        if (OrderPaymentStatuses.IsPaid(order.PaymentStatus))
            return (false, "Đơn đã được ghi nhận thanh toán.");

        if (!OrderPaymentStatuses.IsAwaitingPayment(order.PaymentStatus))
            return (false, "Trạng thái thanh toán hiện tại không cho phép xác nhận.");

        if (order.Status == OrderStatus.Cancelled)
            return (false, "Không thể cập nhật thanh toán cho đơn đã hủy.");

        order.PaymentStatus = OrderPaymentStatuses.Paid;
        await _db.SaveChangesAsync();
        return (true, $"Đã xác nhận khách đã chuyển khoản (đơn #{orderId}).");
    }

    public async Task<IReadOnlyList<ShipperOptionDto>> GetActiveShippersAsync()
    {
        var shippers = await _userManager.GetUsersInRoleAsync("Shipper");
        return shippers
            .Where(u => u.Status)
            .OrderBy(u => u.Name ?? u.UserName)
            .Select(u => new ShipperOptionDto
            {
                UserId = u.Id,
                Name = u.Name ?? u.UserName ?? u.Email ?? u.Id
            })
            .ToList();
    }

    // ─── Apply voucher (AJAX preview) ────────────────────────
    public async Task<(bool Ok, decimal DiscountAmount, string Message)> ApplyVoucherAsync(
        string userId, string code)
    {
        var cart = await _db.Carts.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
            return (false, 0, "Giỏ hàng trống.");

        var items = await _db.CartItems
            .AsNoTracking()
            .Include(i => i.Book)
            .Where(i => i.CartId == cart.CartId)
            .ToListAsync();

        var subTotal = 0m;
        foreach (var item in items)
        {
            var book = item.Book;
            if (book == null || !book.IsActive) continue;
            var stock = book.Stock ?? 0;
            if (stock < 1) continue;
            var qty = Math.Min(item.Quantity, stock);
            subTotal += PricingHelper.GetEffectiveUnitPrice(book) * qty;
        }

        if (subTotal <= 0)
            return (false, 0, "Giỏ hàng trống.");

        try
        {
            var (_, discountAmount) = await _voucherService.ValidateForCheckoutAsync(code, subTotal);
            return (true, discountAmount,
                $"Áp dụng mã {code} thành công! Giảm {discountAmount:N0}đ.");
        }
        catch (ArgumentException ex)
        {
            return (false, 0, ex.Message);
        }
    }

    private static string Translate(OrderStatus s) => s switch
    {
        OrderStatus.Pending => "Chờ xác nhận",
        OrderStatus.Confirmed => "Đã xác nhận",
        OrderStatus.Processing => "Đang xử lý",
        OrderStatus.Shipped => "Đã giao shipper",
        OrderStatus.Delivering => "Đang giao",
        OrderStatus.Delivered => "Đã giao",
        OrderStatus.Cancelled => "Đã hủy",
        OrderStatus.DeliveryFailed => "Giao thất bại",
        _ => s.ToString()
    };
}
