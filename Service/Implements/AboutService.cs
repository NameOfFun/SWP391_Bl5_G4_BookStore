using BookStore.Dtos.Common;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements
{
    public class AboutService : IAboutService
    {
        private readonly BookStoreDbContext _context;
        public AboutService(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AboutDto>> GetAllAboutAsync()
        {
            return await _context.About.AsNoTracking()
                .OrderByDescending(a => a.UpdatedAt)
                .Select(a => ToDto(a)).ToListAsync();
        }

        public async Task<AboutDto?> GetByIdAsync(int id)
        {
            var entity = await _context.About.AsNoTracking()
                .FirstOrDefaultAsync(a => a.AboutId == id);

            return entity == null ? null : ToDto(entity);
        }

        public async Task<AboutDto> CreateAsync(AboutDto dto, string userId)
        {
            var title = (dto.Title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Tiêu đề không được để trống");

            var entity = new About
            {
                Title = title,
                ContentHtml = dto.ContentHtml,
                UpdatedAt = DateTime.UtcNow,
                UpdatedByUserId = userId
            };

            _context.About.Add(entity);
            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<AboutDto> UpdateAsync(int id, AboutDto dto, string userId)
        {
            var entity = await _context.About.FirstOrDefaultAsync(a => a.AboutId == id);
            if(entity == null)
            {
                throw new InvalidOperationException($"Không tìm thấy About có Id = {id}.");
            }

            var title = (dto.Title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Tiêu đề không được để trống", nameof(dto));

            entity.Title = title;
            entity.ContentHtml = dto.ContentHtml ?? string.Empty;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<AboutDto> DeleteAsync(int id)
        {
            var entity = await _context.About.FirstOrDefaultAsync(a => a.AboutId == id);
            if (entity == null)
            {
                throw new InvalidOperationException($"Không tìm thấy About có Id = {id}.");
            }

            var removed = ToDto(entity);
            _context.About.Remove(entity);
            await _context.SaveChangesAsync();

            return removed;
        }

        private static AboutDto ToDto(About a) => new AboutDto
        {
            AboutId = a.AboutId,
            Title = a.Title,
            ContentHtml = a.ContentHtml,
            UpdatedAt = a.UpdatedAt
        };
    }
}
