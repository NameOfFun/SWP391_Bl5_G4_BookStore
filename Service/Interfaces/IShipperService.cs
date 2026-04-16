using BookStore.Dtos;

namespace BookStore.Service.Interfaces;

public interface IShipperService
{
    /// <summary>Lấy dữ liệu tổng quan dashboard cho shipper hiện tại.</summary>
    Task<ShipperDashboardViewModel> GetDashboardAsync(string shipperId);
}
