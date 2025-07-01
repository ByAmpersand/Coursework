using System;
using System.Collections.Generic;
using BookstoreBackend.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreBackend.Entities;

public partial class Order
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public decimal TotalAmount { get; set; }

    public string UserId { get; set; } = null!;

    public int StatusId { get; set; }

    public string ShippingCountry { get; set; } = null!;

    public string ShippingRegion { get; set; } = null!;

    public string ShippingCity { get; set; } = null!;

    public string ShippingAddressLine1 { get; set; } = null!;

    public string? ShippingAddressLine2 { get; set; }

    public string ShippingPostalCode { get; set; } = null!;

    public string RecipientName { get; set; } = null!;

    public string RecipientPhoneNumber { get; set; } = null!;

    public virtual ICollection<OrderBook> OrderBooks { get; set; } = new List<OrderBook>();

    [ForeignKey("StatusId")]
    public virtual OrderStatus OrderStatus { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}