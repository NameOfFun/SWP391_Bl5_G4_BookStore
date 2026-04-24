using BookStore.Dtos.Common;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Staff")]
    public class BookTagController : Controller
    {
        private readonly IBookTagService _bookTagService;

        public BookTagController(IBookTagService bookTagService)
        {
            _bookTagService = bookTagService;
        }

        private const int PageSize = 10;

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? status, int page = 1)
        {
            ViewData["Search"] = search;
            ViewData["Status"] = status;

            var all = await _bookTagService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
                all = all.Where(t => t.Name.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            if (status == "active")
                all = all.Where(t => t.IsActive).ToList();
            else if (status == "inactive")
                all = all.Where(t => !t.IsActive).ToList();

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
            return View(new BookTagDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookTagDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Tên tag không được để trống");
            else if (dto.Name.Length > 99)
                errors.Add("Tên tag không được vượt quá 99 ký tự");

            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View(dto);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _bookTagService.CreateAsync(dto, userId);
                TempData["Success"] = "Thêm tag thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ViewBag.Errors = new List<string> { ex.Message };
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await _bookTagService.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookTagDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Tên tag không được để trống");
            else if (dto.Name.Length > 99)
                errors.Add("Tên tag không được vượt quá 99 ký tự");

            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View(dto);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _bookTagService.UpdateAsync(id, dto, userId);
                TempData["Success"] = "Cập nhật tag thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ViewBag.Errors = new List<string> { ex.Message };
                return View(dto);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _bookTagService.ToggleStatusAsync(id, userId);
            TempData["Success"] = result.IsActive
                ? "Đã kích hoạt tag"
                : "Đã vô hiệu hóa tag";
            return RedirectToAction(nameof(Index));
        }
    }
}
