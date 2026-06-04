using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Models
{
    [Table("PortfolioProjects")]
    public class Project : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public required string Title { get; set; }

        [Required]
        [StringLength(2000)]
        [DataType(DataType.MultilineText)]
        public required string Description { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "GitHub Repository")]
        public string? RepositoryUrl { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "Live Demo URL")]
        public string? LiveDemoUrl { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "Cover Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Featured Project")]
        public bool IsFeatured { get; set; } = false;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order must be a positive integer.")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 1;

        [DataType(DataType.Date)]
        [Display(Name = "Completion Date")]
        public DateTime? CompletionDate { get; set; }

        // --- Foreign Key & Navigation ---
        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
    }
}