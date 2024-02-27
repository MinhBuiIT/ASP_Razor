using Microsoft.EntityFrameworkCore;

namespace ASP_RazorWeb.Models {
    public class BlogContext: DbContext {

        public DbSet<Article> articles { get; set; }
        public BlogContext(DbContextOptions options):base(options) {}

        protected internal void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
        }

        protected internal void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }
    }
}