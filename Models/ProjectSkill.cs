using Portfolio.Models;
using System.ComponentModel.DataAnnotations;

namespace Portfol.io.Models
{
    public class ProjectSkill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        [Required]
        public int SkillId { get; set; }
        public Skill? Skill { get; set; }
   
    }

}

