using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Dtos;

namespace BookStore.Controllers;

[Authorize]
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
    [Authorize(Roles = "Shipper")]
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
    [Authorize(Roles = "Shipper")]
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
    [Authorize(Roles = "Shipper")]
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

    // ─── POST /Shipper/UpdateStatus ───────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(DeliveryStatusUpdateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            return RedirectToAction(nameof(DeliveryDetail), new { id = dto.OrderId });
        }

        // Xử lý upload ảnh nếu có
        if (dto.ProofImage != null && dto.ProofImage.Length > 0)
        {
            try 
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pod");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = $"POD_{dto.OrderId}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(dto.ProofImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProofImage.CopyToAsync(stream);
                }
                
                dto.ProofImagePath = $"/uploads/pod/{fileName}";
            }
            catch (Exception ex)
            {
                // Log error if needed
                TempData["Error"] = "Có lỗi xảy ra khi lưu ảnh minh chứng.";
                return RedirectToAction(nameof(DeliveryDetail), new { id = dto.OrderId });
            }
        }

        var (ok, message) = await _shipperService.UpdateDeliveryStatusAsync(dto, user.Id);

        TempData[ok ? "Success" : "Error"] = message;
        
        // Nếu thành công (dù là Giao được hay Thất bại) -> Về danh sách chung
        return ok
            ? RedirectToAction(nameof(AssignedOrders), new { filter = "all" })
            : RedirectToAction(nameof(DeliveryDetail), new { id = dto.OrderId });
    }

    // ─── GET /Shipper/DeliveryHistory?filter=all|success|failed ──
    [Authorize(Roles = "Shipper")]
    public async Task<IActionResult> DeliveryHistory(string? filter)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var vm = await _shipperService.GetDeliveryHistoryAsync(user.Id, filter);
        
        ViewData["Title"]               = "Lịch sử giao hàng";
        ViewData["BreadcrumbParent"]    = "Shipper";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Dashboard", "Shipper");
        
        return View(vm);
    }

    // ─── GET /Shipper/ManagePerformance ───────────────────────
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ManagePerformance(string? shipperId)
    {
        var vm = string.IsNullOrEmpty(shipperId)
            ? await _shipperService.GetManagementPerformanceAsync()
            : await _shipperService.GetShipperPerformanceAsync(shipperId);
        
        ViewData["Title"]               = string.IsNullOrEmpty(shipperId) ? "Quản lý Hiệu suất Shipper" : $"Hiệu suất: {vm.SelectedShipperName}";
        ViewData["BreadcrumbParent"]    = "Shipper";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Dashboard", "Shipper");
        
        return View(vm);
    }

    // ─── GET /Shipper/MyPerformance ───────────────────────────
    [Authorize(Roles = "Shipper")]
    public async Task<IActionResult> MyPerformance()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var vm = await _shipperService.GetShipperPerformanceAsync(user.Id);
        
        ViewData["Title"]               = "Hiệu suất Cá nhân";
        ViewData["BreadcrumbParent"]    = "Shipper";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Dashboard", "Shipper");
        
        return View(vm);
    }
}
