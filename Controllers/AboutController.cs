using BookStore.Dtos.Common;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
<<<<<<< HEAD
=======
    [Authorize(Roles = "Staff")]
>>>>>>> 5b3246b (update role)
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        /// <summary>Tin tức / Giới thiệu — mọi người xem (template LIBRARIA news list).</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Tin tức & Giới thiệu";
            ViewData["LibrariaInnerHeader"] = true;
            return View(await _aboutService.GetAllAboutAsync());
        }

        /// <summary>Chi tiết một bài About — công khai.</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Article(int id)
        {
            if (id <= 0)
                return NotFound();

            var about = await _aboutService.GetByIdAsync(id);
            if (about == null)
                return NotFound();

            ViewData["Title"] = about.Title;
            ViewData["LibrariaInnerHeader"] = true;
            return View(about);
        }

        /// <summary>Quản lý CRUD — Admin, Staff, Manager.</summary>
        [Authorize(Roles = "Admin,Staff,Manager")]
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            ViewData["Title"] = "Quản lý Giới thiệu / Tin tức";
            return View(await _aboutService.GetAllAboutAsync());
        }

        [Authorize(Roles = "Admin,Staff,Manager")]
        [HttpGet]
        public IActionResult Create() => View(new AboutDto());

        [Authorize(Roles = "Admin,Staff,Manager")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Không xác định được tài khoản. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(Manage));
            }

            await _aboutService.CreateAsync(dto, userId);
            TempData["Success"] = "Thêm thành công";

            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Roles = "Admin,Staff,Manager")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var about = await _aboutService.GetByIdAsync(id);
            if (about == null)
                return NotFound();
            return View(about);
        }

        [Authorize(Roles = "Admin,Staff,Manager")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AboutDto dto)
        {
            if (id != dto.AboutId)
            {
                ModelState.AddModelError(string.Empty, "Mã bài viết không khớp. Vui lòng thử lại.");
                return View(dto);
            }

            if (!ModelState.IsValid) return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Không xác định được tài khoản. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(Manage));
            }

            await _aboutService.UpdateAsync(id, dto, userId);
            TempData["Success"] = "Cập nhật thành công";
            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Roles = "Admin,Staff,Manager")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var about = await _aboutService.GetByIdAsync(id);
            if (about == null)
                return NotFound();
            return View(about);
        }

        [Authorize(Roles = "Admin,Staff,Manager")]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
                return NotFound();

            await _aboutService.DeleteAsync(id);
            TempData["Success"] = "Xóa thành công";
            return RedirectToAction(nameof(Manage));
        }
    }
}
