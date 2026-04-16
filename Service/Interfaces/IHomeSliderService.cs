using BookStore.Dtos.Common;

namespace BookStore.Service.Interfaces
{
    public interface IHomeSliderService
    {
        Task<IReadOnlyList<HomeSliderDto>> GetAllAsync();
        Task<HomeSliderDto?> GetByIdAsync(int id);
        Task<HomeSliderDto> CreateAsync(HomeSliderDto dto, string userId);
        Task<HomeSliderDto> UpdateAsync(int id, HomeSliderDto dto, string userId);
        Task DeleteAsync(int id);
        Task<HomeSliderDto> ToggleStatusAsync(int id, string userId);
    }
}
