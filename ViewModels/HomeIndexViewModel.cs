using BookStore.Dtos.Common;

namespace BookStore.ViewModels;

public class HomeIndexViewModel
{
    public IReadOnlyList<HomeSliderDto> Sliders { get; init; } = Array.Empty<HomeSliderDto>();
    public IReadOnlyList<BookDto> NewBooks { get; init; } = Array.Empty<BookDto>();

    /// <summary>Danh mục hoạt động cho form tìm kiếm trang chủ.</summary>
    public IReadOnlyList<HomeShopCategoryOption> ShopCategories { get; init; } = Array.Empty<HomeShopCategoryOption>();
}

public sealed record HomeShopCategoryOption(int CategoryId, string Name);
