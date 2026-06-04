using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Models
{
    public class PlatformProfile : BaseEntity
    {
        [Required]
        [Column(TypeName = "varchar(25)")]
        [Display(Name = "Platform (e.g., Codeforces, LeetCode, etc.)")]
        public required string PlatformName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Handle can only contain letters, numbers, and underscores.")]
        public required string UserHandle { get; set; }

        [MaxLength(20)]
        public string? Rank { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "Profile URL")]
        public string? ProfileUrl { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order must be a positive integer.")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 1;

        // --- Foreign Key & Navigation ---
        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}