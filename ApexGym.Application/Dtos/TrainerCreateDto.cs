using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Dtos
{
    public class TrainerCreateDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Specialization { get; set; } = string.Empty;

        public int YearsOfExperience { get; set; }
        public string? Bio { get; set; } 
    }
}
