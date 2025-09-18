using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Dtos
{
    public class TrainerUpdateDto
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(255)]
        public string? Specialization { get; set; }

        public int? YearsOfExperience { get; set; }
        public string? Bio { get; set; }
    }
}
