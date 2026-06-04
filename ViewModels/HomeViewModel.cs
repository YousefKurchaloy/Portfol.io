using System.ComponentModel.DataAnnotations;
using Portfolio.Models;

namespace Portfolio.ViewModels
{
    public class HomeViewModel
    {
        public ApplicationUser? AdminUser { get; set; }
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<Credential> Credentials { get; set; } = new List<Credential>();
        public List<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
        public List<PlatformProfile> PlatformProfiles { get; set; } = new List<PlatformProfile>();

        // Message input fields for contact message submission
        [Required(ErrorMessage = "Please tell me your name.")]
        [StringLength(100, ErrorMessage = "Name must be under 100 characters.")]
        public string SenderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please provide an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(254)]
        public string SenderEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please supply a subject line.")]
        [StringLength(100, ErrorMessage = "Subject must be under 100 characters.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please type your message payload.")]
        [StringLength(2000, ErrorMessage = "Message must be under 2000 characters.")]
        public string Body { get; set; } = string.Empty;
    }
}
