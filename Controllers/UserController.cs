using BookStore.Dtos.Admin.User;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    private void SetUserViewData(string title)
    {
        ViewData["Title"] = title;
        ViewData["BreadcrumbParent"] = "Tài Khoản và Vai Trò";
        ViewData["BreadcrumbParentUrl"] = Url.Action("Index", "User");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            SetUserViewData("Quản lý người dùng");
            ViewBag.AllRolesForFilter = await _userService.GetAllRolesForFilterAsync();
            return View(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user list");
            TempData["Error"] = "Đã xảy ra lỗi khi tải danh sách người dùng.";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var dto = new CreateUserDto
        {
            AvailableRoles = await _userService.GetAvailableRolesAsync()
        };
        SetUserViewData("Thêm người dùng mới");
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            dto.AvailableRoles = await _userService.GetAvailableRolesAsync();
            SetUserViewData("Thêm người dùng mới");
            return View(dto);
        }

        try
        {
            await _userService.CreateUserAsync(dto);
            _logger.LogInformation("Admin created user {Email}", dto.Email);
            TempData["Success"] = $"Tạo tài khoản \"{dto.FullName}\" thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            dto.AvailableRoles = await _userService.GetAvailableRolesAsync();
            SetUserViewData("Thêm người dùng mới");
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var dto = await _userService.GetUserForEditAsync(id);
            SetUserViewData("Chỉnh sửa người dùng");
            return View(dto);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            dto.AvailableRoles = await _userService.GetAvailableRolesAsync();
            SetUserViewData("Chỉnh sửa người dùng");
            return View(dto);
        }

        try
        {
            await _userService.UpdateUserAsync(dto);
            _logger.LogInformation("Admin updated user {Id}", dto.Id);
            TempData["Success"] = $"Cập nhật tài khoản \"{dto.FullName}\" thành công.";
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
            dto.AvailableRoles = await _userService.GetAvailableRolesAsync();
            SetUserViewData("Chỉnh sửa người dùng");
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == currentUserId)
        {
            TempData["Error"] = "Không thể vô hiệu hóa tài khoản đang đăng nhập.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var (isNowActive, fullName) = await _userService.ToggleUserActiveAsync(id);
            TempData["Success"] = isNowActive
                ? $"Tài khoản \"{fullName}\" đã được kích hoạt."
                : $"Tài khoản \"{fullName}\" đã bị vô hiệu hóa.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
