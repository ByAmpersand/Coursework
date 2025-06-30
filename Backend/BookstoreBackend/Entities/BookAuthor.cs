using System;
using System.Collections.Generic;

namespace BookstoreBackend.Entities;

public partial class BookAuthor
{
    public int BookId { get; set; }

    public int AuthorId { get; set; }

    public int OrdinalNumber { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
