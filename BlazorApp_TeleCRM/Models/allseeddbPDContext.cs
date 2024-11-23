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
        public DbSet<MasBranches> MasBranches { get; set; }
        public DbSet<MasCustomers>MasCustomers { get; set; }
        public DbSet<CrmActivitys> CrmActivitys { get; set; }
        public DbSet<CrmContactLog> CrmContactLog { get; set; }
        public DbSet<CrmNote> CrmNote { get; set; }
        public DbSet<MasDocumentControl> MasDocumentControl { get; set; }
        public DbSet<CrmOrder> CrmOrder { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Configure the mapping between your entity and table
            modelBuilder.Entity<User>()
                .ToTable("mas_users")
                .HasKey(u => u.id);
          
            modelBuilder.Entity<MasBranches>()
               .ToTable("mas_branches")
               .HasKey(u => u.code);
         
            modelBuilder.Entity<MasCustomers>()
               .ToTable("mas_customers")
               .HasKey(u => u.guid);

            modelBuilder.Entity<CrmActivitys>()
             .ToTable("crm_activitys")
             .HasKey(u => u.guid);

            modelBuilder.Entity<CrmContactLog>()
            .ToTable("crm_contact_logs")
            .HasKey(u => u.log_id);

            modelBuilder.Entity<CrmNote>()
           .ToTable("crm_notes")
           .HasKey(u => u.guid);

            modelBuilder.Entity<CrmOrder>()
            .ToTable("crm_orders")
            .HasKey(u => u.guid);

            modelBuilder.Entity<MasDocumentControl>()
         .ToTable("mas_document_controls")
         .HasKey(e => new { e.document_type, e.branch_code, e.company_code });


            base.OnModelCreating(modelBuilder);
        }
    }
}