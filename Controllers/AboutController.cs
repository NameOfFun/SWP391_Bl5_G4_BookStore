using BookStore.Dtos.Common;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Staff")]
    public class AboutController : Controller
    {
        private static readonly Regex TitleAllowedPattern = new(
            @"^(?=.*[A-Za-z0-9\u00C0-\u024F\u1E00-\u1EFF])[A-Za-z0-9\u00C0-\u024F\u1E00-\u1EFF .,;:!?'""()\-–—…/]+$",
            RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(250));

        private const int TitleMaxLength = 500;
        private const int ContentMaxLength = 200_000;

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
            if (!ValidateAboutDto(dto))
                return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Không xác định được tài khoản. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(Manage));
            }

            try
            {
                await _aboutService.CreateAsync(dto, userId);
                TempData["Success"] = "Thêm thành công";
                return RedirectToAction(nameof(Manage));
            }
            catch (ArgumentException ex)
            {
                ViewData["FormError"] = ex.Message;
                return View(dto);
            }
        }

        [Authorize(Roles = "Staff,Manager")]
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

        [Authorize(Roles = "Staff,Manager")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AboutDto dto)
        {
            if (id <= 0) return NotFound();

            if (id != dto.AboutId)
            {
                ViewData["FormError"] = "Mã bài viết không khớp. Vui lòng thử lại.";
                return View(dto);
            }

            if (!ValidateAboutDto(dto))
                return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Không xác định được tài khoản. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(Manage));
            }

            try
            {
                await _aboutService.UpdateAsync(id, dto, userId);
                TempData["Success"] = "Cập nhật thành công";
                return RedirectToAction(nameof(Manage));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                ViewData["FormError"] = ex.Message;
                return View(dto);
            }
        }

        [Authorize(Roles = "Staff,Manager")]
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

        [Authorize(Roles = "Staff,Manager")]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                await _aboutService.DeleteAsync(id);
                TempData["Success"] = "Xóa thành công";
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Không tìm thấy bài viết cần xóa (có thể đã bị xóa trước đó).";
            }

            return RedirectToAction(nameof(Manage));
        }

        private bool ValidateAboutDto(AboutDto dto)
        {
            dto.Title ??= string.Empty;
            dto.ContentHtml ??= string.Empty;

            var fieldErrors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var title = dto.Title.Trim();
            if (string.IsNullOrWhiteSpace(title))
                fieldErrors[nameof(AboutDto.Title)] = "Tiêu đề không được để trống";
            else if (title.Length > TitleMaxLength)
                fieldErrors[nameof(AboutDto.Title)] = "Tiêu đề tối đa 500 ký tự";
            else if (!TitleAllowedPattern.IsMatch(title))
                fieldErrors[nameof(AboutDto.Title)] = "Tiêu đề chỉ được chứa chữ, số, khoảng trắng và dấu câu cơ bản; phải có ít nhất một chữ/số";

            if (string.IsNullOrWhiteSpace(dto.ContentHtml))
                fieldErrors[nameof(AboutDto.ContentHtml)] = "Nội dung không được để trống";
            else if (dto.ContentHtml.Length > ContentMaxLength)
                fieldErrors[nameof(AboutDto.ContentHtml)] = "Nội dung tối đa 200.000 ký tự";

            if (fieldErrors.Count == 0)
                return true;

            ViewData["FieldErrors"] = fieldErrors;
            return false;
        }
    }
}
