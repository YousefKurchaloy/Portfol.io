using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Models
{
    public class CodingProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(25)")]
        [Display(Name = "Platform (e.g., Codeforces, LeetCode, etc.)")]
        public required string PlatformName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Handle can only contain letters, numbers, and underscores.")]
        public required string UserHandle { get; set; }

        [MaxLength(20)]
        public string? Rank { get; set; }
        // THM: 07xhacker
        // CF:  1600 Expert

        public int ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}