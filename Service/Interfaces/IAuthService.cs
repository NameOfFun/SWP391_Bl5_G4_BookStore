using BookStore.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Service.Interfaces;

public interface IAuthService
{
    Task<LoginResultDto> LoginAsync(LoginDto model);
    Task<RegisterResultDto> RegisterAsync(RegisterDto model);
    Task LogoutAsync();
    Task<bool> IsEmailConfirmedAsync(string userId);
    Task<string> GenerateEmailConfirmationTokenAsync(string userId);
}
