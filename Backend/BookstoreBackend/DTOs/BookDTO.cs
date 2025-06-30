namespace BookstoreBackend.DTOs;

public class BookDTO
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required List<string> Authors { get; set; } // Імена авторів
    public string? PublisherName { get; set; } // Назва видавництва (може бути null, якщо Publisher не завантажено)
    public int PublicationYear { get; set; }
    public required string Isbn { get; set; }
    public int NumberOfPages { get; set; }
    public string? LanguageName { get; set; } // Назва мови (може бути null, якщо Language не завантажено)
    public required List<string> Genres { get; set; } // Назви жанрів
    public decimal Price { get; set; }
    public required string Description { get; set; }
    public string Image { get; set; } // URL або шлях до зображення
    public DateOnly DateAdded { get; set; } // Або string, якщо фронтенд очікує рядок
}