using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public enum ETimelineEventType
    {
        [Display(Name = "Work Experience")]
        Work,

        [Display(Name = "Education")]
        Education,

        [Display(Name = "Award / Recognition")]
        Award,

        [Display(Name = "Volunteer / Open Source")]
        Volunteer,

        [Display(Name = "Other")]
        Other
    }
}
