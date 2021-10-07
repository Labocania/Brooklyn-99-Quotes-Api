using Microsoft.EntityFrameworkCore;

namespace NineNineQuotes.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Quote> Quotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quote>()
                .HasIndex(b => new { b.Episode, b.Character, b.QuoteText })
                .IsTsVectorExpressionIndex("english");
        }
    }
}
