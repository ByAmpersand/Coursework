namespace BookstoreBackend.DTOs.Book;

public class BookDTO
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required List<string> Authors { get; set; }
    public string? PublisherName { get; set; }
    public int PublicationYear { get; set; }
    public required string Isbn { get; set; }
    public int NumberOfPages { get; set; }
    public string? LanguageName { get; set; }
    public required List<string> Genres { get; set; }
    public decimal Price { get; set; }
    public required string Description { get; set; }
    public required string Image { get; set; } // Renamed for consistency
    public DateOnly DateAdded { get; set; }
    public int StockQuantity { get; set; }

    // --- ADDED FIELDS ---
    public double? AverageRating { get; set; }
    public int RatingsCount { get; set; }
}