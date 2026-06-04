using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models
{
    /// <summary>
    /// Abstract base class providing audit timestamps for all domain entities.
    /// CreatedAt/UpdatedAt are auto-populated via the DbContext SaveChanges override.
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime UpdatedAt { get; set; }
    }
}
