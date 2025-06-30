using System;
using System.Collections.Generic;

namespace BookstoreBackend.Entities;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int PublisherId { get; set; }

    public string Isbn { get; set; } = null!;

    public decimal Price { get; set; }

    public int LanguageId { get; set; }

    public int NumberOfPages { get; set; }

    public int PublicationYear { get; set; }

    public string Description { get; set; } = null!;

    public string Image { get; set; } = null!;

    public DateOnly DateAdded { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual Language Language { get; set; } = null!;

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
