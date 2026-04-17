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

        public async Task<IReadOnlyList<HomeSliderDto>> GetActiveForHomeAsync()
        {
            return await _context.HomeSliders.AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.HomeSliderId)
                .Select(s => ToDto(s))
                .ToListAsync();
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

            using var tx = await _context.Database.BeginTransactionAsync();

            int total = await _context.HomeSliders.CountAsync();
            int newOrder = dto.DisplayOrder;

            // Default: append at end
            if (newOrder < 1 || newOrder > total + 1)
                newOrder = total + 1;

            // Shift all sliders with order >= newOrder up by 1
            if (newOrder <= total)
            {
                await _context.HomeSliders
                    .Where(s => s.DisplayOrder >= newOrder)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.DisplayOrder, x => x.DisplayOrder + 1));
            }

            var entity = new HomeSlider
            {
                ImageUrl = imageUrl,
                Caption = dto.Caption?.Trim(),
                LinkUrl = dto.LinkUrl?.Trim(),
                DisplayOrder = newOrder,
                IsActive = dto.IsActive,
                CreatedByUserId = userId
            };

            _context.HomeSliders.Add(entity);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return ToDto(entity);
        }

        public async Task<HomeSliderDto> UpdateAsync(int id, HomeSliderDto dto, string userId)
        {
            var imageUrl = (dto.ImageUrl ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("URL hình ảnh không được để trống");

            using var tx = await _context.Database.BeginTransactionAsync();

            var entity = await _context.HomeSliders.FirstOrDefaultAsync(s => s.HomeSliderId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy slider có Id = {id}.");

            int total = await _context.HomeSliders.CountAsync();
            int oldOrder = entity.DisplayOrder;
            int newOrder = dto.DisplayOrder;

            if (newOrder < 1 || newOrder > total)
                throw new ArgumentException($"Thứ tự phải từ 1 đến {total}.");

            if (newOrder != oldOrder)
            {
                if (newOrder < oldOrder)
                {
                    // Move up: shift [newOrder, oldOrder-1] down by +1
                    await _context.HomeSliders
                        .Where(s => s.HomeSliderId != id
                                 && s.DisplayOrder >= newOrder
                                 && s.DisplayOrder <= oldOrder - 1)
                        .ExecuteUpdateAsync(s => s.SetProperty(x => x.DisplayOrder, x => x.DisplayOrder + 1));
                }
                else
                {
                    // Move down: shift [oldOrder+1, newOrder] up by -1
                    await _context.HomeSliders
                        .Where(s => s.HomeSliderId != id
                                 && s.DisplayOrder >= oldOrder + 1
                                 && s.DisplayOrder <= newOrder)
                        .ExecuteUpdateAsync(s => s.SetProperty(x => x.DisplayOrder, x => x.DisplayOrder - 1));
                }
            }

            entity.ImageUrl = imageUrl;
            entity.Caption = dto.Caption?.Trim();
            entity.LinkUrl = dto.LinkUrl?.Trim();
            entity.DisplayOrder = newOrder;
            entity.IsActive = dto.IsActive;
            entity.CreatedByUserId = userId;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return ToDto(entity);
        }

        public async Task DeleteAsync(int id)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var entity = await _context.HomeSliders.FirstOrDefaultAsync(s => s.HomeSliderId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy slider có Id = {id}.");

            int deletedOrder = entity.DisplayOrder;
            _context.HomeSliders.Remove(entity);
            await _context.SaveChangesAsync();

            // Compact: shift all sliders with order > deletedOrder down by -1
            await _context.HomeSliders
                .Where(s => s.DisplayOrder > deletedOrder)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.DisplayOrder, x => x.DisplayOrder - 1));

            await tx.CommitAsync();
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

        public async Task<HomeSliderDto> ReorderAsync(int id, int newOrder, string userId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var entity = await _context.HomeSliders.FirstOrDefaultAsync(s => s.HomeSliderId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy slider có Id = {id}.");

            int total = await _context.HomeSliders.CountAsync();
            if (newOrder < 1 || newOrder > total)
                throw new ArgumentException($"Thứ tự phải từ 1 đến {total}.");

            int oldOrder = entity.DisplayOrder;
            if (newOrder == oldOrder)
            {
                await tx.CommitAsync();
                return ToDto(entity);
            }

            if (newOrder < oldOrder)
            {
                // Move up: shift [newOrder, oldOrder-1] down by +1
                await _context.HomeSliders
                    .Where(s => s.HomeSliderId != id
                             && s.DisplayOrder >= newOrder
                             && s.DisplayOrder <= oldOrder - 1)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.DisplayOrder, x => x.DisplayOrder + 1));
            }
            else
            {
                // Move down: shift [oldOrder+1, newOrder] up by -1
                await _context.HomeSliders
                    .Where(s => s.HomeSliderId != id
                             && s.DisplayOrder >= oldOrder + 1
                             && s.DisplayOrder <= newOrder)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.DisplayOrder, x => x.DisplayOrder - 1));
            }

            entity.DisplayOrder = newOrder;
            entity.CreatedByUserId = userId;
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

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
