using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Models
{
    [Table("PortfolioProjects")]
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Url]
        [Display(Name = "GitHub Repository")]
        public string? RepositoryUrl { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Completion Date")]
        public DateTime CompletionDate { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; } // Navigation property
        public IEnumerable<ProjectSkill>? ProjectSkills { get; set; } // Navigation property for M-toN relationship with Skill

    }
}