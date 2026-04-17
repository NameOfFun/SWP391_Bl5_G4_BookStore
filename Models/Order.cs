using System;
using System.Collections.Generic;

namespace BookStore.Models;

public class Order
{
    public int OrderId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public OrderStatus Status { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public int? VoucherId { get; set; }

    public string? ShippingName { get; set; }

    public string? ShippingPhone { get; set; }

    public string? ShippingAddress { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual Voucher? Voucher { get; set; }

    public virtual ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<CustomerRequest> CustomerRequests { get; set; } = new List<CustomerRequest>();
}
