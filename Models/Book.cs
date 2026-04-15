using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    /// <summary>List price; effective sale = PromotionalPrice when within date range, else Price.</summary>
    public decimal? PromotionalPrice { get; set; }

    public DateTime? PromotionalStartsAt { get; set; }

    public DateTime? PromotionalEndsAt { get; set; }

    public string? PricingNote { get; set; }

    public DateTime? LastPricingChangeAt { get; set; }

    public string? LastPricingChangeByUserId { get; set; }

    public virtual IdentityUser? LastPricingChangeBy { get; set; }

    public int? Stock { get; set; }

    public int? CategoryId { get; set; }

    public int? AuthorId { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? UpdatedByUserId { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual IdentityUser? UpdatedBy { get; set; }

    public virtual Author? Author { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<BookTag> Tags { get; set; } = new List<BookTag>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public virtual ICollection<BookRating> BookRatings { get; set; } = new List<BookRating>();
}
