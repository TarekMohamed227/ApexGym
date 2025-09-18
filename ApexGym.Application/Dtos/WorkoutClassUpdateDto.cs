using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Dtos
{
    public class WorkoutClassUpdateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Range(1, 100)]
        public int? MaxCapacity { get; set; }

        public int? TrainerId { get; set; } 
    }
}
