using BookStore.Dtos.Auth;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Service.Implements;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _environment;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IEmailService emailService,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _environment = environment;
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




    //Profile Action
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

    public async Task<AvatarUploadResultDto> UploadAvatarAsync(string userId, Stream fileStream, string fileName, long fileSize)
    {
        // Validate file size (max 5MB)
        const long maxFileSize = 5 * 1024 * 1024;
        if (fileSize > maxFileSize)
        {
            return new AvatarUploadResultDto
            {
                Succeeded = false,
                ErrorMessage = "File ảnh không được vượt quá 5MB."
            };
        }

        // Validate file extension
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return new AvatarUploadResultDto
            {
                Succeeded = false,
                ErrorMessage = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp)."
            };
        }

        // Find user
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new AvatarUploadResultDto
            {
                Succeeded = false,
                ErrorMessage = "Không tìm thấy người dùng."
            };
        }

        // Create uploads folder if not exists
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "avatars");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Generate unique filename
        var newFileName = $"{user.Id}_{DateTime.Now.Ticks}{extension}";
        var filePath = Path.Combine(uploadsFolder, newFileName);

        // Save file
        using (var targetStream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(targetStream);
        }

        var avatarUrl = $"/images/avatars/{newFileName}";

        // Update user's avatar in database
        user.Avatar = avatarUrl;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            // Delete the uploaded file if database update fails
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new AvatarUploadResultDto { Succeeded = false, Errors = errors };
        }

        return new AvatarUploadResultDto
        {
            Succeeded = true,
            AvatarUrl = avatarUrl
        };
    }

    public async Task<ProfileResultDto> ChangePasswordAsync(string userId, ChangePasswordDto model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if(user == null)
        {
            return new ProfileResultDto {Succeeded = false, ErrorMessage = "Không tìm thấy người dùng." };
        }
        //Validate pass moi k trung pass cu
        var isSamePassword = await _userManager.CheckPasswordAsync(user, model.NewPassword);
        if(isSamePassword)
        {
            return new ProfileResultDto
            {
                Succeeded = false,
                ErrorMessage = "Mật khẩu mới không được trùng với mật khẩu hiện tại"
            };
        }

        //Change password
        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if(!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new ProfileResultDto { Succeeded = false, Errors = errors };
        }
        return new ProfileResultDto { Succeeded = true };
    }

    public async Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if(user == null)
        {
            return new ForgotPasswordResultDto
            {
                Succeeded = true,
            };
        }

        //Tao token 

        var token = await _userManager.GeneratePasswordResetTokenAsync(user); //Gui email + link reset

        // Tạo URL reset 
        var resetLink = $"https://localhost:7159/Account/ResetPassword?email={Uri.EscapeDataString(user.Email ?? "")}&token={Uri.EscapeDataString(token)}";

        // Gửi email
        await _emailService.SendPasswordResetEmailAsync(user.Email!, resetLink);

        return new ForgotPasswordResultDto
        {
            Succeeded = true
        };

    }

    public async Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if( user == null )
        {
            return new ResetPasswordResultDto
            {
                Succeeded = false,
                ErrorMessage = "Email không tồn tại"
            };
        }

        var isSamePassword = await _userManager.CheckPasswordAsync(user,model.NewPassword);
        if (isSamePassword)
        {
            return new ResetPasswordResultDto
            {
                Succeeded = false,
                ErrorMessage = "Mật khẩu bạn nhập đã được sử dụng gần đây. Hãy nhập mật khẩu mới!"
            };
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new ResetPasswordResultDto
            {
                Succeeded = false,
                Errors = errors
            };
        }
        return new ResetPasswordResultDto { Succeeded = true };
    }
}
