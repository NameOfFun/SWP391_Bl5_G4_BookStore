using BookStore.Dtos.Common;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Staff")]
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;
        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _aboutService.GetAllAboutAsync());
        }

        [HttpGet]
        public IActionResult Create() => View(new AboutDto());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _aboutService.CreateAsync(dto, userId);
            TempData["Success"] = "Thêm thành công";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var about = await _aboutService.GetByIdAsync(id);
            if (about == null)
                return NotFound();
            return View(about);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AboutDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _aboutService.UpdateAsync(id, dto, userId);
            TempData["Success"] = "Cập nhật thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var about = await _aboutService.GetByIdAsync(id);
            if (about == null)
                return NotFound();
            return View(about);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _aboutService.DeleteAsync(id);
            TempData["Success"] = "Xóa thành công";
            return RedirectToAction(nameof(Index));
        }

    }
}
