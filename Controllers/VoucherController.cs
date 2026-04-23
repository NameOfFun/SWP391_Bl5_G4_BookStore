using BookStore.Dtos.Admin.Voucher;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class VoucherController : Controller
{
    private const int PageSize = 10;
    private readonly IVoucherService _voucherService;

    public VoucherController(IVoucherService voucherService)
    {
        _voucherService = voucherService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? status, string? validity, int page = 1)
    {
        ViewData["Search"] = search;
        ViewData["Status"] = status;
        ViewData["Validity"] = validity;

        var (items, totalCount) = await _voucherService.GetPagedAsync(search, status, validity, page, PageSize);

        int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        page = Math.Clamp(page, 1, Math.Max(1, totalPages));

        ViewData["Page"] = page;
        ViewData["TotalPages"] = totalPages;
        ViewData["TotalCount"] = totalCount;
        ViewData["PageSize"] = PageSize;
        ViewData["Title"] = "Quản lý Voucher";

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _voucherService.GetDetailsAsync(id);
        if (dto == null) return NotFound();
        ViewData["Title"] = $"Chi tiết Voucher – {dto.Code}";
        ViewData["BreadcrumbParent"] = "Quản lý Voucher";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "Voucher");
        return View(dto);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateVoucherDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVoucherDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _voucherService.CreateAsync(dto, userId);
            TempData["Success"] = "Thêm voucher thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _voucherService.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateVoucherDto dto)
    {
        if (!ModelState.IsValid)
        {
            var current = await _voucherService.GetForEditAsync(id);
            if (current != null)
            {
                dto.Code = current.Code;
                dto.TimesUsed = current.TimesUsed;
            }
            return View(dto);
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _voucherService.UpdateAsync(id, dto, userId);
            TempData["Success"] = "Cập nhật voucher thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var current = await _voucherService.GetForEditAsync(id);
            if (current != null)
            {
                dto.Code = current.Code;
                dto.TimesUsed = current.TimesUsed;
            }
            return View(dto);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _voucherService.ToggleStatusAsync(id, userId);
            TempData["Success"] = "Đã cập nhật trạng thái voucher.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
