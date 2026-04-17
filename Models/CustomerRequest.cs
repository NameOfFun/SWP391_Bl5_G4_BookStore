using System;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

/// <summary>Support / return ticket. Customer creates; Manager resolves (accept/reject).</summary>
public class CustomerRequest
{
    public int CustomerRequestId { get; set; }

    public string? UserId { get; set; }

    public CustomerRequestType RequestType { get; set; }

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    /// <summary>Optional contact on form if different from profile.</summary>
    public string? ContactPhone { get; set; }

    /// <summary>Related order for return/refund flows.</summary>
    public int? OrderId { get; set; }

    public string Status { get; set; } = "Open";

    public string? AssignedToUserId { get; set; }

    public string? ResolutionNotes { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? ResolvedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ApplicationUser? ResolvedBy { get; set; }
}
