using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class CredentialSkill
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Core Focus")]
        public bool IsCoreFocus { get; set; } = false;
        [Required]
        public int CredentialId { get; set; } // FK
        public Credential? Credential { get; set; } // Navigation property

        [Required]
        public int SkillId { get; set; } // FK
        public Skill? Skill { get; set; } // Navigation property

    }
}