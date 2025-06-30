using System;
using System.Collections.Generic;

namespace BookstoreBackend.Entities;

public partial class Genre
{
    public int Id { get; set; }

    public string GenreName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
