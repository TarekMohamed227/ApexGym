using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Domain.Entities
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Specialization { get; set; } = string.Empty; // e.g., "Yoga", "Weightlifting"
        public int YearsOfExperience { get; set; }
        public string? Bio { get; set; } // Optional biography
        public bool IsActive { get; set; } = true;

        // Navigation Property - A Trainer can teach many WorkoutClasses
        public virtual ICollection<WorkoutClass> WorkoutClasses { get; set; } = new List<WorkoutClass>();
    }
}
