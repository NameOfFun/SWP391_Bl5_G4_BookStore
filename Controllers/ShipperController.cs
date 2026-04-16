using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers;

[Authorize(Roles = "Shipper")]
public class ShipperController : Controller
{
    private readonly IShipperService _shipperService;
    private readonly UserManager<IdentityUser> _userManager;

    public ShipperController(IShipperService shipperService, UserManager<IdentityUser> userManager)
    {
        _shipperService = shipperService;
        _userManager    = userManager;
    }

    // ─── GET /Shipper/Dashboard ───────────────────────────────
    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var vm = await _shipperService.GetDashboardAsync(user.Id);
        ViewData["Title"]               = "Dashboard";
        ViewData["BreadcrumbParent"]    = "Shipper";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Dashboard", "Shipper");
        return View(vm);
    }

    // ─── GET /Shipper/AssignedOrders?filter=all|assigned|delivering ──
    public async Task<IActionResult> AssignedOrders(string? filter)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var vm = await _shipperService.GetAssignedOrdersAsync(user.Id, filter);
        ViewData["Title"]               = "Đơn được giao";
        ViewData["BreadcrumbParent"]    = "Shipper";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Dashboard", "Shipper");
        return View(vm);
    }

    // ─── POST /Shipper/AcceptOrder ────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptOrder(int orderId, string? returnFilter)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var (ok, message) = await _shipperService.AcceptOrderAsync(orderId, user.Id);

        TempData[ok ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(AssignedOrders), new { filter = returnFilter ?? "all" });
    }

    // ─── POST /Shipper/RejectOrder ────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectOrder(int orderId, string? returnFilter)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var (ok, message) = await _shipperService.RejectOrderAsync(orderId, user.Id);

        TempData[ok ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(AssignedOrders), new { filter = returnFilter ?? "all" });
    }
}
