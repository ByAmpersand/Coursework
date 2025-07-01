using System;
using System.Collections.Generic;

namespace BookstoreBackend.Entities;

public partial class OrderBook
{
    public int OrderId { get; set; }

    public int BookId { get; set; }

    public int BookQuantity { get; set; }

    public decimal BookPrice { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
