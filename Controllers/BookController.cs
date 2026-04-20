using BookStore.Dtos.Common;
using BookStore.Helpers;
using BookStore.Models;
using BookStore.Service.Interfaces;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Authorize(Roles = "Staff,Manager")]
    public class BookController : Controller
    {
        private const int ShopQueryMaxLength = 200;

        private static readonly HashSet<string> ShopOrderWhitelist = new(StringComparer.OrdinalIgnoreCase)
        {
            "newest", "title", "price_asc", "price_desc"
        };

        private readonly IBookService _bookService;
        private readonly BookStoreDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(IBookService bookService, BookStoreDbContext context, IWebHostEnvironment env)
        {
            _bookService = bookService;
            _context = context;
            _env = env;
        }

        // GET: /Book — chỉ quản trị; khách chuyển sang Shop
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Staff") || User.IsInRole("Manager"))
                return View(await _bookService.GetAllAsync());
            return RedirectToAction(nameof(Shop));
        }

        /// <summary>Cửa hàng — danh sách sách (template LIBRARIA list/grid).</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Shop(string? q, int? categoryId, string view = "list", string order = "newest")
        {
            ViewData["Title"] = "Danh sách sách";
            ViewData["LibrariaInnerHeader"] = true;

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                if (q.Length > ShopQueryMaxLength)
                    q = q[..ShopQueryMaxLength];
            }
            else
                q = null;

            if (categoryId.HasValue)
            {
                var categoryOk = await _context.Categories.AnyAsync(c => c.IsActive && c.CategoryId == categoryId.Value);
                if (!categoryOk)
                    categoryId = null;
            }

            if (string.IsNullOrWhiteSpace(order) || !ShopOrderWhitelist.Contains(order))
                order = "newest";

            var all = await _bookService.GetAllAsync();
            IEnumerable<BookDto> query = all.Where(b => b.IsActive);

            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId);
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(b => b.Title.Contains(q.Trim(), StringComparison.OrdinalIgnoreCase));

            query = order switch
            {
                "price_asc" => query.OrderBy(b => b.EffectivePrice),
                "price_desc" => query.OrderByDescending(b => b.EffectivePrice),
                "title" => query.OrderBy(b => b.Title),
                 _ => query.OrderByDescending(b => b.CreatedAt ?? DateTime.MinValue)
            };

            var list = query.ToList();
            var catList = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
            var activeAll = all.Where(b => b.IsActive).ToList();
            ViewBag.Categories = catList;
            ViewBag.CategoryCounts = catList.ToDictionary(c => c.CategoryId, c => activeAll.Count(b => b.CategoryId == c.CategoryId));
            ViewBag.TotalActiveCount = activeAll.Count;
            ViewBag.CategoryId = categoryId;
            ViewBag.Search = q;
            ViewBag.ViewMode = string.Equals(view, "grid", StringComparison.OrdinalIgnoreCase) ? "grid" : "list";
            ViewBag.Order = order;
            ViewBag.TotalCount = list.Count;
            return View(list);
        }

        /// <summary>Bảng sách theo từng danh mục (giao diện tối, nhiều nhóm).</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Categories()
        {
            ViewData["Title"] = "Sách theo danh mục";
            ViewData["LibrariaInnerHeader"] = true;

            var all = await _bookService.GetAllAsync();
            var active = all.Where(b => b.IsActive).ToList();
            var cats = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();

            var groups = cats
                .Select(c => new CategoryBooksGroup
                {
                    CategoryName = c.Name ?? "",
                    Books = active.Where(b => b.CategoryId == c.CategoryId).OrderBy(b => b.Title).ToList()
                })
                .Where(g => g.Books.Count > 0)
                .ToList();

            return View(groups);
        }

        // GET: /Book/Details/5
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            var canManage = User.IsInRole("Staff") || User.IsInRole("Manager");
            if (!book.IsActive && !canManage)
                return NotFound();

            ViewData["Title"] = book.Title;
            ViewData["LibrariaInnerHeader"] = true;
            ViewBag.CoverUrl = BookCoverHelper.ResolveCoverPath(_env, book.BookId, book.ImageUrl);
            return View(book);
        }

        // GET: /Book/Create
        [HttpGet]
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new BookDto());
        }

        // POST: /Book/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Manager")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> Create(BookDto dto, IFormFile? coverImage)
        {
            bool canSetPrice = User.IsInRole("Manager");
            if (!canSetPrice)
            {
                dto.Price = 0;
                dto.PromotionalPrice = null;
                dto.PromotionalStartsAt = null;
                dto.PromotionalEndsAt = null;
                ModelState.Remove(nameof(BookDto.Price));
                ModelState.Remove(nameof(BookDto.PromotionalPrice));
                ModelState.Remove(nameof(BookDto.PromotionalStartsAt));
                ModelState.Remove(nameof(BookDto.PromotionalEndsAt));
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(dto);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                if (coverImage != null && coverImage.Length > 0)
                    dto.ImageUrl = null;

                var created = await _bookService.CreateAsync(dto, userId);

                if (coverImage != null && coverImage.Length > 0)
                {
                    var path = await BookCoverHelper.SaveUploadedCoverAsync(_env, created.BookId, coverImage);
                    created.ImageUrl = path;
                    await _bookService.UpdateAsync(created.BookId, created, userId);
                }

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
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            await PopulateDropdownsAsync(book.CategoryId);
            return View(book);
        }

        // POST: /Book/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Manager")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> Edit(int id, BookDto dto, IFormFile? coverImage)
        {
            bool canSetPrice = User.IsInRole("Manager");
            if (!canSetPrice)
            {
                var existing = await _bookService.GetByIdAsync(id);
                if (existing == null) return NotFound();
                dto.Price = existing.Price;
                dto.PromotionalPrice = existing.PromotionalPrice;
                dto.PromotionalStartsAt = existing.PromotionalStartsAt;
                dto.PromotionalEndsAt = existing.PromotionalEndsAt;
                ModelState.Remove(nameof(BookDto.Price));
                ModelState.Remove(nameof(BookDto.PromotionalPrice));
                ModelState.Remove(nameof(BookDto.PromotionalStartsAt));
                ModelState.Remove(nameof(BookDto.PromotionalEndsAt));
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(dto.CategoryId);
                return View(dto);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                if (coverImage != null && coverImage.Length > 0)
                    dto.ImageUrl = await BookCoverHelper.SaveUploadedCoverAsync(_env, id, coverImage);

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
        [Authorize(Roles = "Staff,Manager")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Mã sách không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

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

            ViewBag.AllTags = await _context.BookTags
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .Select(t => new { t.TagId, t.Name })
                .ToListAsync();
        }
    }
}
