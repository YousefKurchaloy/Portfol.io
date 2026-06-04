using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public enum EAvailabilityStatus
    {
        [Display(Name = "Open to Opportunities")]
        OpenForWork,

        [Display(Name = "Currently Employed")]
        Employed,

        [Display(Name = "Accepting Freelance/Contracts")]
        Freelancing,

        [Display(Name = "In Stealth Mode / Building")]
        Building,

        [Display(Name = "Unavailable")]
        Unavailable
    }
}