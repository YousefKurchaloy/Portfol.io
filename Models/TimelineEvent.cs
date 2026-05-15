using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class TimelineEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Organization { get; set; }

        public string? Location { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; } // Nullable = ~Present
        public int ApplicationUserId { get; set; } //FK
        public ApplicationUser? ApplicationUser { get; set; } // Navigation property to ApplicationUser
    }
}