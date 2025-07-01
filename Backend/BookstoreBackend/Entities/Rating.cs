using System;
using System.Collections.Generic;
using BookstoreBackend.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreBackend.Entities;

public partial class Rating
{
    public int BookId { get; set; }

    public decimal RatingStars { get; set; }

    public string? Review { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
}