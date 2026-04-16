using BookStore.Dtos.Common;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements
{
    public class HomeSliderService : IHomeSliderService
    {
        private readonly BookStoreDbContext _context;

        public HomeSliderService(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<HomeSliderDto>> GetAllAsync()
        {
            return await _context.HomeSliders.AsNoTracking()
                .OrderBy(s => s.DisplayOrder)
                .ThenByDescending(s => s.HomeSliderId)
                .Select(s => ToDto(s))
                .ToListAsync();
        }

        public async Task<HomeSliderDto?> GetByIdAsync(int id)
        {
            var entity = await _context.HomeSliders.AsNoTracking()
                .FirstOrDefaultAsync(s => s.HomeSliderId == id);

            return entity == null ? null : ToDto(entity);
        }

        public async Task<HomeSliderDto> CreateAsync(HomeSliderDto dto, string userId)
        {
            var imageUrl = (dto.ImageUrl ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("URL hình ảnh không được để trống");

            var entity = new HomeSlider
            {
                ImageUrl = imageUrl,
                Caption = dto.Caption?.Trim(),
                LinkUrl = dto.LinkUrl?.Trim(),
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                CreatedByUserId = userId
            };

            _context.HomeSliders.Add(entity);
            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<HomeSliderDto> UpdateAsync(int id, HomeSliderDto dto, string userId)
        {
            var entity = await _context.HomeSliders.FirstOrDefaultAsync(s => s.HomeSliderId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy slider có Id = {id}.");

            var imageUrl = (dto.ImageUrl ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("URL hình ảnh không được để trống");

            entity.ImageUrl = imageUrl;
            entity.Caption = dto.Caption?.Trim();
            entity.LinkUrl = dto.LinkUrl?.Trim();
            entity.DisplayOrder = dto.DisplayOrder;
            entity.IsActive = dto.IsActive;
            entity.CreatedByUserId = userId;

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.HomeSliders.FirstOrDefaultAsync(s => s.HomeSliderId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy slider có Id = {id}.");

            _context.HomeSliders.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<HomeSliderDto> ToggleStatusAsync(int id, string userId)
        {
            var entity = await _context.HomeSliders.FirstOrDefaultAsync(s => s.HomeSliderId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy slider có Id = {id}.");

            entity.IsActive = !entity.IsActive;
            entity.CreatedByUserId = userId;

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        private static HomeSliderDto ToDto(HomeSlider s) => new HomeSliderDto
        {
            HomeSliderId = s.HomeSliderId,
            ImageUrl = s.ImageUrl,
            Caption = s.Caption,
            LinkUrl = s.LinkUrl,
            DisplayOrder = s.DisplayOrder,
            IsActive = s.IsActive
        };
    }
}
