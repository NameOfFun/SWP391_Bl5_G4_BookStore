using BookStore.Dtos.Common;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeSliderService _homeSliderService;

        public HomeController(ILogger<HomeController> logger, IHomeSliderService homeSliderService)
        {
            _logger = logger;
            _homeSliderService = homeSliderService;
        }

        public async Task<IActionResult> Index()
        {
            IReadOnlyList<HomeSliderDto> sliders = await _homeSliderService.GetActiveForHomeAsync();
            return View(sliders);
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
