using System.ComponentModel.DataAnnotations;
using Portfolio.Models;

namespace Portfolio.Areas.Admin.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [Required(ErrorMessage = "Bio is required.")]
        [StringLength(2000)]
        [DataType(DataType.MultilineText)]
        public string Bio { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(500)]
        [Display(Name = "Profile Image URL")]
        public string? ProfileImageUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(500)]
        [Display(Name = "GitHub URL")]
        public string? GitHubUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        [StringLength(500)]
        [Display(Name = "LinkedIn URL")]
        public string? LinkedInUrl { get; set; }

        [Required(ErrorMessage = "Availability status is required.")]
        [Display(Name = "Availability Status")]
        public EAvailabilityStatus AvailabilityStatus { get; set; }
    }
}
