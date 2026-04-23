using BookStore.Helpers;
using BookStore.Models;
using BookStore.Service.Interfaces;
using BookStore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements;

public class CartService : ICartService
{
    private readonly BookStoreDbContext _db;

    // Giới hạn số lượng tối đa mỗi dòng giỏ hàng để tránh overflow khi cộng dồn
    // và tránh tình huống bất hợp lý (khách mua 1 triệu cuốn trong một dòng).
    private const int MaxQuantityPerLine = 1000;

    public CartService(BookStoreDbContext db)
    {
        _db = db;
    }

    public async Task<int> GetTotalItemQuantityAsync(string userId)
    {
        var cart = await _db.Carts.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return 0;
        return await _db.CartItems.Where(i => i.CartId == cart.CartId).SumAsync(i => i.Quantity);
    }

    public async Task<CartIndexViewModel> GetCartAsync(string userId)
    {
        var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
            return new CartIndexViewModel();

        var items = await _db.CartItems
            .Include(i => i.Book)
            .Where(i => i.CartId == cart.CartId)
            .ToListAsync();

        var removed = false;
        var lines = new List<CartLineViewModel>();

        foreach (var item in items)
        {
            var book = item.Book;
            if (book == null)
            {
                _db.CartItems.Remove(item);
                removed = true;
                continue;
            }

            if (!book.IsActive)
            {
                _db.CartItems.Remove(item);
                removed = true;
                continue;
            }

            var stock = book.Stock ?? 0;
            if (stock < 1)
            {
                _db.CartItems.Remove(item);
                removed = true;
                continue;
            }

            if (item.Quantity > stock)
            {
                item.Quantity = stock;
                removed = true;
            }

            var unit = PricingHelper.GetEffectiveUnitPrice(book);
            lines.Add(new CartLineViewModel
            {
                CartItemId = item.CartItemId,
                BookId = book.BookId,
                Title = book.Title ?? "Sách",
                CoverUrl = BookCoverHelper.ResolveCoverPath(book.ImageUrl),
                UnitPrice = unit,
                ListPrice = PricingHelper.GetDisplayListPrice(book),
                Quantity = item.Quantity,
                Stock = stock,
                BookActive = book.IsActive
            });
        }

        if (removed)
            await _db.SaveChangesAsync();

        return new CartIndexViewModel { Lines = lines };
    }

    public async Task<(bool Ok, string? Error)> AddItemAsync(string userId, int bookId, int quantity)
    {
        if (bookId <= 0)
            return (false, "Mã sách không hợp lệ.");

        if (quantity < 1)
            return (false, "Số lượng phải ít nhất là 1.");

        if (quantity > MaxQuantityPerLine)
            return (false, $"Số lượng tối đa mỗi lần thêm là {MaxQuantityPerLine}.");

        var book = await _db.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
        if (book == null || !book.IsActive)
            return (false, "Sách không tồn tại hoặc đã ngừng bán.");

        var stock = book.Stock ?? 0;
        if (stock < 1)
            return (false, "Sách đã hết hàng.");

        var cart = await GetOrCreateCartTrackedAsync(userId);
        var existing = await _db.CartItems.FirstOrDefaultAsync(i => i.CartId == cart.CartId && i.BookId == bookId);
        var existingQty = existing?.Quantity ?? 0;

        // Cộng dồn bằng long để tránh tràn số khi quantity gần int.MaxValue.
        long newQtyLong = (long)existingQty + quantity;
        if (newQtyLong > MaxQuantityPerLine)
            return (false, $"Mỗi sản phẩm trong giỏ tối đa {MaxQuantityPerLine} cuốn.");
        if (newQtyLong > stock)
            return (false, $"Trong kho chỉ còn {stock} cuốn.");

        var newQty = (int)newQtyLong;

        if (existing != null)
            existing.Quantity = newQty;
        else
            _db.CartItems.Add(new CartItem { CartId = cart.CartId, BookId = bookId, Quantity = quantity });

        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Ok, string? Error)> SetQuantityAsync(string userId, int cartItemId, int quantity)
    {
        if (cartItemId <= 0)
            return (false, "Mã sản phẩm trong giỏ không hợp lệ.");

        var item = await _db.CartItems
            .Include(i => i.Cart)
            .Include(i => i.Book)
            .FirstOrDefaultAsync(i => i.CartItemId == cartItemId && i.Cart.UserId == userId);

        if (item?.Book == null)
            return (false, "Không tìm thấy sản phẩm trong giỏ.");

        if (quantity < 1)
            return await RemoveItemAsync(userId, cartItemId);

        if (quantity > MaxQuantityPerLine)
            return (false, $"Số lượng tối đa mỗi sản phẩm là {MaxQuantityPerLine}.");

        if (!item.Book.IsActive)
            return (false, "Sách đã ngừng bán — vui lòng xóa khỏi giỏ.");

        var stock = item.Book.Stock ?? 0;
        if (quantity > stock)
            return (false, $"Tối đa {stock} cuốn.");

        item.Quantity = quantity;
        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Ok, string? Error)> RemoveItemAsync(string userId, int cartItemId)
    {
        var item = await _db.CartItems
            .Include(i => i.Cart)
            .FirstOrDefaultAsync(i => i.CartItemId == cartItemId && i.Cart.UserId == userId);

        if (item == null)
            return (true, null);

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task ClearAllAsync(string userId)
    {
        var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
            return;

        var items = await _db.CartItems.Where(i => i.CartId == cart.CartId).ToListAsync();
        if (items.Count == 0)
            return;

        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync();
    }

    private async Task<Cart> GetOrCreateCartTrackedAsync(string userId)
    {
        var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart != null)
            return cart;

        cart = new Cart { UserId = userId };
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync();
        return cart;
    }
}
