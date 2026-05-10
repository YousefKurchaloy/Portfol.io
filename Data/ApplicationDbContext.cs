using Microsoft.EntityFrameworkCore;
using Portfolio.Models; 

namespace Portfolio.Data 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<TimelineEvent> TimelineEvents { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<CodingProfile> CodingProfiles { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }
    }
}