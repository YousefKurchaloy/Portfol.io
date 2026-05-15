using Portfol.io.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        public ESkillCategory Category { get; set; }

        [Range(1, 100)] // Annotation 6
        [Display(Name = "Proficiency Level (%)")]
        public int ProficiencyLevel { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public IEnumerable<ProjectSkill>? ProjectSkil { get; set; }

    }
}

