using BookStore.Dtos.Common;

namespace BookStore.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CategoryDto dto, string userId);
        Task<CategoryDto> UpdateAsync(int id, CategoryDto dto, string userId);
        Task<CategoryDto> ToggleStatusAsync(int id, string userId);
    }
}
