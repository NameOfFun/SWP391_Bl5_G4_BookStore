using BookStore.Dtos.Common;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IVoucherService _voucherService;

    public OrderController(IOrderService orderService, IVoucherService voucherService)
    {
        _orderService = orderService;
        _voucherService = voucherService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private const int PageSize = 10;
    private const int MaxCancelReasonLength = 500;

    // ══════════════════════════════════════════════════════════
    //   CUSTOMER
    // ══════════════════════════════════════════════════════════

    // GET /Order/Checkout
    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Checkout()
    {
        var vm = await _orderService.GetCheckoutAsync(UserId);
        if (vm.Lines.Count == 0)
        {
            TempData["CartError"] = "Giỏ hàng trống — vui lòng thêm sách trước khi thanh toán.";
            return RedirectToAction("Index", "Cart");
        }

        ViewData["Title"] = "Thanh toán";
        ViewData["LibrariaInnerHeader"] = true;
        return View(vm);
    }

    // POST /Order/Checkout
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Checkout(CheckoutDto form)
    {
        var vm = await _orderService.GetCheckoutAsync(UserId);
        vm.Form = form;

        if (vm.Lines.Count == 0)
        {
            TempData["CartError"] = "Giỏ hàng trống — vui lòng thêm sách trước khi thanh toán.";
            return RedirectToAction("Index", "Cart");
        }

        // Restore discount preview for re-render on ModelState failure
        if (!string.IsNullOrWhiteSpace(form.VoucherCode))
        {
            var (vOk, vDiscount, _) = await _orderService.ApplyVoucherAsync(
                UserId, form.VoucherCode.Trim().ToUpperInvariant());
            if (vOk) vm.DiscountAmount = vDiscount;
        }

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Thanh toán";
            ViewData["LibrariaInnerHeader"] = true;
            return View(vm);
        }

        var (ok, err, orderId) = await _orderService.PlaceOrderAsync(UserId, form);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err ?? "Không thể đặt hàng.");
            ViewData["Title"] = "Thanh toán";
            ViewData["LibrariaInnerHeader"] = true;
            return View(vm);
        }

        TempData["Success"] = $"Đặt hàng thành công! Mã đơn: #{orderId}.";
        return RedirectToAction(nameof(Confirmation), new { id = orderId });
    }

    // POST /Order/ApplyVoucher
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ApplyVoucher([FromBody] ApplyVoucherRequest req)
    {
        if (string.IsNullOrWhiteSpace(req?.Code))
            return Json(new { ok = false, message = "Vui lòng nhập mã giảm giá." });

        var (ok, discountAmount, message) = await _orderService.ApplyVoucherAsync(
            UserId, req.Code.Trim().ToUpperInvariant());
        return Json(new { ok, discountAmount, message });
    }

    public class ApplyVoucherRequest
    {
        public string? Code { get; set; }
    }

    // GET /Order/Confirmation/{id}
    [HttpGet]
    public async Task<IActionResult> Confirmation(int id)
    {
        if (id <= 0) return NotFound();
        var detail = await _orderService.GetDetailAsync(id, UserId);
        if (detail == null) return NotFound();

        ViewData["Title"] = "Đặt hàng thành công";
        ViewData["LibrariaInnerHeader"] = true;
        return View(detail);
    }

    // GET /Order/MyOrders
    [HttpGet]
    public async Task<IActionResult> MyOrders()
    {
        var list = await _orderService.GetMyOrdersAsync(UserId);
        ViewData["Title"] = "Đơn hàng của tôi";
        ViewData["LibrariaInnerHeader"] = true;
        return View(list);
    }

    // GET /Order/MyDetail/{id}  (customer xem đơn của chính mình)
    [HttpGet]
    public async Task<IActionResult> MyDetail(int id)
    {
        if (id <= 0) return NotFound();
        var detail = await _orderService.GetDetailAsync(id, UserId);
        if (detail == null) return NotFound();

        ViewData["Title"] = $"Chi tiết đơn #{id}";
        ViewData["LibrariaInnerHeader"] = true;
        return View(detail);
    }

    // ══════════════════════════════════════════════════════════
    //   STAFF / MANAGER / ADMIN — MANAGEMENT
    // ══════════════════════════════════════════════════════════

    // GET /Order/Index?search=&status=&page=
    [HttpGet]
    [Authorize(Roles = "Staff,Manager,Admin")]
    public async Task<IActionResult> Index(string? search, string? status, int page = 1)
    {
        OrderStatus? statusEnum = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var s))
            statusEnum = s;

        page = Math.Max(1, page);
        var (items, totalCount) = await _orderService.GetManagementListAsync(search, statusEnum, page, PageSize);

        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        if (page > totalPages) page = totalPages;

        ViewData["Title"] = "Quản lý đơn hàng";
        ViewData["Search"] = search;
        ViewData["Status"] = status;
        ViewData["Page"] = page;
        ViewData["TotalPages"] = totalPages;
        ViewData["TotalCount"] = totalCount;
        ViewData["PageSize"] = PageSize;
        return View(items);
    }

    // GET /Order/Details/{id}
    [HttpGet]
    [Authorize(Roles = "Staff,Manager,Admin")]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0) return NotFound();
        var detail = await _orderService.GetDetailAsync(id);
        if (detail == null) return NotFound();

        ViewData["Title"] = $"Chi tiết đơn #{id}";
        ViewData["BreadcrumbParent"] = "Quản lý đơn hàng";
        ViewData["BreadcrumbParentUrl"] = Url.Action(nameof(Index));
        ViewData["Shippers"] = await _orderService.GetActiveShippersAsync();
        return View(detail);
    }

    // POST /Order/Confirm
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Staff,Manager,Admin")]
    public async Task<IActionResult> Confirm(int orderId)
    {
        if (orderId <= 0)
        {
            TempData["Error"] = "Mã đơn hàng không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        var (ok, msg) = await _orderService.ConfirmAsync(orderId);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    // POST /Order/MoveToProcessing
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Staff,Manager,Admin")]
    public async Task<IActionResult> MoveToProcessing(int orderId)
    {
        if (orderId <= 0)
        {
            TempData["Error"] = "Mã đơn hàng không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        var (ok, msg) = await _orderService.MoveToProcessingAsync(orderId);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    // POST /Order/AssignShipper
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Staff,Manager,Admin")]
    public async Task<IActionResult> AssignShipper(int orderId, string shipperUserId)
    {
        if (orderId <= 0)
        {
            TempData["Error"] = "Mã đơn hàng không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }
        if (string.IsNullOrWhiteSpace(shipperUserId))
        {
            TempData["Error"] = "Vui lòng chọn shipper.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        var (ok, msg) = await _orderService.AssignShipperAsync(orderId, shipperUserId);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    // POST /Order/Cancel
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Staff,Manager,Admin")]
    public async Task<IActionResult> Cancel(int orderId, string? reason)
    {
        if (orderId <= 0)
        {
            TempData["Error"] = "Mã đơn hàng không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        // Cắt bớt reason ngay từ controller để tránh payload vô lý
        // (service cũng có clamp nhưng chặn sớm giảm tải).
        if (!string.IsNullOrEmpty(reason) && reason.Length > MaxCancelReasonLength)
            reason = reason[..MaxCancelReasonLength];

        var (ok, msg) = await _orderService.CancelAsync(orderId, reason);
        TempData[ok ? "Success" : "Error"] = msg;
        return RedirectToAction(nameof(Details), new { id = orderId });
    }
}
