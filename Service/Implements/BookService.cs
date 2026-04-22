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
            var books = await _context.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.Tags)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return books.Select(ToDto).ToList();
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.BookId == id);

            return entity == null ? null : ToDto(entity);
        }

        public async Task<BookDto> CreateAsync(BookDto dto, string userId)
        {
            var title = (dto.Title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Tiêu đề không được để trống");

            if (dto.PromotionalStartsAt.HasValue && dto.PromotionalEndsAt.HasValue
                && dto.PromotionalStartsAt.Value >= dto.PromotionalEndsAt.Value)
                throw new ArgumentException("Ngày bắt đầu khuyến mãi phải trước ngày kết thúc.");

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

            if (dto.TagIds.Count > 0)
            {
                var tags = await _context.BookTags
                    .Where(t => dto.TagIds.Contains(t.TagId) && t.IsActive)
                    .ToListAsync();
                foreach (var tag in tags)
                    entity.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }

            return ToDto(entity);
        }

        public async Task<BookDto> UpdateAsync(int id, BookDto dto, string userId)
        {
            var entity = await _context.Books
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy sách có Id = {id}.");

            var title = (dto.Title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Tiêu đề không được để trống", nameof(dto));

            if (dto.PromotionalStartsAt.HasValue && dto.PromotionalEndsAt.HasValue
                && dto.PromotionalStartsAt.Value >= dto.PromotionalEndsAt.Value)
                throw new ArgumentException("Ngày bắt đầu khuyến mãi phải trước ngày kết thúc.");

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

            // Sync tags
            entity.Tags.Clear();
            if (dto.TagIds.Count > 0)
            {
                var tags = await _context.BookTags
                    .Where(t => dto.TagIds.Contains(t.TagId) && t.IsActive)
                    .ToListAsync();
                foreach (var tag in tags)
                    entity.Tags.Add(tag);
            }

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<IReadOnlyList<BookDto>> GetNewReleasesForHomeAsync(int take = 12)
        {
            take = Math.Clamp(take, 1, 48);
            var books = await _context.Books
                .AsNoTracking()
                .Where(b => b.IsActive)
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.Tags)
                .OrderByDescending(b => b.CreatedAt ?? DateTime.MinValue)
                .ThenByDescending(b => b.BookId)
                .Take(take)
                .ToListAsync();

            return books.Select(ToDto).ToList();
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
            CategoryName = b.Category?.IsActive == true ? b.Category.Name : null,
            AuthorName = b.Author?.Name,
            TagIds = b.Tags.Where(t => t.IsActive).Select(t => t.TagId).ToList(),
            TagNames = b.Tags.Where(t => t.IsActive).Select(t => t.Name ?? string.Empty).ToList()
        };
    }
}
