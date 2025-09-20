using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Dtos
{
    public class AttendanceCreateDto
    {
        public int MemberId { get; set; }
        public int WorkoutClassId { get; set; }
        // RegistrationDate is set automatically
        // Attended is false by default
    }
}
