using BookStore.Dtos.Common;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Staff")]
    public class HomeSliderController : Controller
    {
        private readonly IHomeSliderService _sliderService;

        public HomeSliderController(IHomeSliderService sliderService)
        {
            _sliderService = sliderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? status, int page = 1)
        {
            const int PageSize = 10;
            var all = await _sliderService.GetAllAsync();

            var filtered = all.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
                filtered = filtered.Where(s =>
                    s.Caption != null && s.Caption.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (status == "active") filtered = filtered.Where(s => s.IsActive);
            else if (status == "inactive") filtered = filtered.Where(s => !s.IsActive);

            var list = filtered.ToList();
            int totalCount = list.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            ViewData["Page"] = page;
            ViewData["TotalPages"] = Math.Max(1, totalPages);
            ViewData["TotalCount"] = totalCount;
            ViewData["PageSize"] = PageSize;
            ViewData["AllSlidersCount"] = all.Count;
            ViewData["Search"] = search;
            ViewData["Status"] = status;

            return View(list.Skip((page - 1) * PageSize).Take(PageSize).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new HomeSliderDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeSliderDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _sliderService.CreateAsync(dto, userId);
                TempData["Success"] = "Thêm slider thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var slider = await _sliderService.GetByIdAsync(id);
            if (slider == null) return NotFound();
            return View(slider);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HomeSliderDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _sliderService.UpdateAsync(id, dto, userId);
                TempData["Success"] = "Cập nhật slider thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sliderService.DeleteAsync(id);
                TempData["Success"] = "Đã xoá slider";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var result = await _sliderService.ToggleStatusAsync(id, userId);
                TempData["Success"] = result.IsActive ? "Đã kích hoạt slider" : "Đã vô hiệu hóa slider";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveUp(int id)
        {
            try
            {
                var slider = await _sliderService.GetByIdAsync(id);
                if (slider != null)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                    await _sliderService.ReorderAsync(id, slider.DisplayOrder - 1, userId);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveDown(int id)
        {
            try
            {
                var slider = await _sliderService.GetByIdAsync(id);
                if (slider != null)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                    await _sliderService.ReorderAsync(id, slider.DisplayOrder + 1, userId);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
