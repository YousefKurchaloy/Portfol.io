using System;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sender Name")]
        public string SenderName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Sender Email")]
        public string SenderEmail { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(2000)] 
        public string Body { get; set; }

        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        // Foreign Key to ApplicationUser
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}