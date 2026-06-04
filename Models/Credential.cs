using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public class Credential : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public required string Name { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Issuing Authority")]
        public required string IssuingAuthority { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Issue Date")]
        public DateTime IssueDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "Verification URL")]
        public string? VerificationUrl { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "Badge URL")]
        public string? BadgeUrl { get; set; }

        // --- Foreign Key & Navigation ---
        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<CredentialSkill> CredentialSkills { get; set; } = new List<CredentialSkill>();
    }
}