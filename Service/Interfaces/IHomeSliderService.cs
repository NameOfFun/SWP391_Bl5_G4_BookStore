using BookStore.Dtos.Common;

namespace BookStore.Service.Interfaces
{
    public interface IHomeSliderService
    {
        /// <summary>Slider đang bật, sắp xếp theo DisplayOrder (trang chủ công khai).</summary>
        Task<IReadOnlyList<HomeSliderDto>> GetActiveForHomeAsync();

        Task<IReadOnlyList<HomeSliderDto>> GetAllAsync();
        Task<HomeSliderDto?> GetByIdAsync(int id);
        Task<HomeSliderDto> CreateAsync(HomeSliderDto dto, string userId);
        Task<HomeSliderDto> UpdateAsync(int id, HomeSliderDto dto, string userId);
        Task DeleteAsync(int id);
        Task<HomeSliderDto> ToggleStatusAsync(int id, string userId);

        /// <summary>Dịch chuyển slider đến vị trí mới theo logic shift (không swap).</summary>
        Task<HomeSliderDto> ReorderAsync(int id, int newOrder, string userId);
    }
}
