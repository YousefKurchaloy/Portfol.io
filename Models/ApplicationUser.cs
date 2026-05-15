using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        public required string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.MultilineText)]
        public required string Bio { get; set; }

        [Display(Name = "Availability Status")]
        public EAvailabilityStatus AvailabilityStatus { get; set; } = EAvailabilityStatus.Unavailable;

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }


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