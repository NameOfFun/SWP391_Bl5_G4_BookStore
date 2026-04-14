namespace BookStore.Models;

public class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int BookId { get; set; }

    public int Quantity { get; set; }

    /// <summary>Unit price at checkout (snapshot).</summary>
    public decimal UnitPrice { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
