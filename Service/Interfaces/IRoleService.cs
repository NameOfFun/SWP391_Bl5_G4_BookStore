using BookStore.Dtos.Admin.Role;

namespace BookStore.Service.Interfaces;

public interface IRoleService
{
    Task<IReadOnlyList<RoleListDto>> GetAllAsync();
    Task<UpdateRoleDto?> GetByIdAsync(string id);
    Task CreateAsync(CreateRoleDto dto);
    Task UpdateAsync(string id, UpdateRoleDto dto);
}
