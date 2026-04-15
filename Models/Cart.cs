using System.Collections.Generic;

namespace BookStore.Models;

public class Cart
{
    public int CartId { get; set; }

    public string UserId { get; set; } = null!;

    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
