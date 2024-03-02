using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_RazorWeb.Models {
    public class BlogContext: IdentityDbContext<AppUser> {

        public DbSet<Article> articles { get; set; }
        public BlogContext(DbContextOptions options):base(options) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            //Configure tên bảng trong Identity
                
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                string? nameTable = entityType.GetTableName();
                if(nameTable.Contains("AspNet")) {
                    entityType.SetTableName(nameTable.Substring(6));
                }
            }
        }
    }
}