using BookStore.Dtos.Admin.Role;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<RoleListDto>> GetAllAsync()
    {
        var roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        var result = new List<RoleListDto>(roles.Count);

        foreach (var role in roles)
        {
            var users = await _userManager.GetUsersInRoleAsync(role.Name!);
            result.Add(new RoleListDto
            {
                Id = role.Id,
                Name = role.Name!,
                Description = role.Description,
                IsActive = role.Status,
                IsSystemRole = role.IsSystemRole,
                UserCount = users.Count
            });
        }

        return result;
    }

    public async Task<UpdateRoleDto?> GetByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return null;

        return new UpdateRoleDto
        {
            Id = role.Id,
            Name = role.Name!,
            Description = role.Description,
            IsActive = role.Status
        };
    }

    public async Task CreateAsync(CreateRoleDto dto)
    {
        var name = (dto.Name ?? string.Empty).Trim();

        var existing = await _roleManager.FindByNameAsync(name);
        if (existing != null)
            throw new ArgumentException("Tên vai trò này đã tồn tại.");

        var role = new ApplicationRole
        {
            Name = name,
            Description = dto.Description?.Trim(),
            IsSystemRole = false,
            Status = true,
            CreatedDate = DateTime.Now
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new ArgumentException(errors);
        }
    }

    public async Task ToggleActiveAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id)
            ?? throw new InvalidOperationException("Không tìm thấy vai trò.");

        if (role.Name == "Admin")
            throw new InvalidOperationException("Không thể thay đổi trạng thái vai trò Admin.");

        role.Status = !role.Status;
        role.UpdatedAt = DateTime.Now;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new ArgumentException(errors);
        }
    }

    public async Task UpdateAsync(string id, UpdateRoleDto dto)
    {
        var role = await _roleManager.FindByIdAsync(id)
            ?? throw new InvalidOperationException("Không tìm thấy vai trò.");

        if (role.Name == "Admin")
            throw new InvalidOperationException("Không thể chỉnh sửa vai trò Admin.");

        role.Description = dto.Description?.Trim();
        role.Status = dto.IsActive;
        role.UpdatedAt = DateTime.Now;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new ArgumentException(errors);
        }
    }
}
