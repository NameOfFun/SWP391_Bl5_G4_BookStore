using BookStore.Dtos.Common;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.Controllers
{
    //[Authorize(Roles = "Admin,Staff,Manager")]
    [Authorize(Roles = "Customer")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly BookStoreDbContext _context;

        public BookController(IBookService bookService, BookStoreDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        // GET: /Book
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _bookService.GetAllAsync());
        }

        // GET: /Book/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // GET: /Book/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new BookDto());
        }

        // POST: /Book/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(dto);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _bookService.CreateAsync(dto, userId);
                TempData["Success"] = "Thêm sách thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownsAsync();
                return View(dto);
            }
        }

        // GET: /Book/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            await PopulateDropdownsAsync(book.CategoryId);
            return View(book);
        }

        // POST: /Book/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(dto.CategoryId);
                return View(dto);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _bookService.UpdateAsync(id, dto, userId);
                TempData["Success"] = "Cập nhật sách thành công";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownsAsync(dto.CategoryId);
                return View(dto);
            }
        }

        // POST: /Book/ChangeStatus/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var book = await _bookService.ChangeStatusAsync(id, userId);
                TempData["Success"] = book.IsActive
                    ? $"Sách \"{book.Title}\" đã được kích hoạt (Active)."
                    : $"Sách \"{book.Title}\" đã bị vô hiệu hóa (Deactive).";
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Không tìm thấy sách cần thay đổi trạng thái.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------------
        private async Task PopulateDropdownsAsync(int? selectedCategory = null)
        {
            ViewBag.Categories = new SelectList(
                await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(),
                "CategoryId", "Name", selectedCategory);
        }
    }
}
