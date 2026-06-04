using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Models;

namespace Portfolio.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Note: Users DbSet is inherited from IdentityDbContext — do NOT re-declare it.
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

            // --- PREVENT MULTIPLE CASCADE PATHS ---
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

            // --- ENFORCE UNIQUE PAIRS IN JOIN TABLES ---
            modelBuilder.Entity<ProjectSkill>()
                .HasIndex(ps => new { ps.ProjectId, ps.SkillId })
                .IsUnique();

            modelBuilder.Entity<CredentialSkill>()
                .HasIndex(cs => new { cs.CredentialId, cs.SkillId })
                .IsUnique();
        }

        /// <summary>
        /// Automatically populates CreatedAt/UpdatedAt on all BaseEntity-derived entities.
        /// </summary>
        public override int SaveChanges()
        {
            ApplyAuditTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditTimestamps()
        {
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    // Prevent overwriting CreatedAt on updates
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                }
            }
        }
    }
}