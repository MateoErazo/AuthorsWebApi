using AuthorsWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new {ba.BookId, ba.AuthorId});
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<BookAuthor> BookAuthor { get; set; }

    }
}
