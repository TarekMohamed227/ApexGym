using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;

namespace ApexGym.Application.Dtos
{
    public class GetTrainerDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Specialization { get; set; } = string.Empty; // e.g., "Yoga", "Weightlifting"
        public int YearsOfExperience { get; set; }
        public string? Bio { get; set; }

        public virtual ICollection<WorkoutClassDto> WorkoutClasses { get; set; } = new List<WorkoutClassDto>();
    }
}
