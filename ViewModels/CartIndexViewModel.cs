namespace BookStore.ViewModels;

public class CartLineViewModel
{
    public int CartItemId { get; set; }
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public string CoverUrl { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public decimal? ListPrice { get; set; }
    public int Quantity { get; set; }
    public int Stock { get; set; }
    public bool BookActive { get; set; } = true;

    public decimal LineTotal => UnitPrice * Quantity;
}

public class CartIndexViewModel
{
    public IReadOnlyList<CartLineViewModel> Lines { get; set; } = Array.Empty<CartLineViewModel>();

    public decimal Subtotal => Lines.Sum(l => l.LineTotal);

    public int TotalQuantity => Lines.Sum(l => l.Quantity);
}
