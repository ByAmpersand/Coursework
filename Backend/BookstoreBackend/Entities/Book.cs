namespace BookstoreBackend.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public string Language { get; set; }
        public int NumberOfPages { get; set; }
        public int PublicationYear { get; set; }
    }
}
