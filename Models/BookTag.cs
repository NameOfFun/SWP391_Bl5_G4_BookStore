using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

public partial class BookTag
{
    public int TagId { get; set; }

    public string? Name { get; set; }

    public string? UpdatedByUserId { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual ApplicationUser? UpdatedBy { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
