using System;
using System.Collections.Generic;
using BookstoreBackend.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreBackend.Entities;

public partial class BookStockLog
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public string UserId { get; set; } = null!;

    public int ChangeTypeId { get; set; }

    public int Quantity { get; set; }

    public DateTime ChangeDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual StockChangeType ChangeType { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}

