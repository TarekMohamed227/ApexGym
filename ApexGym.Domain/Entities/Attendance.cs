using ApexGym.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Domain.Entities
{
    public class Attendance
    {
        // Composite Key: MemberId + WorkoutClassId
        public int MemberId { get; set; }
        public int WorkoutClassId { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public bool Attended { get; set; } // Marks if the member actually showed up

        // Navigation Properties - Links back to the two main entities
        public virtual Member Member { get; set; } = null!;
        public virtual WorkoutClass WorkoutClass { get; set; } = null!;
    }
}
