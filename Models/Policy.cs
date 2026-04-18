using System;

namespace BookStore.Models;

public class Policy
{
    public int PolicyId { get; set; }

    public string Title { get; set; } = null!;

    public string ContentHtml { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string? UpdatedByUserId { get; set; }

    public string? RoleId { get; set; }

    public virtual ApplicationUser? UpdatedBy { get; set; }

    public virtual ApplicationRole? Role { get; set; }
}
