using System;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class TimelineEvent
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        
        public string Organization { get; set; }

        public string Location { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; } // Nullable for ongoing events

        // Foreign Key to ApplicationUser
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}