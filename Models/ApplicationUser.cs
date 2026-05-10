using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)] // Annotation 2
        public string Username { get; set; }

        [Required]
        [EmailAddress] 
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] 
        public string Password { get; set; }

        
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
        public ICollection<Credential> Credentials { get; set; } = new List<Credential>();
        public ICollection<CodingProfile> CodingProfiles { get; set; } = new List<CodingProfile>();
        public ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
    }
}