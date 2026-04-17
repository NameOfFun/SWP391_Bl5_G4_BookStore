using BookStore.Dtos.Auth;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BookStore.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AccountController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        IAuthService authService,
        ILogger<AccountController> logger,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment)
    {
        _authService = authService;
        _logger = logger;
        _userManager = userManager;
        _environment = environment;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.LoginAsync(model);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Đăng nhập thất bại.");
            return View(model);
        }

        _logger.LogInformation("User {Email} logged in successfully.", model.Email);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        // Redirect based on role
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Shipper"))
            {
                return RedirectToAction("Dashboard", "Shipper");
            }
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.RegisterAsync(model);

        if (!result.Succeeded)
        {
            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
            }
            return View(model);
        }

        _logger.LogInformation("User {Email} registered successfully.", model.Email);

        TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return RedirectToAction("Login", new { returnUrl });
        }

        return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }

    // PROFILE ACTIONS
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if(user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new ProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Name = user.Name,
            Address = user.Address,
            Avatar = user.Avatar
        };

        return View(model);
    }

    //Load Form Edit
    [HttpGet]
    [Authorize] 
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }
        var model = new EditProfileDto
        {
            Name = user.Name,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Address = user.Address
        };
        ViewBag.CurrentAvatar = user.Avatar;
        return View(model);
    }

    //Edit Profile
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileDto model)
    {
        var user = await _userManager.GetUserAsync(User);
        if(user == null)
        {
            return RedirectToAction("Login");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.CurrentAvatar = user.Avatar;
            return View(model);
        }

        user.Name = model.Name;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            ViewBag.CurrentAvatar = user.Avatar;
            return View(model);
        }

        _logger.LogInformation("User {Email} updated profile successfully.", user.Email);
        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
        return RedirectToAction("Profile");
    }

    //View ChangePassword
    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    //Change Password
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _userManager.GetUserAsync(User);
        if( user == null)
        {
            return RedirectToAction("Login");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        _logger.LogInformation("User {Email} changed password successfully.", user.Email);
        TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";

        return RedirectToAction("Profile");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    // Upload Avatar
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
    {
        if (avatarFile == null || avatarFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Vui lòng chọn file ảnh.";
            return RedirectToAction("EditProfile");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            TempData["ErrorMessage"] = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp).";
            return RedirectToAction("EditProfile");
        }

        if (avatarFile.Length > 5 * 1024 * 1024)
        {
            TempData["ErrorMessage"] = "File ảnh không được vượt quá 5MB.";
            return RedirectToAction("EditProfile");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "avatars");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{user.Id}_{DateTime.Now.Ticks}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatarFile.CopyToAsync(stream);
        }

        var avatarUrl = $"/images/avatars/{fileName}";
        var result = await _authService.UpdateAvatarAsync(user.Id, avatarUrl);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} updated avatar successfully.", user.Email);
            TempData["SuccessMessage"] = "Cập nhật ảnh đại diện thành công!";
        }
        else
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Cập nhật ảnh đại diện thất bại.";
        }

        return RedirectToAction("EditProfile");
    }
}
