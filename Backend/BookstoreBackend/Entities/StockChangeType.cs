using System;
using System.Collections.Generic;

namespace BookstoreBackend.Entities;

public partial class StockChangeType
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<BookStockLog> BookStockLogs { get; set; } = new List<BookStockLog>();
}
