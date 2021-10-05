using Microsoft.EntityFrameworkCore;

namespace NineNineQuotes.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Quote> Quotes { get; set; }
    }
}
