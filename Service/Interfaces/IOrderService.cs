using BookStore.Dtos.Common;
using BookStore.Models;

namespace BookStore.Service.Interfaces;

public interface IOrderService
{
    /// <summary>Lấy dữ liệu hiển thị trang Checkout (các dòng trong giỏ + thông tin mặc định của user).</summary>
    Task<CheckoutViewModel> GetCheckoutAsync(string userId);

    /// <summary>Đặt đơn từ giỏ hàng: snapshot giá, trừ kho, xóa giỏ, trả về OrderId.</summary>
    Task<(bool Ok, string? Error, int OrderId)> PlaceOrderAsync(string userId, CheckoutDto dto);

    /// <summary>Lịch sử đơn của chính customer (sort mới nhất trước).</summary>
    Task<IReadOnlyList<OrderListItemDto>> GetMyOrdersAsync(string userId);

    /// <summary>Chi tiết đơn (dành cho cả customer lẫn staff). Nếu truyền userIdForOwnership thì chỉ trả khi đúng chủ đơn.</summary>
    Task<OrderDetailDto?> GetDetailAsync(int orderId, string? userIdForOwnership = null);

    /// <summary>Danh sách có phân trang/lọc cho trang quản lý (Staff/Manager/Admin).</summary>
    Task<(IReadOnlyList<OrderListItemDto> Items, int TotalCount)> GetManagementListAsync(
        string? search, OrderStatus? status, int page, int pageSize);

    /// <summary>Pending → Confirmed.</summary>
    Task<(bool Ok, string Message)> ConfirmAsync(int orderId);

    /// <summary>Confirmed → Processing.</summary>
    Task<(bool Ok, string Message)> MoveToProcessingAsync(int orderId);

    /// <summary>Processing → Shipped (gán shipper).</summary>
    Task<(bool Ok, string Message)> AssignShipperAsync(int orderId, string shipperUserId);

    /// <summary>Hủy đơn (chỉ từ Pending/Confirmed/Processing). Tự hoàn kho.</summary>
    Task<(bool Ok, string Message)> CancelAsync(int orderId, string? reason);

    /// <summary>Danh sách shipper đang hoạt động để gán đơn.</summary>
    Task<IReadOnlyList<ShipperOptionDto>> GetActiveShippersAsync();
}
