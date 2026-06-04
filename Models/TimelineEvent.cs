using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class TimelineEvent : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public required string Title { get; set; }

        [Required]
        [StringLength(200)]
        public required string Organization { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Event Type")]
        public ETimelineEventType EventType { get; set; } = ETimelineEventType.Work;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order must be a positive integer.")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 1;

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; } // Nullable = "Present"

        // --- Foreign Key & Navigation ---
        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}