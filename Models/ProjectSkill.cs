using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class ProjectSkill : BaseEntity
    {
        [StringLength(20)]
        [Display(Name = "Version (e.g., v8.0)")]
        public string? VersionUsed { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        [Required]
        public int SkillId { get; set; }
        public Skill? Skill { get; set; }
    }
}
