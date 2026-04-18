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
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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
        var availableRoles = await GetAvailableRolesAsync();
        foreach (var item in availableRoles)
            item.Selected = currentRoles.Contains(item.Value);

        return new EditUserDto
        {
            Id = user.Id,
            FullName = user.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.Status,
            SelectedRoles = currentRoles.ToList(),
            AvailableRoles = availableRoles
        };
    }

    public async Task<List<SelectListItem>> GetAvailableRolesAsync()
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

        if (dto.SelectedRoles == null || dto.SelectedRoles.Count == 0)
            throw new ArgumentException("Phải chọn ít nhất một vai trò.");

        foreach (var role in dto.SelectedRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                throw new ArgumentException($"Vai trò '{role}' không tồn tại.");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Name = dto.FullName,
            Status = true,
            LockoutEnabled = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new ArgumentException(errors);
        }

        await _userManager.AddToRolesAsync(user, dto.SelectedRoles);
    }

    public async Task UpdateUserAsync(EditUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

        if (dto.SelectedRoles == null || dto.SelectedRoles.Count == 0)
            throw new ArgumentException("Phải chọn ít nhất một vai trò.");

        foreach (var role in dto.SelectedRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                throw new ArgumentException($"Vai trò '{role}' không tồn tại.");
        }

        // Check email uniqueness if changed
        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null && existing.Id != user.Id)
                throw new ArgumentException("Email này đã được sử dụng bởi tài khoản khác.");
        }

        user.Name = dto.FullName;
        user.Email = dto.Email;
        user.NormalizedEmail = dto.Email.ToUpperInvariant();
        user.UserName = dto.Email;
        user.NormalizedUserName = dto.Email.ToUpperInvariant();
        user.PhoneNumber = dto.PhoneNumber;
        user.Status = dto.IsActive;

        if (!dto.IsActive)
        {
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
        }
        else
        {
            user.LockoutEnd = null;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
            throw new ArgumentException(errors);
        }

        // Sync roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        var toRemove = currentRoles.Except(dto.SelectedRoles).ToList();
        var toAdd = dto.SelectedRoles.Except(currentRoles).ToList();

        if (toRemove.Count > 0) await _userManager.RemoveFromRolesAsync(user, toRemove);
        if (toAdd.Count > 0) await _userManager.AddToRolesAsync(user, toAdd);
    }

    public async Task<(bool IsNowActive, string FullName)> ToggleUserActiveAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new InvalidOperationException("Không tìm thấy người dùng.");

        if (user.Status)
        {
            user.Status = false;
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
        }
        else
        {
            user.Status = true;
            user.LockoutEnd = null;
        }

        await _userManager.UpdateAsync(user);
        return (user.Status, user.Name ?? "");
    }
}
