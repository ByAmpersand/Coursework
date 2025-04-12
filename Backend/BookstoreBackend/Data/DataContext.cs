using BookstoreBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookstoreBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Book> Books { get; set; }
    }
}
