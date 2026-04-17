using System;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

public class BookRating
{
    public int BookRatingId { get; set; }

    public string UserId { get; set; } = null!;

    public int BookId { get; set; }

    public byte Stars { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsHidden { get; set; }

    public DateTime? HiddenAt { get; set; }

    public string? HiddenByUserId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ApplicationUser? HiddenBy { get; set; }
}
