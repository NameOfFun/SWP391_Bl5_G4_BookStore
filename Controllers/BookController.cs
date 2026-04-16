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

            await PopulateDropdownsAsync(book.CategoryId, book.AuthorId);
            return View(book);
        }

        // POST: /Book/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(dto.CategoryId, dto.AuthorId);
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
                await PopulateDropdownsAsync(dto.CategoryId, dto.AuthorId);
                return View(dto);
            }
        }

        // GET: /Book/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: /Book/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _bookService.DeleteAsync(id);
                TempData["Success"] = "Xóa sách thành công";
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] = "Không tìm thấy sách cần xóa";
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------------
        private async Task PopulateDropdownsAsync(int? selectedCategory = null, int? selectedAuthor = null)
        {
            ViewBag.Categories = new SelectList(
                await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(),
                "CategoryId", "Name", selectedCategory);

            ViewBag.Authors = new SelectList(
                await _context.Authors.Where(a => a.IsActive).OrderBy(a => a.Name).ToListAsync(),
                "AuthorId", "Name", selectedAuthor);
        }
    }
}
