using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    public enum SkillCategory
    {
        [Display(Name = "Backend Development")]
        Backend,

        [Display(Name = "Frontend Development")]
        Frontend,

        [Display(Name = "Mobile Development")]
        Mobile,

        [Display(Name = "Artificial Intelligence & LLMs")]
        ArtificialIntelligence,

        [Display(Name = "Software Design & Architecture")]
        SoftwareDesign,

        [Display(Name = "Cybersecurity & InfoSec")]
        Cybersecurity,

        [Display(Name = "Cloud Computing")]
        CloudComputing,

        [Display(Name = "DevOps & CI/CD")]
        DevOps,

        [Display(Name = "Database Management")]
        Database,

        [Display(Name = "Systems Administration")]
        SystemsAdministration,

        [Display(Name = "Hardware & Infrastructure")]
        Hardware,

        [Display(Name = "Competitive Programming")]
        CompetitiveProgramming,

        [Display(Name = "Networking")]
        Networking,

        [Display(Name = "Data Science & Analytics")]
        DataScience,

        [Display(Name = "Game Development")]
        GameDevelopment,

        [Display(Name = "Quality Assurance & Testing")]
        QualityAssurance,

        [Display(Name = "UI/UX Design")]
        UIUXDesign,

        [Display(Name = "Project Management")]
        ProjectManagement
    }
}