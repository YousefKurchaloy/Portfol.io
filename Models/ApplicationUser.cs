using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)] // Annotation 2
        public required string Username { get; set; }

        [Required]
        [EmailAddress] 
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] 
        public required string Password { get; set; }

        
        public IEnumerable<Project> Projects { get; set; } = new List<Project>();
        public IEnumerable<Skill> Skills { get; set; } = new List<Skill>();
        public IEnumerable<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
        public IEnumerable<Credential> Credentials { get; set; } = new List<Credential>();
        public IEnumerable<CodingProfile> CodingProfiles { get; set; } = new List<CodingProfile>();
        public IEnumerable<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
    }
}