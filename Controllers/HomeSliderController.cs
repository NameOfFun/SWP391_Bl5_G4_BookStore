using BookStore.Dtos.Common;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    //[Authorize(Roles = "Admin,Manager")]
    [Authorize(Roles = "Customer")]
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
            ViewData["Title"] = "Quản Lý Home Slider";
            ViewData["BreadcrumbParent"] = "Trang Chủ";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "Home");
            return View(await _sliderService.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Thêm Slider";
            ViewData["BreadcrumbParent"] = "Home Slider";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index");
            return View(new HomeSliderDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeSliderDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Thêm Slider";
                ViewData["BreadcrumbParent"] = "Home Slider";
                ViewData["BreadcrumbParentUrl"] = Url.Action("Index");
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

            ViewData["Title"] = "Sửa Slider";
            ViewData["BreadcrumbParent"] = "Home Slider";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index");
            return View(slider);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HomeSliderDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Sửa Slider";
                ViewData["BreadcrumbParent"] = "Home Slider";
                ViewData["BreadcrumbParentUrl"] = Url.Action("Index");
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
