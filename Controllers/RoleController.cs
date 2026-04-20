using BookStore.Dtos.Admin.Role;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers;

[Authorize(Roles = "Admin")]
public class RoleController : Controller
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RoleController> _logger;

    public RoleController(IRoleService roleService, ILogger<RoleController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var roles = await _roleService.GetAllAsync();
        ViewData["Title"] = "Quản lý vai trò";
        ViewData["BreadcrumbParent"] = "Quản lý vai trò";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "Role");
        return View(roles);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Thêm vai trò mới";
        ViewData["BreadcrumbParent"] = "Quản lý vai trò";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "Role");
        return View(new CreateRoleDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Thêm vai trò mới";
            ViewData["BreadcrumbParent"] = "Quản lý vai trò";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "Role");
            return View(dto);
        }

        try
        {
            await _roleService.CreateAsync(dto);
            _logger.LogInformation("Admin created role {Name}", dto.Name);
            TempData["Success"] = $"Tạo vai trò \"{dto.Name}\" thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewData["Title"] = "Thêm vai trò mới";
            ViewData["BreadcrumbParent"] = "Quản lý vai trò";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "Role");
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var dto = await _roleService.GetByIdAsync(id);
        if (dto == null) return NotFound();

        if (dto.Name == "Admin")
        {
            TempData["Error"] = "Không thể chỉnh sửa vai trò Admin.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = $"Chỉnh sửa vai trò: {dto.Name}";
        ViewData["BreadcrumbParent"] = "Quản lý vai trò";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "Role");
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        try
        {
            await _roleService.ToggleActiveAsync(id);
            TempData["Success"] = "Cập nhật trạng thái vai trò thành công.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UpdateRoleDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = $"Chỉnh sửa vai trò: {dto.Name}";
            ViewData["BreadcrumbParent"] = "Quản lý vai trò";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "Role");
            return View(dto);
        }

        try
        {
            await _roleService.UpdateAsync(id, dto);
            _logger.LogInformation("Admin updated role {Id}", id);
            TempData["Success"] = $"Cập nhật vai trò \"{dto.Name}\" thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewData["Title"] = $"Chỉnh sửa vai trò: {dto.Name}";
            ViewData["BreadcrumbParent"] = "Quản lý vai trò";
            ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "Role");
            return View(dto);
        }
    }
}
