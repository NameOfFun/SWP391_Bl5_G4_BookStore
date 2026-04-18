using BookStore.Dtos.Admin.User;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Service.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserListDto>> GetAllUsersAsync();
    Task<EditUserDto> GetUserForEditAsync(string id);
    /// <summary>Returns active non-Admin roles for the user assignment dropdown.</summary>
    Task<List<SelectListItem>> GetAvailableRolesAsync();
    /// <summary>Returns ALL roles (including deactivated) for the user list filter dropdown.</summary>
    Task<List<SelectListItem>> GetAllRolesForFilterAsync();
    Task CreateUserAsync(CreateUserDto dto);
    Task UpdateUserAsync(EditUserDto dto);
    Task<(bool IsNowActive, string FullName)> ToggleUserActiveAsync(string id);
}
