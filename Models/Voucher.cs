using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

/// <summary>Discount codes — typically managed by Manager.</summary>
public class Voucher
{
    public int VoucherId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    /// <summary>P = percent, F = fixed amount off order.</summary>
    public string DiscountType { get; set; } = "P";

    public decimal DiscountValue { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public int? UsageLimit { get; set; }

    public int TimesUsed { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? LastManagedByUserId { get; set; }

    public virtual IdentityUser? LastManagedBy { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
