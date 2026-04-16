using BookStore.Dtos;

namespace BookStore.Service.Interfaces;

public interface IShipperService
{
    /// <summary>Lấy dữ liệu tổng quan dashboard cho shipper hiện tại.</summary>
    Task<ShipperDashboardViewModel> GetDashboardAsync(string shipperId);

    /// <summary>Lấy danh sách đơn Assigned + Delivering, có thể filter theo status.</summary>
    /// <param name="filter">null/"all" | "assigned" | "delivering"</param>
    Task<AssignedOrdersViewModel> GetAssignedOrdersAsync(string shipperId, string? filter);

    /// <summary>Shipper chấp nhận đơn: Shipped → Delivering.</summary>
    Task<(bool ok, string message)> AcceptOrderAsync(int orderId, string shipperId);

    /// <summary>Shipper từ chối đơn: Shipped → Processing (trả lại để re-assign).</summary>
    Task<(bool ok, string message)> RejectOrderAsync(int orderId, string shipperId);
}
