using BookStore.Dtos.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Service.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserListDto>> GetAllUsersAsync();
    Task<EditUserDto> GetUserForEditAsync(string id);
    Task<List<SelectListItem>> GetAvailableRolesAsync();
    Task CreateUserAsync(CreateUserDto dto);
    Task UpdateUserAsync(EditUserDto dto);
    Task<(bool IsNowActive, string FullName)> ToggleUserActiveAsync(string id);
}
