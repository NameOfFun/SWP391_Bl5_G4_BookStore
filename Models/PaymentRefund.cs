using System;

namespace BookStore.Models;

/// <summary>One refund line; a payment may have several partial refunds.</summary>
public class PaymentRefund
{
    public int PaymentRefundId { get; set; }

    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public string? Reason { get; set; }

    public DateTime RefundedAt { get; set; }

    public string? ExternalRefundId { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
