using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Domain.Entities
{
    public class WorkoutClass
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty; // e.g., "Morning Yoga"

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCapacity { get; set; }

        // Foreign Key for Trainer (Many-to-One)
        public int TrainerId { get; set; }

        // Navigation Property - The Trainer who teaches this class
        public virtual Trainer Trainer { get; set; } = null!;

        // Navigation Property - The collection of Members attending this class (via the join table)
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
