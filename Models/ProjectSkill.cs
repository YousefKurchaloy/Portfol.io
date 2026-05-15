using Portfolio.Models;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class ProjectSkill
    {
        [Key]
        public int Id { get; set; }

        [StringLength(20)]
        [Display(Name = "Version (e.g., v8.0)")]
        public string? VersionUsed { get; set; }

        [Required]
        public int ProjectId { get; set; } //FK
        public Project? Project { get; set; } // Navigation property

        [Required]
        public int SkillId { get; set; } //FK
        public Skill? Skill { get; set; } // Navigation property

    }

}

