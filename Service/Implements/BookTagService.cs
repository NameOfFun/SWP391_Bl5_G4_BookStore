using BookStore.Dtos.Common;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements
{
    public class BookTagService : IBookTagService
    {
        private readonly BookStoreDbContext _context;

        public BookTagService(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<BookTagDto>> GetAllAsync()
        {
            return await _context.BookTags.AsNoTracking()
                .OrderByDescending(t => t.TagId)
                .Select(t => ToDto(t))
                .ToListAsync();
        }

        public async Task<BookTagDto?> GetByIdAsync(int id)
        {
            var entity = await _context.BookTags.AsNoTracking()
                .FirstOrDefaultAsync(t => t.TagId == id);

            return entity == null ? null : ToDto(entity);
        }

        public async Task<BookTagDto> CreateAsync(BookTagDto dto, string userId)
        {
            var normalized = NormalizeName(dto.Name);
            if (normalized == null)
                throw new ArgumentException("Tên tag không được để trống");

            var duplicate = await _context.BookTags.AsNoTracking()
                .AnyAsync(t => t.Name == normalized);
            if (duplicate)
                throw new ArgumentException($"Tag \"{normalized}\" đã tồn tại.");

            var entity = new BookTag
            {
                Name = normalized,
                IsActive = true,
                UpdatedByUserId = userId
            };

            _context.BookTags.Add(entity);
            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<BookTagDto> UpdateAsync(int id, BookTagDto dto, string userId)
        {
            var entity = await _context.BookTags.FirstOrDefaultAsync(t => t.TagId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy tag có Id = {id}.");

            var normalized = NormalizeName(dto.Name);
            if (normalized == null)
                throw new ArgumentException("Tên tag không được để trống");

            var duplicate = await _context.BookTags.AsNoTracking()
                .AnyAsync(t => t.Name == normalized && t.TagId != id);
            if (duplicate)
                throw new ArgumentException($"Tag \"{normalized}\" đã tồn tại.");

            entity.Name = normalized;
            entity.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        // Strips leading '#' characters, trims whitespace, then prepends a single '#'.
        // Returns null if the result would be empty after stripping.
        private static string? NormalizeName(string? raw)
        {
            var trimmed = (raw ?? string.Empty).TrimStart('#').Trim();
            return string.IsNullOrEmpty(trimmed) ? null : "#" + trimmed;
        }

        public async Task<BookTagDto> ToggleStatusAsync(int id, string userId)
        {
            var entity = await _context.BookTags.FirstOrDefaultAsync(t => t.TagId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy tag có Id = {id}.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        private static BookTagDto ToDto(BookTag t) => new BookTagDto
        {
            TagId = t.TagId,
            Name = t.Name ?? string.Empty,
            IsActive = t.IsActive
        };
    }
}
