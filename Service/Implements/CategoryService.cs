using BookStore.Dtos.Common;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly BookStoreDbContext _context;

        public CategoryService(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories.AsNoTracking()
                .OrderByDescending(c => c.CategoryId)
                .Select(c => ToDto(c))
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            return entity == null ? null : ToDto(entity);
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto dto, string userId)
        {
            var name = (dto.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên danh mục không được để trống");

            var duplicate = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());
            if (duplicate)
                throw new ArgumentException("Tên danh mục đã tồn tại");

            var entity = new Category
            {
                Name = name,
                Description = dto.Description?.Trim(),
                IsActive = true,
                UpdatedByUserId = userId
            };

            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<CategoryDto> UpdateAsync(int id, CategoryDto dto, string userId)
        {
            var entity = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy danh mục có Id = {id}.");

            var name = (dto.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên danh mục không được để trống");

            var duplicate = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.CategoryId != id);
            if (duplicate)
                throw new ArgumentException("Tên danh mục đã tồn tại");

            entity.Name = name;
            entity.Description = dto.Description?.Trim();
            entity.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        public async Task<CategoryDto> ToggleStatusAsync(int id, string userId)
        {
            var entity = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (entity == null)
                throw new InvalidOperationException($"Không tìm thấy danh mục có Id = {id}.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedByUserId = userId;

            await _context.SaveChangesAsync();

            return ToDto(entity);
        }

        private static CategoryDto ToDto(Category c) => new CategoryDto
        {
            CategoryId = c.CategoryId,
            Name = c.Name ?? string.Empty,
            Description = c.Description,
            IsActive = c.IsActive
        };
    }
}
