using System;
using System.Collections.Generic;

namespace BookStore.Models;

/// <summary>Payment record for an order (multiple rows allowed for retries / gateways).</summary>
public class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "VND";

    public PaymentChannel Channel { get; set; }

    public PaymentRecordStatus Status { get; set; }

    /// <summary>Gateway name, e.g. VnPay, Momo.</summary>
    public string? Provider { get; set; }

    public string? ProviderTransactionId { get; set; }

    public string? ProviderOrderRef { get; set; }

    public string? ClientIp { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public string? GatewayRawResponse { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<PaymentRefund> Refunds { get; set; } = new List<PaymentRefund>();
}
