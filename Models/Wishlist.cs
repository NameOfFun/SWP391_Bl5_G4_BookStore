using System;

namespace BookStore.Models;

public class Wishlist
{
    public string UserId { get; set; } = null!;

    public int BookId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Book Book { get; set; } = null!;
}
