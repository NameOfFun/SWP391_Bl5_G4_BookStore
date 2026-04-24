namespace BookStore.Helpers;

/// <summary>Giá trị lưu trong Order.PaymentStatus — thống nhất với seed (Pending / Paid).</summary>
public static class OrderPaymentStatuses
{
    public const string Pending = "Pending";
    public const string Paid = "Paid";

    public static bool IsPaid(string? status) =>
        string.Equals(status, Paid, StringComparison.OrdinalIgnoreCase);

    /// <summary>Chưa ghi nhận thanh toán (gồm bản ghi cũ dùng "Unpaid").</summary>
    public static bool IsAwaitingPayment(string? status) =>
        string.IsNullOrWhiteSpace(status)
        || string.Equals(status, Pending, StringComparison.OrdinalIgnoreCase)
        || string.Equals(status, "Unpaid", StringComparison.OrdinalIgnoreCase);
}
