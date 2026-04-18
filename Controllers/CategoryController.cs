using BookStore.Dtos.Common;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Staff")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private const int PageSize = 10;

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? status, int page = 1)
        {
            ViewData["Search"] = search;
            ViewData["Status"] = status;

            var all = await _categoryService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
                all = all.Where(c => c.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            if (status == "active")
                all = all.Where(c => c.IsActive).ToList();
            else if (status == "inactive")
                all = all.Where(c => !c.IsActive).ToList();

            int totalCount = all.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["TotalCount"] = totalCount;
            ViewData["PageSize"] = PageSize;

            var paged = all.Skip((page - 1) * PageSize).Take(PageSize).ToList();
            return View(paged);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _categoryService.CreateAsync(dto, userId);
            TempData["Success"] = "Thêm danh mục thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _categoryService.UpdateAsync(id, dto, userId);
            TempData["Success"] = "Cập nhật danh mục thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _categoryService.ToggleStatusAsync(id, userId);
            TempData["Success"] = result.IsActive
                ? "Đã kích hoạt danh mục"
                : "Đã vô hiệu hóa danh mục";
            return RedirectToAction(nameof(Index));
        }
    }
}
