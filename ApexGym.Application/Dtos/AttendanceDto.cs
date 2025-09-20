using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Dtos
{
    public class AttendanceDto
    {
        public int MemberId { get; set; }
        public int WorkoutClassId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool Attended { get; set; }

        // Include SIMPLE properties, not full entities
        public MemberDto Member { get; set; } // Use a simple MemberDto
        public WorkoutClassDto WorkoutClass { get; set; } // Use a simple WorkoutClassDto
    }
}
