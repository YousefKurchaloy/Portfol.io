using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class CredentialSkill : BaseEntity
    {
        [Display(Name = "Core Focus")]
        public bool IsCoreFocus { get; set; } = false;

        [Required]
        public int CredentialId { get; set; }
        public Credential? Credential { get; set; }

        [Required]
        public int SkillId { get; set; }
        public Skill? Skill { get; set; }
    }
}