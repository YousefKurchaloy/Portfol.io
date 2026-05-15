using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class Credential
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        [Display(Name = "Issuing Authority")]
        public required string IssuingAuthority { get; set; }

        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Url]
        public string? VerificationUrl { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; } // Navigation property
        public IEnumerable<CredentialSkill>? CredentialSkills { get; set; } // Navigation property for M-to-N relationship with Skill
    }
}