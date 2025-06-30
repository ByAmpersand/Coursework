using System;
using System.Collections.Generic;

namespace BookstoreBackend.Entities;

public partial class Language
{
    public int Id { get; set; }

    public string LanguageName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
