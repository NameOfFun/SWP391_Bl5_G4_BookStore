using BookStore.Dtos.Admin.User;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyList<UserListDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.OrderBy(u => u.Name).ToListAsync();
        var result = new List<UserListDto>(users.Count);
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserListDto
            {
                Id = user.Id,
                Name = user.Name ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.Status,
                IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow,
                Roles = roles
            });
        }
        return result;
    }

    public async Task<EditUserDto> GetUserForEditAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Contains("Admin"))
            throw new InvalidOperationException("Không thể chỉnh sửa tài khoản Admin.");

        var availableRoles = await GetAvailableRolesAsync();

        return new EditUserDto
        {
            Id = user.Id,
            FullName = user.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.Status,
            SelectedRole = currentRoles.FirstOrDefault() ?? string.Empty,
            AvailableRoles = availableRoles
        };
    }

    public async Task<List<SelectListItem>> GetAvailableRolesAsync()
    {
        // Admin is a system-only role; prevent it from appearing in assignment dropdowns
        return await _roleManager.Roles
            .Where(r => r.Name != "Admin" && r.Status)
            .OrderBy(r => r.Name)
            .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            .ToListAsync();
    }

    public async Task<List<SelectListItem>> GetAllRolesForFilterAsync()
    {
        return await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
            .ToListAsync();
    }

    public async Task CreateUserAsync(CreateUserDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            throw new ArgumentException("Email này đã được sử dụng.");

        var role = (dto.SelectedRole ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(role))
            throw new ArgumentException("Vui lòng chọn vai trò.");

        if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Không thể gán vai trò Admin.");

        if (!await _roleManager.RoleExistsAsync(role))
            throw new ArgumentException($"Vai trò '{role}' không tồn tại.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Name = dto.FullName,
            Status = true,
            LockoutEnabled = true  // Must be set explicitly; Identity defaults vary by configuration
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new ArgumentException(errors);
        }

        await _userManager.AddToRoleAsync(user, role);
    }

    public async Task UpdateUserAsync(EditUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Contains("Admin"))
            throw new InvalidOperationException("Không thể chỉnh sửa tài khoản Admin.");

        var newRole = (dto.SelectedRole ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(newRole))
            throw new ArgumentException("Vui lòng chọn vai trò.");

        if (newRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Không thể gán vai trò Admin.");

        if (!await _roleManager.RoleExistsAsync(newRole))
            throw new ArgumentException($"Vai trò '{newRole}' không tồn tại.");

        // Check email uniqueness if changed
        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null && existing.Id != user.Id)
                throw new ArgumentException("Email này đã được sử dụng bởi tài khoản khác.");
        }

        user.Name = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Status = dto.IsActive;

        if (dto.IsActive && user.LockoutEnd == DateTimeOffset.MaxValue)
            user.LockoutEnd = null;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
            throw new ArgumentException(errors);
        }

        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailResult = await _userManager.SetEmailAsync(user, dto.Email);
            if (!emailResult.Succeeded)
                throw new ArgumentException(string.Join(" ", emailResult.Errors.Select(e => e.Description)));

            var userNameResult = await _userManager.SetUserNameAsync(user, dto.Email);
            if (!userNameResult.Succeeded)
                throw new ArgumentException(string.Join(" ", userNameResult.Errors.Select(e => e.Description)));
        }

        // Each user holds exactly one role; remove all others before assigning the new one
        var toRemove = currentRoles.Except([newRole]).ToList();
        if (toRemove.Count > 0) await _userManager.RemoveFromRolesAsync(user, toRemove);
        if (!currentRoles.Contains(newRole)) await _userManager.AddToRoleAsync(user, newRole);

        // Optional password reset
        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            await _userManager.RemovePasswordAsync(user);
            var pwResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!pwResult.Succeeded)
            {
                var errors = string.Join(" ", pwResult.Errors.Select(e => e.Description));
                throw new ArgumentException(errors);
            }
        }
    }

    public async Task<(bool IsNowActive, string FullName)> ToggleUserActiveAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
            throw new InvalidOperationException("Không thể thay đổi trạng thái tài khoản Admin.");

        user.Status = !user.Status;
        if (user.Status && user.LockoutEnd == DateTimeOffset.MaxValue)
            user.LockoutEnd = null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new ArgumentException(errors);
        }

        return (user.Status, user.Name ?? "");
    }
}
