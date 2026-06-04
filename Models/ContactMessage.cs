using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class ContactMessage : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Sender Name")]
        public required string SenderName { get; set; }

        [Required]
        [StringLength(254)] // RFC 5321 max email length
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Sender Email")]
        public required string SenderEmail { get; set; }

        [Required]
        [StringLength(100)]
        public required string Subject { get; set; }

        [Required]
        [StringLength(2000)]
        [DataType(DataType.MultilineText)]
        public required string Body { get; set; }

        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Read")]
        public bool IsRead { get; set; } = false;

        [Display(Name = "Archived")]
        public bool IsArchived { get; set; } = false;

        [Display(Name = "Replied At")]
        public DateTime? RepliedAt { get; set; }

        // --- Foreign Key & Navigation ---
        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}