using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models
{
    // IdentityUser automatically gives: Id, UserName, Email, PasswordHash, SecurityStamp, etc.
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.MultilineText)]
        public required string Bio { get; set; }

        [Display(Name = "Availability Status")]
        public EAvailabilityStatus AvailabilityStatus { get; set; } = EAvailabilityStatus.Unavailable;


        public IEnumerable<Project>? Projects { get; set; }
        public IEnumerable<Skill>? Skills { get; set; }
        public IEnumerable<TimelineEvent>? TimelineEvents { get; set; }
        public IEnumerable<Credential>? Credentials { get; set; }
        public IEnumerable<PlatformProfile>? PlatformProfiles { get; set; }
        public IEnumerable<ContactMessage>? ContactMessages { get; set; }
    }

    // Questions For Dr. Salah:
    // 1. After researching, using ICollection<T> (initialized to a new List) for navigation properties is considered best practice because:
    //    - It prevents NullReferenceExceptions when the collection is not initialized.
    //    - It provides a more flexible and efficient way to work with related data. (.add() and .remove())
    // Would you recommend using ICollection<T> in the next phase of the project?
    // 
}