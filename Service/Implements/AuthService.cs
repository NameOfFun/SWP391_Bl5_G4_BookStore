using BookStore.Dtos.Auth;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Service.Implements;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return new LoginResultDto
            {
                Succeeded = false,
                ErrorMessage = "Email hoặc mật khẩu không đúng."
            };
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return new LoginResultDto
            {
                Succeeded = false,
                ErrorMessage = "Tài khoản đã bị khóa do đăng nhập sai nhiều lần. Vui lòng thử lại sau."
            };
        }

        if (result.IsNotAllowed)
        {
            return new LoginResultDto
            {
                Succeeded = false,
                ErrorMessage = "Tài khoản chưa được xác thực email."
            };
        }

        if (!result.Succeeded)
        {
            return new LoginResultDto
            {
                Succeeded = false,
                ErrorMessage = "Email hoặc mật khẩu không đúng."
            };
        }

        return new LoginResultDto
        {
            Succeeded = true,
            ReturnUrl = model.ReturnUrl ?? "/"
        };
    }

    public async Task<RegisterResultDto> RegisterAsync(RegisterDto model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return new RegisterResultDto
            {
                Succeeded = false,
                ErrorMessage = "Email này đã được đăng ký."
            };
        }

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new RegisterResultDto
            {
                Succeeded = false,
                Errors = errors,
                ErrorMessage = "Đăng ký thất bại. Vui lòng kiểm tra lại thông tin."
            };
        }

        // Gán role mặc định là Customer
        await _userManager.AddToRoleAsync(user, "Customer");

      
        

        return new RegisterResultDto
        {
            Succeeded = true,
            ReturnUrl = model.ReturnUrl ?? "/"
        };
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> IsEmailConfirmedAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("Không tìm thấy người dùng", nameof(userId));
        }
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }


    
}
