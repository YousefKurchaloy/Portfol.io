using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        public ESkillCategory Category { get; set; }

        [Range(1, 100)]
        [Display(Name = "Proficiency Level (%)")]
        public int ProficiencyLevel { get; set; }

        public int ApplicationUserId { get; set; } // FK
        public ApplicationUser? ApplicationUser { get; set; } // Navigation property to ApplicationUser

        public IEnumerable<ProjectSkill>? ProjectSkills { get; set; } // Navigation property for M-to-N relationship with Project
        public IEnumerable<CredentialSkill>? CredentialSkills { get; set; } // Navigation property for M-to-N relationship with Credential
    }
}