using BookStore.Dtos.Common;

namespace BookStore.Service.Interfaces
{
    public interface IAboutService
    {
        Task<IReadOnlyList<AboutDto>> GetAllAboutAsync();
        Task<AboutDto> CreateAsync(AboutDto dto, string userId);
        Task<AboutDto?> GetByIdAsync(int id);
        Task<AboutDto> UpdateAsync(int id, AboutDto dto, string userId);
        Task<AboutDto> DeleteAsync(int id);
    }
}
