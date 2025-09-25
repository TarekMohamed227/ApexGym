using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Domain.Entities;

namespace ApexGym.Application.Dtos
{
    public class TrainerGetDto
    {
     
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

 
        public string Specialization { get; set; } = string.Empty; // e.g., "Yoga", "Weightlifting"
        public int YearsOfExperience { get; set; }
        public string? Bio { get; set; } // Optional biography
   

        // Navigation Property - A Trainer can teach many WorkoutClasses
    
    }
}
