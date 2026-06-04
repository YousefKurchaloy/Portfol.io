using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class Skill : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        public ESkillCategory Category { get; set; }

        [Range(1, 100)]
        [Display(Name = "Proficiency Level (%)")]
        public int ProficiencyLevel { get; set; }

        [StringLength(50)]
        [Display(Name = "Icon CSS Class")]
        public string? IconClass { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order must be a positive integer.")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 1;

        // --- Foreign Key & Navigation ---
        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
        public ICollection<CredentialSkill> CredentialSkills { get; set; } = new List<CredentialSkill>();
    }
}