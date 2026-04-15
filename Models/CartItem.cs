namespace BookStore.Models;

public class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int BookId { get; set; }

    public int Quantity { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
