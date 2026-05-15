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
        public DbSet<PlatformProfile> PlatformProfiles { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<ProjectSkill> ProjectSkills { get; set; }
        public DbSet<CredentialSkill> CredentialSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- PREVENT MULTIPLE CASCADE PATHS
            modelBuilder.Entity<ProjectSkill>()
                .HasOne(ps => ps.Skill)
                .WithMany(s => s.ProjectSkills)
                .HasForeignKey(ps => ps.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CredentialSkill>()
                .HasOne(cs => cs.Skill)
                .WithMany(s => s.CredentialSkills)
                .HasForeignKey(cs => cs.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- ENFORCE UNIQUE PAIRS IN JOIN TABLES
            modelBuilder.Entity<ProjectSkill>()
                .HasIndex(ps => new { ps.ProjectId, ps.SkillId })
                .IsUnique();

            modelBuilder.Entity<CredentialSkill>()
                .HasIndex(cs => new { cs.CredentialId, cs.SkillId })
                .IsUnique();
        }
    }
}