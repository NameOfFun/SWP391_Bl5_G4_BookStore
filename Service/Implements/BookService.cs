using BookStore.Dtos.Common;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements
{
    public class BookService : IBookService
    {
        private readonly BookStoreDbContext _context;

        public BookService(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<BookDto>> GetAllAsync()
        {
            return await _context.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.Author)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => ToDto(b))
                .ToListAsync();
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.BookId == id);

            return entity == null ? null : ToDto(entity);
        }

        public async Task<BookDto> CreateAsync(BookDto dto, string userId)
        {
            var title = (dto.Title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Tiêu đề không được để trống");

            var authorId = await ResolveAuthorIdAsync(dto.AuthorName, userId);

            var entity = new Book
            {
                Title = title,
                Description = dto.Description,
                Price = dto.Price,
                PromotionalPrice = dto.PromotionalPrice,
                PromotionalStartsAt = dto.PromotionalStartsAt,
                PromotionalEndsAt = dto.PromotionalEndsAt,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                AuthorId = authorId,
                ImageUrl = dto.ImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedByUserId = userId
            };

            _context.Books.Add(entity);
            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<BookDto> UpdateAsync(int id, BookDto dto, string userId)
        {
            var entity = await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy sách có Id = {id}.");

            var title = (dto.Title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Tiêu đề không được để trống", nameof(dto));

            var authorId = await ResolveAuthorIdAsync(dto.AuthorName, userId);

            entity.Title = title;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.PromotionalPrice = dto.PromotionalPrice;
            entity.PromotionalStartsAt = dto.PromotionalStartsAt;
            entity.PromotionalEndsAt = dto.PromotionalEndsAt;
            entity.Stock = dto.Stock;
            entity.CategoryId = dto.CategoryId;
            entity.AuthorId = authorId;
            entity.ImageUrl = dto.ImageUrl;
            entity.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<BookDto> ChangeStatusAsync(int id, string userId)
        {
            var entity = await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy sách có Id = {id}.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedByUserId = userId;
            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        /// <summary>
        /// Tìm tác giả theo tên (không phân biệt hoa thường).
        /// Nếu chưa tồn tại thì tạo mới. Trả về null nếu tên trống.
        /// </summary>
        private async Task<int?> ResolveAuthorIdAsync(string? authorName, string userId)
        {
            var name = (authorName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var existing = await _context.Authors
                .FirstOrDefaultAsync(a => a.Name != null &&
                    a.Name.ToLower() == name.ToLower() && a.IsActive);

            if (existing != null)
                return existing.AuthorId;

            var newAuthor = new Author
            {
                Name = name,
                IsActive = true,
                UpdatedByUserId = userId
            };
            _context.Authors.Add(newAuthor);
            await _context.SaveChangesAsync();

            return newAuthor.AuthorId;
        }

        private static BookDto ToDto(Book b) => new BookDto
        {
            BookId = b.BookId,
            Title = b.Title ?? string.Empty,
            Description = b.Description,
            Price = b.Price ?? 0,
            PromotionalPrice = b.PromotionalPrice,
            PromotionalStartsAt = b.PromotionalStartsAt,
            PromotionalEndsAt = b.PromotionalEndsAt,
            Stock = b.Stock ?? 0,
            CategoryId = b.CategoryId,
            AuthorId = b.AuthorId,
            ImageUrl = b.ImageUrl,
            IsActive = b.IsActive,
            CreatedAt = b.CreatedAt,
            CategoryName = b.Category?.Name,
            AuthorName = b.Author?.Name
        };
    }
}
