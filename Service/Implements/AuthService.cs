using BookStore.Dtos.Auth;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Service.Implements;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return new LoginResultDto { Succeeded = false, ErrorMessage = "Email hoặc mật khẩu không đúng." };
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return new LoginResultDto { Succeeded = false, ErrorMessage = "Tài khoản đã bị khóa do đăng nhập sai nhiều lần..." };
        }
        if (result.IsNotAllowed)
        {
            return new LoginResultDto { Succeeded = false, ErrorMessage = "Tài khoản chưa được xác thực email." };
        }
        if (!result.Succeeded)
        {
            return new LoginResultDto { Succeeded = false, ErrorMessage = "Email hoặc mật khẩu không đúng." };
        }

        // Status is checked after sign-in so the lockout counter still increments on bad credentials
        if (!user.Status)
        {
            await _signInManager.SignOutAsync();
            return new LoginResultDto { Succeeded = false, ErrorMessage = "Tài khoản đã bị vô hiệu hóa." };
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles.Count > 0)
        {
            var appRole = await _roleManager.FindByNameAsync(userRoles[0]) as ApplicationRole;
            if (appRole != null && !appRole.Status)
            {
                await _signInManager.SignOutAsync();
                return new LoginResultDto { Succeeded = false, ErrorMessage = "Vai trò tài khoản đã bị vô hiệu hóa." };
            }
        }

        return new LoginResultDto
        {
            Succeeded = true,
            ReturnUrl = model.ReturnUrl ?? "/",
            Role = userRoles.FirstOrDefault()
        };
    }

    public async Task<RegisterResultDto> RegisterAsync(RegisterDto model)
    {
        //Check user exits
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if(existingUser != null)
        {
            return new RegisterResultDto { Succeeded = false, ErrorMessage = "Email này đã được đăng ký. " };
        }

        //Tao ApplicationUser moi
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Name = model.FullName,
            Address = model.Address,
            Status = true
        };

        //Tao User + HashPassWord

        var result = await _userManager.CreateAsync(user, model.Password);

        //Return Error

        if(!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new RegisterResultDto { Succeeded = false, Errors = errors, ErrorMessage = "Đăng ký thất bại..." };
        }

        //Gán role mặc định 

        await _userManager.AddToRoleAsync(user, "Customer");

        return new RegisterResultDto
        {
            Succeeded = true,
            ReturnUrl = model.ReturnUrl ?? "/"
        };
    }

    //Logout 
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


    //
    public async Task<ProfileDto?> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) 
        {
            return null;
        }

        return new ProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Name = user.Name,
            Address = user.Address,
            Avatar = user.Avatar
        };
    }
    
    //Update Profile

    public async Task<ProfileResultDto> UpdateProfileAsync(string userId, EditProfileDto model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ProfileResultDto { Succeeded = false, ErrorMessage = "Không tìm thấy người dùng. " };
        }
        user.Name = model.Name;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;
        var result = await _userManager.UpdateAsync(user);

        if(!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new ProfileResultDto { Succeeded=false, Errors = errors };
        }

        return new ProfileResultDto { Succeeded = true };
    }

    public async Task<ProfileResultDto> UpdateAvatarAsync(string userId, string avatarUrl)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ProfileResultDto { Succeeded = false, ErrorMessage = "Không tìm thấy người dùng." };
        }
        user.Avatar = avatarUrl;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new ProfileResultDto { Succeeded = false, Errors = errors };
        }
        return new ProfileResultDto { Succeeded = true };
    }

    public async Task<ProfileResultDto> ChangePasswordAsync(string userId, ChangePasswordDto model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if(user == null)
        {
            return new ProfileResultDto {Succeeded = false, ErrorMessage = "Không tìm thấy người dùng." };
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if(!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new ProfileResultDto { Succeeded = false, Errors = errors };
        }
        return new ProfileResultDto { Succeeded = true };
    }
}
