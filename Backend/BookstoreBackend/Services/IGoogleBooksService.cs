using BookstoreBackend.DTOs.GoogleBooks;

namespace BookstoreBackend.Services
{
    public interface IGoogleBooksService
    {
        Task<List<GoogleBookItem>> SearchBooksAsync(string query, int maxResults = 20);
    }
}