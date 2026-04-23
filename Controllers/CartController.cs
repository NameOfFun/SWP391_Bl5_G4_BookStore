using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Giỏ hàng";
        ViewData["LibrariaInnerHeader"] = true;
        var vm = await _cartService.GetCartAsync(UserId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int bookId, int quantity = 1)
    {
        if (bookId <= 0)
        {
            TempData["CartError"] = "Mã sách không hợp lệ.";
            return RedirectToAction("Shop", "Book");
        }

        var (ok, err) = await _cartService.AddItemAsync(UserId, bookId, quantity);
        if (!ok)
        {
            TempData["CartError"] = err;
            return RedirectToAction("Details", "Book", new { id = bookId });
        }

        TempData["CartMessage"] = "Đã thêm sách vào giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int cartItemId, int quantity)
    {
        if (cartItemId <= 0)
        {
            TempData["CartError"] = "Mã sản phẩm trong giỏ không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        var (ok, err) = await _cartService.SetQuantityAsync(UserId, cartItemId, quantity);
        if (!ok)
            TempData["CartError"] = err;
        else
            TempData["CartMessage"] = "Đã cập nhật số lượng.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        if (cartItemId <= 0)
        {
            TempData["CartError"] = "Mã sản phẩm trong giỏ không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        await _cartService.RemoveItemAsync(UserId, cartItemId);
        TempData["CartMessage"] = "Đã xóa sản phẩm khỏi giỏ.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        await _cartService.ClearAllAsync(UserId);
        TempData["CartMessage"] = "Đã xóa toàn bộ sản phẩm trong giỏ.";
        return RedirectToAction(nameof(Index));
    }
}
