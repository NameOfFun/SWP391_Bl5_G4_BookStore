using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;

namespace BookStore.Controllers;

[Authorize(Roles = "Shipper")]
public class ShipperController : Controller
{
    private readonly IShipperService _shipperService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ShipperController(IShipperService shipperService, UserManager<ApplicationUser> userManager)
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

    // ─── GET /Shipper/DeliveryDetail/{id} ─────────────────────
    public async Task<IActionResult> DeliveryDetail(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var vm = await _shipperService.GetDeliveryDetailAsync(id, user.Id);
        if (vm is null) return NotFound();

        ViewData["Title"]               = $"Chi tiết đơn #{id}";
        ViewData["BreadcrumbParent"]    = "Đơn được giao";
        ViewData["BreadcrumbParentUrl"] = Url.Action("AssignedOrders", "Shipper");
        return View(vm);
    }

    // ─── POST /Shipper/MarkDelivered ──────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkDelivered(int orderId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var (ok, message) = await _shipperService.MarkDeliveredAsync(orderId, user.Id);

        TempData[ok ? "Success" : "Error"] = message;
        // Sau khi giao thành công → về danh sách
        return ok
            ? RedirectToAction(nameof(AssignedOrders), new { filter = "all" })
            : RedirectToAction(nameof(DeliveryDetail), new { id = orderId });
    }
}
