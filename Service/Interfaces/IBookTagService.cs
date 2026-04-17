using BookStore.Dtos.Common;

namespace BookStore.Service.Interfaces
{
    public interface IBookTagService
    {
        Task<IReadOnlyList<BookTagDto>> GetAllAsync();
        Task<BookTagDto?> GetByIdAsync(int id);
        Task<BookTagDto> CreateAsync(BookTagDto dto, string userId);
        Task<BookTagDto> UpdateAsync(int id, BookTagDto dto, string userId);
        Task<BookTagDto> ToggleStatusAsync(int id, string userId);
    }
}
