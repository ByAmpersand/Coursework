using BookstoreBackend.DTOs.GoogleBooks;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace BookstoreBackend.Services
{
    public class GoogleBooksService : IGoogleBooksService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        public GoogleBooksService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["GoogleBooks:ApiKey"]
                ?? throw new InvalidOperationException("Google Books API key is not configured.");
        }

        public async Task<List<GoogleBookItem>> SearchBooksAsync(string query, int maxResults = 20)
        {
            var searchQuery = query;

            // Проста перевірка, чи запит схожий на ISBN (складається з 10 або 13 цифр, можливо з дефісами)
            var isbnPattern = new Regex("^[0-9-]{10,17}$");
            if (isbnPattern.IsMatch(query))
            {
                searchQuery = $"isbn:{query.Replace("-", "")}";
            }

            var client = _httpClientFactory.CreateClient("GoogleBooks");
            var url = $"volumes?q={searchQuery}&maxResults={maxResults}&key={_apiKey}";

            try
            {
                var response = await client.GetFromJsonAsync<GoogleApiSearchResult>(url);
                return response?.Items ?? new List<GoogleBookItem>();
            }
            catch (Exception ex)
            {
                // У реальному проекті тут варто логувати помилку
                return new List<GoogleBookItem>();
            }
        }
    }
}