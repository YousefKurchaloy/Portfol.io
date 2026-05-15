using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sender Name")]
        public required string SenderName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Sender Email")]
        public required string SenderEmail { get; set; }

        [Required]
        [StringLength(100)]
        public required string Subject { get; set; }

        [Required]
        [MaxLength(2000)]
        public required string Body { get; set; }


        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}