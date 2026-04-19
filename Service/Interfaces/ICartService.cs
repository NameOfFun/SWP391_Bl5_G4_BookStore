using BookStore.ViewModels;

namespace BookStore.Service.Interfaces;

public interface ICartService
{
    Task<int> GetTotalItemQuantityAsync(string userId);

    Task<CartIndexViewModel> GetCartAsync(string userId);

    Task<(bool Ok, string? Error)> AddItemAsync(string userId, int bookId, int quantity);

    Task<(bool Ok, string? Error)> SetQuantityAsync(string userId, int cartItemId, int quantity);

    Task<(bool Ok, string? Error)> RemoveItemAsync(string userId, int cartItemId);

    Task ClearAllAsync(string userId);
}
