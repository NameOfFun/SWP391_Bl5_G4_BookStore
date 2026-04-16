using BookStore.Dtos.Common;

namespace BookStore.Service.Interfaces
{
    public interface IBookService
    {
        Task<IReadOnlyList<BookDto>> GetAllAsync();
        Task<BookDto?> GetByIdAsync(int id);
        Task<BookDto> CreateAsync(BookDto dto, string userId);
        Task<BookDto> UpdateAsync(int id, BookDto dto, string userId);
        Task<BookDto> ChangeStatusAsync(int id, string userId);
    }
}
