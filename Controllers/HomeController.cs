using BookStore.Models;
using BookStore.Service.Interfaces;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeSliderService _homeSliderService;
        private readonly IBookService _bookService;
        private readonly BookStoreDbContext _db;

        public HomeController(
            ILogger<HomeController> logger,
            IHomeSliderService homeSliderService,
            IBookService bookService,
            BookStoreDbContext db)
        {
            _logger = logger;
            _homeSliderService = homeSliderService;
            _bookService = bookService;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var shopCategories = await _db.Categories.AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new HomeShopCategoryOption(c.CategoryId, c.Name ?? ""))
                .ToListAsync();

            var vm = new HomeIndexViewModel
            {
                Sliders = await _homeSliderService.GetActiveForHomeAsync(),
                NewBooks = await _bookService.GetNewReleasesForHomeAsync(5),
                ShopCategories = shopCategories
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
