using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Portfolio.Models
{
    // IdentityUser<int> automatically provides: Id, UserName, Email, PasswordHash,
    // SecurityStamp, ConcurrencyStamp, PhoneNumber, LockoutEnd, AccessFailedCount, etc.
    public class ApplicationUser : IdentityUser<int>
    {
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [Required]
        [StringLength(2000)]
        [DataType(DataType.MultilineText)]
        public required string Bio { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "Profile Image URL")]
        public string? ProfileImageUrl { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "GitHub URL")]
        public string? GitHubUrl { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "LinkedIn URL")]
        public string? LinkedInUrl { get; set; }

        [Display(Name = "Availability Status")]
        public EAvailabilityStatus AvailabilityStatus { get; set; } = EAvailabilityStatus.Unavailable;

        // --- Navigation Properties (initialized to prevent NRE) ---
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
        public ICollection<Credential> Credentials { get; set; } = new List<Credential>();
        public ICollection<PlatformProfile> PlatformProfiles { get; set; } = new List<PlatformProfile>();
        public ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
    }
}