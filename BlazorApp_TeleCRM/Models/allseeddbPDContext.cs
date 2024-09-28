using Microsoft.EntityFrameworkCore;

namespace BlazorApp_TeleCRM.Models
{
    public class allseeddbPDContext : DbContext
    {
        public allseeddbPDContext(DbContextOptions<allseeddbPDContext> options) : base(options)
        {
        }

        // DbSet for Users table
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Configure the mapping between your entity and table
            modelBuilder.Entity<User>()
                .ToTable("mas_users")
                .HasKey(u => u.id);
            base.OnModelCreating(modelBuilder);
        }
    }
}