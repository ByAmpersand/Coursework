using System;
using System.Collections.Generic;
using BookstoreBackend.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreBackend.Entities;

public partial class Wishlist
{
    public string UserId { get; set; } = null!;

    public int BookId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}
