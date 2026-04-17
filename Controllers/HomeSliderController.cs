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
        public async Task<IActionResult> Index()
        {
            return View(await _sliderService.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new HomeSliderDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeSliderDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _sliderService.CreateAsync(dto, userId);
            TempData["Success"] = "Thêm slider thành công";
            return RedirectToAction(nameof(Index));
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
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _sliderService.UpdateAsync(id, dto, userId);
            TempData["Success"] = "Cập nhật slider thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _sliderService.DeleteAsync(id);
            TempData["Success"] = "Đã xoá slider";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _sliderService.ToggleStatusAsync(id, userId);
            TempData["Success"] = result.IsActive ? "Đã kích hoạt slider" : "Đã vô hiệu hóa slider";
            return RedirectToAction(nameof(Index));
        }
    }
}
