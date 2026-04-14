using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string? Name { get; set; }

    public string? Bio { get; set; }

    public string? UpdatedByUserId { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual IdentityUser? UpdatedBy { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
