using BookStore.Dtos.Common;
using BookStore.Models;

namespace BookStore.Service.Interfaces;

public interface IOrderService
{
    Task<CheckoutViewModel> GetCheckoutAsync(string userId);

    Task<(bool Ok, string? Error, int OrderId)> PlaceOrderAsync(string userId, CheckoutDto dto);

    Task<IReadOnlyList<OrderListItemDto>> GetMyOrdersAsync(string userId);

    Task<OrderDetailDto?> GetDetailAsync(int orderId, string? userIdForOwnership = null);

    Task<(IReadOnlyList<OrderListItemDto> Items, int TotalCount)> GetManagementListAsync(
        string? search, OrderStatus? status, int page, int pageSize);

    /// <summary>Pending → Confirmed.</summary>
    Task<(bool Ok, string Message)> ConfirmAsync(int orderId);

    /// <summary>Confirmed → Processing.</summary>
    Task<(bool Ok, string Message)> MoveToProcessingAsync(int orderId);

    /// <summary>Processing → Delivered. Tự động đánh dấu đã thu tiền (COD).</summary>
    Task<(bool Ok, string Message)> MarkDeliveredAsync(int orderId);

    /// <summary>Hủy đơn (chỉ từ Pending/Confirmed/Processing). Tự hoàn kho và khôi phục giỏ hàng.</summary>
    Task<(bool Ok, string Message)> CancelAsync(int orderId, string? reason);

    Task<(bool Ok, decimal DiscountAmount, string Message)> ApplyVoucherAsync(string userId, string code);
}
